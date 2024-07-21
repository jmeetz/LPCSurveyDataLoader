using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryPublishingDataLoader.Types
{
    internal class Media
    {
        public static readonly string[] ValidMediaTypes = new string[] {
            "audio",
            "concept maps or other visualizations",
            "concept maps/modeling maps/visualizations",
            "data",
            "images",
            "modeling",
            "multimedia/interactive content",
            "text",
            "video"
        };
        public bool Audio { get; set; }
        public bool ConceptMaps { get; set; }
        public bool Data { get; set; }
        public bool Images { get; set; }
        public bool Modeling { get; set; }
        public bool MultimediaInteractive { get; set; }
        public bool Text { get; set; }
        public bool Video { get; set; }
        public bool Other { get; set; }
    }
}
