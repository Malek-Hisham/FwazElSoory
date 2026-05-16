using FwazElSoory.Data;
using FwazElSoory.Models;

namespace FwazElSoory.Forms;

public class KitchenForm : Form
{
    private FlowLayoutPanel _flow = null!;
    private Label _lblTime = null!;
    private System.Windows.Forms.Timer _timer = null!;

    public KitchenForm()
    {
        Build();
        Refresh();
        _timer = new System.Windows.Forms.Timer { Interval = 5000 };
        _timer.Tick += (_, _) => Refresh();
        _timer.Start();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _timer?.Dispose();
        base.Dispose(disposing);
    }

    private void Build()
    {
        SuspendLayout();
        Text              = "شاشة المطبخ — فواز السوري";
        Size              = new Size(1200, 800);
        MinimumSize       = new Size(900, 600);
        StartPosition     = FormStartPosition.CenterParent;
        BackColor         = AppTheme.BgDark;
        RightToLeft       = RightToLeft.Yes;
        RightToLeftLayout = true;
        Font              = AppTheme.FontBody;

        // Header
        var header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(50, 30, 0) };
        header.Controls.Add(new Label { Text = "🍳  شاشة المطبخ", Font = AppTheme.FontHeader, ForeColor = Color.White, AutoSize = true, Location = new Point(15, 18) });
        _lblTime = new Label { Font = AppTheme.FontBold, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(900, 22) };
        header.Controls.Add(_lblTime);

