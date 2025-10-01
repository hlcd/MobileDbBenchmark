using Android.App;
using MobileDbBenchamark.Common;

namespace MobileDbBenchmark.Droid
{
    public class MemoryService : IMemoryService
    {
        public long GetAllocatedMemory()
        {
            ActivityManager activityManager = ActivityManager.FromContext(Android.App.Application.Context);
            var memoryInfo = new ActivityManager.MemoryInfo();
            activityManager.GetMemoryInfo(memoryInfo);
            return memoryInfo.TotalMem;

        }
    }
}