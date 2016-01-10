using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetGeo
{
    class ConnectionUtils
    {
        public static ConnectionUtils sInstance;
        public static String DOMAIN = "http://ip-api.com/json/";

        public ConnectionUtils()
        {
            sInstance = this;
        }

        public static ConnectionUtils GetInstance()
        {
            if (sInstance == null)
                sInstance = new ConnectionUtils();
            return sInstance;
        }

        /// <summary>
        /// Performs a pathping
        /// </summary>
        /// <param name="ipaTarget">The target</param>
        /// <param name="iHopcount">The maximum hopcount</param>
        /// <param name="iTimeout">The timeout for each ping</param>
        /// <returns>An array of PingReplys for the whole path</returns>
        public PingReply[] PerformPathping(IPAddress ipaTarget, int iHopcount = 20, int iTimeout = 500)
        {
            System.Collections.ArrayList arlPingReply = new System.Collections.ArrayList();
            Ping myPing = new Ping();
            PingReply prResult;
            for (int iC1 = 1; iC1 < iHopcount; iC1++)
            {
                try
                {
                    prResult = myPing.Send(ipaTarget, iTimeout, new byte[10], new PingOptions(iC1, false)); //ttl zwiekszany
                }
                catch (PingException e)
                {
                    return null;
                }
                if (prResult.Status == IPStatus.Success) //osiagnieto hosta przy danym ttl 
                {
                    iC1 = iHopcount; // sprawdzic czy nie wystarcyz po prostu dac breaka
                }
                arlPingReply.Add(prResult);
                Console.WriteLine(((PingReply)prResult).Address);
            }
            PingReply[] prReturnValue = new PingReply[arlPingReply.Count];
            for (int iC1 = 0; iC1 < arlPingReply.Count; iC1++)
            {
                prReturnValue[iC1] = (PingReply)arlPingReply[iC1];
            }
            return prReturnValue;
        }

        //posiada RTT & IPStatus
        public PingReply PingHost(IPAddress host, bool minimumBuffer = true)
        {
            int iTimeout = 2000;
            int iTTL = 30;
            Ping pingSender = new Ping();
            PingReply reply = null;
            PingOptions options = new PingOptions(iTTL, true);
            //options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            //string data = "aaaaaaaa";//8 bytes
            byte[] buffer = new byte[8];
            if (!minimumBuffer)
            {
                buffer = new byte[1024];
                //      for (int i = 0; i < 10; i++)//data 8kB
                //         data += data;
            }
            //    byte[] buffer = Encoding.ASCII.GetBytes(data);

            try
            {
                reply = pingSender.Send(host, iTimeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    return reply;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
                //MOSTLY HOST NOT FOUND
            }
        }

        public GeoApiResponse UserDetailsFromGeoApi()
        {
            return HostDetailsFromGeoApi("");
        }

        public GeoApiResponse HostDetailsFromGeoApi(string hostAddress)
        {
            WebClient c = new WebClient();
            GeoApiResponse temp = null;
            try
            {
                var data = c.DownloadString(DOMAIN + hostAddress);
                //Console.WriteLine(data);
                temp = JsonConvert.DeserializeObject<GeoApiResponse>(data);
                Console.WriteLine(temp.query + "  " + temp.lat + "  " + temp.lon);
            }
            catch (Exception e)
            {

            }
            return temp;
            //JObject o = JObject.Parse(data);
            //Console.WriteLine("Name: " + o["name"]);
            //Console.WriteLine("Email Address[1]: " + o["email"][0]);
            //Console.WriteLine("Email Address[2]: " + o["email"][1]);
            //Console.WriteLine("Website [home page]: " + o["websites"]["home page"]);
            //Console.WriteLine("Website [blog]: " + o["websites"]["blog"]);
            //Console.ReadLine();
        }

        public double GetDistanceBetweenGeographicPoints(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            return dist * 1.609344;
        }
    }
}
