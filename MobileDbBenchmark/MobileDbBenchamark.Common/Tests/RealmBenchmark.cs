using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public static readonly RealmConfiguration Config = new RealmConfiguration("test.realm")
        {
            SchemaVersion = 2,
            MigrationCallback = MigrationCallback,
//#if DEBUG
//            ShouldDeleteIfMigrationNeeded = true
//#endif
        };

        private static void MigrationCallback(Migration migration, ulong oldSchemaVersion)
        {
            var newPublications = migration.NewRealm.All<Publication>();
            var oldPublications = migration.OldRealm.All("Publication");

            for (int i = 0; i < newPublications.Count(); i++)
            {
                var oldPublication = oldPublications.ElementAt(i);
                var newPublication = newPublications.ElementAt(i);

                // Migrate Person from version 0 to 1: replace FirstName and LastName with FullName
                //if (oldSchemaVersion < 1)
                //{
                //    newPublication.FullName = oldPerson.FirstName + " " + oldPerson.LastName;
                //}

                // Migrate Person from version 1 to 2: replace Age with Birthday
                if (oldSchemaVersion < 2)
                {
                    newPublication.SomeDescription = "Description " + oldPublication.Title; // newPublication.Title
                    Debug.WriteLine($"update publication {newPublication}");

                }
            }
        }

        public override IDisposable OpenDB()
        {
            return _realm = Realm.GetInstance(Config);
        }

        public override void DeleteDB()
        {
            Realm.DeleteRealm(Config);
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
                RemoteId = index,
                DownloadPercentage = 0
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
            var upperBound = lowerBound + itemsInCollection; //((index + 1) * itemsInCollection) % publicationsCount;

            var publications = _realm.All<Publication>()
                .Where(x => x.RemoteId >= lowerBound && x.RemoteId < upperBound);

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
            var publications = _realm.All<Publication>().Where(x => x.Version == 1);
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
            var publications = _realm.All<Publication>().Where(x => x.Version == 1);
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
            var publications = _realm.All<Publication>().Where(x => x.Version == 1);
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

        public async Task<Publication> GetPublication(int remoteId)
        {
            // return await Task.Run(() =>
            // {
            var realm = Realm.GetInstance(Config);
            var id = Guid.NewGuid().ToString();
            await realm.WriteAsync(r =>
            {
                r.Add(new Publication()
                {
                    Id = id,
                    CoverUrl = PublicationCoverUrl(1),
                    Title = PublicationTitle(1),
                    Version = PublicationVersion(1),
                    RemoteId = remoteId,
                    DownloadPercentage = 0
                });
            });
            var publication = realm.Find<Publication>(id);

            return publication;
            //  });

        }
    }
}