using Explorer.BuildingBlocks.Core.Domain;
using System.Net.Mail;
using System.Text.Json;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Club : Entity
    {
        public long OwnerId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<string> ImageUrls { get; private set; } = new();
        public List<long> MemberIds { get; private set; } = new();

        private Club() { }
        public Club(long ownerId, string name, string desc, IEnumerable<string>? imageUrls = null)
        {
            OwnerId = ownerId;
            Name = name;
            Description = desc;
            if (imageUrls != null)
            {
                ImageUrls = imageUrls.ToList();
            }

            Validate();
        }
        private void Validate()
        {
            if (OwnerId == 0) throw new ArgumentException("Invalid OwnerId");
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
            if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description");
        }
        public void AddImage(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
                ImageUrls.Add(url);
        }
        public void Update(string name, string description, IEnumerable<string>? imageUrls = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Name");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description");
            Name = name;
            Description = description;
            if (imageUrls != null)
                MemberwiseUpdateImages(imageUrls);
        }
        private void MemberwiseUpdateImages(IEnumerable<string> imageUrls)
        {
            ImageUrls = imageUrls.ToList();
        }
        public void AddMember(long touristId)
        {
            if (touristId == 0) throw new ArgumentException("Invalid touristId");
            if (!MemberIds.Contains(touristId))
                MemberIds.Add(touristId);
        }
        public void RemoveMember(long touristId)
        {
            MemberIds.Remove(touristId);
        }

        public bool IsOwner(long touristId) => OwnerId == touristId;



    }
}
