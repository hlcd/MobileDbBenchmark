namespace MobileDbBenchamark.Common.Models
{
    public interface IPublicationCollection
    {
        string Id { get; }

        string Name { get; set; }

        int RemoteId { get; set; }
    }
}