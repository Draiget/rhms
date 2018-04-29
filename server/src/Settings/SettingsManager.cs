using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using server.Modules.Base;
using server.Utils.Logging;

namespace server.Settings
{
    public static class SettingsManager
    {
        public const string LookupDirectory = "configs";

        static SettingsManager() {
            Initialize();
        }

        private static void Initialize() {
            if (!Directory.Exists(LookupDirectory)) {
                Directory.CreateDirectory(LookupDirectory);
            }
        }

        public static SettingsIoResult UpdateOrCreate(BaseSettings settings) {
            var fileName = $"{LookupDirectory}//{settings.FileName}.json";
            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                var result = new SettingsIoResult {
                    IsOk = false,
                    ErrorMessage = "Unknown"
                };

                WriteToDisk(fs, settings, ref result);
                return result;
            }
        }

        private static void WriteToDisk(FileStream fs, BaseSettings settings, ref SettingsIoResult result) {
            try {
                var data = SerializeSettings(settings);
                using (var sw = new StreamWriter(fs)) {
                    sw.Write(data);
                    result.IsOk = true;
                }
            } catch (Exception e) {
                result.IsOk = false;
                result.Error = e;
                result.ErrorMessage = e.Message;
            }
        }

        public static SettingsIoResult LoadFromDisk<T>(ref T type) where T : BaseSettings {
            var result = new SettingsIoResult();
            var fileName = $"{LookupDirectory}//{type.FileName}.json";
            try {
                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                    if (fs.Length == 0) {
                        Logger.Debug($"Create default settings file {type.FileName}");
                        WriteToDisk(fs, type.Default, ref result);
                        return result;
                    }

                    using (var sr = new StreamReader(fs)) {
                        var data = sr.ReadToEnd();
                        type = (T)JsonConvert.DeserializeObject(data, type.GetType());
                        result.IsOk = true;
                    }
                }
            } catch (Exception e) {
                result.IsOk = false;
                result.Error = e;
                result.ErrorMessage = e.Message;
            }

            return result;
        }

        private static string SerializeSettings(BaseSettings settings) {
            var jsonSerializerSettings = new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All
            };

            return JsonConvert.SerializeObject(settings, Formatting.Indented, jsonSerializerSettings);
        }

        public static string FixFileName(string value) {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(value, invalidRegStr, "_");
        }
    }
}
