using System.Collections.Generic;
using System.Linq;
using Realms;

namespace MobileDbBenchamark.Common.Models.Realm
{
    public class Publication : RealmObject, IPublication
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Title { get; set; }

        public string CoverUrl { get; set; }

        public int Version { get; set; }

        [Indexed]
        public int RemoteId { get; set; }

        public bool HasCover => string.IsNullOrEmpty(CoverUrl) == false;

        public int DownloadPercentage { get; set; }

        public string SomeDescription { get; set; }

        [Backlink(nameof(PublicationCollection.Publications))]
        public IQueryable<PublicationCollection> Collections { get; }
    }

    public class PublicationCollection : RealmObject, IPublicationCollection
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string Name { get; set; }

        [Indexed]
        public int RemoteId { get; set; }

        public IList<Publication> Publications { get; }

        public int PublicationsCount => Publications.Count;

        public string CoverUrl => Publications.FirstOrDefault(x => x.CoverUrl != null)?.CoverUrl;

        public bool HasCover => CoverUrl != null;
    }
}