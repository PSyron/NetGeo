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

namespace NetGeo
{
    public partial class Form1 : Form
    {
        GeoApiResponse domainLeft;
        GeoApiResponse domainRight;
        private static String SUCCESS = "success";

        public Form1()
        {
            InitializeComponent();
            InitLeftValues(ConnectionUtils.GetInstance().UserDetailsFromGeoApi());
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
                tbTimezone.Text = geoApiResponse.timezone;
                tbIspNameR.Text = geoApiResponse.isp;
                tbOrganizationNameR.Text = geoApiResponse.org;
                tbAsNumberR.Text = geoApiResponse.@as;
            }
        }

        private void bCheckL_Click(object sender, EventArgs e)
        {
            domainLeft = ConnectionUtils.GetInstance().HostDetailsFromGeoApi(tbIPL.Text);
            InitLeftValues(domainLeft);
            FillDistanceBetweenDomains();
            FillRTTAndHopCount(domainLeft);
        }

        private void bCheckR_Click(object sender, EventArgs e)
        {
            domainRight = ConnectionUtils.GetInstance().HostDetailsFromGeoApi(tbIPR.Text);
            InitRightValues(domainRight);
            FillDistanceBetweenDomains();
            FillRTTAndHopCount(domainRight);
        }

        private void FillDistanceBetweenDomains()
        {
            if (domainLeft != null && domainLeft.status.Equals(SUCCESS) && domainRight != null && domainRight.status.Equals(SUCCESS))
                tbDistanceGeo.Text = ConnectionUtils.GetInstance().GetDistanceBetweenGeographicPoints(domainLeft.lat, domainLeft.lon, domainRight.lat, domainRight.lon) + " km";
        }

        private void FillRTTAndHopCount(GeoApiResponse geoApiResponse)
        {
            if (geoApiResponse.status.Equals(SUCCESS))
            {
                tbRTT.Text = ConnectionUtils.GetInstance().PingHost(IPAddress.Parse(geoApiResponse.query)).RoundtripTime + "";
                tbDistanceIP.Text = ConnectionUtils.GetInstance().PerformPathping(IPAddress.Parse(geoApiResponse.query)).Length + "";
            }
        }
    }
}
