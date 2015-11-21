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
        }
    }
}
