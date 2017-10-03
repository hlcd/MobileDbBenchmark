using System;
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
            _connection = new SQLiteConnection(StorageManager.Instance.GetDbPath(_filePath));//, SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            _connection.CreateTable<Publication>();
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
                Version = 1
            });
        }

        public override int PublicationCount()
        {
            return _connection.Table<Publication>().Count();
        }

        public override int EnumeratePubblications()
        {
            var total = 0;
            foreach (var publication in _connection.Table<Publication>())
            {
                total += publication.Version;
            }

            return total;
        }
    }
}