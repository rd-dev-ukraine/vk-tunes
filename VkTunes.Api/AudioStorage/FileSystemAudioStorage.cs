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
                        var audioId = Int32.Parse(m.Groups[1].Value);

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

        public async Task<LocalAudioRecord> Save(Stream source, RemoteAudioRecord audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            var fileName = GenerateFileName(audio.Id, audio.Artist, audio.Title);
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

        private string GenerateFileName(int audioId, string artist, string title)
        {
            artist = SanitizeFileName(artist);
            title = SanitizeFileName(title);
            return $"{artist} - {title}.{audioId}.mp3";
        }

        private static string SanitizeFileName(string value)
        {
            return Path.GetInvalidFileNameChars()
                       .Aggregate(value, (current, ch) => current.Replace(ch.ToString(), String.Empty));
        }
    }
}