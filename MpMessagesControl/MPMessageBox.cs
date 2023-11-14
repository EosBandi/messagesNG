using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using MpMessageControlStoreNS;
using System.IO;

namespace MPMessageControlBoxNS
{
    public enum DisplayFormat
    {
        Folded,
        Unfolded,
        FoldedWithSeverity
    }

    public enum ColorMode
    {
        Dark,
        Light
    }

    public partial class MpMessageControlBox : UserControl, ISupportInitialize
    {

        private TextSource lines;
        private int maxLineLength;
        private int lineInterval;
        private Color paddingBackColor;
        private bool _scrollBars = true;
        private Size _localAutoScrollMinSize;
        private MessagesStore msgStore;
        private Keys lastModifiers;
        private int _zoom = 100;
        private DisplayFormat _dispFormat = DisplayFormat.Folded;
        private Font _originalFont;
        private Form messageForm;
        private ColorMode _colorMode = ColorMode.Dark;
        private SeverityLevels _showSeverity = SeverityLevels.All;

        public event EventHandler linesRefreshed;
        public event EventHandler zoomChanged;

        [DefaultValue(typeof(Font), "Courier New, 9.75")]
        private Font BaseFont { get; set; }

        //Default Color Scheme
        private SolidBrush background = new SolidBrush(Color.Black);
        private SolidBrush infoColor = new SolidBrush(Color.White);
        private SolidBrush warningColor = new SolidBrush(Color.BurlyWood);
        private SolidBrush errorColor = new SolidBrush(Color.Red);

        public Dictionary<SeverityLevels,int> msgCountBySeverity= new Dictionary<SeverityLevels,int>();


        public int MaxMessages { get; set; }

        public ColorMode ColorMode
        {
            get { return _colorMode; }
            set
            {
                _colorMode = value;
                switch (_colorMode)
                {
                    case ColorMode.Dark:
                        background = new SolidBrush(Color.Black);
                        infoColor = new SolidBrush(Color.White);
                        this.BackColor = Color.Black;
                        break;
                    case ColorMode.Light:
                        background = new SolidBrush(Color.White);
                        infoColor = new SolidBrush(Color.Black);
                        this.BackColor = Color.White;
                        break;
                    default:
                        background = new SolidBrush(Color.Black);
                        infoColor = new SolidBrush(Color.White);
                        this.BackColor = Color.Black;
                        break;
                }
                Invalidate();
            }
        }
        public DisplayFormat DisplayFormat
        {
            get { return _dispFormat; }
            set
            {
                _dispFormat = value;
                refreshLines();
            }
        }
        public SeverityLevels ShowSeverity
        {
            get { return _showSeverity; }
            set
            {
                _showSeverity = value;
                refreshLines();
            }
        }
        [Browsable(false)]
        public int Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                DoZoom(_zoom / 100f);
                OnZoomChanged(EventArgs.Empty);

            }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Scollbars visibility.")]
        public bool ShowScrollBars
        {
            get { return _scrollBars; }
            set
            {
                if (value == _scrollBars) return;
                _scrollBars = value;
                //needRecalc = true;
                Invalidate();
            }
        }
        public new Size AutoScrollMinSize
        {
            set
            {
                if (_scrollBars)
                {
                    if (!base.AutoScroll)
                        base.AutoScroll = true;
                    Size newSize = value;
                    base.AutoScrollMinSize = newSize;
                }
                else
                {
                    if (base.AutoScroll)
                        base.AutoScroll = false;
                    base.AutoScrollMinSize = new Size(0, 0);
                    VerticalScroll.Visible = false;
                    HorizontalScroll.Visible = false;
                    VerticalScroll.Maximum = Math.Max(0, value.Height - ClientSize.Height);
                    HorizontalScroll.Maximum = Math.Max(0, value.Width - ClientSize.Width);
                    _localAutoScrollMinSize = value;
                }
            }

            get
            {
                if (_scrollBars)
                    return base.AutoScrollMinSize;
                else
                    //return new Size(HorizontalScroll.Maximum, VerticalScroll.Maximum);
                    return _localAutoScrollMinSize;
            }
        }

        [Description("Interval between lines in pixels")]
        [DefaultValue(0)]
        public int LineInterval
        {
            get { return lineInterval; }
            set
            {
                lineInterval = value;
                SetFont(Font);
                Invalidate();
            }
        }
        [Browsable(false)]
        public int CharWidth { get; set; }
        [Browsable(false)]
        public int CharHeight { get; set; }
        [Browsable(false)]
        public int TextHeight { get; set; }

