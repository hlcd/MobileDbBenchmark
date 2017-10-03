using System;
using System.IO;
using MobileDbBenchamark.Common;
using MobileDbBenchmark.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidStorageManager))]
namespace MobileDbBenchmark.Droid
{
    public class AndroidStorageManager : IStorageManager
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