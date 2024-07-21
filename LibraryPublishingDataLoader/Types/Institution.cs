using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryPublishingDataLoader.Types
{
    internal class Institution
    {
        public string Name { get; set; }
        public Dictionary<string, InstitutionData> Data { get; }
        public Dictionary<string, Services> Services { get; }
        public Dictionary<string, Staffing> Staffing { get; }

        public Institution()
        {
            Data = new Dictionary<string, InstitutionData>();
            Services = new Dictionary<string, Services>();
            Staffing = new Dictionary<string, Staffing>();
        }
    }
}
