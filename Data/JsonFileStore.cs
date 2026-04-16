using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SchoolMangementSystem.Data
{
    internal static class JsonFileStore
    {
        public static List<T> LoadList<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            using (var stream = File.OpenRead(filePath))
            {
                if (stream.Length == 0)
                {
                    return new List<T>();
                }

                var serializer = new DataContractJsonSerializer(typeof(List<T>));
                return (List<T>)serializer.ReadObject(stream);
            }
        }

        public static void SaveList<T>(string filePath, List<T> items)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = File.Create(filePath))
            {
                var serializer = new DataContractJsonSerializer(typeof(List<T>));
                serializer.WriteObject(stream, items);
            }
        }

        public static string SaveImageCopy(string sourcePath, string targetDirectory, string targetPrefix)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
            {
                return string.Empty;
            }

            Directory.CreateDirectory(targetDirectory);
            var extension = Path.GetExtension(sourcePath);
            var safePrefix = string.IsNullOrWhiteSpace(targetPrefix) ? "record" : targetPrefix;
            var fileName = safePrefix + "_" + System.Guid.NewGuid().ToString("N") + extension;
            var targetPath = Path.Combine(targetDirectory, fileName);

            File.Copy(sourcePath, targetPath, true);
            return targetPath;
        }

        public static string HashText(string input)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input ?? string.Empty);
                var hash = sha.ComputeHash(bytes);
                var builder = new StringBuilder();

                foreach (var b in hash)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
