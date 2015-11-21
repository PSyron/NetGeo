using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CsvHelper;
using System.IO;
using System.Net.NetworkInformation;

namespace NetGeo
{
    public partial class Form1 : Form
    {
        GeoApiResponse domainLeft;
        GeoApiResponse domainRight;
        private static String SUCCESS = "success";
        private OpenFileDialog ofd;
        private CsvReader csv;
        private List<CsvStructure> csvRecords;


        public Form1()
        {
            InitializeComponent();
            domainLeft = ConnectionUtils.GetInstance().UserDetailsFromGeoApi();
            InitLeftValues(domainLeft);
            ofd = new OpenFileDialog();
            ofd.Filter = "CSV files (*.csv) | *.csv;";
        }

        private void InitLeftValues(GeoApiResponse geoApiResponse)
        {
            if (geoApiResponse != null)
            {
                tbStatusL.Text = geoApiResponse.status;
                tbCountryL.Text = geoApiResponse.country;
                tbRegionNameL.Text = geoApiResponse.regionName;
                tbCityL.Text = geoApiResponse.regionName;
                tbZipCodeL.Text = geoApiResponse.zip;
                tbLatL.Text = "" + geoApiResponse.lat;
                tbLonL.Text = "" + geoApiResponse.lon;
                tbTimezoneL.Text = geoApiResponse.timezone;
                tbIspNameL.Text = geoApiResponse.isp;
                tbOrganizationNameL.Text = geoApiResponse.org;
                tbAsNumberL.Text = geoApiResponse.@as;

                FillDistanceBetweenDomains();
                FillRTTAndHopCountLeft(domainLeft);
            }
        }

        private void InitRightValues(GeoApiResponse geoApiResponse)
        {
            if (geoApiResponse != null)
            {
                tbStatusR.Text = geoApiResponse.status;
                tbCountryR.Text = geoApiResponse.country;
                tbRegionNameR.Text = geoApiResponse.regionName;
                tbCityR.Text = geoApiResponse.regionName;
                tbZipCodeR.Text = geoApiResponse.zip;
                textBox7.Text = "" + geoApiResponse.lat;
                tbLonR.Text = "" + geoApiResponse.lon;
                tbTimezoneR.Text = geoApiResponse.timezone;
                tbIspNameR.Text = geoApiResponse.isp;
                tbOrganizationNameR.Text = geoApiResponse.org;
                tbAsNumberR.Text = geoApiResponse.@as;

                FillDistanceBetweenDomains();
                FillRTTAndHopCountRight(domainRight);
            }
        }

        private void bCheckL_Click(object sender, EventArgs e)
        {
            domainLeft = ConnectionUtils.GetInstance().HostDetailsFromGeoApi(tbIPL.Text);
            InitLeftValues(domainLeft);
        }

        private void bCheckR_Click(object sender, EventArgs e)
        {
            domainRight = ConnectionUtils.GetInstance().HostDetailsFromGeoApi(tbIPR.Text);
            InitRightValues(domainRight);
        }

        private void FillDistanceBetweenDomains()
        {
            if (domainLeft != null && domainLeft.status.Equals(SUCCESS) && domainRight != null && domainRight.status.Equals(SUCCESS))
                tbDistanceGeo.Text = ConnectionUtils.GetInstance().GetDistanceBetweenGeographicPoints(domainLeft.lat, domainLeft.lon, domainRight.lat, domainRight.lon) + " km";
        }

        private void FillRTTAndHopCountLeft(GeoApiResponse geoApiResponse)
        {
            if (geoApiResponse.status.Equals(SUCCESS))
            {
                tbRTTL.Text = ConnectionUtils.GetInstance().PingHost(IPAddress.Parse(geoApiResponse.query)).RoundtripTime + "";
                tbDistanceIPL.Text = ConnectionUtils.GetInstance().PerformPathping(IPAddress.Parse(geoApiResponse.query)).Length + "";
            }
        }

        private void FillRTTAndHopCountRight(GeoApiResponse geoApiResponse)
        {
            if (geoApiResponse.status.Equals(SUCCESS))
            {
                tbRTTR.Text = ConnectionUtils.GetInstance().PingHost(IPAddress.Parse(geoApiResponse.query)).RoundtripTime + "";
                tbDistanceIPR.Text = ConnectionUtils.GetInstance().PerformPathping(IPAddress.Parse(geoApiResponse.query)).Length + "";
            }
        }

