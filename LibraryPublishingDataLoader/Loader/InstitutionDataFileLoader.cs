using CsvHelper;
using LibraryPublishingDataLoader.Types;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryPublishingDataLoader.Loader
{
    internal class InstitutionDataFileLoader
    {
        public string InputDir;
        public string OutputDir;

        private Dictionary<string, InstitutionDataCsvParser> fileParsers = new Dictionary<string, InstitutionDataCsvParser>();
        private Dictionary<string, Institution> institutions = new Dictionary<string, Institution>();

        public InstitutionDataFileLoader()
        {
        }

        public void LoadCsvReaders()
        {
            Console.WriteLine($"Loading input files in directory [{InputDir}]");

            foreach (string inputFilePath in Directory.EnumerateFiles(InputDir, "*.csv"))
            {
                string fileName = Path.GetFileName(inputFilePath);
                string surveyYear = fileName.Split("_")[3];

                Console.WriteLine($"Adding file [{fileName}]");

                fileParsers.Add(fileName, new InstitutionDataCsvParser
                {
                    InputFilePath = inputFilePath,
                    SurveyYear = surveyYear
                });
            }

            Console.WriteLine($"Loaded {fileParsers.Count()} input files");
            Console.WriteLine();
        }

        public void ParseCsvFiles()
        {
            foreach (string pKey in fileParsers.Keys)
            {
                Console.WriteLine($"Parsing CSV data for [{pKey}]");

                InstitutionDataCsvParser parser = fileParsers[pKey];

                parser.ParseCsvData(institutions);
            }

            Console.WriteLine($"Completed parsing data for {fileParsers.Keys.Count} files");
            Console.WriteLine();
        }

        public void SaveOutputFiles()
        {
            string outputPath = Path.Combine(OutputDir, $"Output_{DateTimeOffset.Now.ToUnixTimeSeconds()}.csv");

            File.Create(outputPath).Close();

            using (var sw = new StreamWriter(outputPath))
            {
                using (var csv = new CsvWriter(sw, CultureInfo.InvariantCulture))
                {
                    var records = new List<dynamic>();

                    foreach (string instName in institutions.Keys)
                    {
                        foreach (string yearLib in institutions[instName].Data.Keys)
                        {
                            dynamic record = new ExpandoObject();
                            var instData = institutions[instName].Data[yearLib];
                            var services = institutions[instName].Services[yearLib];
                            var staffing = institutions[instName].Staffing[yearLib];

                            record.RecordKey = yearLib;
                            record.Year = yearLib.Split("||")[0];
                            record.InstitutionName = instName;
                            record.InstitutionLibraryName = instData.LibraryName;
                            record.InstitutionUnitName = instData.UnitName;
                            record.InstitutionType = instData.Type;
                            record.InstitutionCountry = instData.Country;
                            record.InstitutionYearEstablished = instData.YearEstablished;
                            record.InstitutionProgramType = instData.ProgramType;
                            record.MediaAudio = services.Media.Audio;
                            record.MediaConceptMaps = services.Media.ConceptMaps;
                            record.MediaData = services.Media.Data;
                            record.MediaImages = services.Media.Images;
                            record.MediaModeling = services.Media.Modeling;
                            record.MediaMultimediaInteractive = services.Media.MultimediaInteractive;
                            record.MediaText = services.Media.Text;
                            record.MediaVideo = services.Media.Video;
                            record.MediaOther = services.Media.Other;
                            record.PartnersDepartments = services.Partners.Departments;
                            record.PartnersFaculty = services.Partners.Faculty;
                            record.PartnersGradStudents = services.Partners.GradStudents;
                            record.PartnersUndergrads = services.Partners.Undergrads;
                            record.PartnersConsortia = services.Partners.Consortia;
                            record.PartnersPreference = services.Partners.Preference;
                            record.PubTypeGeneralFacultyConference = services.PublicationTypes.FacultyConference[PubTypeCategory.General];
                            record.PubTypeGeneralStudentConference = services.PublicationTypes.StudentConference[PubTypeCategory.General];
                            record.PubTypeGeneralDatabases = services.PublicationTypes.Databases[PubTypeCategory.General];
                            record.PubTypeGeneralDatasets = services.PublicationTypes.Datasets[PubTypeCategory.General];
                            record.PubTypeGeneralEducationalResources = services.PublicationTypes.EducationalResources[PubTypeCategory.General];
                            record.PubTypeGeneralETDs = services.PublicationTypes.ETDs[PubTypeCategory.General];
                            record.PubTypeGeneralDHProjects = services.PublicationTypes.DHProjects[PubTypeCategory.General];
                            record.PubTypeGeneralFacultyJournals = services.PublicationTypes.FacultyJournals[PubTypeCategory.General];
                            record.PubTypeGeneralStudentJournals = services.PublicationTypes.StudentJournals[PubTypeCategory.General];
                            record.PubTypeGeneralExternalJournals = services.PublicationTypes.ExternalJournals[PubTypeCategory.General];
                            record.PubTypeGeneralInactiveJournals = services.PublicationTypes.InactiveJournals[PubTypeCategory.General];
                            record.PubTypeGeneralMonographs = services.PublicationTypes.Monographs[PubTypeCategory.General];
                            record.PubTypeGeneralNewsletters = services.PublicationTypes.Newsletters[PubTypeCategory.General];
                            record.PubTypeGeneralReports = services.PublicationTypes.Reports[PubTypeCategory.General];
                            record.PubTypeGeneralTextbooks = services.PublicationTypes.Textbooks[PubTypeCategory.General];
                            record.PubTypeGeneralUndergradETDs = services.PublicationTypes.UndergradETDs[PubTypeCategory.General];
                            record.PubTypeGeneralOther = services.PublicationTypes.Other[PubTypeCategory.General];
                            record.PubTypeOpenAccessFacultyConference = services.PublicationTypes.FacultyConference[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessStudentConference = services.PublicationTypes.StudentConference[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessDatabases = services.PublicationTypes.Databases[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessDatasets = services.PublicationTypes.Datasets[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessEducationalResources = services.PublicationTypes.EducationalResources[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessETDs = services.PublicationTypes.ETDs[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessDHProjects = services.PublicationTypes.DHProjects[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessFacultyJournals = services.PublicationTypes.FacultyJournals[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessStudentJournals = services.PublicationTypes.StudentJournals[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessExternalJournals = services.PublicationTypes.ExternalJournals[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessInactiveJournals = services.PublicationTypes.InactiveJournals[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessMonographs = services.PublicationTypes.Monographs[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessNewsletters = services.PublicationTypes.Newsletters[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessReports = services.PublicationTypes.Reports[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessTextbooks = services.PublicationTypes.Textbooks[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessUndergradETDs = services.PublicationTypes.UndergradETDs[PubTypeCategory.OpenAccess];
                            record.PubTypeOpenAccessOther = services.PublicationTypes.Other[PubTypeCategory.OpenAccess];
                            record.PubTypePaidFacultyConference = services.PublicationTypes.FacultyConference[PubTypeCategory.Paid];
                            record.PubTypePaidStudentConference = services.PublicationTypes.StudentConference[PubTypeCategory.Paid];
                            record.PubTypePaidDatabases = services.PublicationTypes.Databases[PubTypeCategory.Paid];
                            record.PubTypePaidDatasets = services.PublicationTypes.Datasets[PubTypeCategory.Paid];
                            record.PubTypePaidEducationalResources = services.PublicationTypes.EducationalResources[PubTypeCategory.Paid];
                            record.PubTypePaidETDs = services.PublicationTypes.ETDs[PubTypeCategory.Paid];
                            record.PubTypePaidDHProjects = services.PublicationTypes.DHProjects[PubTypeCategory.Paid];
                            record.PubTypePaidFacultyJournals = services.PublicationTypes.FacultyJournals[PubTypeCategory.Paid];
                            record.PubTypePaidStudentJournals = services.PublicationTypes.StudentJournals[PubTypeCategory.Paid];
                            record.PubTypePaidExternalJournals = services.PublicationTypes.ExternalJournals[PubTypeCategory.Paid];
                            record.PubTypePaidInactiveJournals = services.PublicationTypes.InactiveJournals[PubTypeCategory.Paid];
                            record.PubTypePaidMonographs = services.PublicationTypes.Monographs[PubTypeCategory.Paid];
                            record.PubTypePaidNewsletters = services.PublicationTypes.Newsletters[PubTypeCategory.Paid];
                            record.PubTypePaidReports = services.PublicationTypes.Reports[PubTypeCategory.Paid];
                            record.PubTypePaidTextbooks = services.PublicationTypes.Textbooks[PubTypeCategory.Paid];
                            record.PubTypePaidUndergradETDs = services.PublicationTypes.UndergradETDs[PubTypeCategory.Paid];
                            record.PubTypePaidOther = services.PublicationTypes.Other[PubTypeCategory.Paid];
                            record.PubTypeHybridFacultyConference = services.PublicationTypes.FacultyConference[PubTypeCategory.Hybrid];
                            record.PubTypeHybridStudentConference = services.PublicationTypes.StudentConference[PubTypeCategory.Hybrid];
                            record.PubTypeHybridDatabases = services.PublicationTypes.Databases[PubTypeCategory.Hybrid];
                            record.PubTypeHybridDatasets = services.PublicationTypes.Datasets[PubTypeCategory.Hybrid];
                            record.PubTypeHybridEducationalResources = services.PublicationTypes.EducationalResources[PubTypeCategory.Hybrid];
                            record.PubTypeHybridETDs = services.PublicationTypes.ETDs[PubTypeCategory.Hybrid];
                            record.PubTypeHybridDHProjects = services.PublicationTypes.DHProjects[PubTypeCategory.Hybrid];
                            record.PubTypeHybridFacultyJournals = services.PublicationTypes.FacultyJournals[PubTypeCategory.Hybrid];
                            record.PubTypeHybridStudentJournals = services.PublicationTypes.StudentJournals[PubTypeCategory.Hybrid];
                            record.PubTypeHybridExternalJournals = services.PublicationTypes.ExternalJournals[PubTypeCategory.Hybrid];
                            record.PubTypeHybridInactiveJournals = services.PublicationTypes.InactiveJournals[PubTypeCategory.Hybrid];
                            record.PubTypeHybridMonographs = services.PublicationTypes.Monographs[PubTypeCategory.Hybrid];
                            record.PubTypeHybridNewsletters = services.PublicationTypes.Newsletters[PubTypeCategory.Hybrid];
                            record.PubTypeHybridReports = services.PublicationTypes.Reports[PubTypeCategory.Hybrid];
                            record.PubTypeHybridTextbooks = services.PublicationTypes.Textbooks[PubTypeCategory.Hybrid];
                            record.PubTypeHybridUndergradETDs = services.PublicationTypes.UndergradETDs[PubTypeCategory.Hybrid];
                            record.PubTypeHybridOther = services.PublicationTypes.Other[PubTypeCategory.Hybrid];
                            record.PubTypeUniPressFacultyConference = services.PublicationTypes.FacultyConference[PubTypeCategory.UniPress];
                            record.PubTypeUniPressStudentConference = services.PublicationTypes.StudentConference[PubTypeCategory.UniPress];
                            record.PubTypeUniPressDatabases = services.PublicationTypes.Databases[PubTypeCategory.UniPress];
                            record.PubTypeUniPressDatasets = services.PublicationTypes.Datasets[PubTypeCategory.UniPress];
                            record.PubTypeUniPressEducationalResources = services.PublicationTypes.EducationalResources[PubTypeCategory.UniPress];
                            record.PubTypeUniPressETDs = services.PublicationTypes.ETDs[PubTypeCategory.UniPress];
                            record.PubTypeUniPressDHProjects = services.PublicationTypes.DHProjects[PubTypeCategory.UniPress];
                            record.PubTypeUniPressFacultyJournals = services.PublicationTypes.FacultyJournals[PubTypeCategory.UniPress];
                            record.PubTypeUniPressStudentJournals = services.PublicationTypes.StudentJournals[PubTypeCategory.UniPress];
                            record.PubTypeUniPressExternalJournals = services.PublicationTypes.ExternalJournals[PubTypeCategory.UniPress];
                            record.PubTypeUniPressInactiveJournals = services.PublicationTypes.InactiveJournals[PubTypeCategory.UniPress];
                            record.PubTypeUniPressMonographs = services.PublicationTypes.Monographs[PubTypeCategory.UniPress];
                            record.PubTypeUniPressNewsletters = services.PublicationTypes.Newsletters[PubTypeCategory.UniPress];
                            record.PubTypeUniPressReports = services.PublicationTypes.Reports[PubTypeCategory.UniPress];
                            record.PubTypeUniPressTextbooks = services.PublicationTypes.Textbooks[PubTypeCategory.UniPress];
                            record.PubTypeUniPressUndergradETDs = services.PublicationTypes.UndergradETDs[PubTypeCategory.UniPress];
                            record.PubTypeUniPressOther = services.PublicationTypes.Other[PubTypeCategory.UniPress];
                            record.ServiceTypeAnalytics = services.ServiceTypes.Analytics;
                            record.ServiceTypeApplyingCatPubData = services.ServiceTypes.ApplyingCatPubData;
                            record.ServiceTypeAudioVideoStreaming = services.ServiceTypes.AudioVideoStreaming;
                            record.ServiceTypeAuthorAdvisoryCopyright = services.ServiceTypes.AuthorAdvisoryCopyright;
                            record.ServiceTypeAuthorAdvisoryOther = services.ServiceTypes.AuthorAdvisoryOther;
                            record.ServiceTypeBudgetPreparation = services.ServiceTypes.BudgetPreparation;
                            record.ServiceTypeBusinessModelDev = services.ServiceTypes.BusinessModelDev;
                            record.ServiceTypeCataloging = services.ServiceTypes.Cataloging;
                            record.ServiceTypeCompilingIndexesToc = services.ServiceTypes.CompilingIndexesToc;
                            record.ServiceTypeContractLicensePrep = services.ServiceTypes.ContractLicensePrep;
                            record.ServiceTypeCopyediting = services.ServiceTypes.Copyediting;
                            record.ServiceTypeDataViz = services.ServiceTypes.DataViz;
                            record.ServiceTypeDatasetManage = services.ServiceTypes.DatasetManage;
                            record.ServiceTypeDigitization = services.ServiceTypes.Digitization;
                            record.ServiceTypeDoiAss = services.ServiceTypes.DoiAss;
                            record.ServiceTypeDoiDis = services.ServiceTypes.DoiDis;
                            record.ServiceTypeGraphicDesign = services.ServiceTypes.GraphicDesign;
                            record.ServiceTypeHostingSuppCont = services.ServiceTypes.HostingSuppCont;
                            record.ServiceTypeImageServices = services.ServiceTypes.ImageServices;
                            record.ServiceTypeIsbn = services.ServiceTypes.Isbn;
                            record.ServiceTypeIssn = services.ServiceTypes.Issn;
                            record.ServiceTypeMarketing = services.ServiceTypes.Marketing;
                            record.ServiceTypeMetadata = services.ServiceTypes.Metadata;
                            record.ServiceTypeAI = services.ServiceTypes.AI;
                            record.ServiceTypeOpenUrl = services.ServiceTypes.OpenUrl;
                            record.ServiceTypeOutreach = services.ServiceTypes.Outreach;
                            record.ServiceTypePeerReview = services.ServiceTypes.PeerReview;
                            record.ServiceTypePrintOnDemand = services.ServiceTypes.PrintOnDemand;
                            record.ServiceTypeTraining = services.ServiceTypes.Training;
                            record.ServiceTypeTypesetting = services.ServiceTypes.Typesetting;
                            record.ServiceTypeOther = services.ServiceTypes.Other;
                            record.StaffingGrads = staffing.StaffType.Grads;
                            record.StaffingUndergrads = staffing.StaffType.Undergrads;
                            record.StaffingParaprofessional = staffing.StaffType.Paraprofessional;
                            record.StaffingProfessional = staffing.StaffType.Professional;

                            records.Add(record);
                        }
                    }

                    Console.WriteLine($"Writing output to [{outputPath}]");

                    csv.WriteRecords(records);

                    Console.WriteLine($"Completed writing output file!");
                    Console.WriteLine();
                }
            }
        }
    }
}
