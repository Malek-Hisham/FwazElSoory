using FwazElSoory.Data;
using FwazElSoory.Models;

namespace FwazElSoory.Forms;

public class MainForm : Form
{
    private DataGridView _dgv = null!;
    private Label _lblStats = null!;
    private System.Windows.Forms.Timer _timer = null!;

    public MainForm()
    {
        Build();
        Refresh();
        _timer = new System.Windows.Forms.Timer { Interval = 8000 };
        _timer.Tick += (_, _) => Refresh();
        _timer.Start();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _timer?.Dispose();
        base.Dispose(disposing);
    }

    // ───────────────────────────────────────── Build UI ──
    private void Build()
    {
        SuspendLayout();
        Text            = "فواز السوري — نظام إدارة المطعم";
        Size            = new Size(1180, 800);
        MinimumSize     = new Size(900, 650);
        StartPosition   = FormStartPosition.CenterScreen;
        BackColor       = AppTheme.BgDark;
        RightToLeft     = RightToLeft.Yes;
        RightToLeftLayout = true;
        Font            = AppTheme.FontBody;

        // ── Header ──
        var header = new Panel { Dock = DockStyle.Top, Height = 72, BackColor = AppTheme.Primary };
        var lblName = new Label
        {
            Text      = "فواز السوري  🍽️",
            Font      = AppTheme.FontTitle,
            ForeColor = Color.White,
            AutoSize  = true,
            Location  = new Point(20, 10),
        };
        var lblSub = new Label
        {
            Text      = "مطعم سوري أصيل",
            Font      = AppTheme.FontSmall,
            ForeColor = Color.FromArgb(255, 200, 150),
            AutoSize  = true,
            Location  = new Point(25, 46),
        };
        _lblStats = new Label
        {
            Font      = AppTheme.FontBody,
            ForeColor = Color.White,
            AutoSize  = true,
            Location  = new Point(650, 26),
        };
        header.Controls.AddRange(new Control[] { lblName, lblSub, _lblStats });

        // ── Order-type buttons ──
        var orderBar = new Panel { Dock = DockStyle.Top, Height = 125, BackColor = AppTheme.Surface, Padding = new Padding(14, 10, 14, 10) };
        var tbl = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 3,
            RowCount    = 1,
            BackColor   = Color.Transparent,
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));

        var btnD = MakeTypeBtn("🚚  ديليفري",         "توصيل للمنزل",         AppTheme.Delivery);
        var btnP = MakeTypeBtn("🏪  استلام",           "الزبون يستلم بنفسه",   AppTheme.Pickup);
        var btnR = MakeTypeBtn("🍽️  أكل في المطعم",   "جلوس في الصالة",       AppTheme.DineIn);

        btnD.Click += (_, _) => OpenNew(OrderType.Delivery);
        btnP.Click += (_, _) => OpenNew(OrderType.Pickup);
        btnR.Click += (_, _) => OpenNew(OrderType.DineIn);

        tbl.Controls.Add(btnD, 0, 0);
        tbl.Controls.Add(btnP, 1, 0);
        tbl.Controls.Add(btnR, 2, 0);
        orderBar.Controls.Add(tbl);

        // ── Active-orders label ──
        var lblActive = new Label
        {
            Text      = "   الطلبات النشطة",
            Dock      = DockStyle.Top,
            Height    = 38,
            Font      = AppTheme.FontHeader,
            ForeColor = Color.White,
            BackColor = AppTheme.Surface2,
            TextAlign = ContentAlignment.MiddleLeft,
        };

        // ── Grid ──
        _dgv = AppTheme.MakeGrid();
        _dgv.Dock = DockStyle.Fill;
        _dgv.CellDoubleClick += OnRowDoubleClick;
        _dgv.CellFormatting  += OnCellFormatting;
        BuildColumns();

        // ── Toolbar ──
        var toolbar = new Panel { Dock = DockStyle.Bottom, Height = 52, BackColor = AppTheme.Surface };
        var btnAll = AppTheme.MakeButton("📋  جميع الطلبات", AppTheme.Surface3, 175, 34);
        btnAll.Location = new Point(15, 9);
        btnAll.Click   += (_, _) => { new OrdersListForm().ShowDialog(this); Refresh(); };

        var btnKitchen = AppTheme.MakeButton("🍳  المطبخ", AppTheme.Surface3, 145, 34);
        btnKitchen.Location = new Point(200, 9);
        btnKitchen.Click   += (_, _) => { new KitchenForm().ShowDialog(this); Refresh(); };

        var btnRefresh = AppTheme.MakeButton("🔄  تحديث", AppTheme.Surface3, 130, 34);
        btnRefresh.Location = new Point(355, 9);
        btnRefresh.Click   += (_, _) => Refresh();

        toolbar.Controls.AddRange(new Control[] { btnAll, btnKitchen, btnRefresh });

        // ── Assemble ──
        var body = new Panel { Dock = DockStyle.Fill };
        body.Controls.Add(_dgv);
        body.Controls.Add(lblActive);

        Controls.Add(body);
        Controls.Add(toolbar);
        Controls.Add(orderBar);
        Controls.Add(header);

        ResumeLayout();
    }

    // ───────────────────────────────────────── Helpers ──
    private static Button MakeTypeBtn(string title, string sub, Color color)
    {
        var btn = new Button
        {
            Dock      = DockStyle.Fill,
            Text      = $"{title}\r\n{sub}",
            Font      = AppTheme.FontLarge,
            ForeColor = Color.White,
            BackColor = color,
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
            Margin    = new Padding(8),
            TextAlign = ContentAlignment.MiddleCenter,
        };
        btn.FlatAppearance.BorderSize          = 0;
        btn.FlatAppearance.MouseOverBackColor  = ControlPaint.Light(color, 0.15f);
        return btn;
    }

    private void BuildColumns()
    {
        _dgv.Columns.Clear();
        AddCol("Id",       "رقم الطلب",  80,  false);
        AddCol("Type",     "النوع",       130, false);
        AddCol("Customer", "العميل / الطاولة", 0, true);
        AddCol("Status",   "الحالة",      120, false);
        AddCol("Total",    "المجموع",     100, false);
        AddCol("Time",     "الوقت",       90,  false);
    }

    private void AddCol(string name, string header, int width, bool fill)
    {
        var col = new DataGridViewTextBoxColumn
        {
            Name          = name,
            HeaderText    = header,
            SortMode      = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
            HeaderCell    = { Style = { Alignment = DataGridViewContentAlignment.MiddleCenter } },
        };
        if (fill) col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        else col.Width = width;
        _dgv.Columns.Add(col);
    }

    // ───────────────────────────────────────── Data ──
    private new void Refresh()
    {
        var active = DataStore.GetActiveOrders();
        _dgv.Rows.Clear();
        foreach (var o in active)
        {
            int i = _dgv.Rows.Add($"#{o.Id:D4}", o.TypeDisplay, o.CustomerDisplay,
                                   o.StatusDisplay, $"{o.Total:F0} ر.س",
                                   o.OrderTime.ToString("hh:mm tt"));
            _dgv.Rows[i].Tag = o.Id;
        }

        var today = DataStore.Orders.Where(o => o.OrderTime.Date == DateTime.Today).ToList();
        _lblStats.Text = $"طلبات اليوم: {today.Count}    |    الإيرادات: {today.Sum(o => o.Total):F0} ر.س";
    }

    private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (_dgv.Columns[e.ColumnIndex].Name != "Status") return;
        var row = _dgv.Rows[e.RowIndex];
        if (row.Tag is int id)
        {
            var order = DataStore.GetOrder(id);
            if (order != null && e.CellStyle != null)
            {
                e.CellStyle.ForeColor = AppTheme.StatusColor(order.Status);
                e.CellStyle.Font      = AppTheme.FontBold;
            }
        }
    }

    private void OnRowDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (_dgv.Rows[e.RowIndex].Tag is not int id) return;
        var order = DataStore.GetOrder(id);
        if (order == null) return;
        using var frm = new OrderDetailForm(order);
        frm.ShowDialog(this);
        Refresh();
    }

    private void OpenNew(OrderType type)
    {
        using var frm = new NewOrderForm(type);
        frm.ShowDialog(this);
        Refresh();
    }
}