        private void bLoadFile_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                tbFileName.Text = ofd.SafeFileName;
                tbFilePath.Text = ofd.FileName;
                //Status juz po parsowaniu csv
                MessageBox.Show("Plik wczytano pomyślnie!");
                ReadCSV(ofd.FileName);
            }
            else
                //ew po nieudanym parsowaniu csv
                MessageBox.Show("Nie wybrano pliku!!");
        }

        private void ReadCSV(string inCsvPath)
        {
            StreamReader reader = new StreamReader(inCsvPath);
            csv = new CsvReader(reader);
            csvRecords = csv.GetRecords<CsvStructure>().ToList();
            MakeResearch(csvRecords);
        }


        private void MakeResearch(List<CsvStructure> list)
        {
            ConnectionUtils utils = ConnectionUtils.GetInstance();
            List<ResearchModel> reasearchResult = new List<ResearchModel>();
            ResearchModel temp = new ResearchModel();
            float distance = 0;
            GeoApiResponse geoResponse;
            PingReply tempReply;
            GeoApiResponse userDetails = utils.UserDetailsFromGeoApi();
            PingReply smallReply;
            PingReply biggerReply;
            PingReply[] ttl;
            foreach (CsvStructure record in list)
            {
                //First find domain IP address
                geoResponse = utils.HostDetailsFromGeoApi(record.URL);

                if (geoResponse != null)
                {
                    //This will be same for all 10 results
                    temp = new ResearchModel();
                    temp.IPAdrress = geoResponse.query;
                    temp.DomainName = record.URL;
                    temp.Latitude = geoResponse.lat.ToString();
                    temp.Longitude = geoResponse.lon.ToString();
                    temp.DistanceTo = utils.GetDistanceBetweenGeographicPoints(userDetails.lat, userDetails.lon, geoResponse.lat, geoResponse.lon).ToString();
                    try {
                        ttl = utils.PerformPathping(IPAddress.Parse(geoResponse.query));
                    }catch(System.FormatException e)
                    {
                        break;
                    }
                    if (ttl == null)
                        temp.TTL = "error";
                    else
                    { temp.TTL = ttl.Length.ToString(); }

                    for (int i = 0; i < 10; i++)//10 measures for each domain
                    {
                        smallReply = utils.PingHost(IPAddress.Parse(geoResponse.query), true);
                        if (smallReply != null)
                            temp.Time8B = smallReply.RoundtripTime.ToString();
                        else
                            temp.Time8B = "-1";

                        biggerReply = utils.PingHost(IPAddress.Parse(geoResponse.query), false);
                        if (biggerReply != null)
                            temp.Time64kB = biggerReply.RoundtripTime.ToString();
                        else
                            temp.Time64kB = "-1";
                        
                        reasearchResult.Add(temp);
                    }
                }

            }
            writeToFile(reasearchResult);
        }

        private void writeToFile(List<ResearchModel> toWrite)
        {
            string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = pathDesktop + "\\mycsvfile.csv";

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            string delimter = ";";
            List<string[]> output = new List<string[]>();
            output.Add(new string[] { "DomainName", "IPAddress", "Latitude", "Longitude", "DistanceTo", "TTL", "Time8B", "Time64kB", });
        
            //flexible part ... add as many object as you want based on your app logic
            //output.Add(new string[] { "DomainName", temp });
            //output.Add(new string[] { "IPAddress", "TEST4" });
            //output.Add(new string[] { "Latitude", "TEST2" });
            //output.Add(new string[] { "Longitude", "TEST4" });
            //output.Add(new string[] { "DistanceTo", "TEST2" });
            //output.Add(new string[] { "TTL", "TEST4" });
            //output.Add(new string[] { "Time8B", "TEST2" });
            //output.Add(new string[] { "Time64kB", "TEST4" });
            foreach (ResearchModel single in toWrite)
            {
                output.Add(new string[] { single.DomainName,single.IPAdrress,single.Latitude,single.Longitude,single.DistanceTo,single.TTL,single.Time8B,single.Time64kB });
            }

            int length = output.Count;

            using (System.IO.TextWriter writer = File.CreateText(filePath))
            {
                for (int index = 0; index < length; index++)
                {
                    writer.WriteLine(string.Join(delimter, output[index]));
                }
            }
        }
    }
}
