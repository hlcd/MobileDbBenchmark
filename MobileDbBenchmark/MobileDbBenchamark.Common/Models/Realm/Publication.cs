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

        public bool HasCover => string.IsNullOrEmpty(CoverUrl) == false;

        //[Backlink(nameof(PublicationCollection.Publications))]
        //public IQueryable<PublicationCollection> Collections { get; }
    }

    //public class PublicationCollection : RealmObject, IPublicationCollection
    //{
    //    public string Id { get; set; }

    //    public string Nane { get; set; }

    //    //public IList<Publication> Publications { get; }
    //}
}