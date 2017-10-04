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

        public abstract int PublicationCount();

        public abstract int EnumeratePubblications();

        public abstract int UpdatePublicationVersions();
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
                Version = PublicationVersion(index)
            });
        }

        public override int PublicationCount()
        {
            return _realm.All<Publication>().Count();

        }

        public override int EnumeratePubblications()
        {
            var total = 0;
            foreach (var publication in _realm.All<Publication>())
            {
                total += publication.Version;
            }

            return total;
        }

        public override int UpdatePublicationVersions()
        {
            var total = 0;

            //foreach (var publication in _realm.All<Publication>().Where(x => x.Version == 1))
            //{
            //    _realm.Write(() =>
            //    {
            //        publication.Version = 2;
            //    });
            //    total++;
            //}

            _realm.Write(() =>
            {
                foreach (var publication in _realm.All<Publication>().Where(x => x.Version == 1))
                {

                    publication.Version = 2;

                    total++;
                }
            });

            var versions = _realm.All<Publication>().Count(x => x.Version == 1);

            return total + versions;
        }
    }
}