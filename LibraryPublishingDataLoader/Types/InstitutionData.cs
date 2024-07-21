using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryPublishingDataLoader.Types
{
    internal class InstitutionData
    {
        public string LibraryName { get; set; }
        public string UnitName { get; set; }
        public string Type { get; set; }
        public string Country { get; set; }
        public string YearEstablished { get; set; }
        public string ProgramType { get; set; }
    }
}
