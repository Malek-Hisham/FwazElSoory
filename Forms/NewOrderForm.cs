using FwazElSoory.Data;
using FwazElSoory.Models;

namespace FwazElSoory.Forms;

public class NewOrderForm : Form
{
    private readonly OrderType _type;
    private readonly List<OrderItem> _cart = new();

    private Panel _menuPanel = null!;
    private DataGridView _dgvCart = null!;
    private Label _lblTotal = null!;

    // Customer-info controls
    private TextBox _txtName    = null!;
    private TextBox _txtPhone   = null!;
    private TextBox _txtAddress = null!;
    private TextBox _txtNotes   = null!;
    private Panel   _pnlAddress = null!;
    private ComboBox _cmbTable  = null!;
    private Panel   _pnlTable   = null!;

    public NewOrderForm(OrderType type)
    {
        _type = type;
        Build();
        LoadCategory(DataStore.Categories.First());
    }

    // ───────────────────────────────────────── Build UI ──
    private void Build()
    {
        SuspendLayout();
        string typeIcon = _type switch
        {
            OrderType.Delivery => "🚚 ديليفري",
            OrderType.Pickup   => "🏪 استلام",
            _                  => "🍽️ أكل في المطعم",
        };
        Text              = $"طلب جديد — {typeIcon}";
        Size              = new Size(1200, 820);
        MinimumSize       = new Size(1000, 700);
        StartPosition     = FormStartPosition.CenterParent;
        BackColor         = AppTheme.BgDark;
        RightToLeft       = RightToLeft.Yes;
        RightToLeftLayout = true;
        Font              = AppTheme.FontBody;

        // ── Top header ──
        var headerColor = _type switch
        {
            OrderType.Delivery => AppTheme.Delivery,
            OrderType.Pickup   => AppTheme.Pickup,
            _                  => AppTheme.DineIn,
        };
        var header = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = headerColor };
        var lbl = new Label
        {
            Text      = $"طلب جديد  —  {typeIcon}",
            Font      = AppTheme.FontHeader,
            ForeColor = Color.White,
            AutoSize  = true,
            Location  = new Point(20, 15),
        };
        var btnBack = AppTheme.MakeButton("← رجوع", AppTheme.Surface3, 110, 36);
        btnBack.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
        btnBack.Location = new Point(1050, 10);
        btnBack.Click   += (_, _) => Close();
        header.Controls.AddRange(new Control[] { lbl, btnBack });

        // ── Customer info ──
        var infoPanel = BuildInfoPanel();

        // ── Bottom buttons ──
        var btnBar = new Panel { Dock = DockStyle.Bottom, Height = 54, BackColor = AppTheme.Surface };
        var btnConfirm = AppTheme.MakeButton("✅  تأكيد الطلب", AppTheme.Success, 190, 36);
        btnConfirm.Location = new Point(15, 9);
        btnConfirm.Click   += OnConfirm;
        var btnCancel = AppTheme.MakeButton("❌  إلغاء", AppTheme.Danger, 130, 36);
        btnCancel.Location = new Point(215, 9);
        btnCancel.Click   += (_, _) => Close();
        btnBar.Controls.AddRange(new Control[] { btnConfirm, btnCancel });

