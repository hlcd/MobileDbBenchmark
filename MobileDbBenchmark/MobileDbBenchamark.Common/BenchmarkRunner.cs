using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MobileDbBenchamark.Common.Tests;

namespace MobileDbBenchamark.Common
{
    public enum DbType
    {
        Realm
    }

    public enum BenchmarkTest
    {
        Insert,
        Count,
        Select,
        UpdateManyTransactions,
        UpdateSingleTransaction,
        DeleteManyTransaction,
        DeleteSingleTransaction,
        ManyToManyInsert,
        SelectCollections
    }

    public class TestSpec
    {
        public int RepeatTimes { get; set; }

        public DbType DbType { get; set; }

        public BenchmarkTest TestCase { get; set; }

        public bool RemoveDbBetweenIterations { get; set; }

        public int NumberOfItems { get; set; }

        public string Name { get; set; }

        public string Description => BuildDescription();

        public string BuildDescription()
        {
            StringBuilder sb = new StringBuilder("Wstawia ");

            sb.Append($"{NumberOfItems} elementów do bazy, powtarza {RepeatTimes} razy. ");
            sb.Append(RemoveDbBetweenIterations
                ? "Każde powtórzenie kasuje bazę."
                : "Każde powtórzenie nie kasuje bazy.");

            switch (TestCase)
            {
                case BenchmarkTest.Count:
                    sb.Append("Robi count na elementach.");
                    break;
                case BenchmarkTest.Select:
                    sb.Append("Robi selecta na elementach.");
                    break;
                case BenchmarkTest.UpdateManyTransactions:
                    sb.AppendLine($"Robi update na {NumberOfItems / 2} elementach w wielu transakcjach.");
                    break;
                case BenchmarkTest.UpdateSingleTransaction:
                    sb.AppendLine($"Robi update na {NumberOfItems / 2} elementach w jednej transakcji.");
                    break;
                case BenchmarkTest.DeleteManyTransaction:
                    sb.AppendLine($"Robi delete na {NumberOfItems} elementach w wielu transakcjach.");
                    break;
                case BenchmarkTest.DeleteSingleTransaction:
                    sb.AppendLine($"Robi delete na {NumberOfItems} elementach w jednej transakcji.");
                    break;
                case BenchmarkTest.ManyToManyInsert:
                case BenchmarkTest.SelectCollections:
                    var numberOfCollections = NumberOfItems / 50;
                    var numberOfItemsInEachCollections = numberOfCollections / 2;
                    sb.AppendLine($"Wstawia {numberOfCollections} kolekcji i {numberOfItemsInEachCollections } elementów do każdej kolekcji.");
                    break;
            }

            if (TestCase == BenchmarkTest.SelectCollections)
            {
                sb.AppendLine("Select kolekcji wraz z liczba elementow w kolekcji oraz z adresem okładki");
            }

            return sb.ToString();
        }
    }

    public class BenchmarkRunner
    {