        [Browsable(true)]
        [Description("Paddings of text area.")]
        public Padding Paddings { get; set; }

        /// <remarks>Use only monospaced font</remarks>
        [DefaultValue(typeof(Font), "Courier New, 9.75")]
        public override Font Font
        {
            get { return BaseFont; }
            set
            {
                _originalFont = (Font)value.Clone();
                SetFont(value);
            }
        }

        [DefaultValue(typeof(Color), "Transparent")]
        [Description("Background color of padding area")]
        public Color PaddingBackColor
        {
            get { return paddingBackColor; }
            set
            {
                paddingBackColor = value;
                Invalidate();
            }
        }
        public Rectangle TextAreaRect
        {
            get
            {
                int rightPaddingStartX = maxLineLength * CharWidth + Paddings.Left + 1;
                rightPaddingStartX = Math.Max(ClientSize.Width - Paddings.Right, rightPaddingStartX);
                int bottomPaddingStartY = TextHeight + Paddings.Top;
                bottomPaddingStartY = Math.Max(ClientSize.Height - Paddings.Bottom, bottomPaddingStartY);
                var top = Math.Max(0, Paddings.Top - 1) - VerticalScroll.Value;
                var left = HorizontalScroll.Value - 2 + Math.Max(0, Paddings.Left - 1);
                var rect = Rectangle.FromLTRB(left, top, rightPaddingStartX - HorizontalScroll.Value, bottomPaddingStartY - VerticalScroll.Value);
                return rect;
            }
        }


        /// ////////////// Methods //////////////

        /// <summary>
        /// Constructor for the Message Control Box
        /// </summary>
        /// 

        public MpMessageControlBox()
        {
            InitializeComponent();
            lines = new TextSource();
            SetFont(new Font("Courier New", 9.75f, FontStyle.Regular, GraphicsUnit.Point));

            base.AutoScroll = true;
            ShowScrollBars = true;
            //drawing optimization

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            //append monospace font

            Font = new Font(FontFamily.GenericMonospace, 9.75f);
            AutoScrollMinSize = new Size(0, TextHeight);

            msgStore = new MessagesStore();

            ColorMode = ColorMode.Dark;
            MaxMessages = 1000;
        }

        public void addMessage(GCSMessage msg)
        {
            //To keep it fast, delete the oldest message if we have more than 1000
            if (msgStore.getCount() > MaxMessages) msgStore.deleteOldestMessage();
            msgStore.AddMessage(msg);
            refreshLines();
        }

        public void deleteAllMessages(SeverityLevels severity)
        {
            //Delete all messages with a given severity
            msgStore.deleteAllMessages(severity);
            refreshLines();
        }

        public void clearIgnoreList()
        {
            msgStore.clearIgnoreList();
            refreshLines();
        }

        public void refreshLines()
        {
            //Lock lines to prevent other threads from accessing it

            lock (lines)
            {
                refreshLinesInternal();
            }
            this.OnLinesRefreshed(EventArgs.Empty);

        }
        internal void refreshLinesInternal()
        {

            lines.Clear();
            //Clear severity counts
            msgCountBySeverity.Clear();
            msgCountBySeverity.Add(SeverityLevels.All, 0);
            msgCountBySeverity.Add(SeverityLevels.Info, 0);
            msgCountBySeverity.Add(SeverityLevels.Warning,0);
            msgCountBySeverity.Add(SeverityLevels.Error,0);
            msgCountBySeverity.Add(SeverityLevels.ErrorAndWarning,0);


            List<GCSMessage> sortedmsg;
            switch (_dispFormat)
            {
                case DisplayFormat.Folded:
                    sortedmsg = msgStore.GetMessagesSortedByLastSeen();
                    break;
                case DisplayFormat.Unfolded:
                    sortedmsg = msgStore.GetUnfoldedMessages();
                    break;
                case DisplayFormat.FoldedWithSeverity:
                    sortedmsg = msgStore.GetMessagesSortedBySeverityAndLastSeen();
                    break;
                default:
                    sortedmsg = msgStore.GetMessagesSortedByLastSeen();
                    break;
            }
            foreach (GCSMessage m in sortedmsg)
            {
                //Ignore the message if it is suppresed
                if (m.Supressed) continue;

                msgCountBySeverity[m.SeverityLevel]++;
                //check if m.SeverityLevel is in the _showSeverity
                if ((_showSeverity & m.SeverityLevel) == 0)
                    continue;

                Line l = new Line() { Msg = m };
                lines.Add(l);
            }
            //
            msgCountBySeverity[SeverityLevels.ErrorAndWarning] = msgCountBySeverity[SeverityLevels.Error] + msgCountBySeverity[SeverityLevels.Warning];
            msgCountBySeverity[SeverityLevels.All] = msgCountBySeverity[SeverityLevels.ErrorAndWarning] + msgCountBySeverity[SeverityLevels.Info];
            RecalcLinePos();
            Invalidate();
        }

