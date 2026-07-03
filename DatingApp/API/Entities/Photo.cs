namespace API.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public string? PublicId { get; set; }

        // Navigation Property
        public string MemberId { get; set; } = null!;
        public Member Member { get; set; } = null!;
    }
}
