using System;
using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public class TourChatRoomDto
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
        public TourChatMessageDto? LastMessage { get; set; }
    }

    public class TourChatMemberDto
    {
        public long Id { get; set; }
        public long TourChatRoomId { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastReadAt { get; set; }
    }

    public class TourChatMessageDto
    {
        public long Id { get; set; }
        public long TourChatRoomId { get; set; }
        public long SenderId { get; set; }
        public string? SenderName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