        // ── Main split ──
        var split = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 2,
            RowCount    = 1,
        };
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

        split.Controls.Add(BuildMenuPanel(), 0, 0);
        split.Controls.Add(BuildCartPanel(), 1, 0);

        Controls.Add(split);
        Controls.Add(btnBar);
        Controls.Add(infoPanel);
        Controls.Add(header);

        ResumeLayout();
    }

    // ── Menu (left) ──
    private Panel BuildMenuPanel()
    {
        var outer = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.BgDark, Padding = new Padding(10, 8, 6, 8) };

        // Category tabs
        var catFlow = new FlowLayoutPanel
        {
            Dock       = DockStyle.Top,
            Height     = 46,
            AutoScroll = false,
            BackColor  = Color.Transparent,
            Padding    = new Padding(0, 4, 0, 4),
        };
        foreach (var cat in DataStore.Categories)
        {
            string c = cat;
            var btn = AppTheme.MakeButton(c, AppTheme.Surface2, 120, 34);
            btn.Margin = new Padding(0, 0, 6, 0);
            btn.Click += (_, _) => LoadCategory(c);
            catFlow.Controls.Add(btn);
        }

        // Items grid
        _menuPanel = new Panel
        {
            Dock       = DockStyle.Fill,
            AutoScroll = true,
            BackColor  = Color.Transparent,
        };

        outer.Controls.Add(_menuPanel);
        outer.Controls.Add(catFlow);
        return outer;
    }

    private void LoadCategory(string category)
    {
        _menuPanel.Controls.Clear();
        var items = DataStore.MenuItems.Where(m => m.Category == category && m.IsAvailable).ToList();

        var flow = new FlowLayoutPanel
        {
            Dock      = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.Transparent,
            Padding   = new Padding(4),
        };

        foreach (var item in items)
        {
            var card = MakeItemCard(item);
            flow.Controls.Add(card);
        }

        _menuPanel.Controls.Add(flow);
    }

    private Panel MakeItemCard(MenuItem item)
    {
        var card = new Panel
        {
            Size      = new Size(195, 90),
            BackColor = AppTheme.Surface,
            Margin    = new Padding(5),
            Cursor    = Cursors.Hand,
        };

        var lblName = new Label
        {
            Text      = item.Name,
            Font      = AppTheme.FontBold,
            ForeColor = AppTheme.TextLight,
            Location  = new Point(8, 10),
            Size      = new Size(178, 36),
            TextAlign = ContentAlignment.TopRight,
        };
        var lblPrice = new Label
        {
            Text      = $"{item.Price:F0} ر.س",
            Font      = AppTheme.FontBody,
            ForeColor = AppTheme.Primary,
            Location  = new Point(8, 52),
            AutoSize  = true,
        };
        var lblPlus = new Label
        {
            Text      = "+",
            Font      = new Font("Segoe UI", 22f, FontStyle.Bold),
            ForeColor = AppTheme.Primary,
            Location  = new Point(160, 42),
            AutoSize  = true,
        };

        card.Controls.AddRange(new Control[] { lblName, lblPrice, lblPlus });

        // Hover effect
        void Enter(object? s, EventArgs e) => card.BackColor = AppTheme.Surface2;
        void Leave(object? s, EventArgs e) => card.BackColor = AppTheme.Surface;
        void Click(object? s, EventArgs e) => AddToCart(item);

        card.MouseEnter    += Enter; card.MouseLeave    += Leave; card.Click    += Click;
        lblName.MouseEnter += Enter; lblName.MouseLeave += Leave; lblName.Click += Click;
        lblPrice.MouseEnter+= Enter; lblPrice.MouseLeave+= Leave; lblPrice.Click+= Click;
        lblPlus.MouseEnter += Enter; lblPlus.MouseLeave += Leave; lblPlus.Click += Click;

        return card;
    }

    // ── Cart (right) ──
    private Panel BuildCartPanel()
    {
        var outer = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Surface, Padding = new Padding(10, 8, 10, 8) };

        var lbl = new Label
        {
            Text      = "سلة الطلب",
            Font      = AppTheme.FontHeader,
            ForeColor = AppTheme.TextLight,
            Dock      = DockStyle.Top,
            Height    = 36,
            TextAlign = ContentAlignment.MiddleRight,
        };

        _dgvCart = AppTheme.MakeGrid();
        _dgvCart.Dock = DockStyle.Fill;
        _dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="الصنف",   Name="Name",  AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill });
        _dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="الكمية",  Name="Qty",   Width=60  });
        _dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="السعر",   Name="Price", Width=80  });

        var btnCol = new DataGridViewButtonColumn
        {
            HeaderText = "",
            Name       = "Del",
            Width      = 40,
            Text       = "🗑",
            UseColumnTextForButtonValue = true,
            FlatStyle  = FlatStyle.Flat,
        };
        _dgvCart.Columns.Add(btnCol);

        foreach (DataGridViewColumn c in _dgvCart.Columns)
            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        _dgvCart.CellClick += OnCartCellClick;

        // Qty +/- buttons
        var qtyBar = new Panel { Dock = DockStyle.Bottom, Height = 42, BackColor = AppTheme.Surface };
        var btnPlus = AppTheme.MakeButton("+", AppTheme.Surface3, 55, 32);
        btnPlus.Location = new Point(5, 5);
        btnPlus.Click   += (_, _) => ChangeQty(+1);
        var btnMinus = AppTheme.MakeButton("−", AppTheme.Surface3, 55, 32);
        btnMinus.Location = new Point(65, 5);
        btnMinus.Click   += (_, _) => ChangeQty(-1);
        qtyBar.Controls.AddRange(new Control[] { btnPlus, btnMinus });

        _lblTotal = new Label
        {
            Text      = "المجموع: 0 ر.س",
            Font      = AppTheme.FontHeader,
            ForeColor = AppTheme.Primary,
            Dock      = DockStyle.Bottom,
            Height    = 42,
            TextAlign = ContentAlignment.MiddleRight,
            BackColor = AppTheme.Surface,
            Padding   = new Padding(0, 0, 10, 0),
        };

        outer.Controls.Add(_dgvCart);
        outer.Controls.Add(qtyBar);
        outer.Controls.Add(_lblTotal);
        outer.Controls.Add(lbl);
        return outer;
    }

    // ── Customer info ──
    private Panel BuildInfoPanel()
    {
        var panel = new Panel { Dock = DockStyle.Bottom, BackColor = AppTheme.Surface2, Padding = new Padding(14, 8, 14, 8) };

        var rows = new TableLayoutPanel
        {
            Dock        = DockStyle.Fill,
            ColumnCount = 4,
            RowCount    = 2,
            BackColor   = Color.Transparent,
        };
        rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        _txtName    = MakeTxtBox();
        _txtPhone   = MakeTxtBox();
        _txtAddress = MakeTxtBox();
        _txtNotes   = MakeTxtBox();

        _cmbTable = new ComboBox
        {
            Font      = AppTheme.FontBody,
            BackColor = AppTheme.Surface,
            ForeColor = AppTheme.TextLight,
            FlatStyle = FlatStyle.Flat,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Dock      = DockStyle.Fill,
            Margin    = new Padding(4),
        };
        for (int i = 1; i <= 20; i++) _cmbTable.Items.Add($"طاولة {i}");
        _cmbTable.SelectedIndex = 0;

        rows.Controls.Add(Labeled("اسم العميل",  _txtName),    0, 0);
        rows.Controls.Add(Labeled("رقم الهاتف",  _txtPhone),   1, 0);

        _pnlAddress = Labeled("عنوان التوصيل", _txtAddress);
        _pnlTable   = Labeled("رقم الطاولة",   _cmbTable);

        if (_type == OrderType.DineIn)
        {
            rows.Controls.Add(_pnlTable,   2, 0);
        }
        else
        {
            if (_type == OrderType.Delivery)
                rows.Controls.Add(_pnlAddress, 2, 0);
        }

        rows.Controls.Add(Labeled("ملاحظات", _txtNotes), 3, 0);

        panel.Height = 90;
        panel.Controls.Add(rows);
        return panel;
    }

    private static TextBox MakeTxtBox() => new()
    {
        Font      = AppTheme.FontBody,
        BackColor = AppTheme.Surface,
        ForeColor = AppTheme.TextLight,
        BorderStyle = BorderStyle.FixedSingle,
        Dock      = DockStyle.Fill,
        Margin    = new Padding(4),
    };

    private static Panel Labeled(string lbl, Control ctrl)
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        var l = new Label
        {
            Text      = lbl,
            Font      = AppTheme.FontSmall,
            ForeColor = AppTheme.TextMuted,
            Dock      = DockStyle.Top,
            Height    = 20,
            TextAlign = ContentAlignment.BottomRight,
        };
        ctrl.Dock = DockStyle.Fill;
        p.Controls.Add(ctrl);
        p.Controls.Add(l);
        return p;
    }

    // ───────────────────────────────────────── Cart logic ──
    private void AddToCart(MenuItem item)
    {
        var existing = _cart.FirstOrDefault(x => x.MenuItem.Id == item.Id);
        if (existing != null)
            existing.Quantity++;
        else
            _cart.Add(new OrderItem { MenuItem = item, Quantity = 1 });
        RefreshCart();
    }

    private void ChangeQty(int delta)
    {
        if (_dgvCart.CurrentRow == null) return;
        int idx = _dgvCart.CurrentRow.Index;
        if (idx < 0 || idx >= _cart.Count) return;
        _cart[idx].Quantity += delta;
        if (_cart[idx].Quantity <= 0) _cart.RemoveAt(idx);
        RefreshCart();
    }

    private void OnCartCellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
        if (_dgvCart.Columns[e.ColumnIndex].Name != "Del") return;
        if (e.RowIndex < _cart.Count) _cart.RemoveAt(e.RowIndex);
        RefreshCart();
    }

    private void RefreshCart()
    {
        _dgvCart.Rows.Clear();
        foreach (var ci in _cart)
            _dgvCart.Rows.Add(ci.MenuItem.Name, ci.Quantity, $"{ci.Subtotal:F0} ر.س");
        _lblTotal.Text = $"المجموع:  {_cart.Sum(x => x.Subtotal):F0}  ر.س";
    }

    // ───────────────────────────────────────── Submit ──
    private void OnConfirm(object? sender, EventArgs e)
    {
        if (_cart.Count == 0)
        {
            MessageBox.Show("الرجاء إضافة أصناف للطلب.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_type != OrderType.DineIn && string.IsNullOrWhiteSpace(_txtName.Text))
        {
            MessageBox.Show("الرجاء إدخال اسم العميل.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_type == OrderType.Delivery && string.IsNullOrWhiteSpace(_txtAddress.Text))
        {
            MessageBox.Show("الرجاء إدخال عنوان التوصيل.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var order = new Order
        {
            Type            = _type,
            Items           = new List<OrderItem>(_cart),
            CustomerName    = _txtName.Text.Trim(),
            CustomerPhone   = _txtPhone.Text.Trim(),
            DeliveryAddress = _txtAddress.Text.Trim(),
            Notes           = _txtNotes.Text.Trim(),
            TableNumber     = _type == OrderType.DineIn
                                ? int.Parse((_cmbTable.Text ?? "طاولة 1").Replace("طاولة ", ""))
                                : 0,
        };

        int id = DataStore.AddOrder(order);
        MessageBox.Show($"تم تسجيل الطلب بنجاح!\nرقم الطلب: #{id:D4}",
                        "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
        Close();
    }
}
