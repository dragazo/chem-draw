using System;
using System.Text;
using System.Windows.Forms;

namespace Chemipad
{
    public partial class RenameDialog : Form
    {
        public string Value
        {
            get
            {
                return CharDatabase.ParseToNormal(NameBox.Text);
            }
            set
            {
                NameBox.Text = CharDatabase.ParseToFormat(value);
            }
        }

        public BondAlign Alignment
        {
            get
            {
                if (CenterRadio.Checked)
                    return BondAlign.Center;
                if (RightRadio.Checked)
                    return BondAlign.Right;
                return BondAlign.Left;
            }
            set
            {
                if (value == BondAlign.Center)
                    CenterRadio.Checked = true;
                else if (value == BondAlign.Right)
                    RightRadio.Checked = true;
                else if (value == BondAlign.Left)
                    LeftRadio.Checked = true;
            }
        }

        public RenameDialog()
        {
            InitializeComponent();

            NameBox.KeyDown += (object o, KeyEventArgs e) =>
            {
                if (e.KeyCode == Keys.Return)
                    button1_Click(o, e);
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Value)) Value = string.Empty;
            DialogResult = DialogResult.OK;
        }
    }
}