        public List<GCSMessage> getLastNMessages(int num)
        {
            //Get the last 10 messages from msgStore
            return msgStore.getLastNMessages(num);
        }


        private void SetFont(Font newFont)
        {
            BaseFont = newFont;
            //check monospace font
            SizeF sizeM = GetCharSize(BaseFont, 'M');
            SizeF sizeDot = GetCharSize(BaseFont, '.');
            if (sizeM != sizeDot)
                BaseFont = new Font("Courier New", BaseFont.SizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
            //clac size
            SizeF size = GetCharSize(BaseFont, 'M');
            CharWidth = (int)Math.Round(size.Width * 1f /*0.85*/) - 1 /*0*/;
            CharHeight = lineInterval + (int)Math.Round(size.Height * 1f /*0.9*/) - 1 /*0*/;
            //
            Invalidate();
        }

        public static SizeF GetCharSize(Font font, char c)
        {
            Size sz2 = TextRenderer.MeasureText("<" + c.ToString() + ">", font);
            Size sz3 = TextRenderer.MeasureText("<>", font);

            return new SizeF(sz2.Width - sz3.Width + 1, /*sz2.Height*/font.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Brush paddingBrush = new SolidBrush(PaddingBackColor);
            //draw padding area
            var textAreaRect = TextAreaRect;
            //top
            e.Graphics.FillRectangle(paddingBrush, 0, -VerticalScroll.Value, ClientSize.Width, Math.Max(0, Paddings.Top));
            //bottom
            e.Graphics.FillRectangle(paddingBrush, 0, textAreaRect.Bottom, ClientSize.Width, ClientSize.Height);
            //right
            e.Graphics.FillRectangle(paddingBrush, textAreaRect.Right, 0, ClientSize.Width, ClientSize.Height);
            //left
            e.Graphics.FillRectangle(paddingBrush, 0, 0, Paddings.Left, ClientSize.Height);

            int firstChar = (Math.Max(0, HorizontalScroll.Value - Paddings.Left)) / CharWidth;
            int lastChar = (HorizontalScroll.Value + ClientSize.Width) / CharWidth;

            int startLine = YtoLineIndex(VerticalScroll.Value);
            if (startLine < 0) startLine = 0;
            int iLine;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;


            //draw text
            for (iLine = startLine; iLine < lines.Count; iLine++)
            {
                Line line = lines[iLine];
                //
                if (line.startY > VerticalScroll.Value + ClientSize.Height)
                    break;

                int y = line.startY - VerticalScroll.Value;
                //


                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var x = -HorizontalScroll.Value;

                SolidBrush textColor;
                switch (line.Msg.SeverityLevel)
                {
                    case SeverityLevels.Info:
                        textColor = infoColor;
                        break;
                    case SeverityLevels.Warning:
                        textColor = warningColor;
                        break;
                    case SeverityLevels.Error:
                        textColor = errorColor;
                        break;
                    default:
                        textColor = infoColor;
                        break;
                }

                String Text = line.Msg.LastSeen.ToString("hh:mm:ss") + "(" + line.Msg.Severity.ToString() + ")[" + line.Msg.occurences.Count.ToString() + "] " + line.Msg.Text;
                e.Graphics.DrawString(Text, Font, textColor, x, y);
            }

            TextHeight = 0;
            maxLineLength = RecalcLinePos();
            int minWidth = (maxLineLength) * CharWidth + 2 + Paddings.Left + Paddings.Right;
            AutoScrollMinSize = new Size(minWidth, TextHeight + Paddings.Top + Paddings.Bottom);
            UpdateScrollbars();

            base.OnPaint(e);
        }

        public void UpdateScrollbars()
        {
            if (ShowScrollBars)
            {
                OnMagicUpdateScrollBars();
            }
            else
                PerformLayout();
        }

        private void OnMagicUpdateScrollBars()
        {
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(OnMagicUpdateScrollBars));
            }
            else
            {
                base.AutoScrollMinSize -= new Size(1, 0);
                base.AutoScrollMinSize += new Size(1, 0);
            }
        }
        public void AddLine(Line line)
        {
            lines.Add(line);
            RecalcLinePos();
            Invalidate();
        }
        public void InsertLineToTop(Line line)
        {
            lines.Insert(0, line);
            RecalcLinePos();
            Invalidate();
        }
        private int RecalcLinePos()
        {
            int maxLineLength = 0;
            TextSource lines = this.lines;
            int count = lines.Count;
            int charHeight = CharHeight;
            int topIndent = Paddings.Top;
            TextHeight = topIndent;

            for (int i = 0; i < count; i++)
            {
                Line l = lines[i];

                int lineLength = l.Msg.Text.Length;
                if (lineLength > maxLineLength)
                    maxLineLength = lineLength;

                l.startY = TextHeight;
                TextHeight += charHeight; // Padding ?
                lines[i] = l;
            }

            TextHeight -= topIndent;

            return maxLineLength;
        }
        public int YtoLineIndex(int y)
        {
            int i = lines.lines.BinarySearch(new Line() { startY = -10 }, new Line.LineYComparer(y));
            i = i < 0 ? -i - 2 : i;
            if (i < 0) return 0;
            if (i > lines.Count - 1) return lines.Count - 1;
            return i;
        }
        public GCSMessage PointToPlace(Point point)
        {
            point.Offset(HorizontalScroll.Value, VerticalScroll.Value);
            point.Offset(Paddings.Left, 0);
            int iLine = YtoLineIndex(point.Y);
            if (iLine < 0) iLine = 0;

            return lines[iLine].Msg;
        }

