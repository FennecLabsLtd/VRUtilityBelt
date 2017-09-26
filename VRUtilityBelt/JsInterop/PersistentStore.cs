using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRUB.Utility;

namespace VRUB.JsInterop
{
    public class PersistentStore
    {
        public static readonly string StorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VRUtilityBelt\\PersistentStore");

        string _key;
        Dictionary<string, object> _temporaryStore = new Dictionary<string, object>();
        Dictionary<string, object> _persistentStore = new Dictionary<string, object>();

        public PersistentStore(string addonKey)
        {
            _key = addonKey;
            Load();
        }

        public void Set(string key, object value, bool temporary = false)
        {
            if(temporary)
            {
                _temporaryStore[key] = value;
            } else
            {
                _persistentStore[key] = value;
                Save();
            }
        }

        public object Get(string key, bool temporary = false)
        {
            return (temporary ? _temporaryStore : _persistentStore).ContainsKey(key) ? (temporary ? _temporaryStore : _persistentStore)[key] : null;
        }

        public void Clear(string key, bool temporary = false)
        {
            if (temporary && _temporaryStore.ContainsKey(key))
                _temporaryStore.Remove(key);
            else if (!temporary && _persistentStore.ContainsKey(key))
            {
                _persistentStore.Remove(key);
                Save();
            }
        }

        public void ClearAll(bool temporary = false)
        {
            if (temporary)
                _temporaryStore.Clear();
            else
            {
                _persistentStore.Clear();
                Save();
            }
        }

        public Dictionary<string, object> GetAll(bool temporary = false)
        {
            return temporary ? _temporaryStore : _persistentStore;
        }

        void Save()
        {
            if(!Directory.Exists(GetFilePath()))
            {
                Directory.CreateDirectory(StorePath);
            }

            try
            {
                File.WriteAllText(GetFilePath(), JsonConvert.SerializeObject(_persistentStore));
            } catch(Exception e)
            {
                Logger.Error("[PSTORE] Failed to write to store file at " + GetFilePath() + ": " + e.Message);
            }
        }

        void Load()
        {
            if(File.Exists(GetFilePath()))
            {
                try
                {
                    _persistentStore = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(GetFilePath()));
                } catch(Exception e)
                {
                    Logger.Error("[PSTORE] Failed to read store file at " + GetFilePath() + ": " + e.Message);
                    _persistentStore = new Dictionary<string, object>();
                }
            }

            if (_persistentStore == null)
                _persistentStore = new Dictionary<string, object>();
        }

        string GetFilePath()
        {
            return Path.Combine(StorePath, _key + ".json");
        }
    }
}
