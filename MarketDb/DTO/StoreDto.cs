namespace MarketDb.DTO
{
    public class StoreDto
    {
        public int Id { get; set; }
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;
        public int? Quantity { get; set; } = 0;
        public string Adress { get; set; }
    }
}
