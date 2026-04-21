using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace CynologicalCenter.Helpers
{
    public static class PhotoHelper
    {
        private static readonly string Root =
            ConfigurationManager.AppSettings["PhotosRootPath"] ?? AppDomain.CurrentDomain.BaseDirectory + "Photos\\";

        public static readonly string DefaultDog =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "dog_placeholder.png");

        public static readonly string DefaultTrainer =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "trainer_placeholder.png");

        public static string GetFullPath(string? relativePath, string defaultPath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return defaultPath;
            string full = Path.Combine(Root, relativePath);
            return File.Exists(full) ? full : defaultPath;
        }

        public static string SaveDogPhoto(int dogId, string sourceFilePath)
            => SavePhoto(dogId, sourceFilePath, "dogs", "dog");

        public static string SaveTrainerPhoto(int trainerId, string sourceFilePath)
            => SavePhoto(trainerId, sourceFilePath, "trainers", "trainer");

        public static void DeletePhoto(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            string full = Path.Combine(Root, relativePath);
            if (File.Exists(full)) File.Delete(full);
        }

        private static string SavePhoto(int id, string src, string folder, string prefix)
        {
            string ext = Path.GetExtension(src).ToLower();
            string fileName = $"{prefix}_{id}{ext}";
            string relativePath = Path.Combine(folder, fileName);
            string destPath = Path.Combine(Root, relativePath);

            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
            File.Copy(src, destPath, overwrite: true);

            return relativePath;
        }
    }
}
