// Copyright (c) 2024 Softbery by Paweł Tobis
using Player.Subtiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Player.Subtiles
{
    public class SubtileControl : PictureBox
    {
        [Category("Subtiles")]
        public int Id
        {
            get;
            set;
        }

        [Category("Subtiles")]
        public string StartTime
        {
            get;
            set;
        }

        [Category("Subtiles")]
        public string EndTime
        {
            get;
            set;
        }

        public delegate void TextLinesChangedEvent(object sender, SrtSub subtiles);
        public event TextLinesChangedEvent TextLinesChanged;

        [Category("Subtiles")]
        public string[] TextLines
        {
            get;
            set;
        }

        private SubtilesManager _subtilesManager;
        [Category("Subtiles")]
        public string File { get; set; }

        [Category("Subtiles")]
        public float LineSpace
        {
            get;
            set;
        } = 5;


        [Category("Subtiles")]
        public Color SubtilesColor { get; set; } = ColorTranslator.FromHtml("#FFFFFF");

        /// <summary>
        /// 
        /// </summary>
        public SubtileControl() : base()
        {
            AutoSize = false;
        }

        /// <summary>
        /// On control painting
        /// </summary>
        /// <param name="e">PaintEventArgs</param>
        /*protected override void OnPaint(PaintEventArgs e)
        {
            // Create solid brush
            var brush = new SolidBrush(ForeColor);
            var str_format = new StringFormat(StringFormatFlags.NoWrap);
            str_format.Alignment = StringAlignment.Center;
            
            float height = 0;
            if (_subtilesManager != null)
            {
                if (TextLines.Length > 0)
                {
                    var sizef = e.Graphics.MeasureString(TextLines[0], Font);
                    for (int i = 0; i < TextLines.Length; i++)
                    {
                        var width = this.Width / 2;
                        e.Graphics.DrawString(TextLines[i], Font, brush, width, height, str_format);
                        height += sizef.Height + LineSpace;
                    }
                }
                else
                {
                    e.Graphics.DrawString("", Font, brush, 0, 0);
                }
            }
            base.OnPaint(e);
        }*/

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap bmp = new Bitmap(Width, Height);
            Graphics graphic = Graphics.FromImage(bmp);
            graphic.Clear(Color.Black);

            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.CompositingQuality = CompositingQuality.GammaCorrected;
            graphic.CompositingMode = CompositingMode.SourceOver;
            graphic.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;

            graphic.TextContrast = 3;

            //Font font = new Font("", 20, FontStyle.Bold);
            //Color color = ColorTranslator.FromHtml("#191970");
            int opacity = 20;
            SolidBrush brush = new SolidBrush(Color.FromArgb(opacity, ForeColor));

            //graphic.DrawString("hello", font, brush, 20, 20);
            //var brush = new SolidBrush(ForeColor);
            var str_format = new StringFormat(StringFormatFlags.NoWrap);
            str_format.Alignment = StringAlignment.Center;

            float height = 0;
            if (_subtilesManager != null)
            {
                if (TextLines.Length > 0)
                {
                    var sizef = graphic.MeasureString(TextLines[0], Font);
                    for (int i = 0; i < TextLines.Length; i++)
                    {
                        var width = this.Width / 2;
                        graphic.DrawString(TextLines[i], Font, brush, width, height, str_format);
                        height += sizef.Height + LineSpace;
                    }
                }
                else
                {
                    graphic.DrawString("", Font, brush, 0, 0);
                }
            }
            bmp.MakeTransparent(Color.Black);

            graphic.Save();
            graphic.Dispose();

            e.Graphics.DrawImage(bmp, 0, 0);
            bmp.Dispose();
            base.OnPaint(e);
        }

        private void MakeTransparentBitmap(PaintEventArgs e)
        {
            // Create bitmap object
            Bitmap bitmap = new Bitmap("");

            // Draw bitmap on screen
            e.Graphics.DrawImage(bitmap, 0, 0, Width, Height);

            // Make default transparent color
            bitmap.MakeTransparent();

            // Draw text on bitmap
            var brush = new SolidBrush(ForeColor);
            var str_format = new StringFormat(StringFormatFlags.NoWrap);
            str_format.Alignment = StringAlignment.Center;

            float height = 0;
            if (_subtilesManager != null)
            {
                if (TextLines.Length > 0)
                {
                    var sizef = e.Graphics.MeasureString(TextLines[0], Font);
                    for (int i = 0; i < TextLines.Length; i++)
                    {
                        var width = this.Width / 2;
                        e.Graphics.DrawString(TextLines[i], Font, brush, width, height, str_format);
                        height += sizef.Height + LineSpace;
                    }
                }
                else
                {
                    e.Graphics.DrawString("", Font, brush, 0, 0);
                }
            }

            // Draw transparent bitmap on screen
            e.Graphics.DrawImage(bitmap, bitmap.Width, 0, bitmap.Width, bitmap.Height);
        }

        /// <summary>
        /// Pobiera wszystkie linie napisów w wyznaczonym czasie;
        /// </summary>
        /// <param name="time">String</param>
        /// <returns>Obiekt SrtSub</returns>
        private SrtSub getSubtilesLinesOnTime(string time)
        {
            foreach (var item in _subtilesManager.GetSubtilesByStringTime())
            {
                if (TimeSpanFromString.GetTimeSpan(time) >= TimeSpanFromString.GetTimeSpan(item.Key) && TimeSpanFromString.GetTimeSpan(time) <= TimeSpanFromString.GetTimeSpan(item.Value.EndTime))
                {
                    return item.Value;
                }
            }
            return null;
        }

        private SrtSub currentSub(string time)
        {
            foreach (var item in _subtiles)
            {
                if (TimeSpanFromString.GetTimeSpan(time) >= TimeSpanFromString.GetTimeSpan(item.Key) && TimeSpanFromString.GetTimeSpan(time) <= TimeSpanFromString.GetTimeSpan(item.Value.EndTime))
                {
                    return item.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public void DrawCurrentSubtiles(string time)
        {
            if (_subtilesManager != null)
            {
                if (getSubtilesLinesOnTime(time) != null)
                {
                    //this.TextLines = getSubtilesLinesOnTime(time).Text.ToArray();//getSubtilesLinesOnTime(time).Text.ToArray();
                    this.TextLines = currentSub(time).Text.ToArray();
                }
            }
            Invalidate();
        }

        protected virtual void OnTextLineChange(object sender, SrtSub subtiles)
        {
            if (TextLinesChanged != null)
            {
                //Subtiles = Subtiles[_time].;
                //Invalidate();
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
        }

        public void GetSubtiles()
        {
            //_subtilesManager = new Player.Subtiles.SubtilesManager(File);
            var file = new FileInfo(File);
            if (file.Exists)
            {
                _subtilesManager = new SubtilesManager(File);
                _subtiles = _subtilesManager.GetSubtilesByStringTime();
            }
            else
                _subtilesManager = null;
        }

        public delegate void dlgSubtilesTimeHandler(string current_time);
        public event dlgSubtilesTimeHandler OnSubtilesTimeChanged;

        private Dictionary<string, SrtSub> _subtiles = new Dictionary<string, SrtSub>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current_time"></param>
        protected virtual void SubtilesTimeChanged(string current_time)
        {
            if (OnSubtilesTimeChanged!=null)
            {
                this.TextLines = _subtiles[current_time].Text.ToArray();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SubtilesEventArgs : EventArgs
    {
        public SubtilesManager Subtiles { get; private set; }
        public SrtSub CurrentTime { get; private set; }
        public SubtilesEventArgs(string file) {
            Subtiles = new SubtilesManager(file);
        }
    }
}
