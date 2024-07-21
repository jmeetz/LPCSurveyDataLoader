using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryPublishingDataLoader.Types
{
    internal class Partners
    {
        public bool Departments { get; set; }
        public bool Faculty { get; set; }
        public bool GradStudents { get; set; }
        public bool Undergrads { get; set; }
        public bool Consortia { get; set; }
        public string Preference { get; set; }
    }
}
