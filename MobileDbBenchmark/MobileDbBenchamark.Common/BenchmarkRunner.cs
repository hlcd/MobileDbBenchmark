using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MobileDbBenchamark.Common.Tests;
using Realms;

namespace MobileDbBenchamark.Common
{
    public enum DbType
    {
        Realm,
        Sqlite,
    }

    public enum BenchmarkTest
    {
        Insert,
        Count,
        Select,
        Update
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
            StringBuilder sb = new StringBuilder();
            switch (TestCase)
            {
                case BenchmarkTest.Insert:
                case BenchmarkTest.Count:
                case BenchmarkTest.Select:
                case BenchmarkTest.Update:
                    sb.Append("Wstawia ");
                    break;
            }

            sb.Append($"{NumberOfItems} elementów do bazy, powtarza {RepeatTimes} razy. ");
            sb.Append(RemoveDbBetweenIterations
                ? "Każde powtórzenie kasuje bazę."
                : "Każde powtórzenie nie kasuje bazy.");

            if (TestCase == BenchmarkTest.Count)
            {
                sb.Append("Robi count na elementach.");
            }

            if (TestCase == BenchmarkTest.Select)
            {
                sb.Append("Robi selecta na elementach.");
            }

            if (TestCase == BenchmarkTest.Update)
            {
                sb.Append($"Robi update na {NumberOfItems / 2} elementach.");
            }

            return sb.ToString();
        }
    }

    public class BenchmarkRunner
    {

        public async Task<List<TimeSpan>> RunTest(TestSpec spec)
        {
            BenchmarkBase benchmark = spec.DbType == DbType.Realm ? (BenchmarkBase)new RealmBenchmark() : new SqliteBenchmark();
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
                case BenchmarkTest.Update:
                    testAction = UpdatePublicationVersion;
                    prepareDbAction = b => InsertPublications(b, spec.NumberOfItems);
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

        private void UpdatePublicationVersion(BenchmarkBase benchmark)
        {
            var total = benchmark.UpdatePublicationVersions();
            Debug.WriteLine($"Total publications updated: {total}");
        }


        private void EnumeratePublications(BenchmarkBase benchmark)
        {
            var total = benchmark.EnumeratePubblications();
            Debug.WriteLine($"Counting enumate publications: {total}");
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