        public int getClickedLineIndex(Point point)
        {
            point.Offset(HorizontalScroll.Value, VerticalScroll.Value);
            point.Offset(Paddings.Left, 0);

            int i = lines.lines.BinarySearch(new Line() { startY = -10 }, new Line.LineYComparer(point.Y));
            i = i < 0 ? -i - 2 : i;

            //if (i < 0) return 0;  //-1 means we have no lines...
            //Known bug, if clicking past the last line, we will got the last line, deal with it.
            if (i > lines.Count - 1) return lines.Count - 1;


            return i;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {

                if (messageForm?.Visible == true)
                {
                    messageForm.Close();
                    messageForm = null;
                }

                var place = getClickedLineIndex(e.Location);
                if (place < 0) return;
                if (place > lines.Count - 1) return;

                createMessageForm(lines[place].Msg);
            }
        }
        public void ChangeFontSize(int step)
        {
            var points = Font.SizeInPoints;
            using (var gr = Graphics.FromHwnd(Handle))
            {
                var dpi = gr.DpiY;
                var newPoints = points + step * 72f / dpi;
                if (newPoints < 1f) return;
                var k = newPoints / _originalFont.SizeInPoints;
                Zoom = (int)Math.Round(100 * k);
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (Focused)
                lastModifiers = e.Modifiers;
            e.Handled = true;
            Invalidate();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.ShiftKey)
                lastModifiers &= ~Keys.Shift;
            if (e.KeyCode == Keys.Alt)
                lastModifiers &= ~Keys.Alt;
            if (e.KeyCode == Keys.ControlKey)
                lastModifiers &= ~Keys.Control;
        }


        protected override void OnScroll(ScrollEventArgs se)
        {
            OnScroll(se, true);
        }

