using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetGeo
{
    class ResearchModel
    {
        public string DomainName { get; set; }
        public string IPAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string DistanceTo { get; set; }
        public string TTL { get; set; }
        public string Time8B { get; set; }
        public string Time64kB { get; set; }
        
    }
}
