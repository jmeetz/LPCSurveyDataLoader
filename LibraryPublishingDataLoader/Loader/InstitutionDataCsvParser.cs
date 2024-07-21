using LibraryPublishingDataLoader.Types;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LibraryPublishingDataLoader.Loader
{
    internal class InstitutionDataCsvParser
    {
        public string InputFilePath { get; set; }
        public string OutputData { get; set; }
        public string SurveyYear { get; set; }

        public void ParseCsvData(Dictionary<string, Institution> institutions)
        {
            using (StreamReader sr = new StreamReader(InputFilePath))
            {
                using (CsvReader csv = new CsvReader(sr))
                {
                    TransformData(csv, institutions);
                }
            }
        }

        private void TransformData(CsvReader csv, Dictionary<string, Institution> institutions)
        {
            while (csv.ReadNextRecord())
            {
                string instName = ParseField(csv, "institution_name");
                string libName = ParseField(csv, "library_name");
                string unitName = ParseField(csv, "unit_name");

                //Get or Create institution record
                if (!institutions.ContainsKey(instName))
                {
                    institutions.Add(instName, new Institution());
                }

                Institution inst = institutions[instName];

                //Create key for this institution's response(s) for the given survey year
                //Multiple reponses for an insitution will be broken down by library/unit name
                string libKey = $"{SurveyYear}||{libName}";
                if (inst.Data.ContainsKey(libKey))
                {
                    libKey = $"{SurveyYear}||{unitName}";
                }

                //Parse survey data for this institution and year
                inst.Data.Add(libKey, ParseInstitutionData(csv, libName, unitName));

                //Parse all services data
                inst.Services.Add(libKey, ParseServicesData(csv));

                //Parse all staffing data
                inst.Staffing.Add(libKey, ParseStaffingData(csv));
            }
        }

        private InstitutionData ParseInstitutionData(CsvReader csv, string libName, string unitName)
        {
            string instType = ParseField(csv, "institution_type");
            string country = ParseField(csv, "location");
            string est = ParseField(csv, "year_est");
            string orgType = ParseField(csv, "org");

            return new InstitutionData
            {
                LibraryName = libName,
                UnitName = unitName,
                Type = instType,
                Country = country,
                YearEstablished = est,
                ProgramType = orgType
            };
        }

        private Services ParseServicesData(CsvReader csv)
        {
            Services service = new Services();

            //Parse media fields
            service.Media = ParseMediaData(csv);

            //Parse partner fields
            service.Partners = ParsePartnersData(csv);

            //Parse publication types fields
            service.PublicationTypes = ParsePubTypesData(csv);

            //Parse service types
            service.ServiceTypes = ParseServiceTypesData(csv);

            return service;
        }

        private Staffing ParseStaffingData(CsvReader csv)
        {
            Staffing staffing = new Staffing();

            //Parse staffing data
            staffing.StaffType = ParseStaffTypesData(csv);

            return staffing;
        }

        private Media ParseMediaData(CsvReader csv)
        {
            //Parse media fields
            string mediaFormats = ParseField(csv, "media_formats");
            string mediaFormatsOther = ParseField(csv, "media_formats_other");

            string[] mediaTypesComps = mediaFormats.Split(new char[] { '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> mediaTypes = new List<string>();
            foreach (string mt in mediaTypesComps)
            {
                mediaTypes.Add(mt.Trim());
            }

            bool mediaOtherExists = !string.IsNullOrWhiteSpace(mediaFormatsOther) ||
                                    mediaTypes.Any(mt => !Media.ValidMediaTypes.Contains(mt));

            return new Media
            {
                Audio = mediaTypes.Contains("audio"),
                ConceptMaps = mediaTypes.Contains("concept maps or other visualizations") ||
                              mediaTypes.Contains("concept maps/modeling maps/visualizations"),
                Data = mediaTypes.Contains("data"),
                Images = mediaTypes.Contains("images"),
                Modeling = mediaTypes.Contains("modeling") ||
                           mediaTypes.Contains("concept maps/modeling maps/visualizations"),
                MultimediaInteractive = mediaTypes.Contains("multimedia/interactive content"),
                Text = mediaTypes.Contains("text"),
                Video = mediaTypes.Contains("video"),
                Other = mediaOtherExists
            };
        }

        private Partners ParsePartnersData(CsvReader csv)
        {
            //Parse partner fields
            string partnersInternal = ParseField(csv, "partners_internal").Trim();
            string partnersCampus = ParseField(csv, "partners_campus").Trim();
            string partnersConsortia = ParseField(csv, "partners_consortia").Trim();
            string partnersConsortiaSupport = ParseField(csv, "partners_consortia_support").Trim();
            string partnersConsortiaTF = ParseField(csv, "partners_consortia_tf").Trim();
            string partnersPreference = ParseField(csv, "partners_preference").Trim();

            string[] partnersCampusComps = partnersCampus.Split(new char[] { '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> partnersCampusValues = new List<string>();
            foreach (string pc in partnersCampusComps)
            {
                partnersCampusValues.Add(pc.Trim().ToLower());
            }

            bool partnersConsortiaExists = !string.IsNullOrWhiteSpace(partnersConsortia) ||
                                           !string.IsNullOrWhiteSpace(partnersConsortiaSupport) ||
                                           !string.IsNullOrWhiteSpace(partnersConsortiaTF);

            return new Partners
            {
                Consortia = partnersConsortiaExists,
                Departments = partnersCampusValues.Contains("campus departments or programs") ||
                              partnersInternal == "campus departments or programs",
                Faculty = partnersCampusValues.Contains("individual faculty") ||
                          partnersInternal == "individual faculty",
                GradStudents = partnersCampusValues.Contains("graduate students") ||
                               partnersInternal == "graduate students",
                Preference = partnersPreference,
                Undergrads = partnersCampusValues.Contains("undergraduate students") ||
                             partnersInternal == "undergraduate students"
            };
        }
        private PublicationTypes ParsePubTypesData(CsvReader csv)
        {
            PublicationTypes pubType = new PublicationTypes();

            string pubTypeCount = ParseField(csv, "pub_types_count");

            if (!string.IsNullOrEmpty(pubTypeCount))
            {
                //Parse the pub types from single field
                string[] pubTypeCounts = pubTypeCount.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (string pubTypeStr in pubTypeCounts)
                {
                    string pubTypeStrTrim = pubTypeStr.Trim();
                    string[] pubTypeComp = pubTypeStrTrim.Split('(', StringSplitOptions.RemoveEmptyEntries);
                    int count = 0;
                    string type = string.Empty;

                    if (pubTypeComp.Length == 0)
                    {
                        Console.WriteLine($"Detected pub type with zero components (ignore): '{pubTypeStrTrim}'");
                        continue;
                    }
                    else if (pubTypeComp.Length == 1)
                    {
                        type = pubTypeComp[0].Trim().ToLower();
                        count = -1;
                        Console.WriteLine($"Detected pub type with no count: '{type}'");
                    }
                    else if (pubTypeComp.Length > 2)
                    {
                        Console.WriteLine($"Detected pub type with too many components (ignore): '{pubTypeStrTrim}'");
                        continue;
                    }
                    else
                    {
                        type = pubTypeComp[0].Trim().ToLower();
                        string countStr = pubTypeComp[1].Replace(")", string.Empty)
                                                        .Replace(",", string.Empty)
                                                        .Trim();

                        if (!int.TryParse(countStr, out count))
                        {
                            Console.WriteLine($"Failed to parse '{countStr}' as a number");
                            continue;
                        }
                    }

                    switch (type)
                    {
                        case "faculty-driven journals": case "campus-based faculty-driven journals": case "campus-based faculty driven journals": pubType.FacultyJournals[PubTypeCategory.General] = count; break;
                        case "student-driven journals": case "campus-based student-driven journals": case "campus-based student driven journals": pubType.StudentJournals[PubTypeCategory.General] = count; break;
                        case "journals produced under contract/mou for external groups": pubType.ExternalJournals[PubTypeCategory.General] = count; break;
                        case "monographs": pubType.Monographs[PubTypeCategory.General] = count; break;
                        case "technical/research reports": pubType.Reports[PubTypeCategory.General] = count; break;
                        case "faculty conference papers and proceedings": pubType.FacultyConference[PubTypeCategory.General] = count; break;
                        case "student conference papers and proceedings": pubType.StudentConference[PubTypeCategory.General] = count; break;
                        case "etds": pubType.ETDs[PubTypeCategory.General] = count; break;
                        case "undergraduate capstone/honors theses": case "undergraduate capstone/ honors theses": case "undergraduate capstones/honors theses": case "undergraduate capstones/theses": pubType.UndergradETDs[PubTypeCategory.General] = count; break;
                        case "textbooks": pubType.Textbooks[PubTypeCategory.General] = count; break;
                        case "newsletters": pubType.Newsletters[PubTypeCategory.General] = count; break;
                        case "databases": pubType.Databases[PubTypeCategory.General] = count; break;
                        case "datasets": pubType.Datasets[PubTypeCategory.General] = count; break;
                        default: Console.WriteLine($"Unknown pub type (treat as [other]): '{type}'"); pubType.Other[PubTypeCategory.General]++; break;
                    }
                }
            }
            else
            {
                //Parse individual pub type fields
                foreach (string category in new string [] { "types", "oa", "paid", "hybrid", "up"})
                {
                    string ptHeader = $"pubs_{category}_";

                    int backissues = ParseFieldInt(csv, $"{ptHeader}backissues", 0);
                    int cbfdj = ParseFieldInt(csv, $"{ptHeader}cbfdj", 0);
                    int cbsdj = ParseFieldInt(csv, $"{ptHeader}cbsdj", 0);
                    int data = ParseFieldInt(csv, $"{ptHeader}data", 0);
                    int datasets = ParseFieldInt(csv, $"{ptHeader}datasets", 0);
                    int etds = ParseFieldInt(csv, $"{ptHeader}etds", 0);
                    int external = ParseFieldInt(csv, $"{ptHeader}external", 0);
                    int fcpp = ParseFieldInt(csv, $"{ptHeader}fcpp", 0);
                    int monographs = ParseFieldInt(csv, $"{ptHeader}monographs", 0);
                    int newsletters = ParseFieldInt(csv, $"{ptHeader}newsletters", 0);
                    int other = ParseFieldInt(csv, $"{ptHeader}other", 0);
                    int reports = ParseFieldInt(csv, $"{ptHeader}reports", 0);
                    int scpp = ParseFieldInt(csv, $"{ptHeader}scpp", 0);
                    int textbooks = ParseFieldInt(csv, $"{ptHeader}textbooks", 0);
                    int theses = ParseFieldInt(csv, $"{ptHeader}theses", 0);

                    PubTypeCategory ptCat = category switch
                    {
                        "types" => PubTypeCategory.General,
                        "oa" => PubTypeCategory.OpenAccess,
                        "paid" => PubTypeCategory.Paid,
                        "hybrid" => PubTypeCategory.Hybrid,
                        "up" => PubTypeCategory.UniPress,
                        _ => throw new ArgumentOutOfRangeException(nameof(category), $"Unable to map PubType category {category}")
                    };

                    pubType.FacultyJournals[ptCat] = cbfdj;
                    pubType.StudentJournals[ptCat] = cbsdj;
                    pubType.Datasets[ptCat] = data;
                    pubType.Databases[ptCat] = datasets;
                    pubType.ETDs[ptCat] = etds;
                    pubType.ExternalJournals[ptCat] = external;
                    pubType.FacultyConference[ptCat] = fcpp;
                    pubType.Monographs[ptCat] = monographs;
                    pubType.Newsletters[ptCat] = newsletters;
                    pubType.Other[ptCat] = other;
                    pubType.Reports[ptCat] = reports;
                    pubType.StudentConference[ptCat] = scpp;
                    pubType.Textbooks[ptCat] = textbooks;
                    pubType.UndergradETDs[ptCat] = theses;
                }
            }

            return pubType;
        }

        private ServiceTypes ParseServiceTypesData(CsvReader csv)
        {
            string svcs = ParseField(csv, "svcs");
            string svcsOther = ParseField(csv, "svcs_other");
            bool ai = false, analytics = false, applyingCatPubData = false, avStreaming = false, authorAdvCopyright = false, authorAdvOther = false, budgetPrep = false,
                 businessModelDev = false, cataloging = false, compilingIndexesToc = false, contractLicensePrep = false, copyediting = false, datasetManage = false, dataViz = false, digitization = false,
                 doiAss = false, doiDis = false, graphicDesign = false, hostingSuppCont = false, imageServices = false, isbn = false, issn = false, marketing = false, metadata = false,
                 openUrl = false, outreach = false, peerReview = false, printOnDemand = false, training = false, typesetting = false;

            string[] svcsComps = svcs.Split(new char[] { '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> svcsValues = new List<string>();
            foreach (string svc in svcsComps)
            {
                svcsValues.Add(svc.Trim().ToLower());
            }

            bool svcsOtherExists = !string.IsNullOrWhiteSpace(svcsOther);

            foreach (string val in svcsValues)
            {
                switch (val)
                {
                    case "notification of a&i sources": ai = true; break;
                    case "analytics": analytics = true; break;
                    case "applying for cataloging in publication data": applyingCatPubData = true; break;
                    case "audio/video streaming": case "audio/ video streaming": avStreaming = true; break;
                    case "author copyright advisory": case "author advisory - copyright": case "copyright advisory": authorAdvCopyright = true; break;
                    case "author other advisory": case "author advisory - other": case "other advisory": case "other author advisory": authorAdvOther =true; break;
                    case "budget preparation": budgetPrep = true; break;
                    case "business model development": businessModelDev = true; break;
                    case "cataloging": case "cataloguing": cataloging = true; break;
                    case "compiling indexes and/or tocs": case "tocs": compilingIndexesToc = true; break;
                    case "contract/license preparation": contractLicensePrep = true; break;
                    case "copyediting": case "copy-editing": copyediting = true; break;
                    case "dataset management": case "data management": datasetManage = true; break;
                    case "data visualization": dataViz = true; break;
                    case "digitization": digitization = true; break;
                    case "doi assignment/allocation of identifiers": case "doi assignment/ allocation of identifiers": case "doi assignment": case "doi registration": case "dois": doiAss = true; break;
                    case "doi distribution": doiDis = true; break;
                    case "graphic design (print or web)": graphicDesign = true; break;
                    case "hosting of supplemental content": case "hosting supplemental content": hostingSuppCont = true; break;
                    case "image services": imageServices = true; break;
                    case "isbn registration": case "isbn registry": case "isbns": isbn = true; break;
                    case "issn registration": case "issn registry": issn = true; break;
                    case "marketing": marketing = true; break;
                    case "metadata": metadata = true; break;
                    case "open url support": openUrl = true; break;
                    case "outreach": outreach = true; break;
                    case "peer review management": peerReview = true; break;
                    case "print-on-demand": printOnDemand = true; break;
                    case "training": training = true; break;
                    case "typesetting": typesetting = true; break;
                    default: Console.WriteLine($"Unknown service type (treat as other): '{val}'"); svcsOtherExists = true; break;
                };
            }

            return new ServiceTypes
            {
                AI = ai,
                Analytics = analytics,
                ApplyingCatPubData = applyingCatPubData,
                AudioVideoStreaming = avStreaming,
                AuthorAdvisoryCopyright = authorAdvCopyright,
                AuthorAdvisoryOther = authorAdvOther,
                BudgetPreparation = budgetPrep,
                BusinessModelDev = businessModelDev,
                Cataloging = cataloging,
                CompilingIndexesToc = compilingIndexesToc,
                ContractLicensePrep = contractLicensePrep,
                Copyediting = copyediting,
                DatasetManage = datasetManage,
                DataViz = dataViz,
                Digitization = digitization,
                DoiAss = doiAss,
                DoiDis = doiDis,
                GraphicDesign = graphicDesign,
                HostingSuppCont = hostingSuppCont,
                ImageServices = imageServices,
                Isbn = isbn,
                Issn = issn,
                Marketing = marketing,
                Metadata = metadata,
                OpenUrl = openUrl,
                Other = svcsOtherExists,
                Outreach = outreach,
                PeerReview = peerReview,
                PrintOnDemand = printOnDemand,
                Training = training,
                Typesetting = typesetting
            };
        }

        private StaffType ParseStaffTypesData(CsvReader csv)
        {
            StaffType staffType = new StaffType();

            string staffTypeCount = ParseField(csv, "staff");

            if (!string.IsNullOrEmpty(staffTypeCount))
            {
                //Parse the pub types from single field
                string[] staffTypeCounts = staffTypeCount.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (string staffTypeStr in staffTypeCounts)
                {
                    string staffTypeStrTrim = staffTypeStr.Trim();
                    string[] staffTypeComp = staffTypeStrTrim.Split('(', StringSplitOptions.RemoveEmptyEntries);
                    double count = 0.0;
                    string type = string.Empty;

                    if (staffTypeComp.Length == 0)
                    {
                        Console.WriteLine($"Detected staff type with zero components (ignore): '{staffTypeStrTrim}'");
                        continue;
                    }
                    else if (staffTypeComp.Length == 1)
                    {
                        type = staffTypeComp[0].Trim().ToLower();
                        count = -1.0;
                        Console.WriteLine($"Detected staff type with no count: '{type}'");
                    }
                    else if (staffTypeComp.Length > 2)
                    {
                        Console.WriteLine($"Detected staff type with too many components (ignore): '{staffTypeStrTrim}'");
                        continue;
                    }
                    else
                    {
                        type = staffTypeComp[0].Trim().ToLower();
                        string countStr = staffTypeComp[1].Replace(")", string.Empty)
                                                          .Replace(",", string.Empty)
                                                          .Trim();

                        if (!double.TryParse(countStr, out count))
                        {
                            Console.WriteLine($"Failed to parse '{countStr}' as a number");
                            continue;
                        }
                    }

                    switch (type)
                    {
                        case "library staff": case "professional staff": case "librarian": staffType.Professional = count; break;
                        case "paraprofessional staff": staffType.Paraprofessional = count; break;
                        case "graduate students": staffType.Grads = count; break;
                        case "undergraduate students": case "undergraduates": staffType.Undergrads = count; break;
                        default: Console.WriteLine($"Unknown staff type: '{type}'"); break;
                    }
                }
            }
            else
            {
                string stHeader = $"staff_";

                double grads = ParseFieldDouble(csv, $"{stHeader}grads", 0.0);
                double paraProf = ParseFieldDouble(csv, $"{stHeader}paraprofessional", 0.0);
                double prof = ParseFieldDouble(csv, $"{stHeader}professional", 0.0);
                double undergrads = ParseFieldDouble(csv, $"{stHeader}undergrads", 0.0);

                staffType.Grads = grads;
                staffType.Undergrads = undergrads;
                staffType.Paraprofessional = paraProf;
                staffType.Professional = prof;
            }

            return staffType;
        }

        #region Utilities
        private string ParseField(CsvReader row, string fieldName)
        {
            try
            {
                return row[fieldName];
            }
            catch (ArgumentException aex)
            {
                if (aex.Message.Contains("field header not found"))
                {
                    return string.Empty;
                }

                throw aex;
            }
        }

        private int ParseFieldInt(CsvReader row, string fieldName, int def = 0)
        {
            string val = ParseField(row, fieldName);

            if (!int.TryParse(val, out int result))
            {
                result = def;
            }

            return result;
        }

        private double ParseFieldDouble(CsvReader row, string fieldName, double def = 0.0)
        {
            string val = ParseField(row, fieldName);

            if (!double.TryParse(val, out double result))
            {
                result = def;
            }

            return result;
        }
        #endregion
    }
}
