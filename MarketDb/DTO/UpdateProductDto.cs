namespace MarketDb.DTO
{
    public class UpdateProductDto
    {
        public decimal? Price { get; set; }
        public int? Count { get; set; }
        public int? StoreId { get; set; }
        public int? ProductGroupId { get; set; }
    }
}
