using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryPublishingDataLoader.Types
{
    public enum PubTypeCategory
    {
        General,
        OpenAccess,
        Paid,
        Hybrid,
        UniPress
    }

    internal class PublicationTypes
    {
        public PublicationTypes()
        {
            FacultyConference = new Dictionary<PubTypeCategory, int>();
            StudentConference = new Dictionary<PubTypeCategory, int>();
            Databases = new Dictionary<PubTypeCategory, int>();
            Datasets = new Dictionary<PubTypeCategory, int>();
            EducationalResources = new Dictionary<PubTypeCategory, int>();
            ETDs = new Dictionary<PubTypeCategory, int>();
            DHProjects = new Dictionary<PubTypeCategory, int>();
            FacultyJournals = new Dictionary<PubTypeCategory, int>();
            StudentJournals = new Dictionary<PubTypeCategory, int>();
            ExternalJournals = new Dictionary<PubTypeCategory, int>();
            InactiveJournals = new Dictionary<PubTypeCategory, int>();
            Monographs = new Dictionary<PubTypeCategory, int>();
            Newsletters = new Dictionary<PubTypeCategory, int>();
            Reports = new Dictionary<PubTypeCategory, int>();
            Textbooks = new Dictionary<PubTypeCategory, int>();
            UndergradETDs = new Dictionary<PubTypeCategory, int>();
            Other = new Dictionary<PubTypeCategory, int>();

            foreach (PubTypeCategory cat in Enum.GetValues(typeof(PubTypeCategory)))
            {
                FacultyConference.Add(cat, 0);
                StudentConference.Add(cat, 0);
                Databases.Add(cat, 0);
                Datasets.Add(cat, 0);
                EducationalResources.Add(cat, 0);
                ETDs.Add(cat, 0);
                DHProjects.Add(cat, 0);
                FacultyJournals.Add(cat, 0);
                StudentJournals.Add(cat, 0);
                ExternalJournals.Add(cat, 0);
                InactiveJournals.Add(cat, 0);
                Monographs.Add(cat, 0);
                Newsletters.Add(cat, 0);
                Reports.Add(cat, 0);
                Textbooks.Add(cat, 0);
                UndergradETDs.Add(cat, 0);
                Other.Add(cat, 0);
            }
        }

        public Dictionary<PubTypeCategory, int> FacultyConference { get; set; }
        public Dictionary<PubTypeCategory, int> StudentConference { get; set; }
        public Dictionary<PubTypeCategory, int> Databases { get; set; }
        public Dictionary<PubTypeCategory, int> Datasets { get; set; }
        public Dictionary<PubTypeCategory, int> EducationalResources { get; set; }
        public Dictionary<PubTypeCategory, int> ETDs { get; set; }
        public Dictionary<PubTypeCategory, int> DHProjects { get; set; }
        public Dictionary<PubTypeCategory, int> FacultyJournals { get; set; }
        public Dictionary<PubTypeCategory, int> StudentJournals { get; set; }
        public Dictionary<PubTypeCategory, int> ExternalJournals  { get; set; }
        public Dictionary<PubTypeCategory, int> InactiveJournals { get; set; }
        public Dictionary<PubTypeCategory, int> Monographs { get; set; }
        public Dictionary<PubTypeCategory, int> Newsletters { get; set; }
        public Dictionary<PubTypeCategory, int> Reports { get; set; }
        public Dictionary<PubTypeCategory, int> Textbooks { get; set; }
        public Dictionary<PubTypeCategory, int> UndergradETDs { get; set; }
        public Dictionary<PubTypeCategory, int> Other { get; set; }
    }
}
