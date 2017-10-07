using SQLite;

namespace MobileDbBenchamark.Common.Models.Sqlite
{
    public class Publication: IPublication
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Title { get; set; }

        public string CoverUrl { get; set; }

        [Indexed]
        public int RemoteId { get; set; }

        public int Version { get; set; }

        [Ignore]
        public bool HasCover => string.IsNullOrEmpty(CoverUrl) == false;

        public int DownloadPercentage { get; set; }
    }

    public class PublicationCollection : IPublicationCollection
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Name { get; set; }

        [Indexed]
        public int RemoteId { get; set; }
    }

    public class PublicationCollectionLink
    {
        [PrimaryKey, AutoIncrement]
        public  int Id { get; set; }

        [Indexed]
        public string PublicationId { get; set; }

        [Indexed]
        public string CollectionId { get; set; }
    }
}