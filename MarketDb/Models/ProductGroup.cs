namespace MarketDb.Models
{
    public partial class ProductGroup: BaseModel
    {
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    }
}