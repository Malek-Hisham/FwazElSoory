namespace FwazElSoory.Models;

public enum OrderType { Delivery, Pickup, DineIn }

public enum OrderStatus { Pending, Confirmed, Preparing, Ready, Delivered, Cancelled }

public class Order
{
    public int Id { get; set; }
    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public List<OrderItem> Items { get; set; } = new();
    public string CustomerName { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string DeliveryAddress { get; set; } = "";
    public int TableNumber { get; set; }
    public DateTime OrderTime { get; set; } = DateTime.Now;
    public string Notes { get; set; } = "";

    public decimal Total => Items.Sum(i => i.Subtotal);

    public string TypeDisplay => Type switch
    {
        OrderType.Delivery => "🚚 ديليفري",
        OrderType.Pickup   => "🏪 استلام",
        OrderType.DineIn   => "🍽️ أكل في المطعم",
        _                  => ""
    };

    public string StatusDisplay => Status switch
    {
        OrderStatus.Pending   => "⏳ معلق",
        OrderStatus.Confirmed => "✅ مؤكد",
        OrderStatus.Preparing => "🔥 يُحضَّر",
        OrderStatus.Ready     => "🟢 جاهز",
        OrderStatus.Delivered => "📦 تم التوصيل",
        OrderStatus.Cancelled => "❌ ملغي",
        _                     => ""
    };

    public string CustomerDisplay =>
        Type == OrderType.DineIn
            ? $"طاولة {TableNumber}"
            : string.IsNullOrWhiteSpace(CustomerName) ? "—" : CustomerName;
}
