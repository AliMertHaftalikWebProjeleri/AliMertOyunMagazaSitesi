namespace AliMertOyunMagaza.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public int GameId { get; set; }
        public Game? Game { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
