using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUtilityBelt.UI.Workshop
{
    class WorkshoppableAddon
    {
        [JsonProperty("title")]
        public String Title { get; set; }

        [JsonProperty("preview_image")]
        public String PreviewImage { get; set; }

        [JsonProperty("type")]
        public String[] Type { get; set; }

        [JsonProperty("tags")]
        public String[] Tags { get; set; }

        [JsonProperty("ignore")]
        public String[] Ignore { get; set; }

        [JsonIgnore]
        public String Path { get; set; }

        [JsonProperty("file_id")]
        public ulong FileId { get; set; }
    }
}
