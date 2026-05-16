using FwazElSoory.Models;

namespace FwazElSoory.Data;

public static class DataStore
{
    private static int _nextId = 1;

    public static List<MenuItem> MenuItems { get; } = new()
    {
        // مشاوي
        new() { Id=1,  Name="كباب حلبي",       Category="مشاوي",           Price=25, Description="كباب لحم بهارات حلبية أصيلة" },
        new() { Id=2,  Name="شيش طاووق",        Category="مشاوي",           Price=22, Description="دجاج متبل مشوي على الفحم" },
        new() { Id=3,  Name="كفتة مشوية",       Category="مشاوي",           Price=20, Description="لحم مفروم بالبصل والبقدونس" },
        new() { Id=4,  Name="مشاوي مشكلة",      Category="مشاوي",           Price=55, Description="كباب + شيش طاووق + كفتة" },
        new() { Id=5,  Name="لحم مشوي",         Category="مشاوي",           Price=38, Description="قطع لحم طازجة مشوية" },
        new() { Id=6,  Name="دجاج مشوي كامل",   Category="مشاوي",           Price=45, Description="دجاج كامل متبل ومشوي" },

        // أطباق رئيسية
        new() { Id=7,  Name="شاورما دجاج",      Category="أطباق رئيسية",    Price=18, Description="شاورما دجاج بالتوابل السورية" },
        new() { Id=8,  Name="شاورما لحم",       Category="أطباق رئيسية",    Price=22, Description="شاورما لحم بالتوابل السورية" },
        new() { Id=9,  Name="فتة بالدجاج",      Category="أطباق رئيسية",    Price=28, Description="خبز محمص مع الدجاج واللبن والصنوبر" },
        new() { Id=10, Name="مجبوس دجاج",       Category="أطباق رئيسية",    Price=35, Description="أرز بالبهارات مع قطع الدجاج" },
        new() { Id=11, Name="منسف سوري",        Category="أطباق رئيسية",    Price=45, Description="أرز باللبن مع اللحم والصنوبر" },

        // مقبلات
        new() { Id=12, Name="حمص بالطحينة",     Category="مقبلات",          Price=8,  Description="حمص ناعم مع زيت زيتون وبابريكا" },
        new() { Id=13, Name="متبل",             Category="مقبلات",          Price=9,  Description="باذنجان مشوي بالطحينة والليمون" },
        new() { Id=14, Name="فتوش",             Category="مقبلات",          Price=10, Description="سلطة خضار طازجة مع خبز محمص" },
        new() { Id=15, Name="تبولة",            Category="مقبلات",          Price=9,  Description="برغل ناعم مع بقدونس وطماطم" },
        new() { Id=16, Name="ورق عنب",          Category="مقبلات",          Price=14, Description="أوراق عنب محشية بالأرز والبهارات" },
        new() { Id=17, Name="مزة متنوعة",       Category="مقبلات",          Price=35, Description="صحن مزة يحتوي 6 أصناف" },

        // مشروبات
        new() { Id=18, Name="عيران",            Category="مشروبات",         Price=5,  Description="لبن مخفوق بارد" },
        new() { Id=19, Name="عصير طازج",        Category="مشروبات",         Price=8,  Description="عصير فاكهة طازج" },
        new() { Id=20, Name="شاي",              Category="مشروبات",         Price=4,  Description="شاي أسود مع النعناع" },
        new() { Id=21, Name="قهوة عربية",       Category="مشروبات",         Price=6,  Description="قهوة عربية بالهيل" },
        new() { Id=22, Name="ماء معدني",        Category="مشروبات",         Price=2,  Description="زجاجة ماء 500 مل" },

        // حلويات
        new() { Id=23, Name="بقلاوة",           Category="حلويات",          Price=12, Description="بقلاوة بالفستق والقطر" },
        new() { Id=24, Name="كنافة",            Category="حلويات",          Price=16, Description="كنافة نابلسية بالجبن" },
        new() { Id=25, Name="أيس كريم",         Category="حلويات",          Price=10, Description="آيس كريم بالمكسرات والعسل" },
    };

    public static List<Order> Orders { get; } = new();

    public static IEnumerable<string> Categories =>
        MenuItems.Select(m => m.Category).Distinct();

    public static int AddOrder(Order order)
    {
        order.Id = _nextId++;
        Orders.Add(order);
        return order.Id;
    }

    public static Order? GetOrder(int id) =>
        Orders.FirstOrDefault(o => o.Id == id);

    public static List<Order> GetActiveOrders() =>
        Orders
            .Where(o => o.Status != OrderStatus.Delivered && o.Status != OrderStatus.Cancelled)
            .OrderByDescending(o => o.OrderTime)
            .ToList();
}
