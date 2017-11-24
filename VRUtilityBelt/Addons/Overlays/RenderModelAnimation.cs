using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using VRUB.Utility;

namespace VRUB.Addons.Overlays
{
    public class RenderModelAnimation
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("frame_pattern")]
        public string FramePattern { get; set; }

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("end")]
        public int End { get; set; }
        
        [JsonProperty("frametime")]
        public int FrameTime { get; set; }

        [JsonProperty("pingpong")]
        public bool PingPong { get; set; }

        public List<string> Frames { get; private set; }

        public void GetFrames(string BasePath)
        { 
            Frames = new List<string>();

            for(int i = Start; i <= End; i++)
            {
                string file = PathUtilities.GetTruePath(BasePath, System.IO.Path.Combine(Path, String.Format(FramePattern, i)));

                if (File.Exists(file))
                {
                    Frames.Add(file);
                }
            }
        }
    }
}