        // Legend
        var legend = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = AppTheme.Surface, Padding = new Padding(10, 6, 10, 6) };
        var items = new[]
        {
            ("⏳ معلق",        AppTheme.Warning),
            ("✅ مؤكد",        Color.FromArgb(33, 150, 243)),
            ("🔥 يُحضَّر",    AppTheme.Primary),
        };
        int lx = 10;
        foreach (var (t, c) in items)
        {
            legend.Controls.Add(new Label { Text = t, Font = AppTheme.FontBold, ForeColor = c, AutoSize = true, Location = new Point(lx, 8) });
            lx += 130;
        }

        // Scrollable orders
        _flow = new FlowLayoutPanel
        {
            Dock      = DockStyle.Fill,
            AutoScroll = true,
            BackColor = AppTheme.BgDark,
            Padding   = new Padding(10),
        };

        // Bottom
        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = AppTheme.Surface };
        var btnClose = AppTheme.MakeButton("إغلاق", AppTheme.Surface3, 120, 34);
        btnClose.Location = new Point(12, 8);
        btnClose.Click += (_, _) => Close();
        var btnRefresh = AppTheme.MakeButton("🔄 تحديث", AppTheme.Surface3, 130, 34);
        btnRefresh.Location = new Point(144, 8);
        btnRefresh.Click += (_, _) => Refresh();
        bottom.Controls.AddRange(new Control[] { btnClose, btnRefresh });

        Controls.Add(_flow);
        Controls.Add(bottom);
        Controls.Add(legend);
        Controls.Add(header);

        ResumeLayout();
    }

    private new void Refresh()
    {
        _lblTime.Text = DateTime.Now.ToString("hh:mm:ss tt");

        var orders = DataStore.Orders
            .Where(o => o.Status == OrderStatus.Pending ||
                        o.Status == OrderStatus.Confirmed ||
                        o.Status == OrderStatus.Preparing)
            .OrderBy(o => o.OrderTime)
            .ToList();

        _flow.SuspendLayout();
        _flow.Controls.Clear();

        if (orders.Count == 0)
        {
            _flow.Controls.Add(new Label
            {
                Text      = "لا توجد طلبات نشطة الآن",
                Font      = AppTheme.FontLarge,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Margin    = new Padding(30),
            });
        }
        else
        {
            foreach (var order in orders)
                _flow.Controls.Add(MakeOrderCard(order));
        }

        _flow.ResumeLayout();
    }

    private Panel MakeOrderCard(Order order)
    {
        Color borderColor = order.Status switch
        {
            OrderStatus.Pending   => AppTheme.Warning,
            OrderStatus.Confirmed => Color.FromArgb(33, 150, 243),
            OrderStatus.Preparing => AppTheme.Primary,
            _                     => AppTheme.Surface3,
        };

        var card = new Panel
        {
            Size      = new Size(280, 0),
            BackColor = AppTheme.Surface,
            Margin    = new Padding(8),
            Padding   = new Padding(0, 0, 0, 10),
        };

        // Colored top bar
        var topBar = new Panel { Dock = DockStyle.Top, Height = 6, BackColor = borderColor };

        // Title row
        var titleBar = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = AppTheme.Surface2 };
        titleBar.Controls.Add(new Label
        {
            Text      = $"#{order.Id:D4}  {order.TypeDisplay}",
            Font      = AppTheme.FontBold,
            ForeColor = Color.White,
            AutoSize  = true,
            Location  = new Point(8, 12),
        });

        var elapsed = (int)(DateTime.Now - order.OrderTime).TotalMinutes;
        titleBar.Controls.Add(new Label
        {
            Text      = $"⏱ {elapsed} دقيقة",
            Font      = AppTheme.FontSmall,
            ForeColor = elapsed > 15 ? AppTheme.Danger : AppTheme.TextMuted,
            AutoSize  = true,
            Location  = new Point(8, 28),
        });

        // Status label
        var lblStatus = new Label
        {
            Text      = order.StatusDisplay,
            Font      = AppTheme.FontBold,
            ForeColor = borderColor,
            Dock      = DockStyle.Top,
            Height    = 28,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Transparent,
        };

        // Items list
        var itemsFlow = new FlowLayoutPanel
        {
            Dock        = DockStyle.Top,
            Height      = order.Items.Count * 26 + 6,
            BackColor   = Color.Transparent,
            FlowDirection = FlowDirection.TopDown,
            Padding     = new Padding(8, 4, 8, 4),
        };
        foreach (var item in order.Items)
        {
            itemsFlow.Controls.Add(new Label
            {
                Text      = $"• {item.MenuItem.Name}  x{item.Quantity}",
                Font      = AppTheme.FontBody,
                ForeColor = AppTheme.TextLight,
                AutoSize  = true,
            });
        }

        // Customer
        var lblCust = new Label
        {
            Text      = order.CustomerDisplay,
            Font      = AppTheme.FontSmall,
            ForeColor = AppTheme.TextMuted,
            Dock      = DockStyle.Top,
            Height    = 24,
            TextAlign = ContentAlignment.MiddleCenter,
        };

        // Action buttons
        var actionBar = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent, Padding = new Padding(8, 5, 8, 5) };

        var (nextLabel, nextStatus) = order.Status switch
        {
            OrderStatus.Pending   => ("✅ تأكيد",        OrderStatus.Confirmed),
            OrderStatus.Confirmed => ("🔥 بدء التحضير", OrderStatus.Preparing),
            OrderStatus.Preparing => ("🟢 جاهز",        OrderStatus.Ready),
            _                     => (null, order.Status),
        };

        if (nextLabel != null)
        {
            var btn = AppTheme.MakeButton(nextLabel, borderColor, 180, 30);
            btn.Location = new Point(8, 5);
            btn.Click += (_, _) =>
            {
                order.Status = nextStatus;
                Refresh();
            };
            actionBar.Controls.Add(btn);
        }

        // Notes
        if (!string.IsNullOrWhiteSpace(order.Notes))
        {
            var lblNotes = new Label
            {
                Text      = $"📝 {order.Notes}",
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(255, 200, 100),
                Dock      = DockStyle.Top,
                Height    = 22,
                TextAlign = ContentAlignment.MiddleCenter,
            };
            card.Controls.Add(actionBar);
            card.Controls.Add(lblNotes);
        }
        else
        {
            card.Controls.Add(actionBar);
        }

        card.Controls.Add(lblCust);
        card.Controls.Add(itemsFlow);
        card.Controls.Add(lblStatus);
        card.Controls.Add(titleBar);
        card.Controls.Add(topBar);

        // Recalculate card height
        int h = 6 + 44 + 28 + order.Items.Count * 26 + 6 + 24 + 40 + 10;
        if (!string.IsNullOrWhiteSpace(order.Notes)) h += 22;
        card.Height = h;

        return card;
    }
}
