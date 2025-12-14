using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class FollowerMessage : Entity
{
    public long SenderId { get; private set; }
    public string Content { get; private set; }
    public DateTime SentAt { get; private set; }
    
    // Optional attachment
    public ResourceType? AttachmentType { get; private set; }
    public long? AttachmentResourceId { get; private set; }

    public FollowerMessage(long senderId, string content, ResourceType? attachmentType = null, long? attachmentResourceId = null)
    {
        if (senderId <= 0)
            throw new ArgumentException("Invalid sender ID.");
  
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty.");
      
        if (content.Length > 280)
            throw new ArgumentException("Message content cannot exceed 280 characters.");

        if (attachmentType.HasValue && !attachmentResourceId.HasValue)
            throw new ArgumentException("Attachment resource ID is required when attachment type is specified.");

        if (!attachmentType.HasValue && attachmentResourceId.HasValue)
            throw new ArgumentException("Attachment type is required when attachment resource ID is specified.");

        SenderId = senderId;
        Content = content;
        SentAt = DateTime.UtcNow;
        AttachmentType = attachmentType;
        AttachmentResourceId = attachmentResourceId;
    }

    // EF Core constructor
    private FollowerMessage() { }
}

public enum ResourceType
{
    Tour = 0,
    BlogPost = 1
}