        public async Task<List<TimeSpan>> RunTest(TestSpec spec)
        {
            var benchmark = new RealmBenchmark();
            Action<BenchmarkBase> testAction = null;
            Action<BenchmarkBase> prepareDbAction = null;

            switch (spec.TestCase)
            {
                case BenchmarkTest.Insert:
                    testAction = b => InsertPublications(b, spec.NumberOfItems);
                    break;
                case BenchmarkTest.Count:
                    testAction = CountPublications;
                    prepareDbAction = b => InsertPublications(b, spec.NumberOfItems);
                    break;
                case BenchmarkTest.Select:
                    testAction = EnumeratePublications;
                    prepareDbAction = b => InsertPublications(b, spec.NumberOfItems);
                    break;
                case BenchmarkTest.UpdateManyTransactions:
                    testAction = UpdatePublicationsInManyTransactions;
                    prepareDbAction = b => InsertPublications(b, spec.NumberOfItems);
                    break;
                case BenchmarkTest.UpdateSingleTransaction:
                    testAction = UpdatePublicationsInSingleTransaction;
                    prepareDbAction = b => InsertPublications(b, spec.NumberOfItems);
                    break;
                case BenchmarkTest.DeleteManyTransaction:
                    testAction = DeletePublicationsInManyTransactions;
                    prepareDbAction = b => InsertPublications(b, spec.NumberOfItems);
                    break;
                case BenchmarkTest.DeleteSingleTransaction:
                    testAction = DeletePublicationsInSingleTransaction;
                    prepareDbAction = b => InsertPublications(b, spec.NumberOfItems);
                    break;
                case BenchmarkTest.ManyToManyInsert:
                    testAction = b => InsertPublicationsAndCollections(b, spec.NumberOfItems);
                    //prepareDbAction = b => InsertPublications(b, spec.NumberOfItems);
                    break;
                case BenchmarkTest.SelectCollections:
                    prepareDbAction = b => InsertPublicationsAndCollections(b, spec.NumberOfItems);
                    testAction = SelectCollections;
                    //prepareDbAction = b => InsertPublications(b, spec.NumberOfItems);
                    break;

            }

            benchmark.DeleteDB();

            var results = new List<TimeSpan>();
            for (int i = 0; i < spec.RepeatTimes; i++)
            {
                await Task.Run(() =>
                {
                    using (benchmark.OpenDB())
                    {
                        prepareDbAction?.Invoke(benchmark);

                        results.Add(benchmark.PerformTest(testAction));
                    }
                });

                if (spec.RemoveDbBetweenIterations)
                {
                    benchmark.DeleteDB();
                }
            }

            return results;
        }

        private void SelectCollections(BenchmarkBase benchmark)
        {
            benchmark.SelectCollections();
        }

        private void InsertPublicationsAndCollections(BenchmarkBase benchmark, int numberOfItems)
        {
            InsertPublications(benchmark, numberOfItems);
            var numberOfCollection = numberOfItems / 50;
            var itemsInCollection = numberOfCollection / 2;

            for (int i = 0; i < numberOfCollection; i++)
            {
                var index = i;
                benchmark.RunInTransaction(() =>
                {
                    benchmark.InsertCollection(index, itemsInCollection, numberOfItems);
                });
            }
        }

        private void DeletePublicationsInSingleTransaction(BenchmarkBase benchmark)
        {
            benchmark.DeletePublicationsInSingleTransaction();
            Debug.WriteLine($"All publications have been removed");
        }

        private void DeletePublicationsInManyTransactions(BenchmarkBase benchmark)
        {
            var total = benchmark.DeletePublicationsInManyTransactions();
            Debug.WriteLine($"Total publications deleted: {total}");
        }

        private void UpdatePublicationsInSingleTransaction(BenchmarkBase benchmark)
        {
            var total = benchmark.UpdatePublicationsInSingleTransaction();
            Debug.WriteLine($"Total publications updated: {total}");
        }

        private void UpdatePublicationsInManyTransactions(BenchmarkBase benchmark)
        {
            var total = benchmark.UpdatePublicationsInManyTransactions();
            Debug.WriteLine($"Total publications updated: {total}");
        }


        private void EnumeratePublications(BenchmarkBase benchmark)
        {
            var total = benchmark.EnumeratePublications();
            Debug.WriteLine($"Counting enumerate publications: {total}");
        }

        private static void CountPublications(BenchmarkBase benchmark)
        {
            var count = benchmark.PublicationCount();
            Debug.WriteLine($"Counting publications: {count}");
        }

        private static void InsertPublications(BenchmarkBase benchmark, int numberOfPublications = 5000)
        {
            benchmark.RunInTransaction(() =>
            {
                for (int i = 0; i < numberOfPublications; i++)
                {
                    benchmark.InsertPublication(i);
                }
            });

        }
    }
}