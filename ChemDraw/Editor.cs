using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Chemipad
{
    public partial class Editor : Form
    {
        public Molecule Molecule = new Molecule();

        public static readonly Pen SelectionRectPen = new Pen(Color.RoyalBlue, 1f);

        private bool IgnoreDraw = false;

        private Point ContextPoint;
        public Editor()
        {
            InitializeComponent();

            ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Add Node",(object o,EventArgs e) =>
                    {
                        Molecule.AddNode(ContextPoint, ActionMode.Snap | ActionMode.Log | ActionMode.Update);
                    }),
                new MenuItem("Rotate Selection",(object o,EventArgs e) => { Task temp = Molecule.RotateSelection(); }),
                new MenuItem("Scale Selection",(object o,EventArgs e) => {Task temp = Molecule.ScaleSelection(); }),
                new MenuItem("Insert",new MenuItem[] {
                    new MenuItem("Ring",new MenuItem[] {
                        new MenuItem("3",(object o,EventArgs e) => Molecule.SpawnRing(ContextPoint,50d,3)),
                        new MenuItem("4",(object o,EventArgs e) => Molecule.SpawnRing(ContextPoint,50d,4)),
                        new MenuItem("5",(object o,EventArgs e) => Molecule.SpawnRing(ContextPoint,50d,5)),
                        new MenuItem("6",(object o,EventArgs e) => Molecule.SpawnRing(ContextPoint,50d,6)),
                        new MenuItem("7",(object o,EventArgs e) => Molecule.SpawnRing(ContextPoint,50d,7)),
                        new MenuItem("8",(object o,EventArgs e) => Molecule.SpawnRing(ContextPoint,50d,8)),
                        new MenuItem("9",(object o,EventArgs e) => Molecule.SpawnRing(ContextPoint,50d,9)),
                        new MenuItem("10",(object o,EventArgs e) => Molecule.SpawnRing(ContextPoint,50d,10))
                    }),
                    new MenuItem("Chain",new MenuItem[] {
                        new MenuItem("3",(object o,EventArgs e) => Molecule.SpawnChain(ContextPoint,50d,3)),
                        new MenuItem("4",(object o,EventArgs e) => Molecule.SpawnChain(ContextPoint,50d,4)),
                        new MenuItem("5",(object o,EventArgs e) => Molecule.SpawnChain(ContextPoint,50d,5)),
                        new MenuItem("6",(object o,EventArgs e) => Molecule.SpawnChain(ContextPoint,50d,6)),
                        new MenuItem("7",(object o,EventArgs e) => Molecule.SpawnChain(ContextPoint,50d,7)),
                        new MenuItem("8",(object o,EventArgs e) => Molecule.SpawnChain(ContextPoint,50d,8)),
                        new MenuItem("9",(object o,EventArgs e) => Molecule.SpawnChain(ContextPoint,50d,9)),
                        new MenuItem("10",(object o,EventArgs e) => Molecule.SpawnChain(ContextPoint,50d,10))
                    })
                }),
                new MenuItem("Flip Link",(object o,EventArgs e) =>
                    {
                        NodePair p = Molecule.GetClosestPair(ContextPoint);
                        if(p != null) {
                            Molecule.Snap();
                            Molecule.CurrentSnap._Undo += () => p.Flip();
                            Molecule.CurrentSnap._Redo += () => p.Flip();

                            p.Flip();
                            Invalidate();
                        }
                    }),
                new MenuItem("Remove Link",(object o,EventArgs e) =>
                    {
                        NodePair p = Molecule.GetClosestPair(ContextPoint);
                        if(p != null) {
                            Molecule.Snap();
                            Molecule.CurrentSnap._Undo += () => Molecule.ReturnPair(p);
                            Molecule.CurrentSnap._Redo += () => Molecule.Pairs.Remove(p);

                            Molecule.Pairs.Remove(p);
                            Invalidate();
                        }
                    }),
                new MenuItem("Link Type",new MenuItem[] {
                    new MenuItem("Single",(object o,EventArgs e) => Molecule.SetBondType(ContextPoint, BondType.Single)),
                    new MenuItem("Double",(object o,EventArgs e) => Molecule.SetBondType(ContextPoint, BondType.Double)),
                    new MenuItem("Triple",(object o,EventArgs e) => Molecule.SetBondType(ContextPoint, BondType.Triple)),
                    new MenuItem("Wedge",(object o,EventArgs e) => Molecule.SetBondType(ContextPoint, BondType.Wedge)),
                    new MenuItem("Dash",(object o,EventArgs e) => Molecule.SetBondType(ContextPoint, BondType.Dash)),
                    new MenuItem("Thick",(object o,EventArgs e) => Molecule.SetBondType(ContextPoint, BondType.Thick)),
                    new MenuItem("Semi",(object o,EventArgs e) => Molecule.SetBondType(ContextPoint, BondType.Semi)),
                    new MenuItem("Wavy",(object o,EventArgs e) => Molecule.SetBondType(ContextPoint, BondType.Wavy))
                })
            });

            Molecule.OnUpdate += () => Invalidate();
            Molecule.OnFileChange = () => Rename();

            Molecule.FollowParent = this;
            Molecule.Controls = Controls;

            Molecule.GetPosition = () => PointToClient(MousePosition);
            Molecule.GetMouseButtons = () => MouseButtons;
            Molecule.GetModifierKeys = () => ModifierKeys;

            Molecule.MouseOccupy = () => IgnoreDraw = true;
            Molecule.MouseUnoccupy = () => IgnoreDraw = false;

            ContextMenu.Popup += (object o, EventArgs e) => ContextPoint = PointToClient(MousePosition);

            Settings.OnForceRedraw += Invalidate;

            Rename();
        }

        private void Rename()
        {
            if (Molecule.WorkingPath == string.Empty)
                Text = "Chemipad";
            else
                Text = string.Format("Chemipad - {0}", Molecule.WorkingPath);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.DrawRectangle(SelectionRectPen, Molecule.SelectionRect);

            Node a, b;
            foreach (NodePair pair in Molecule.Pairs)
            {
                a = Molecule.GetNode(pair.A);
                b = Molecule.GetNode(pair.B);

                if (a == null || b == null) continue;

                pair.DrawLink(a.Center, b.Center, g);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;

            g.FillRectangle(Molecule.BackgroundBrush, e.ClipRectangle);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.SaveAs();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.Open();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Molecule.UpToDate) return;

            ClosingSaveDialog d = new ClosingSaveDialog();

            if (d.ShowDialog() == DialogResult.Yes)
                Molecule.Save();
            if (d.DialogResult == DialogResult.Cancel)
                e.Cancel = true;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (IgnoreDraw ||MouseButtons == MouseButtons.Right) return;
            Task temp = Molecule.SelectRectangle();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.ExportAs(this);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.Redo();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.Clear();
        }

        private void mirrorVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.Mirror(MirrorMode.Vertical);
        }

        private void mirrorHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.Mirror(MirrorMode.Horizontal);
        }

        private void cropToSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Molecule.Crop();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplicationSettings ex = new ApplicationSettings();
            ex.ShowDialog();
            ex.Dispose();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Molecule.Selection.Count == 0) return;
            Molecule.Snap();
            
            for (int i = Molecule.Selection.Count - 1; i >= 0; i--)
                Molecule.RemoveNode(Molecule.Selection[i], ActionMode.Log);

            Invalidate();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Node n in Molecule.Nodes)
                Molecule.Select(n);
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = Molecule.Selection.Count - 1; i >= 0; i--)
                Molecule.Deselect(Molecule.Selection[i]);
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Node n in Molecule.Nodes)
                Molecule.ToggleSelect(n);
        }
    }
}
