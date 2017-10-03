namespace MobileDbBenchamark.Common
{
    public interface IStorageManager
    {
        void DeleteFile(string path);

        string GetDbPath(string path);
    }

    public class StorageManager
    {
        public static IStorageManager Instance { get; set; }
    }
}