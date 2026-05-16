using FwazElSoory.Models;

namespace FwazElSoory.Forms;

public class OrderDetailForm : Form
{
    private readonly Order _order;
    private Label _lblStatus = null!;

    public OrderDetailForm(Order order)
    {
        _order = order;
        Build();
    }

    private void Build()
    {
        SuspendLayout();
        Text              = $"تفاصيل الطلب #{_order.Id:D4}";
        Size              = new Size(620, 600);
        StartPosition     = FormStartPosition.CenterParent;
        BackColor         = AppTheme.BgDark;
        RightToLeft       = RightToLeft.Yes;
        RightToLeftLayout = true;
        Font              = AppTheme.FontBody;
        FormBorderStyle   = FormBorderStyle.FixedDialog;
        MaximizeBox       = false;

        // ── Header ──
        var headerColor = _order.Type switch
        {
            OrderType.Delivery => AppTheme.Delivery,
            OrderType.Pickup   => AppTheme.Pickup,
            _                  => AppTheme.DineIn,
        };
        var header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = headerColor };
        header.Controls.Add(new Label
        {
            Text      = $"{_order.TypeDisplay}   —   #{_order.Id:D4}",
            Font      = AppTheme.FontHeader,
            ForeColor = Color.White,
            AutoSize  = true,
            Location  = new Point(15, 18),
        });

        // ── Info ──
        var infoPanel = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = AppTheme.Surface, Padding = new Padding(15, 10, 15, 10) };
        var info = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, BackColor = Color.Transparent };
        info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        string customer = _order.Type == OrderType.DineIn ? $"طاولة {_order.TableNumber}" : _order.CustomerName;
        string phone    = _order.Type == OrderType.DineIn ? "—" : _order.CustomerPhone;
        string address  = _order.Type == OrderType.Delivery ? _order.DeliveryAddress : "—";

        info.Controls.Add(InfoRow("العميل", customer),    0, 0);
        info.Controls.Add(InfoRow("الهاتف", phone),       1, 0);
        info.Controls.Add(InfoRow("العنوان", address),    0, 1);
        info.Controls.Add(InfoRow("الوقت", _order.OrderTime.ToString("yyyy/MM/dd  hh:mm tt")), 1, 1);
        if (!string.IsNullOrWhiteSpace(_order.Notes))
            info.Controls.Add(InfoRow("ملاحظات", _order.Notes), 0, 2);

        infoPanel.Controls.Add(info);

        // ── Status ──
        _lblStatus = new Label
        {
            Text      = _order.StatusDisplay,
            Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
            ForeColor = AppTheme.StatusColor(_order.Status),
            Dock      = DockStyle.Top,
            Height    = 44,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.Surface2,
        };

        // ── Items ──
        var dgv = AppTheme.MakeGrid();
        dgv.Dock = DockStyle.Fill;
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="الصنف",   Name="Name",  AutoSizeMode=DataGridViewAutoSizeColumnMode.Fill });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="الكمية",  Name="Qty",   Width=70 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText="السعر",   Name="Price", Width=100 });
        foreach (DataGridViewColumn c in dgv.Columns)
            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        foreach (var item in _order.Items)
            dgv.Rows.Add(item.MenuItem.Name, item.Quantity, $"{item.Subtotal:F0} ر.س");

        // ── Total ──
        var lblTotal = new Label
        {
            Text      = $"المجموع الكلي:  {_order.Total:F0}  ر.س",
            Font      = AppTheme.FontHeader,
            ForeColor = AppTheme.Primary,
            Dock      = DockStyle.Bottom,
            Height    = 44,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.Surface,
        };

        // ── Action buttons ──
        var btnBar = new Panel { Dock = DockStyle.Bottom, Height = 54, BackColor = AppTheme.Surface2 };
        AddActionButtons(btnBar);

        // ── Assemble ──
        var body = new Panel { Dock = DockStyle.Fill };
        body.Controls.Add(dgv);
        body.Controls.Add(_lblStatus);

        Controls.Add(body);
        Controls.Add(lblTotal);
        Controls.Add(btnBar);
        Controls.Add(infoPanel);
        Controls.Add(header);

        ResumeLayout();
    }

    private void AddActionButtons(Panel bar)
    {
        bar.Controls.Clear();
        int x = 10;

        // Advance status
        var (nextLabel, nextStatus) = _order.Status switch
        {
            OrderStatus.Pending   => ("✅ تأكيد",          OrderStatus.Confirmed),
            OrderStatus.Confirmed => ("🔥 بدء التحضير",   OrderStatus.Preparing),
            OrderStatus.Preparing => ("🟢 جاهز",          OrderStatus.Ready),
            OrderStatus.Ready     => ("📦 تم التوصيل",    OrderStatus.Delivered),
            _                     => (null, _order.Status),
        };

        if (nextLabel != null && _order.Status != OrderStatus.Cancelled)
        {
            var btnNext = AppTheme.MakeButton(nextLabel, AppTheme.Success, 185, 36);
            btnNext.Location = new Point(x, 9);
            btnNext.Click   += (_, _) =>
            {
                _order.Status = nextStatus;
                _lblStatus.Text      = _order.StatusDisplay;
                _lblStatus.ForeColor = AppTheme.StatusColor(_order.Status);
                AddActionButtons(bar);
            };
            bar.Controls.Add(btnNext);
            x += 200;
        }

        // Cancel
        if (_order.Status != OrderStatus.Delivered && _order.Status != OrderStatus.Cancelled)
        {
            var btnCancel = AppTheme.MakeButton("❌ إلغاء الطلب", AppTheme.Danger, 165, 36);
            btnCancel.Location = new Point(x, 9);
            btnCancel.Click   += (_, _) =>
            {
                if (MessageBox.Show("هل تريد إلغاء الطلب؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _order.Status = OrderStatus.Cancelled;
                    _lblStatus.Text      = _order.StatusDisplay;
                    _lblStatus.ForeColor = AppTheme.StatusColor(_order.Status);
                    AddActionButtons(bar);
                }
            };
            bar.Controls.Add(btnCancel);
        }

        // Close
        var btnClose = AppTheme.MakeButton("إغلاق", AppTheme.Surface3, 110, 36);
        btnClose.Anchor   = AnchorStyles.Top | AnchorStyles.Left;
        btnClose.Location = new Point(460, 9);
        btnClose.Click   += (_, _) => Close();
        bar.Controls.Add(btnClose);
    }

    private static Panel InfoRow(string label, string value)
    {
        var p = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
        p.Controls.Add(new Label { Text = value, Font = AppTheme.FontBold, ForeColor = AppTheme.TextLight, AutoSize = true, Location = new Point(0, 22) });
        p.Controls.Add(new Label { Text = label, Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(0, 4) });
        return p;
    }
}
