using SQLite;

namespace MobileDbBenchamark.Common.Models.Sqlite
{
    public class Publication: IPublication
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Title { get; set; }

        public string CoverUrl { get; set; }

        public int Version { get; set; }

        [Ignore]
        public bool HasCover => string.IsNullOrEmpty(CoverUrl) == false;
    }
}