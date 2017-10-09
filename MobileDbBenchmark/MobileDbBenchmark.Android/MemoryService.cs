using Android.App;
using MobileDbBenchamark.Common;
using MobileDbBenchmark.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(MemoryService))]
namespace MobileDbBenchmark.Droid
{
    public class MemoryService : IMemoryService
    {
        public long GetAllocatedMemory()
        {
            ActivityManager activityManager = ActivityManager.FromContext(Application.Context);
            var memoryInfo = new ActivityManager.MemoryInfo();
            activityManager.GetMemoryInfo(memoryInfo);
            return memoryInfo.TotalMem;

        }
    }
}