using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VkTunes.Api.AudioStorage
{
    public class FileSystemAudioStorage : IVkAudioFileStorage
    {
        private readonly string storageFolder;
        private readonly Regex extractAudioId;

        public FileSystemAudioStorage(Configuration configuration)
        {
            storageFolder = configuration.AudioFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            extractAudioId = new Regex(@"\.(\d{1,})\.\w*$");
        }

        public Task<Dictionary<int,  StoredAudioRecord>> Load()
        {
            return Task.Run(() =>
            {
                var files = Directory.GetFiles(storageFolder);
                var result = new Dictionary<int, StoredAudioRecord>();

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var m = extractAudioId.Match(file);
                    if (m.Success)
                    {
                        var audioId = Int32.Parse( m.Captures[0].Value);

                        result[audioId] = new StoredAudioRecord
                        {
                            Name = fileName,
                            FilePath = file,
                            Id = audioId
                        };
                    }
                }

                return result;
            });
        }
    }
}