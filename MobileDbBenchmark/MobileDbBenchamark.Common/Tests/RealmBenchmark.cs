using System;
using System.Diagnostics;
using System.Linq;
using MobileDbBenchamark.Common.Models.Realm;
using Realms;

namespace MobileDbBenchamark.Common.Tests
{
    public abstract class BenchmarkBase
    {
        internal TimeSpan PerformTest(Action<BenchmarkBase> test)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            test(this);
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        public abstract IDisposable OpenDB();

        public abstract void DeleteDB();

        public abstract void RunInTransaction(Action action);

        public abstract void InsertPublication(int index);

        public abstract void InsertCollection(int index, int itemsInCollection, int publicationsCount);

        protected static string PublicationTitle(int index)
        {
            return $"Book {index}";
        }

        protected static string PublicationCoverUrl(int index)
        {
            return index % 2 == 0 ? "https://dummyimage.com/600x400/000/00ffd5.png" : null;
        }

        protected static int PublicationVersion(int index)
        {
            return index % 2 == 0 ? 2 : 1;
        }

        protected static string CollectionName(int index) => $"Collection {index}";


        public abstract int PublicationCount();

        public abstract int EnumeratePublications();

        public abstract int UpdatePublicationsInManyTransactions();

        public abstract int UpdatePublicationsInSingleTransaction();

        public abstract int DeletePublicationsInManyTransactions();

        public abstract void DeletePublicationsInSingleTransaction();

        public abstract int SelectCollections();

    }

    public class RealmBenchmark : BenchmarkBase
    {

        private Realm _realm;

        private readonly RealmConfiguration _config = new RealmConfiguration("test.realm")
        {
#if DEBUG
            ShouldDeleteIfMigrationNeeded = true
#endif
        };

        public override IDisposable OpenDB()
        {
            return _realm = Realm.GetInstance(_config);
        }

        public override void DeleteDB()
        {
            Realm.DeleteRealm(_config);
        }


        public override void RunInTransaction(Action action)
        {
            _realm.Write(action);
        }

        public override void InsertPublication(int index)
        {
            _realm.Add(new Publication()
            {
                Id = Guid.NewGuid().ToString(),
                CoverUrl = PublicationCoverUrl(index),
                Title = PublicationTitle(index),
                Version = PublicationVersion(index),
                RemoteId = index
            });
        }

        public override void InsertCollection(int index, int itemsInCollection, int publicationsCount)
        {
            var collection = new PublicationCollection()
            {
                Id = Guid.NewGuid().ToString(),
                Name = CollectionName(index),
                RemoteId = index
            };

            _realm.Add(collection);

            var lowerBound = (index * itemsInCollection) % publicationsCount;
            var upperBound = lowerBound + itemsInCollection;//((index + 1) * itemsInCollection) % publicationsCount;

            var publications = _realm.All<Publication>()
                .Where(x => x.RemoteId >= lowerBound && x.RemoteId < upperBound)
                .ToList();

            foreach (var publication in publications)
            {
                collection.Publications.Add(publication);
            }
        }

        public override int PublicationCount()
        {
            return _realm.All<Publication>().Count();

        }

        public override int EnumeratePublications()
        {
            var total = 0;
            foreach (var publication in _realm.All<Publication>())
            {
                total += publication.Version;
            }

            return total;
        }

        public override int UpdatePublicationsInManyTransactions()
        {
            var total = 0;
            var publications = _realm.All<Publication>().Where(x => x.Version == 1).ToList();
            foreach (var publication in publications)
            {
                _realm.Write(() =>
                {
                    publication.Version = 2;
                });
                total++;
            }

            return total;
        }

        public override int UpdatePublicationsInSingleTransaction()
        {
            var total = 0;
            var publications = _realm.All<Publication>().Where(x => x.Version == 1).ToList();
            _realm.Write(() =>
            {
                foreach (var publication in publications)
                {
                    publication.Version = 2;
                    total++;
                }
            });

            return total;
        }

        public override int DeletePublicationsInManyTransactions()
        {
            var total = 0;
            var publications = _realm.All<Publication>().Where(x => x.Version == 1).ToList();
            foreach (var publication in publications)
            {
                _realm.Write(() =>
                {
                    _realm.Remove(publication);
                });
                total++;
            }

            return total;
        }

        public override void DeletePublicationsInSingleTransaction()
        {
            _realm.Write(() =>
            {
                _realm.RemoveAll<Publication>();
            });
        }

        public override int SelectCollections()
        {
            var colletions = _realm.All<PublicationCollection>()
                .ToList().Select(x => new
                 {
                     Name = x.Name,
                     Count = x.PublicationsCount,
                     CoverUrl = x.CoverUrl
                 }).ToList();
            //foreach (var publicationCollection in colletions)
            //{
            //    Debug.WriteLine(publicationCollection.Name);
            //    Debug.WriteLine(publicationCollection.Count);
            //    Debug.WriteLine(publicationCollection.CoverUrl);
            //}

            return colletions.Count;
        }
    }
}