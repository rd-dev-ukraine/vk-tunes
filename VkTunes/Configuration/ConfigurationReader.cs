using System;
using System.IO;
using System.Reflection;

using Newtonsoft.Json;

namespace VkTunes.Configuration
{
    public class ConfigurationReader
    {
        public ApplicationConfiguration Read()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var configFileUri = new Uri(Path.Combine(path, "config.json"));

            var configFileName = configFileUri.LocalPath;

            if (!File.Exists(configFileName))
                return new ApplicationConfiguration();

            var data = File.ReadAllText(configFileName);
            return JsonConvert.DeserializeObject<ApplicationConfiguration>(data);
        }
    }
}