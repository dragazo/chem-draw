using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace Chemipad
{
    public static class CharDatabase
    {
        public static readonly char[,] Supers = {
            {'0','⁰'},
            {'1','¹'},
            {'2','²'},
            {'3','³'},
            {'4','⁴'},
            {'5','⁵'},
            {'6','⁶'},
            {'7','⁷'},
            {'8','⁸'},
            {'9','⁹'},
            {'+','⁺'},
            {'-','⁻'},
            {'(','⁽'},
            {')','⁾'}
        };
        public static readonly char[,] Subs = {
            {'0','₀'},
            {'1','₁'},
            {'2','₂'},
            {'3','₃'},
            {'4','₄'},
            {'5','₅'},
            {'6','₆'},
            {'7','₇'},
            {'8','₈'},
            {'9','₉'},
            {'+','₊'},
            {'-','₋'},
            {'(','₍'},
            {')','₎'}
        };

        private static readonly char SuperChar = '^';
        private static readonly char SubChar = '_';

        public static string ParseToFormat(string s)
        {
            StringBuilder b = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                for (int j = 0; j < Subs.GetLength(0); j++)
                {
                    if (s[i] == Subs[j, 1])
                    {
                        b.Append(SubChar);
                        b.Append(Subs[j, 0]);
                        goto End;
                    }
                }
                for (int j = 0; j < Supers.GetLength(0); j++)
                {
                    if (s[i] == Supers[j, 1])
                    {
                        b.Append(SuperChar);
                        b.Append(Supers[j, 0]);
                        goto End;
                    }
                }
                b.Append(s[i]);
            End:;
            }

            return b.ToString();
        }

        public static string ParseToNormal(string s)
        {
            StringBuilder b = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == SubChar)
                {
                    if (i == s.Length - 1)
                        i++;
                    else
                        for (int j = 0; j < Subs.GetLength(0); j++)
                        {
                            if (s[i + 1] == Subs[j, 0])
                            {
                                b.Append(Subs[j, 1]);
                                i++;
                                break;
                            }
                        }
                }
                else if (s[i] == SuperChar)
                {
                    if (i == s.Length - 1)
                        i++;
                    else
                        for (int j = 0; j < Supers.GetLength(0); j++)
                        {
                            if (s[i + 1] == Supers[j, 0])
                            {
                                b.Append(Supers[j, 1]);
                                i++;
                                break;
                            }
                        }
                }
                else
                {
                    b.Append(s[i]);
                }
            }

            return b.ToString();
        }
    }

    public static class IO
    {
        public const string Xml = ".cpxml";
        public const string Bin = ".cpbin";

        public const string SaveFileFilter = "Chemipad XML File (.cpxml)|*.cpxml|Chemipad Binary File (.cpbin)|*.cpbin";
        public const string OpenFileFilter = "Chemipad XML File (.cpxml) Chemipad Binary File (.cpbin)|*.cpxml;*.cpbin";

        public const string Png = ".png";
        public const string Jpg = ".jpg";

        public const string ExportFilter = "PNG Image File (.png)|*.png|JPG Image File (.jpg)|*.jpg";

        public static readonly Color Transparent = Color.FromArgb(0, 0, 0, 0);
    }

    public static class Settings
    {
        public const int FollowPeriod = 10;

        public static event Think OnSettingsUpdate;
        public static void TriggerSettingsUpdate()
        {
            if (OnSettingsUpdate != null) OnSettingsUpdate();

            TriggerForceRedraw();
        }

        public static event Think OnForceRedraw;
        private static void TriggerForceRedraw()
        {
            if (OnForceRedraw != null) OnForceRedraw();
        }
    }

    public delegate void Think();
    public delegate Point PositionDelegate();
    public delegate MouseButtons MouseButtonsDelegate();
    public delegate Keys KeysDelegate();

    public static class Extensions
    {
        public static void Add(this Control.ControlCollection c, params Control[] controls)
        {
            foreach (Control thing in controls)
                c.Add(thing);
        }
        public static void Remove(this Control.ControlCollection c, params Control[] controls)
        {
            foreach (Control thing in controls)
                c.Remove(thing);
        }

        public static Point AddNew(this Point me, Point other)
        {
            return new Point(me.X + other.X, me.Y + other.Y);
        }
        public static Point SubNew(this Point me, Point other)
        {
            return new Point(me.X - other.X, me.Y - other.Y);
        }

        public static Point Add(this Point me, Point other)
        {
            me.X += other.X;
            me.Y += other.Y;

            return me;
        }
        public static Point Sub(this Point me, Point other)
        {
            me.X -= other.X;
            me.Y -= other.Y;

            return me;
        }

        public static bool Within(this Point point, Rectangle rect)
        {
            return point.X >= rect.Left && point.X <= rect.Right
                && point.Y >= rect.Top && point.Y <= rect.Bottom;
        }

        public static Point GetCenter(this Rectangle rect)
        {
            return new Point((rect.Right + rect.Left) / 2, (rect.Top + rect.Bottom) / 2);
        }
    }

    public static class ShortHand
    {
        public static bool Confirm()
        {
            ConfirmationDialog d = new ConfirmationDialog();

            DialogResult r = d.ShowDialog();

            d.Dispose();

            return r == DialogResult.Yes;
        }

        public static Rectangle CreateRect(Point a, Point b)
        {
            Rectangle rect = new Rectangle();

            rect.X = Math.Min(a.X, b.X);
            rect.Y = Math.Min(a.Y, b.Y);

            rect.Width = Math.Max(a.X - b.X, b.X - a.X);
            rect.Height = Math.Max(a.Y - b.Y, b.Y - a.Y);

            return rect;
        }

        public static Point CenterMass(IEnumerable<Point> points)
        {
            int x = 0;
            int y = 0;

            int i = 0;
            foreach (Point p in points)
            {
                x += p.X;
                y += p.Y;

                i++;
            }

            return new Point(x / i, y / i);
        }
        public static Rectangle BindingRect(IEnumerable<Point> points)
        {
            Point low = new Point(int.MaxValue, int.MaxValue);
            Point high = new Point(0, 0);
            
            foreach (Point p in points)
            {
                if (p.X < low.X) low.X = p.X;
                if (p.Y < low.Y) low.Y = p.Y;

                if (p.X > high.X) high.X = p.X;
                if (p.Y > high.Y) high.Y = p.Y;
            }

            return CreateRect(low, high);
        }

        public static Size MeasureString(string text, Font font)
        {
            Bitmap map = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(map);

            RectangleF rect = new RectangleF(0f, 0f, float.MaxValue, float.MaxValue);
            StringFormat format = new StringFormat();

            CharacterRange[] ranges = { new CharacterRange(0, text.Length) };
            Region[] regions;

            format.SetMeasurableCharacterRanges(ranges);
            regions = g.MeasureCharacterRanges(text, font, rect, format);

            rect = regions[0].GetBounds(g);

            g.Dispose();
            map.Dispose();
            format.Dispose();

            return new Size((int)rect.Right + 1, (int)rect.Bottom + 1);
        }
    }
}