        public void OnScroll(ScrollEventArgs se, bool alignByLines)
        {
            //HideHints();

            if (se.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                //align by line height
                int newValue = se.NewValue;
                if (alignByLines)
                    newValue = (int)(Math.Ceiling(1d * newValue / CharHeight) * CharHeight);
                //
                VerticalScroll.Value = Math.Max(VerticalScroll.Minimum, Math.Min(VerticalScroll.Maximum, newValue));
            }
            if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                HorizontalScroll.Value = Math.Max(HorizontalScroll.Minimum, Math.Min(HorizontalScroll.Maximum, se.NewValue));

            UpdateScrollbars();

            //RestoreHints();

            Invalidate();
            //
            base.OnScroll(se);
            //OnVisibleRangeChanged();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Invalidate();
            if (lastModifiers == Keys.Control)
            {
                ChangeFontSize(2 * Math.Sign(e.Delta));
                ((HandledMouseEventArgs)e).Handled = true;
            }
            else
            if (VerticalScroll.Visible || !ShowScrollBars)
            {
                DoScrollVertical(1, e.Delta);
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            lastModifiers = Keys.None;
            base.OnLostFocus(e);
            Invalidate();
        }

        private void DoZoom(float koeff)
        {
            //remember first displayed line
            var iLine = YtoLineIndex(VerticalScroll.Value);
            var points = _originalFont.SizeInPoints;
            points *= koeff;

            if (points < 1f || points > 300f) return;

            var oldFont = Font;
            SetFont(new Font(Font.FontFamily, points, Font.Style, GraphicsUnit.Point));
            oldFont.Dispose();
            //restore first displayed line
            if (iLine < lines.Count)
                VerticalScroll.Value = Math.Min(VerticalScroll.Maximum, lines[iLine].startY - Paddings.Top);
            UpdateScrollbars();
            Invalidate();
        }
        private void DoScrollVertical(int countLines, int direction)
        {
            if (VerticalScroll.Visible || !ShowScrollBars)
            {
                int numberOfVisibleLines = ClientSize.Height / CharHeight;

                int offset;
                if ((countLines == -1) || (countLines > numberOfVisibleLines))
                    offset = CharHeight * numberOfVisibleLines;
                else
                    offset = CharHeight * countLines;

                var newScrollPos = VerticalScroll.Value - Math.Sign(direction) * offset;

                var ea =
                    new ScrollEventArgs(direction > 0 ? ScrollEventType.SmallDecrement : ScrollEventType.SmallIncrement,
                                        VerticalScroll.Value,
                                        newScrollPos,
                                        ScrollOrientation.VerticalScroll);

                OnScroll(ea);
            }
        }
        void createMessageForm(GCSMessage msg)
        {

            Label lMessage;
            Label lSeverity;
            Label lOccurences;
            Label lFirstOccurence;
            Label lLastOccurence;
            Label label1;
            Label label2;
            Label label3;
            Label label4;
            Label label5;
            Label lAge;
            Button btnDelete;
            Button btnKeep;
            Button btnIgnore;


            //Create a new form
            messageForm = new Form();

            lMessage = new System.Windows.Forms.Label();
            lSeverity = new System.Windows.Forms.Label();
            lOccurences = new System.Windows.Forms.Label();
            lFirstOccurence = new System.Windows.Forms.Label();
            lLastOccurence = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            lAge = new System.Windows.Forms.Label();
            btnDelete = new System.Windows.Forms.Button();
            btnKeep = new System.Windows.Forms.Button();
            btnIgnore = new System.Windows.Forms.Button();
            // 
            // lMessage
            // 
            lMessage.AutoSize = true;
            lMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lMessage.Location = new System.Drawing.Point(12, 83);
            lMessage.Name = "lMessage";
            lMessage.Size = new System.Drawing.Size(329, 50);
            lMessage.AutoSize = false;
            lMessage.TabIndex = 0;
            lMessage.Text = msg.Text;
            // 
            // lSeverity
            // 
            lSeverity.AutoSize = true;
            lSeverity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lSeverity.Location = new System.Drawing.Point(13, 46);
            lSeverity.Name = "lSeverity";
            lSeverity.Size = new System.Drawing.Size(80, 18);
            lSeverity.TabIndex = 1;
            //Set text to severity level
            lSeverity.Text = msg.SeverityLevel.ToString();

            // 
            // lOccurences
            // 
            lOccurences.AutoSize = true;
            lOccurences.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lOccurences.Location = new System.Drawing.Point(126, 46);
            lOccurences.Name = "lOccurences";
            lOccurences.Size = new System.Drawing.Size(56, 18);
            lOccurences.TabIndex = 2;
            lOccurences.Text = msg.occurences.Count().ToString() + " times";
            // 
            // lFirstOccurence
            // 
            lFirstOccurence.AutoSize = true;
            lFirstOccurence.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lFirstOccurence.Location = new System.Drawing.Point(225, 46);
            lFirstOccurence.Name = "lFirstOccurence";
            lFirstOccurence.Size = new System.Drawing.Size(64, 18);
            lFirstOccurence.TabIndex = 3;
            //get the earliest occurence
            lFirstOccurence.Text = msg.occurences[0].ToString("hh:mm:ss");
            // 
            // lLastOccurence
            // 
            lLastOccurence.AutoSize = true;
            lLastOccurence.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lLastOccurence.Location = new System.Drawing.Point(300, 46);
            lLastOccurence.Name = "lLastOccurence";
            lLastOccurence.Size = new System.Drawing.Size(64, 18);
            lLastOccurence.TabIndex = 4;
            lLastOccurence.Text = msg.LastSeen.ToString("hh:mm:ss");
            // 
            // lAge
            // 
            lAge.AutoSize = true;
            lAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lAge.Location = new System.Drawing.Point(373, 46);
            lAge.Name = "lAge";
            lAge.Size = new System.Drawing.Size(80, 18);
            lAge.TabIndex = 10;
            //Get the age of the last occurence in seconds and minutes
            TimeSpan age = DateTime.Now - msg.LastSeen;
            //show the age in minutes and seconds
            lAge.Text = age.Minutes.ToString() + "m " + age.Seconds.ToString() + "s";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Location = new System.Drawing.Point(299, 18);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(36, 18);
            label1.TabIndex = 8;
            label1.Text = "Last";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label2.Location = new System.Drawing.Point(224, 18);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(37, 18);
            label2.TabIndex = 7;
            label2.Text = "First";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label3.Location = new System.Drawing.Point(125, 18);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(89, 18);
            label3.TabIndex = 6;
            label3.Text = "Occurences";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label4.Location = new System.Drawing.Point(12, 18);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(60, 18);
            label4.TabIndex = 5;
            label4.Text = "Severity";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label5.Location = new System.Drawing.Point(373, 18);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(33, 18);
            label5.TabIndex = 9;
            label5.Text = "Age";
            // 
            // btnDelete
            // 
            btnDelete.Location = new System.Drawing.Point(13, 133);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new System.Drawing.Size(80, 36);
            btnDelete.TabIndex = 11;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += (sender, e) => { msgStore.deleteMessage(msg); refreshLines(); messageForm.Close(); };
            // 
            // btnKeep
            // 
            btnKeep.Location = new System.Drawing.Point(367, 133); ;
            btnKeep.Name = "btnKeep";
            btnKeep.Size = new System.Drawing.Size(86, 37);
            btnKeep.TabIndex = 12;
            btnKeep.Text = "Keep";
            btnKeep.Click += (sender, e) => { messageForm.Close(); };
            btnKeep.UseVisualStyleBackColor = true;
            // 
            // btnIgnore
            // 
            btnIgnore.Location = new System.Drawing.Point(111, 133);
            btnIgnore.Name = "btnIgnore";
            btnIgnore.Size = new System.Drawing.Size(80, 36);
            btnIgnore.TabIndex = 13;
            btnIgnore.Text = "Ignore";
            btnIgnore.UseVisualStyleBackColor = true;
            btnIgnore.Click += (sender, e) => { msgStore.ignoreMessage(msg); refreshLines(); messageForm.Close(); };

            messageForm.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            messageForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            messageForm.ClientSize = new System.Drawing.Size(479, 197);
            messageForm.StartPosition = FormStartPosition.CenterScreen;
            messageForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            messageForm.MaximizeBox = false;
            messageForm.MinimizeBox = false;
            messageForm.ShowIcon = false;
            messageForm.ShowInTaskbar = false;
            messageForm.TopMost = true;

            messageForm.Controls.Add(btnKeep);
            messageForm.Controls.Add(btnDelete);
            messageForm.Controls.Add(btnIgnore);
            messageForm.Controls.Add(lAge);
            messageForm.Controls.Add(label5);
            messageForm.Controls.Add(label1);
            messageForm.Controls.Add(label2);
            messageForm.Controls.Add(label3);
            messageForm.Controls.Add(label4);
            messageForm.Controls.Add(lLastOccurence);
            messageForm.Controls.Add(lFirstOccurence);
            messageForm.Controls.Add(lOccurences);
            messageForm.Controls.Add(lSeverity);
            messageForm.Controls.Add(lMessage);
            messageForm.Name = "messageForm";
            messageForm.Text = "Message Details";

            messageForm.Show();
        }

        protected virtual void OnLinesRefreshed(EventArgs e)
        {
            EventHandler handler = this.linesRefreshed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnZoomChanged(EventArgs e)
        {
            EventHandler handler = this.zoomChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void BeginInit()
        {
        }

        public void EndInit()
        {
        }
    }
}
