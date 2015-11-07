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
                prResult = myPing.Send(ipaTarget, iTimeout, new byte[10], new PingOptions(iC1, false)); //ttl zwiekszany
                if (prResult.Status == IPStatus.Success) //osiagnieto hosta przy danym ttl 
                {
                    iC1 = iHopcount; // sprawdzic czy nie wystarcyz po prostu dac breaka
                }
                arlPingReply.Add(prResult);
            }
            PingReply[] prReturnValue = new PingReply[arlPingReply.Count];
            for (int iC1 = 0; iC1 < arlPingReply.Count; iC1++)
            {
                prReturnValue[iC1] = (PingReply)arlPingReply[iC1];
            }
            return prReturnValue;
        }

        //posiada RTT & IPStatus
        public PingReply PingHost(IPAddress host, int iTimeout = 500, int iTTL = 20)
        {
            Ping pingSender = new Ping();
            PingReply reply = null;
            PingOptions options = new PingOptions(iTTL, true);
            //options.DontFragment = true;
          
            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            try
            {
                reply = pingSender.Send(host, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    //Ping was successful
                }
                else
                {
                    //Ping failed
                }
            }
            catch (Exception ex)
            {
                //MOSTLY HOST NOT FOUND
            }
            return reply;
        }

        public GeoApiResponse UserDetailsFromGeoApi()
        {
            return HostDetailsFromGeoApi("");
        }

        public GeoApiResponse HostDetailsFromGeoApi(string hostAddress)
        {
            WebClient c = new WebClient();
            var data = c.DownloadString(DOMAIN + hostAddress);
            //Console.WriteLine(data);
            GeoApiResponse temp =  JsonConvert.DeserializeObject<GeoApiResponse>(data);
            Console.WriteLine(temp.query+"  " + temp.lat + "  " +temp.lon );
            return temp;
            //JObject o = JObject.Parse(data);
            //Console.WriteLine("Name: " + o["name"]);
            //Console.WriteLine("Email Address[1]: " + o["email"][0]);
            //Console.WriteLine("Email Address[2]: " + o["email"][1]);
            //Console.WriteLine("Website [home page]: " + o["websites"]["home page"]);
            //Console.WriteLine("Website [blog]: " + o["websites"]["blog"]);
            //Console.ReadLine();
        }
    }
}
