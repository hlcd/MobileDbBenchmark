using System.Linq;

namespace MobileDbBenchamark.Common.Models
{
    public interface IPublication
    {
        string Id { get; }

        string Title { get; set; }

        string CoverUrl { get; set; }

        int RemoteId { get; set; }

        int Version { get; set; }

        bool HasCover { get; }

        int DownloadPercentage { get; }
    }
}