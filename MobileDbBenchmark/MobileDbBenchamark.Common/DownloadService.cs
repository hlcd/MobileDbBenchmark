using System;
using System.Threading.Tasks;
using MobileDbBenchamark.Common.Models.Realm;
using MobileDbBenchamark.Common.Tests;
using Realms;

namespace MobileDbBenchamark.Common
{
    public class DownloadPubblicationEventArgs
    {
        public string Id { get; internal set; }
        public int DownloadPercentage { get; internal set; }
    }

    public class DownloadService
    {
        public async Task DownloadRealmPublication(string id)
        {
            var realm = Realm.GetInstance(RealmBenchmark.Config);
            var publication = realm.Find<Publication>(id);
            for (int i = 1; i <= 100; i++)
            {
                var perentage = i;
                await Task.Delay(TimeSpan.FromMilliseconds(100));
                realm.Write(() =>
                {
                    publication.DownloadPercentage = perentage;
                });
            }

            realm.Dispose();
        }
    }
}