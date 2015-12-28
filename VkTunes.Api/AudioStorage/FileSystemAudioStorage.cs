using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using VkTunes.Api.Api;

namespace VkTunes.Api.AudioStorage
{
    public class FileSystemAudioStorage : IVkAudioFileStorage
    {
        private readonly string storageFolder;
        private readonly Regex extractAudioId;

        public FileSystemAudioStorage(Configuration configuration)
        {
            storageFolder = configuration.AudioFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            extractAudioId = new Regex(@"\.(\d{1,})_(\d{1,})\.\w*$");
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
                        var ownerId = Int32.Parse(m.Groups[1].Value);
                        var audioId = Int32.Parse(m.Groups[2].Value);

                        result[audioId] = new LocalAudioRecord
                        {
                            Id = audioId,
                            OwnerId = ownerId,
                            Name = fileName,
                            FilePath = file
                        };
                    }
                }

                return result;
            });
        }

        public async Task<LocalAudioRecord> Save(Stream source, RemoteAudioRecord audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            var fileName = GenerateFileName(audio);
            var path = Path.Combine(storageFolder, fileName);

            source.Position = 0;
            using (var file = File.OpenWrite(path))
            {
                await source.CopyToAsync(file);
                file.Close();
            }

            return new LocalAudioRecord
            {
                Id = audio.Id,
                FilePath = path,
                Name = fileName
            };
        }

        private string GenerateFileName(RemoteAudioRecord audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            var artist = SanitizeFileName(audio.Artist);
            var title = SanitizeFileName(audio.Title);
            return $"{artist} - {title}.{audio.Owner}_{audio.Id}.mp3";
        }

        private static string SanitizeFileName(string value)
        {
            return Path.GetInvalidFileNameChars()
                       .Aggregate(value, (current, ch) => current.Replace(ch.ToString(), String.Empty));
        }
    }
}