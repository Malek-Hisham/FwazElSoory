using FwazElSoory.Data;
using FwazElSoory.Models;

namespace FwazElSoory.Forms;

public class OrdersListForm : Form
{
    private DataGridView _dgv = null!;
    private ComboBox _cmbStatus = null!;
    private ComboBox _cmbType   = null!;

    public OrdersListForm()
    {
        Build();
        LoadOrders();
    }

    private void Build()
    {
        SuspendLayout();
        Text              = "جميع الطلبات";
        Size              = new Size(1100, 720);
        MinimumSize       = new Size(850, 550);
        StartPosition     = FormStartPosition.CenterParent;
        BackColor         = AppTheme.BgDark;
        RightToLeft       = RightToLeft.Yes;
        RightToLeftLayout = true;
        Font              = AppTheme.FontBody;

        // Header
        var header = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = AppTheme.Primary };
        header.Controls.Add(new Label { Text = "📋  جميع الطلبات", Font = AppTheme.FontHeader, ForeColor = Color.White, AutoSize = true, Location = new Point(15, 16) });

        // Filter bar
        var filterBar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = AppTheme.Surface, Padding = new Padding(12, 10, 12, 10) };

        _cmbStatus = new ComboBox { Font = AppTheme.FontBody, BackColor = AppTheme.Surface2, ForeColor = AppTheme.TextLight, FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList, Width = 160, Location = new Point(12, 12) };
        _cmbStatus.Items.AddRange(new object[] { "كل الحالات", "⏳ معلق", "✅ مؤكد", "🔥 يُحضَّر", "🟢 جاهز", "📦 تم التوصيل", "❌ ملغي" });
        _cmbStatus.SelectedIndex = 0;
        _cmbStatus.SelectedIndexChanged += (_, _) => LoadOrders();

        _cmbType = new ComboBox { Font = AppTheme.FontBody, BackColor = AppTheme.Surface2, ForeColor = AppTheme.TextLight, FlatStyle = FlatStyle.Flat, DropDownStyle = ComboBoxStyle.DropDownList, Width = 160, Location = new Point(185, 12) };
        _cmbType.Items.AddRange(new object[] { "كل الأنواع", "🚚 ديليفري", "🏪 استلام", "🍽️ أكل في المطعم" });
        _cmbType.SelectedIndex = 0;
        _cmbType.SelectedIndexChanged += (_, _) => LoadOrders();

        var lblFilter1 = new Label { Text = "الحالة:", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(12, -2) };
        var lblFilter2 = new Label { Text = "النوع:", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(185, -2) };

        var btnRefresh = AppTheme.MakeButton("🔄 تحديث", AppTheme.Surface3, 110, 32);
        btnRefresh.Location = new Point(360, 10);
        btnRefresh.Click += (_, _) => LoadOrders();

        filterBar.Controls.AddRange(new Control[] { _cmbStatus, _cmbType, btnRefresh });

        // Grid
        _dgv = AppTheme.MakeGrid();
        _dgv.Dock = DockStyle.Fill;
        _dgv.CellDoubleClick += OnRowDoubleClick;
        _dgv.CellFormatting  += OnCellFormatting;

        AddCol("Id",       "رقم الطلب",      75,  false);
        AddCol("Time",     "الوقت",           130, false);
        AddCol("Type",     "النوع",           130, false);
        AddCol("Customer", "العميل / الطاولة", 0, true);
        AddCol("Status",   "الحالة",          120, false);
        AddCol("Items",    "عدد الأصناف",     90,  false);
        AddCol("Total",    "المجموع",         100, false);

        // Bottom
        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = AppTheme.Surface };
        var btnClose = AppTheme.MakeButton("إغلاق", AppTheme.Surface3, 120, 34);
        btnClose.Location = new Point(12, 8);
        btnClose.Click += (_, _) => Close();
        bottom.Controls.Add(btnClose);

        Controls.Add(_dgv);
        Controls.Add(bottom);
        Controls.Add(filterBar);
        Controls.Add(header);

        ResumeLayout();
    }

    private void AddCol(string name, string header, int width, bool fill)
    {
        var col = new DataGridViewTextBoxColumn
        {
            Name     = name,
            HeaderText = header,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
        };
        if (fill) col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        else col.Width = width;
        _dgv.Columns.Add(col);
    }

    private void LoadOrders()
    {
        var orders = DataStore.Orders.AsEnumerable();

        if (_cmbStatus.SelectedIndex > 0)
        {
            OrderStatus? filter = _cmbStatus.SelectedIndex switch
            {
                1 => OrderStatus.Pending,
                2 => OrderStatus.Confirmed,
                3 => OrderStatus.Preparing,
                4 => OrderStatus.Ready,
                5 => OrderStatus.Delivered,
                6 => OrderStatus.Cancelled,
                _ => null,
            };
            if (filter != null) orders = orders.Where(o => o.Status == filter);
        }

        if (_cmbType.SelectedIndex > 0)
        {
            OrderType? tFilter = _cmbType.SelectedIndex switch
            {
                1 => OrderType.Delivery,
                2 => OrderType.Pickup,
                3 => OrderType.DineIn,
                _ => null,
            };
            if (tFilter != null) orders = orders.Where(o => o.Type == tFilter);
        }

        _dgv.Rows.Clear();
        foreach (var o in orders.OrderByDescending(o => o.OrderTime))
        {
            int i = _dgv.Rows.Add(
                $"#{o.Id:D4}",
                o.OrderTime.ToString("MM/dd  hh:mm tt"),
                o.TypeDisplay,
                o.CustomerDisplay,
                o.StatusDisplay,
                o.Items.Sum(x => x.Quantity),
                $"{o.Total:F0} ر.س"
            );
            _dgv.Rows[i].Tag = o.Id;
        }
    }

    private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (_dgv.Columns[e.ColumnIndex].Name != "Status") return;
        if (_dgv.Rows[e.RowIndex].Tag is int id)
        {
            var o = DataStore.GetOrder(id);
            if (o != null && e.CellStyle != null)
            {
                e.CellStyle.ForeColor = AppTheme.StatusColor(o.Status);
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
        LoadOrders();
    }
}
