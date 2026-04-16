using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace aydocs.NotchWin.Utils
{
    internal class SaveManager
    {
        private static Dictionary<string, object> data = new Dictionary<string, object>();
        public static Dictionary<string, object> SaveData { get { return data; } set => data = value; }

        public static string SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "aydocs.NotchWin");
        static string fileName = "Settings.json";

        static string cachedJsonSave = "";

        public static void LoadData()
        {
            System.Diagnostics.Debug.WriteLine(SavePath);

            if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);

            var fullPath = Path.Combine(SavePath, fileName);

            if (!File.Exists(fullPath))
            {
                var fs = File.Create(fullPath);
                fs.Close();
                File.WriteAllText(fullPath, JsonConvert.SerializeObject(new Dictionary<string, object>()));
            }

            var json = File.ReadAllText(fullPath);
            cachedJsonSave = json;

            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public static void SaveAll()
        {
            if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);

            var fullPath = Path.Combine(SavePath, fileName);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            if (!File.Exists(fullPath))
                File.Create(fullPath);

            File.WriteAllText(fullPath, json);
        }

        public static void Add(string key, object value)
        {
            if (!Contains(key))
                data.Add(key, value);
            else
                data[key] = value;
        }

        public static void Remove(string key)
        {
            if (Contains(key))
                data.Remove(key);
        }

        public static object Get(string key)
        {
            if (Contains(key))
                return data[key];
            else
                return null;
        }

        public static T GetValue<T>(string key, T defaultValue = default)
        {
            if (!Contains(key)) return defaultValue;

            try
            {
                var val = data[key];
                if (val is T result) return result;

                // Handle Newtonsoft types (like JArray, JObject, or numbers coming in as Int64)
                if (val is Newtonsoft.Json.Linq.JToken token)
                {
                    return token.ToObject<T>();
                }

                // Generic conversion for other types (e.g., Int64 to Int32)
                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T Get<T>(string key)
        {
            return GetValue<T>(key);
        }

        public static bool Contains(string key)
        {
            return data.ContainsKey(key);
        }
    }
}
