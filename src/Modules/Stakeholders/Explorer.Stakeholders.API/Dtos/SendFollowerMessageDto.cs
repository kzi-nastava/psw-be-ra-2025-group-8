namespace Explorer.Stakeholders.API.Dtos;

public class SendFollowerMessageDto
{
    public string Content { get; set; }
    public string? AttachmentType { get; set; }  // "Tour" or "BlogPost"
    public long? AttachmentResourceId { get; set; }
}
