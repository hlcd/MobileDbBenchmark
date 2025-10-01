using System;
using System.IO;
using MobileDbBenchamark.Common;

namespace MobileDbBenchmark.iOS
{
    public class IOSStorageManager : IStorageManager
    {
        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public string GetDbPath(string path)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), path);
        }
    }
}