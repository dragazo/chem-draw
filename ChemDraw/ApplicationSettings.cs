using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chemipad
{
    public partial class ApplicationSettings : Form
    {
        public Color ExportDrawingColor
        {
            get
            {
                return DrawingColorDisplay.Color;
            }
            set
            {
                DrawingColorDisplay.Color = value;
            }
        }
        public Color ExportBackgroundColor
        {
            get
            {
                return BackgroundColorDisplay.Color;
            }
            set
            {
                BackgroundColorDisplay.Color = value;
            }
        }
        public bool ExportTransparentBackground
        {
            get
            {
                return TransparentCheck.Checked;
            }
            set
            {
                TransparentCheck.Checked = value;
            }
        }

        public Color DisplayUnselectedColor
        {
            get
            {
                return UnselectedColorDisplay.Color;
            }
            set
            {
                UnselectedColorDisplay.Color = value;
            }
        }
        public Color DisplaySelectedColor
        {
            get
            {
                return SelectedColorDisplay.Color;
            }
            set
            {
                SelectedColorDisplay.Color = value;
            }
        }

        public ApplicationSettings()
        {
            InitializeComponent();

            GetCurrentSettings();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DrawingColor = ExportDrawingColor;
            Properties.Settings.Default.BackgroundColor = ExportBackgroundColor;
            Properties.Settings.Default.TransparentBackground = ExportTransparentBackground;

            Properties.Settings.Default.UnselectedColor = DisplayUnselectedColor;
            Properties.Settings.Default.SelectedColor = DisplaySelectedColor;

            Properties.Settings.Default.Save();
            Settings.TriggerSettingsUpdate();

            DialogResult = DialogResult.OK;
        }

        private void GetCurrentSettings()
        {
            ExportDrawingColor = Properties.Settings.Default.DrawingColor;
            ExportBackgroundColor = Properties.Settings.Default.BackgroundColor;
            ExportTransparentBackground = Properties.Settings.Default.TransparentBackground;

            DisplayUnselectedColor = Properties.Settings.Default.UnselectedColor;
            DisplaySelectedColor = Properties.Settings.Default.SelectedColor;
        }

        private void DefaultsButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();

            Properties.Settings.Default.Save();
            Settings.TriggerSettingsUpdate();

            DialogResult = DialogResult.OK;
        }
    }
}
