namespace PrintingPlatform.Models.Order
{
    public class UserOrderCardViewModel
    {
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty; // Pending / InProgress / Completed

        public string Material { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}