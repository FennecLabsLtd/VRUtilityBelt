using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRUB.Addons.Data
{
    public class InjectableFiles
    {
        [JsonProperty("js")]
        public List<string> JS = null;

        [JsonProperty("css")]
        public List<string> CSS = null;
    }
}
