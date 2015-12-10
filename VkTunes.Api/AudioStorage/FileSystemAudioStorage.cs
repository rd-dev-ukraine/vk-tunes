using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public Task<Dictionary<int, LocalAudioRecord>> Load()
        {
            return Task.Run(() =>
            {
                var files = Directory.GetFiles(storageFolder);
                var result = new Dictionary<int, LocalAudioRecord>();

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var m = extractAudioId.Match(file);
                    if (m.Success)
                    {
                        var audioId = Int32.Parse(m.Captures[0].Value);

                        result[audioId] = new LocalAudioRecord
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

        public string GenerateFileName(int audioId, string artist, string title)
        {
            artist = SanitizeFileName(artist);
            title = SanitizeFileName(title);
            return $"{artist} - {title}.{audioId}.mp3";
        }

        public Stream OpenSave(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            var path = Path.Combine(storageFolder, fileName);
            return File.OpenWrite(path);
        }

        private static string SanitizeFileName(string value)
        {
            return Path.GetInvalidFileNameChars()
                       .Aggregate(value, (current, ch) => current.Replace(ch.ToString(), String.Empty));
        }
    }
}