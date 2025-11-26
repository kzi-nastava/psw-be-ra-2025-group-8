using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ConversationSummaryDto
    {
        public long WithUserId { get; set; }
        public string WithUserName { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
    }
}