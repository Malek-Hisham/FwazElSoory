namespace FwazElSoory.Forms;

internal static class AppTheme
{
    public static readonly Color Primary     = Color.FromArgb(230, 81,  0);
    public static readonly Color PrimaryHover= Color.FromArgb(255, 111, 0);
    public static readonly Color BgDark      = Color.FromArgb(22,  22,  22);
    public static readonly Color Surface     = Color.FromArgb(38,  38,  38);
    public static readonly Color Surface2    = Color.FromArgb(52,  52,  52);
    public static readonly Color Surface3    = Color.FromArgb(68,  68,  68);
    public static readonly Color TextLight   = Color.White;
    public static readonly Color TextMuted   = Color.FromArgb(170, 170, 170);
    public static readonly Color Delivery    = Color.FromArgb(21,  101, 192);
    public static readonly Color Pickup      = Color.FromArgb(46,  125, 50);
    public static readonly Color DineIn      = Color.FromArgb(106, 27,  154);
    public static readonly Color Success     = Color.FromArgb(46,  125, 50);
    public static readonly Color Warning     = Color.FromArgb(230, 119, 0);
    public static readonly Color Danger      = Color.FromArgb(183, 28,  28);
    public static readonly Color Ready       = Color.FromArgb(0,   150, 136);

    public static readonly Font FontTitle    = new("Segoe UI", 18f, FontStyle.Bold);
    public static readonly Font FontHeader   = new("Segoe UI", 13f, FontStyle.Bold);
    public static readonly Font FontBody     = new("Segoe UI", 10f);
    public static readonly Font FontBold     = new("Segoe UI", 10f, FontStyle.Bold);
    public static readonly Font FontSmall    = new("Segoe UI", 9f);
    public static readonly Font FontLarge    = new("Segoe UI", 15f, FontStyle.Bold);

    public static Button MakeButton(string text, Color bg, int w = 160, int h = 40)
    {
        var btn = new Button
        {
            Text       = text,
            Size       = new Size(w, h),
            BackColor  = bg,
            ForeColor  = Color.White,
            FlatStyle  = FlatStyle.Flat,
            Font       = FontBold,
            Cursor     = Cursors.Hand,
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bg, 0.15f);
        return btn;
    }

    public static Label MakeLabel(string text, Font font, Color? fg = null)
        => new()
        {
            Text      = text,
            Font      = font,
            ForeColor = fg ?? TextLight,
            AutoSize  = true,
        };

    public static DataGridView MakeGrid()
    {
        var dgv = new DataGridView
        {
            BackgroundColor         = Surface,
            ForeColor               = TextLight,
            GridColor               = Surface3,
            BorderStyle             = BorderStyle.None,
            RowHeadersVisible       = false,
            AllowUserToAddRows      = false,
            AllowUserToDeleteRows   = false,
            ReadOnly                = true,
            SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
            CellBorderStyle         = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersBorderStyle= DataGridViewHeaderBorderStyle.None,
            EnableHeadersVisualStyles = false,
            Font                    = FontBody,
            RowTemplate             = { Height = 46 },
            AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill,
        };
        dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor  = Surface3,
            ForeColor  = TextLight,
            Font       = FontBold,
            Padding    = new Padding(6),
        };
        dgv.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor         = Surface,
            ForeColor         = TextLight,
            SelectionBackColor = Primary,
            SelectionForeColor = Color.White,
            Padding           = new Padding(6),
        };
        dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor          = Surface2,
            SelectionBackColor = Primary,
        };
        return dgv;
    }

    public static Color StatusColor(Models.OrderStatus s) => s switch
    {
        Models.OrderStatus.Pending   => Color.FromArgb(255, 193,  7),
        Models.OrderStatus.Confirmed => Color.FromArgb( 33, 150, 243),
        Models.OrderStatus.Preparing => Color.FromArgb(255, 152,   0),
        Models.OrderStatus.Ready     => Color.FromArgb( 76, 175,  80),
        Models.OrderStatus.Delivered => Color.FromArgb(100, 100, 100),
        Models.OrderStatus.Cancelled => Color.FromArgb(200,  50,  50),
        _                            => Color.White
    };
}
