namespace eBookStore.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
