namespace Explorer.Stakeholders.API.Dtos
{
    public class SendMessageWithCouponDto
    {
        public long RecipientId { get; set; }
        public string Content { get; set; }
        public long CouponId { get; set; }
    }
}
