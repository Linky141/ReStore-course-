namespace API.Entities;

[Table("BasketItems")]
public class BasketItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    #region navigation props
  
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int BasketId { get; set; }
    public Basket Basket { get; set; }

    #endregion navigation props
}