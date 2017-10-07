using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MobileDbBenchamark.Common.Models.Sqlite;
using SQLite;

namespace MobileDbBenchamark.Common.Tests
{
    public class SqliteBenchmark : BenchmarkBase
    {
        private SQLiteConnection _connection;

        private readonly string _filePath = "benchmark.db";

        public override IDisposable OpenDB()
        {
            _connection =
                new SQLiteConnection(
                    StorageManager.Instance
                        .GetDbPath(
                            _filePath)); //, SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            _connection.CreateTable<Publication>();
            _connection.CreateTable<PublicationCollection>();
            _connection.CreateTable<PublicationCollectionLink>();
            return _connection;
        }

        public override void DeleteDB()
        {
            StorageManager.Instance.DeleteFile(StorageManager.Instance.GetDbPath(_filePath));
        }

        public override void RunInTransaction(Action action)
        {
            _connection.BeginTransaction();
            try
            {
                action();
                _connection.Commit();
            }
            catch
            {
                _connection.Rollback();
            }
        }

        public override void InsertPublication(int index)
        {
            _connection.Insert(new Publication()
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

            _connection.Insert(collection);

            var lowerBound = (index * itemsInCollection) % publicationsCount;
            var upperBound = lowerBound + itemsInCollection;//((index + 1) * itemsInCollection) % publicationsCount;

            var publications = _connection.Table<Publication>()
                .Where(x => x.RemoteId >= lowerBound && x.RemoteId < upperBound)
                .ToList();

            foreach (var publication in publications)
            {
                var link = new PublicationCollectionLink
                {
                    CollectionId = collection.Id,
                    PublicationId = publication.Id
                };
                _connection.Insert(link);
            }
        }

        public override int PublicationCount()
        {
            return _connection.Table<Publication>().Count();
        }

        public override int EnumeratePublications()
        {
            var total = 0;
            foreach (var publication in _connection.Table<Publication>())
            {
                total += publication.Version;
            }

            return total;
        }

        public override int UpdatePublicationsInManyTransactions()
        {
            var total = 0;
            var publications = _connection.Table<Publication>().Where(x => x.Version == 1).ToList();
            foreach (var publication in publications)
            {
                publication.Version = 2;
                _connection.Update(publication);
                total++;
            }

            return total;
        }

        public override int UpdatePublicationsInSingleTransaction()
        {
            var total = 0;
            var toUpdate = new List<Publication>();
            var publications = _connection.Table<Publication>().Where(x => x.Version == 1).ToList();
            foreach (var publication in publications)
            {
                publication.Version = 2;
                total++;
                toUpdate.Add(publication);
            }

            _connection.UpdateAll(toUpdate, true);

            return total;
        }

        public override int DeletePublicationsInManyTransactions()
        {
            var total = 0;
            var publications = _connection.Table<Publication>().Where(x => x.Version == 1).ToList();
            foreach (var publication in publications)
            {
                _connection.Delete(publication);
                total++;
            }

            return total;
        }

        public override void DeletePublicationsInSingleTransaction()
        {
            _connection.DeleteAll<Publication>();
        }

        public override int SelectCollections()
        {
            var colletions = _connection.Table<PublicationCollection>().ToList();
            //var links = _connection.Table<PublicationCollectionLink>().ToList();
            foreach (var publicationCollection in colletions)
            {
                //Debug.WriteLine(
                var links = _connection.Table<PublicationCollectionLink>()
                    .Count(x => x.CollectionId == publicationCollection.Id);
                   // .ToDictionary(link => link.PublicationId, link => link);
                //links.Count();
                //TODO how to join SqliteNetExtensions does not work
                //_connection.Table<Publication>()
                //    .Where(publication => links.ContainsKey(publication.Id) && publication.CoverUrl != null)
                //    .FirstOrDefault();
                //_connection.Table<PublicationCollection>()
                //);
                //var coverUrl = _connection
                //            .Table<PublicationCollectionLink>()
                //            .Where(link => link.CollectionId == publicationCollection.Id)
                //            .Join(_connection.Table<Publication>(),
                //                link => link.CollectionId,
                //                publication => publication.Id,
                //                (link, publication) => publication.CoverUrl)
                //            .FirstOrDefault(s => s != null);


                //Debug.WriteLine(coverUrl);
            }

            return colletions.Count;

        }
    }
}