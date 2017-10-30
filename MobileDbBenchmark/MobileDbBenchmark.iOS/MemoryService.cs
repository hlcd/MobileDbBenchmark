using Foundation;
using MobileDbBenchamark.Common;
using MobileDbBenchmark.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(MemoryService))]
namespace MobileDbBenchmark.iOS
{
    public class MemoryService : IMemoryService
    {
        public long GetAllocatedMemory()
        {          
            return 0;
        }
    }
}