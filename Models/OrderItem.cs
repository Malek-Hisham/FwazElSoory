namespace FwazElSoory.Models;

public class OrderItem
{
    public MenuItem MenuItem { get; set; } = new();
    public int Quantity { get; set; }
    public decimal Subtotal => MenuItem.Price * Quantity;
}
