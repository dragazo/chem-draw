using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Media;

namespace Chemipad
{
    public class Vector2
    {
        public double X, Y;

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double SqrMagnitude
        {
            get
            {
                return X * X + Y * Y;
            }
        }

        public double Magnitude
        {
            get
            {
                return Math.Sqrt(SqrMagnitude);
            }
        }

        public Vector2 Unit
        {
            get
            {
                return this / Magnitude;
            }
        }

        public double PolarAngle
        {
            get
            {
                return Y >= 0 ? Angle(Right) : -Angle(Right);
            }
        }

        public static Vector2 FromPolar(double angle, double mag)
        {
            return new Vector2(mag * Math.Cos(angle), mag * Math.Sin(angle));
        }

        public double Angle(Vector2 other)
        {
            return Math.Acos((X * other.X + Y * other.Y) / (Magnitude * other.Magnitude));
        }

        public static Vector2 operator -(Vector2 vector)
        {
            return new Vector2(-vector.X, -vector.Y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }
        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator *(Vector2 a, double b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }
        public static Vector2 operator *(double a, Vector2 b)
        {
            return new Vector2(a * b.X, a * b.Y);
        }

        public static Vector2 operator /(Vector2 a, double b)
        {
            return new Vector2(a.X / b, a.Y / b);
        }

        public static explicit operator Vector2(Point point)
        {
            return new Vector2(point.X, point.Y);
        }
        public static explicit operator Point(Vector2 vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }

        public static readonly Vector2 Right = new Vector2(1d, 0d);
        public static readonly Vector2 Up = new Vector2(0d, 1d);

        public static readonly Vector2 Left = new Vector2(-1d, 0d);
        public static readonly Vector2 Down = new Vector2(0d, -1d);
    }

    [Serializable]
    public class MoleculePersist
    {
        public NodePersist[] Nodes;
        public NodePairPersist[] Pairs;
    }
    #region Molecule
    //main
    public partial class Molecule
    {
        public List<Node> Nodes = new List<Node>();
        public List<NodePair> Pairs = new List<NodePair>();

        private Control _FollowParent;
        public Control FollowParent
        {
            get
            {
                return _FollowParent;
            }
            set
            {
                _FollowParent = value;

                foreach (Node point in Nodes)
                    point.FollowParent = _FollowParent;
            }
        }

        private Control.ControlCollection _Controls;
        public Control.ControlCollection Controls
        {
            get
            {
                return _Controls;
            }
            set
            {
                if (_Controls != null)
                    foreach (Node point in Nodes)
                        _Controls.Remove(point);

                _Controls = value;

                if (_Controls != null)
                    foreach (Node point in Nodes)
                        _Controls.Add(point);
            }
        }

        public PositionDelegate GetPosition;
        public MouseButtonsDelegate GetMouseButtons;
        public KeysDelegate GetModifierKeys;
        public Think MouseOccupy, MouseUnoccupy;

        public Think OnUpdate;
        public void TriggerUpdate()
        {
            if (OnUpdate != null) OnUpdate();
        }

        public static uint _ID = 0;
        public static uint NextID
        {
            get
            {
                return _ID++;
            }
            set
            {
                _ID = value;
            }
        }

        public Molecule()
        {
            XmlSettings = new XmlWriterSettings();
            XmlSettings.Indent = true;
            XmlSettings.NewLineHandling = NewLineHandling.Entitize;
            XmlSettings.OmitXmlDeclaration = true;

            XmlNamespaces = new XmlSerializerNamespaces();
            XmlNamespaces.Add(string.Empty, string.Empty);

            XmlSerializer = new XmlSerializer(typeof(MoleculePersist));

            BinaryFormatter = new BinaryFormatter();

            ClearSnaps();

            Settings.OnForceRedraw += () =>
            {
                foreach (Node n in Nodes)
                    n.Invalidate();
            };
        }

        static Molecule()
        {
            UpdateBrushes();

            Settings.OnSettingsUpdate += UpdateBrushes;
        }
    }

    //utility
    public partial class Molecule
    {
        public Node AddNode(Point spot, ActionMode mode, Node from = null, BondType type = BondType.Single)
        {
            Node temp = new Node();
            temp.ID = NextID;
            temp.Center = spot;

            temp.Owner = this;

            Nodes.Add(temp);
            if (Controls != null) Controls.Add(temp);

            temp.FollowParent = FollowParent;

            ContextMenu menu = new ContextMenu(new MenuItem[] {
                new MenuItem("Add",new MenuItem[] {
                    new MenuItem("Single",(object o,EventArgs e) => SpawnNode(temp,BondType.Single)),
                    new MenuItem("Double",(object o,EventArgs e) => SpawnNode(temp,BondType.Double)),
                    new MenuItem("Triple",(object o,EventArgs e) => SpawnNode(temp,BondType.Triple)),
                    new MenuItem("Wedge",(object o,EventArgs e) => SpawnNode(temp,BondType.Wedge)),
                    new MenuItem("Dash",(object o,EventArgs e) => SpawnNode(temp,BondType.Dash)),
                    new MenuItem("Thick",(object o,EventArgs e) => SpawnNode(temp,BondType.Thick)),
                    new MenuItem("Semi",(object o,EventArgs e) => SpawnNode(temp,BondType.Semi)),
                    new MenuItem("Wavy",(object o,EventArgs e) => SpawnNode(temp,BondType.Wavy))
                    }),
                new MenuItem("Remove",(object o,EventArgs e) =>
                    {
                        if(Selection.Contains(temp)) {
                            Snap();

                            for(int i = Selection.Count - 1; i >= 0; i--)
                                RemoveNode(Selection[i], ActionMode.Log);
                            
                            TriggerUpdate();
                        }
                        else
                            RemoveNode(temp,ActionMode.Snap | ActionMode.Log | ActionMode.Update);
                    }),
                new MenuItem("Clone",(object o,EventArgs e) =>
                    {
                        if(Selection.Contains(temp))
                            CloneSelection(GetPosition != null ? GetPosition() : Point.Empty, temp);
                        else
                            CloneNode(GetPosition != null ? GetPosition() : Point.Empty, temp);
                    }),
                new MenuItem("Merge",(object o,EventArgs e) => Merge(temp)),
                new MenuItem("Link",new MenuItem[] {
                    new MenuItem("Single",(object o,EventArgs e) => Link(temp,BondType.Single)),
                    new MenuItem("Double",(object o,EventArgs e) => Link(temp,BondType.Double)),
                    new MenuItem("Triple",(object o,EventArgs e) => Link(temp,BondType.Triple)),
                    new MenuItem("Wedge",(object o,EventArgs e) => Link(temp,BondType.Wedge)),
                    new MenuItem("Dash",(object o,EventArgs e) => Link(temp,BondType.Dash)),
                    new MenuItem("Thick",(object o,EventArgs e) => Link(temp,BondType.Thick)),
                    new MenuItem("Semi",(object o,EventArgs e) => Link(temp,BondType.Semi)),
                    new MenuItem("Wavy",(object o,EventArgs e) => Link(temp,BondType.Wavy))
                    }),
                new MenuItem("Unlink",new MenuItem[] {
                    new MenuItem("All",(object o,EventArgs e) => Unlink(temp,TargetMode.All)),
                    new MenuItem("Closest",(object o,EventArgs e) => Unlink(temp,TargetMode.Closest))
                    }),
                new MenuItem("Select Attached", (object o,EventArgs e) => SelectAttached(temp))
            });

            if ((mode & ActionMode.Snap) == ActionMode.Snap)
                Snap();

            if ((mode & ActionMode.Log) == ActionMode.Log)
            {
                CurrentSnap._Undo += () => RemoveNode(temp, ActionMode.None);
                CurrentSnap._Redo += () => ReturnNode(temp);
            }

            temp.ContextMenu = menu;
            if (from != null)
            {
                NodePair bond = new NodePair(from.ID, temp.ID, type);
                Pairs.Add(bond);

                if ((mode & ActionMode.Log) == ActionMode.Log)
                {
                    CurrentSnap._Undo += () => Pairs.Remove(bond);
                    CurrentSnap._Redo += () => ReturnPair(bond);
                }
            }

            if ((mode & ActionMode.Update) == ActionMode.Update) TriggerUpdate();

            return temp;
        }
        public void RemoveNode(Node thing, ActionMode mode)
        {
            Nodes.Remove(thing);
            Selection.Remove(thing);

            Controls.Remove(thing);

            if ((mode & ActionMode.Snap) == ActionMode.Snap)
                Snap();

            if ((mode & ActionMode.Log) == ActionMode.Log)
            {
                CurrentSnap._Undo += () => ReturnNode(thing);
                CurrentSnap._Redo += () => RemoveNode(thing, ActionMode.None);
            }

            for (int i = Pairs.Count - 1; i >= 0; i--)
            {
                NodePair p = Pairs[i];
                if (p.Contains(thing.ID))
                {
                    if ((mode & ActionMode.Log) == ActionMode.Log)
                    {
                        CurrentSnap._Undo += () => ReturnPair(p);
                        CurrentSnap._Redo += () => Pairs.Remove(p);
                    }
                    Pairs.RemoveAt(i);
                }
            }

            if ((mode & ActionMode.Update) == ActionMode.Update) TriggerUpdate();
        }

        public void ReturnNode(Node node)
        {
            Nodes.Add(node);
            Controls.Add(node);
        }
        public void ReturnPair(NodePair pair)
        {
            Pairs.Add(pair);
            TriggerUpdate();
        }

        //shorthand for AddNode
        public Node SpawnNode(Node from, BondType type)
        {
            Node n = AddNode(GetPosition != null ? GetPosition() : Point.Empty,
                ActionMode.Snap | ActionMode.Log | ActionMode.Update, from, type);

            return n;
        }

        public void SpawnRing(Point spot, double radius, int num)
        {
            if (num < 3) return;

            ClearSelection();
            Node[] nodes = new Node[num];
            NodePair[] pairs = new NodePair[num];

            for (int i = 0; i < num; i++)
            {
                Point radial = new Point((int)Math.Round(radius * Math.Cos(i * 2 * Math.PI / num - Math.PI / 2)),
                    (int)Math.Round(radius * Math.Sin(i * 2 * Math.PI / num - Math.PI / 2)));
                nodes[i] = AddNode(radial.Add(spot), ActionMode.None);

                Select(nodes[i]);
            }

            for (int i = 0; i < nodes.Length - 1; i++)
            {
                pairs[i] = new NodePair(nodes[i + 1].ID, nodes[i].ID, BondType.Single);
                Pairs.Add(pairs[i]);
            }
            pairs[pairs.Length - 1] = new NodePair(nodes[0].ID, nodes[nodes.Length - 1].ID, BondType.Single);
            Pairs.Add(pairs[pairs.Length - 1]);

            Snap();

            foreach (Node n in nodes)
            {
                CurrentSnap._Undo += () => RemoveNode(n, ActionMode.None);
                CurrentSnap._Redo += () => ReturnNode(n);
            }

            foreach (NodePair p in pairs)
            {
                CurrentSnap._Undo += () => Pairs.Remove(p);
                CurrentSnap._Redo += () => ReturnPair(p);
            }

            TriggerUpdate();
        }
        public void SpawnChain(Point spot, double length, int num)
        {
            if (num < 3) return;

            ClearSelection();
            Node[] nodes = new Node[num];
            NodePair[] pairs = new NodePair[num - 1];

            Point delta = new Point((int)Math.Round(length * Math.Cos(Math.PI / 12)),
                (int)Math.Round(length * Math.Sin(Math.PI / 12)));
            bool up = true;

            for (int i = 0; i < num; i++)
            {
                nodes[i] = AddNode(spot.AddNew(new Point(i * delta.X, up ? delta.Y : -delta.Y)), ActionMode.None);
                Selection.Add(nodes[i]);

                up = !up;
            }

            for (int i = 0; i < nodes.Length - 1; i++)
            {
                pairs[i] = new NodePair(nodes[i + 1].ID, nodes[i].ID, BondType.Single);
                Pairs.Add(pairs[i]);
            }

            Snap();

            foreach (Node n in nodes)
            {
                CurrentSnap._Undo += () => RemoveNode(n, ActionMode.None);
                CurrentSnap._Redo += () => ReturnNode(n);
            }

            foreach (NodePair p in pairs)
            {
                CurrentSnap._Undo += () => Pairs.Remove(p);
                CurrentSnap._Redo += () => ReturnPair(p);
            }

            TriggerUpdate();
        }

        public Node CloneNode(Point spot, Node from, bool background = false)
        {
            Node temp = AddNode(spot, ActionMode.None);

            temp.NodeName = from.NodeName;
            temp.Alignment = from.Alignment;

            temp.Center = spot;

            if (!background)
            {
                Snap();
                CurrentSnap._Undo += () => RemoveNode(temp, ActionMode.None);
                CurrentSnap._Redo += () => ReturnNode(temp);

                TriggerUpdate();
            }

            return temp;
        }
        public void CloneSelection(Point spot, Node from)
        {
            if (Selection.Count == 0) return;

            Node[] temp = new Node[Selection.Count];

            Snap();

            for (int i = 0; i < Selection.Count; i++)
            {
                Node n = CloneNode(spot.AddNew(Selection[i].Center).Sub(from.Center), Selection[i], true);
                temp[i] = n;

                CurrentSnap._Undo += () => RemoveNode(n, ActionMode.None);
                CurrentSnap._Redo += () => ReturnNode(n);
            }

            for (int i = Pairs.Count - 1; i >= 0; i--)
            {
                for (int x = 0; x < Selection.Count; x++)
                {
                    for (int y = 0; y < Selection.Count; y++)
                    {
                        if (Pairs[i].A == Selection[x].ID && Pairs[i].B == Selection[y].ID)
                        {
                            NodePair p = new NodePair(temp[x].ID, temp[y].ID, Pairs[i].Type);
                            Pairs.Add(p);

                            CurrentSnap._Undo += () => Pairs.Remove(p);
                            CurrentSnap._Redo += () => ReturnPair(p);
                        }
                    }
                }
            }

            ClearSelection();

            foreach (Node n in temp)
            {
                Selection.Add(n);
                n.Invalidate();
            }

            TriggerUpdate();
        }

        public void Clear()
        {
            Snap();

            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                RemoveNode(Nodes[i], ActionMode.Log);
            }

            TriggerUpdate();
        }
        public void Crop()
        {
            Snap();

            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                if (!Selection.Contains(Nodes[i])) RemoveNode(Nodes[i], ActionMode.Log);
            }

            TriggerUpdate();
        }

        public Node GetClosestNode(Node from, IEnumerable<Node> list = null)
        {
            Node other = null;
            int dist = int.MaxValue;

            if (list == null)
                list = from thing in Nodes
                       select thing;

            foreach (Node thing in list)
            {
                if (thing == from) continue;

                int d = (thing.Center.X - from.Center.X) * (thing.Center.X - from.Center.X)
                    + (thing.Center.Y - from.Center.Y) * (thing.Center.Y - from.Center.Y);

                if (d < dist)
                {
                    dist = d;
                    other = thing;
                }
            }

            return other;
        }
        public NodePair GetClosestPair(Point from, IEnumerable<NodePair> list = null)
        {
            NodePair other = null;
            int dist = int.MaxValue;

            if (list == null)
                list = from thing in Pairs
                       select thing;

            foreach (NodePair thing in list)
            {
                Node a = GetNode(thing.A);
                Node b = GetNode(thing.B);

                Point c = new Point(a.Center.X + (b.Center.X - a.Center.X) / 2, a.Center.Y + (b.Center.Y - a.Center.Y) / 2);

                int d = (c.X - from.X) * (c.X - from.X)
                    + (c.Y - from.Y) * (c.Y - from.Y);

                if (d < dist)
                {
                    dist = d;
                    other = thing;
                }
            }

            return other;
        }

        public void Merge(Node from)
        {
            Node other = GetClosestNode(from);
            if (other == null) return;

            Snap();

            for (int i = Pairs.Count - 1; i >= 0; i--)
            {
                if (!Pairs[i].Contains(other.ID)) continue;
                NodePair pair = Pairs[i];

                if (Pairs[i].A == other.ID)
                {
                    uint orig = pair.A;
                    pair.A = from.ID;

                    CurrentSnap._Undo += () => pair.A = orig;
                    CurrentSnap._Redo += () => pair.A = from.ID;
                }
                if (Pairs[i].B == other.ID)
                {
                    uint orig = pair.B;
                    pair.B = from.ID;

                    CurrentSnap._Undo += () => pair.B = orig;
                    CurrentSnap._Redo += () => pair.B = from.ID;
                }
            }

            RemoveNode(other, ActionMode.Log);

            CurrentSnap._Undo += () => ReturnNode(other);
            CurrentSnap._Redo += () => RemoveNode(other, ActionMode.None);

            TriggerUpdate();
        }

        public void Link(Node node, BondType type)
        {
            IEnumerable<Node> list =
                Nodes.Where(n => !Linked(n, node));

            Node other = GetClosestNode(node, list);
            if (other == null) return;

            Snap();

            NodePair p = new NodePair(node.ID, other.ID, type);
            Pairs.Add(p);

            CurrentSnap._Undo += () => Pairs.Remove(p);
            CurrentSnap._Redo += () => ReturnPair(p);

            TriggerUpdate();
        }
        public void Unlink(Node node, TargetMode mode)
        {
            if (!Pairs.Select(p => p.Contains(node.ID)).Any(b => b)) return;

            Snap();

            if (mode == TargetMode.All)
            {
                for (int i = Pairs.Count - 1; i >= 0; i--)
                    if (Pairs[i].Contains(node.ID))
                    {
                        NodePair pair = Pairs[i];

                        CurrentSnap._Undo += () => ReturnPair(pair);
                        CurrentSnap._Redo += () => Pairs.Remove(pair);

                        Pairs.RemoveAt(i);
                    }
            }
            else if (mode == TargetMode.Closest)
            {
                IEnumerable<Node> list = from thing in Pairs
                                         where thing.Contains(node.ID)
                                         select thing.A != node.ID ? GetNode(thing.A) : GetNode(thing.B);

                Node closest = GetClosestNode(node, list);
                if (closest == null) return;

                for (int i = Pairs.Count - 1; i >= 0; i--)
                    if (Pairs[i].Contains(closest.ID) && Pairs[i].Contains(node.ID))
                    {
                        NodePair pair = Pairs[i];

                        CurrentSnap._Undo += () => ReturnPair(pair);
                        CurrentSnap._Redo += () => Pairs.Remove(pair);

                        Pairs.RemoveAt(i);
                    }
            }

            TriggerUpdate();
        }

        private bool Linked(Node a, Node b)
        {
            foreach (NodePair pair in Pairs)
                if (pair.Contains(a.ID) && pair.Contains(b.ID)) return true;
            return false;
        }
        private bool Linked(uint a, uint b)
        {
            foreach (NodePair pair in Pairs)
                if (pair.Contains(a) && pair.Contains(b)) return true;
            return false;
        }

        public Node GetNode(uint ID)
        {
            foreach (Node node in Nodes)
                if (node.ID == ID) return node;
            return null;
        }

        public void SetBondType(Point where, BondType type)
        {
            NodePair p = GetClosestPair(where);
            if (p != null)
            {
                BondType orig = p.Type;

                Snap();
                CurrentSnap._Undo += () => p.Type = orig;
                CurrentSnap._Redo += () => p.Type = type;

                p.Type = type;
                TriggerUpdate();
            }
        }
    }

    //selection
    public partial class Molecule
    {
        public List<Node> Selection = new List<Node>();

        public void Mirror(MirrorMode mode)
        {
            if (Selection.Count < 2) return;

            Point c = ShortHand.BindingRect(Selection.Select(n => n.Center)).GetCenter();

            Snap();

            foreach (Node n in Selection)
            {
                Point orig = n.Location;
                Point temp = n.Center.SubNew(c);

                CurrentSnap._Undo += () => n.Location = orig;
                if (mode == MirrorMode.Horizontal)
                {
                    temp = c.AddNew(new Point(-temp.X, temp.Y));

                    n.Center = temp;
                    CurrentSnap._Redo += () => n.Center = temp;
                }
                else
                {
                    temp = c.AddNew(new Point(temp.X, -temp.Y));

                    n.Center = temp;
                    CurrentSnap._Redo += () => n.Center = temp;
                }

                n.Invalidate();
            }

            TriggerUpdate();
        }

        public void Select(Rectangle rect)
        {
            foreach (Node n in Nodes)
                if (n.Center.Within(rect) && !Selection.Contains(n))
                {
                    Selection.Add(n);
                    n.Invalidate();
                }
        }

        public void Select(Node n)
        {
            if (Selection.Contains(n)) return;
            Selection.Add(n);
            n.Invalidate();
        }
        public void Select(IEnumerable<Node> list)
        {
            foreach (Node n in list)
                Select(n);
        }

        public void Deselect(Node n)
        {
            Selection.Remove(n);
            n.Invalidate();
        }
        public void Deselect(IEnumerable<Node> list)
        {
            foreach (Node n in list)
                Deselect(n);
        }

        public void ToggleSelect(Node n)
        {
            if (Selection.Contains(n))
                Selection.Remove(n);
            else
                Selection.Add(n);

            n.Invalidate();
        }
        public void ClearSelection()
        {
            Selection.Clear();
            foreach (Node n in Nodes)
                n.Invalidate();
        }

        public async Task FollowSelection()
        {
            if (Selection.Count == 0 || GetPosition == null || GetMouseButtons == null) return;

            Point origMouse = GetPosition();
            Point[] orig = new Point[Selection.Count];

            for (int i = 0; i < Selection.Count; i++)
            {
                orig[i] = Selection[i].Location;
                Selection[i].GrabPoint = orig[i].SubNew(origMouse);
            }

            Point raw;
            while (GetMouseButtons() == MouseButtons.Left)
            {
                await Task.Delay(Settings.FollowPeriod);

                raw = GetPosition();
                foreach (Node node in Selection)
                    node.Location = node.GrabPoint.AddNew(raw);

                TriggerUpdate();
            }

            if (origMouse == GetPosition()) return;

            Snap();

            for (int i = 0; i < Selection.Count; i++)
            {
                Node n = Selection[i];
                Point now = n.Location;
                int _i = i;

                CurrentSnap._Undo += () => n.Location = orig[_i];
                CurrentSnap._Redo += () => n.Location = now;
            }
        }

        public async Task RotateSelection()
        {
            if (GetPosition == null || GetMouseButtons == null) return;
            if (Selection.Count < 2) return;

            if (MouseOccupy != null) MouseOccupy();

            Point init = GetPosition();

            Snap();

            Point c = ShortHand.BindingRect(Selection.Select(n => n.Center)).GetCenter();
            Vector2[] list = new Vector2[Selection.Count];
            for (int i = 0; i < Selection.Count; i++)
            {
                Node node = Selection[i];

                Point then = node.Location;
                CurrentSnap._Undo += () =>
                {
                    node.Location = then;
                    Select(node);
                };

                list[i] = (Vector2)node.Center.SubNew(c);
            }

            double initangle = ((Vector2)init.SubNew(c)).PolarAngle;
            while (GetMouseButtons() != MouseButtons.Left)
            {
                await Task.Delay(Settings.FollowPeriod);

                double theta = initangle - ((Vector2)GetPosition().Sub(c)).PolarAngle;
                for (int i = 0; i < Selection.Count; i++)
                    Selection[i].Center = c.AddNew(new Point(
                        (int)Math.Round(list[i].Magnitude * Math.Cos(list[i].PolarAngle - theta)),
                        (int)Math.Round(list[i].Magnitude * Math.Sin(list[i].PolarAngle - theta))
                        ));
                TriggerUpdate();
            }

            foreach (Node n in Selection)
            {
                Point now = n.Location;
                CurrentSnap._Redo += () =>
                {
                    n.Location = now;
                    Select(n);
                };
            }

            if (MouseUnoccupy != null) MouseUnoccupy();
        }

        public async Task ScaleSelection()
        {
            if (GetPosition == null || GetMouseButtons == null) return;
            if (Selection.Count < 2) return;

            if (MouseOccupy != null) MouseOccupy();

            Point init = GetPosition();

            Snap();

            Point c = ShortHand.BindingRect(Selection.Select(n => n.Center)).GetCenter();
            Vector2[] list = new Vector2[Selection.Count];
            for (int i = 0; i < Selection.Count; i++)
            {
                Node node = Selection[i];

                Point then = node.Location;
                CurrentSnap._Undo += () =>
                {
                    node.Location = then;
                    Select(node);
                };

                list[i] = ((Vector2)node.Center.SubNew(c));
            }

            double initdist = ((Vector2)init.SubNew(c)).Magnitude;
            while (GetMouseButtons() != MouseButtons.Left)
            {
                await Task.Delay(Settings.FollowPeriod);

                double dist = ((Vector2)GetPosition().Sub(c)).Magnitude;
                for (int i = 0; i < Selection.Count; i++)
                    Selection[i].Center = c.AddNew((Point)(list[i] * (dist / initdist)));
                TriggerUpdate();
            }

            foreach (Node n in Selection)
            {
                Point now = n.Location;
                CurrentSnap._Redo += () =>
                {
                    n.Location = now;
                    Select(n);
                };
            }

            if (MouseUnoccupy != null) MouseUnoccupy();
        }

        public Rectangle SelectionRect
        {
            get; private set;
        }
        public async Task SelectRectangle()
        {
            if (GetPosition == null || GetMouseButtons == null || GetModifierKeys == null) return;

            Point a = GetPosition();
            Point b;

            while (GetMouseButtons() == MouseButtons.Left)
            {
                await Task.Delay(Settings.FollowPeriod);

                b = GetPosition();
                SelectionRect = ShortHand.CreateRect(a, b);

                TriggerUpdate();
            }

            if (GetModifierKeys() != Keys.Control)
                ClearSelection();
            Select(SelectionRect);

            SelectionRect = Rectangle.Empty;
            TriggerUpdate();
        }

        public void SelectAttached(Node n)
        {
            List<uint> ids = new List<uint>(Nodes.Count);
            ids.Add(n.ID);

            bool added;
            do
            {
                added = false;
                for (int i = ids.Count - 1; i >= 0; i--)
                    foreach (NodePair p in Pairs)
                    {
                        if (p.A == ids[i] && !ids.Contains(p.B))
                        {
                            ids.Add(p.B);
                            added = true;
                        }
                        if (p.B == ids[i] && !ids.Contains(p.A))
                        {
                            ids.Add(p.A);
                            added = true;
                        }
                    }
            } while (added);

            foreach (uint id in ids)
                Select(GetNode(id));
        }
    }

    //IO
    public partial class Molecule
    {
        public MoleculePersist Persist()
        {
            MoleculePersist to = new MoleculePersist();

            to.Nodes = new NodePersist[Nodes.Count];
            for (int i = 0; i < Nodes.Count; i++)
                to.Nodes[i] = Nodes[i].Persist();

            to.Pairs = new NodePairPersist[Pairs.Count];
            for (int i = 0; i < Pairs.Count; i++)
                to.Pairs[i] = Pairs[i].Persist();

            for (uint id = 0; id < to.Nodes.Length; id++)
            {
                bool taken = false;
                foreach (NodePersist n in to.Nodes)
                    if (n.ID == id)
                    {
                        taken = true;
                        break;
                    }

                if (taken) continue;

                uint old = to.Nodes[id].ID;
                to.Nodes[id].ID = id;

                foreach (NodePairPersist p in to.Pairs)
                {
                    if (p.A == old) p.A = id;
                    if (p.B == old) p.B = id;
                }
            }

            return to;
        }
        public void Import(MoleculePersist from)
        {
            Clear();

            foreach (NodePersist node in from.Nodes)
            {
                AddNode(Point.Empty,ActionMode.None).Import(node);

                if (node.ID >= _ID) NextID = node.ID + 1;
            }
            foreach (NodePairPersist pair in from.Pairs)
            {
                Pairs.Add(new NodePair(pair.A, pair.B, pair.Type));
            }

            ClearSnaps();
            TriggerUpdate();
        }

        public static Font NodeFont = new Font(FontFamily.GenericSansSerif, 18f);

        public Think OnFileChange;
        private void FireFileChange()
        {
            if (OnFileChange != null) OnFileChange();
        }

        public bool UpToDate { get; private set; } = true;

        private string _WorkingPath = string.Empty;
        public string WorkingPath
        {
            get
            {
                return _WorkingPath;
            }
            private set
            {
                _WorkingPath = value;
                FireFileChange();
            }
        }

        public XmlWriterSettings XmlSettings;
        public XmlSerializerNamespaces XmlNamespaces;

        public XmlSerializer XmlSerializer;
        public BinaryFormatter BinaryFormatter;

        public void Save()
        {
            if (string.IsNullOrEmpty(WorkingPath))
            {
                SaveAs();
                return;
            }

            FileInfo info = new FileInfo(WorkingPath);
            if (!info.Directory.Exists)
            {
                MessageBox.Show("Directory does not exist");
                return;
            }

            FileStream file = null;

            try
            {
                file = File.Open(WorkingPath, FileMode.Create);

                if (info.Extension == IO.Xml)
                {
                    using (XmlWriter writer = XmlWriter.Create(file, XmlSettings))
                        XmlSerializer.Serialize(writer, Persist(), XmlNamespaces);

                    UpToDate = true;
                    SystemSounds.Hand.Play();
                }
                else if (info.Extension == IO.Bin)
                {
                    BinaryFormatter.Serialize(file, Persist());

                    UpToDate = true;
                    SystemSounds.Hand.Play();
                }
                else
                {
                    SystemSounds.Exclamation.Play();

                    MessageBox.Show("Selected file type not supported");
                }
            }
            catch (Exception ex)
            {
                SystemSounds.Exclamation.Play();

                MessageBox.Show(string.Format("Something went wrong while saving:\n\n{0}", ex.ToString()));
            }
            finally
            {
                file.Close();
                file.Dispose();
            }
        }
        public void SaveAs()
        {
            SaveFileDialog d = new SaveFileDialog();
            d.DefaultExt = IO.Xml;
            d.Filter = IO.SaveFileFilter;

            if (!string.IsNullOrEmpty(WorkingPath))
            {
                FileInfo info = new FileInfo(WorkingPath);
                if (info.Directory.Exists)
                    d.InitialDirectory = info.DirectoryName;
                d.FileName = Path.GetFileNameWithoutExtension(WorkingPath);
            }

            if (d.ShowDialog() == DialogResult.OK)
            {
                WorkingPath = d.FileName;

                Save();
            }

            d.Dispose();
        }

        public MoleculePersist Read(string path)
        {
            FileInfo info = new FileInfo(path);
            FileStream file = null;

            try
            {
                file = File.Open(path, FileMode.Open);

                if (info.Extension == IO.Xml)
                {
                    return XmlSerializer.Deserialize(file) as MoleculePersist;
                }
                else if (info.Extension == IO.Bin)
                {
                    return BinaryFormatter.Deserialize(file) as MoleculePersist;
                }

                MessageBox.Show("Selected file type not supported");
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occured while opening a file\n\n{0}", ex.ToString()));
            }
            finally
            {
                file.Close();
                file.Dispose();
            }

            return null;
        }

        public void Open(string path)
        {
            MoleculePersist bin = Read(path);
            if (bin != null)
            {
                if (!UpToDate && ShortHand.Confirm()) Save();

                WorkingPath = path;
                Import(bin);
            }
        }
        public void Open()
        {
            OpenFileDialog d = new OpenFileDialog();
            d.DefaultExt = IO.Xml;
            d.Filter = IO.OpenFileFilter;

            if (!string.IsNullOrEmpty(WorkingPath))
            {
                FileInfo info = new FileInfo(WorkingPath);
                if (info.Directory.Exists)
                    d.InitialDirectory = info.DirectoryName;
                d.FileName = Path.GetFileNameWithoutExtension(WorkingPath);
            }

            if (d.ShowDialog() == DialogResult.OK)
                Open(d.FileName);

            d.Dispose();
        }

        public static SolidBrush DrawingBrush = new SolidBrush(Color.Black);
        public static SolidBrush BackgroundBrush = new SolidBrush(Color.Black);

        private static void UpdateBrushes()
        {
            DrawingBrush.Color = Properties.Settings.Default.DrawingColor;

            if (Properties.Settings.Default.TransparentBackground)
                BackgroundBrush.Color = IO.Transparent;
            else
                BackgroundBrush.Color = Properties.Settings.Default.BackgroundColor;
        }

        public void Export(string path, Control canvas)
        {
            try
            {
                Bitmap map = new Bitmap(canvas.Width, canvas.Height);
                Graphics g = Graphics.FromImage(map);

                g.Clear(BackgroundBrush.Color);
                Region fullRegion = g.Clip;

                foreach (NodePair pair in Pairs)
                    pair.DrawLink(GetNode(pair.A).Center, GetNode(pair.B).Center, g);

                foreach (Node node in Nodes)
                    if (!string.IsNullOrEmpty(node.NodeName))
                    {
                        g.SetClip(new Rectangle(node.Location.X, node.Location.Y, node.Width, node.Height), CombineMode.Replace);
                        g.Clear(BackgroundBrush.Color);
                        g.SetClip(fullRegion, CombineMode.Replace);

                        g.DrawString(node.NodeName, NodeFont, DrawingBrush, node.Location.X, node.Location.Y);
                    }

                map.Save(path);
                SystemSounds.Hand.Play();
            }
            catch (Exception ex)
            {
                SystemSounds.Exclamation.Play();
                MessageBox.Show(string.Format("An error occured while exporting\n\n{0}", ex.ToString()));
            }
        }
        public void ExportAs(Control Canvas)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.DefaultExt = IO.Png;
            d.Filter = IO.ExportFilter;

            if (!string.IsNullOrEmpty(WorkingPath))
            {
                FileInfo info = new FileInfo(WorkingPath);
                if (info.Directory.Exists)
                    d.InitialDirectory = info.DirectoryName;
                d.FileName = Path.GetFileNameWithoutExtension(WorkingPath);
            }

            if (d.ShowDialog() == DialogResult.OK)
            {
                Export(d.FileName, Canvas);
            }

            d.Dispose();
        }
    }

    //snapshots
    public partial class Molecule
    {
        private List<SnapLogEntry> Snapshots = new List<SnapLogEntry>();
        public SnapLogEntry CurrentSnap
        {
            get
            {
                return Snapshots[ActiveSnap];
            }
        }

        private int ActiveSnap = 0;

        public void Undo()
        {
            if (ActiveSnap == 0) return;
            ClearSelection();

            CurrentSnap.Undo();
            Select(CurrentSnap.Selection);
            ActiveSnap--;

            TriggerUpdate();
        }

        public void Redo()
        {
            if (ActiveSnap >= Snapshots.Count - 1) return;
            ClearSelection();

            ActiveSnap++;
            CurrentSnap.Redo();
            Select(CurrentSnap.Selection);

            TriggerUpdate();
        }

        public void Snap()
        {
            for (int i = Snapshots.Count - 1; i > ActiveSnap; i--)
                Snapshots.RemoveAt(i);

            Snapshots.Add(new SnapLogEntry(this));
            ActiveSnap = Snapshots.Count - 1;

            UpToDate = false;
        }

        public void ClearSnaps()
        {
            Snapshots.Clear();
            Snap();

            UpToDate = true;
        }
    }
    #endregion

    public class SnapLogEntry
    {
        public Think _Undo, _Redo;

        public readonly Node[] Selection;
        public SnapLogEntry(Molecule from)
        {
            Selection = from.Selection.ToArray();
        }

        public void Undo()
        {
            if (_Undo != null) _Undo();
        }
        public void Redo()
        {
            if (_Redo != null) _Redo();
        }
    }

    public class ColorDisplay : UserControl
    {
        private Color _Color;
        public Color Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;

                Brush.Color = value;

                Invalidate();
            }
        }

        public SolidBrush Brush
        {
            get; private set;
        }

        private Pen _BorderPen = new Pen(Color.Black, 1f);
        public Pen BorderPen
        {
            get
            {
                return _BorderPen;
            }
            set
            {
                _BorderPen = value;
                Invalidate();
            }
        }

        public ColorDisplay(Color init)
        {
            Size = new Size(200, 20);

            Brush = new SolidBrush(init);
            Color = init;

            Invalidate();
        }
        public ColorDisplay() : this(Color.White) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.FillRectangle(Brush, new Rectangle(0, 0, Width, Height));
            g.DrawRectangle(BorderPen, new Rectangle(
                (int)(BorderPen.Width / 2),
                (int)(BorderPen.Width / 2),
                Width - (int)BorderPen.Width,
                Height - (int)BorderPen.Width));
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button != MouseButtons.Left) return;

            PromptColor();
        }

        protected void PromptColor()
        {
            ColorDialog d = new ColorDialog();
            d.FullOpen = true;

            d.Color = Color;

            if (d.ShowDialog() == DialogResult.OK)
                Color = d.Color;

            d.Dispose();
            Invalidate();
        }
    }

    [Serializable]
    public class NodePairPersist
    {
        public uint A, B;
        public BondType Type;
    }
    public class NodePair
    {
        public uint A, B;
        public BondType Type = BondType.Triple;

        public NodePair(uint a, uint b, BondType type)
        {
            A = a;
            B = b;
            Type = type;
        }

        public bool Contains(uint thing)
        {
            return A == thing || B == thing;
        }

        public void Flip()
        {
            uint a = A;
            A = B;
            B = a;
        }

        public NodePairPersist Persist()
        {
            NodePairPersist to = new NodePairPersist();

            to.A = A;
            to.B = B;

            to.Type = Type;

            return to;
        }

        public static SolidBrush BondBrush = new SolidBrush(Properties.Settings.Default.DrawingColor);
        public static Pen BondPen = new Pen(BondBrush.Color, 3f);

        static NodePair()
        {
            Settings.OnSettingsUpdate += () =>
            {
                BondBrush.Color = Properties.Settings.Default.DrawingColor;
                BondPen.Color = BondBrush.Color;
            };
        }

        public const float bondSpace = 8f;
        public void DrawLink(Point a, Point b, Graphics g)
        {
            if (a == b) return;

            Vector2 d = new Vector2(b.X - a.X, b.Y - a.Y);
            Vector2 n = new Vector2(d.Y, -d.X).Unit * bondSpace;

            switch (Type)
            {
                case BondType.Single:
                    g.DrawLine(BondPen, a, b);

                    g.FillEllipse(BondBrush,
                            a.X - BondPen.Width / 2,
                            a.Y - BondPen.Width / 2,
                            BondPen.Width, BondPen.Width);
                    g.FillEllipse(BondBrush,
                            b.X - BondPen.Width / 2,
                            b.Y - BondPen.Width / 2,
                            BondPen.Width, BondPen.Width);
                    break;
                case BondType.Double:
                    g.DrawLine(BondPen, a, b);
                    g.DrawLine(BondPen, a.AddNew((Point)n), b.AddNew((Point)n));

                    g.FillEllipse(BondBrush,
                            a.X - BondPen.Width / 2,
                            a.Y - BondPen.Width / 2,
                            BondPen.Width, BondPen.Width);
                    g.FillEllipse(BondBrush,
                            b.X - BondPen.Width / 2,
                            b.Y - BondPen.Width / 2,
                            BondPen.Width, BondPen.Width);
                    break;
                case BondType.Triple:
                    g.DrawLine(BondPen, a, b);
                    g.DrawLine(BondPen, a.AddNew((Point)n), b.AddNew((Point)n));
                    g.DrawLine(BondPen, a.SubNew((Point)n), b.SubNew((Point)n));

                    g.FillEllipse(BondBrush,
                            a.X - BondPen.Width / 2,
                            a.Y - BondPen.Width / 2,
                            BondPen.Width, BondPen.Width);
                    g.FillEllipse(BondBrush,
                            b.X - BondPen.Width / 2,
                            b.Y - BondPen.Width / 2,
                            BondPen.Width, BondPen.Width);
                    break;
                case BondType.Wedge:
                    g.FillPolygon(BondBrush, new Point[] {
                        a,
                        b.AddNew((Point)n),
                        b.SubNew((Point)n)
                    });
                    break;
                case BondType.Dash:
                    double mag = d.Magnitude;
                    for (float i = 0; i < mag; i += bondSpace)
                        g.DrawLine(BondPen, a.AddNew((Point)(d.Unit * i + (n * i / mag))),
                            a.AddNew((Point)(d.Unit * i - (n * i / mag))) );
                    break;
                case BondType.Thick:
                    g.FillPolygon(BondBrush, new Point[] {
                        a.AddNew((Point)n),
                        b.AddNew((Point)n),
                        b.SubNew((Point)n),
                        a.SubNew((Point)n)
                    });
                    break;
                case BondType.Semi:
                    mag = d.Magnitude;
                    float delta = bondSpace * 2;

                    for (float i = 0; i <= mag - delta; i += delta)
                        g.DrawLine(BondPen, a.AddNew((Point)(d.Unit * i)), a.AddNew((Point)(d.Unit * (i + bondSpace))));
                    g.DrawLine(BondPen, b.SubNew((Point)(d.Unit * (mag % delta))), b);
                    break;
                case BondType.Wavy:
                    List<Point> points = new List<Point>();
                    mag = d.Magnitude;
                    bool up = true;

                    delta = bondSpace;
                    points.Add(a);
                    for (float i = delta / 2; i <= mag - delta / 2; i += delta)
                    {
                        points.Add(a.AddNew((Point)(d.Unit * i)).AddNew((Point)(up ? n : -n)));

                        up = !up;
                    }
                    points.Add(b);

                    g.DrawCurve(BondPen, points.ToArray());

                    g.FillEllipse(BondBrush,
                            a.X - BondPen.Width / 2,
                            a.Y - BondPen.Width / 2,
                            BondPen.Width, BondPen.Width);
                    g.FillEllipse(BondBrush,
                            b.X - BondPen.Width / 2,
                            b.Y - BondPen.Width / 2,
                            BondPen.Width, BondPen.Width);

                    break;
            }
        }
    }

    [Serializable]
    public class NodePersist
    {
        public string Name;
        public uint ID;

        public Point Pos;
        public BondAlign Align;
    }
    public class Node : UserControl
    {
        public BondAlign Alignment = BondAlign.Left;

        private string _NodeName;
        public string NodeName
        {
            get
            {
                return _NodeName;
            }
            set
            {
                _NodeName = value;

                if (_NodeName == string.Empty)
                    Size = new Size(15, 15);
                else
                    Size = ShortHand.MeasureString(_NodeName, Molecule.NodeFont);

                Invalidate();
                TriggerUpdate();
            }
        }

        public uint ID = 0;

        public Molecule Owner = null;
        private void TriggerUpdate()
        {
            if (Owner != null) Owner.TriggerUpdate();
        }

        public Node()
        {
            AutoSize = false;
            NodeName = string.Empty;
        }

        public static readonly SolidBrush UnselectedBrush = new SolidBrush(Properties.Settings.Default.UnselectedColor);
        public static readonly SolidBrush SelectedBrush = new SolidBrush(Properties.Settings.Default.SelectedColor);

        static Node()
        {
            Settings.OnSettingsUpdate += () =>
            {
                UnselectedBrush.Color = Properties.Settings.Default.UnselectedColor;
                SelectedBrush.Color = Properties.Settings.Default.SelectedColor;
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (NodeName == string.Empty)
                e.Graphics.FillRectangle(
                    Owner != null && Owner.Selection.Contains(this) ? SelectedBrush : UnselectedBrush,
                    0, 0, 15, 15);
            else
                e.Graphics.DrawString(NodeName, Molecule.NodeFont,
                    Owner != null && Owner.Selection.Contains(this) ? SelectedBrush : UnselectedBrush, 0, 0);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;

            g.FillRectangle(Molecule.BackgroundBrush, e.ClipRectangle);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            Rename();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (MouseButtons != MouseButtons.Left) return;

            if (ModifierKeys == Keys.Control && Owner != null)
            {
                Owner.ToggleSelect(this);
                return;
            }

            if (Owner != null && Owner.Selection.Contains(this))
            {
                Task temp = Owner.FollowSelection();
            }
            else
            {
                Task temp = follow();
            }
        }

        public Point Center
        {
            get
            {
                if (NodeName == string.Empty)
                    return new Point(Location.X + Height / 2, Location.Y + Height / 2);
                else
                    switch (Alignment)
                    {
                        case BondAlign.Right:
                            return new Point(Location.X + Width - Height / 2,
                                Location.Y + Height / 2);
                        case BondAlign.Center:
                            return new Point(Location.X + Width / 2,
                                Location.Y + Height / 2);
                        default:
                            return new Point(Location.X + Height / 2,
                                Location.Y + Height / 2);
                    }
            }
            set
            {
                if (NodeName == string.Empty)
                    Location = new Point(value.X - Height / 2, value.Y - Height / 2);
                else
                    switch (Alignment)
                    {
                        case BondAlign.Right:
                            Location = new Point(value.X - Width + Height / 2,
                                value.Y - Height / 2);
                            break;
                        case BondAlign.Center:
                            Location = new Point(value.X - Width / 2,
                                value.Y - Height / 2);
                            break;
                        default:
                            Location = new Point(value.X - Height / 2,
                                value.Y - Height / 2);
                            break;
                    }
            }
        }

        public Control FollowParent = null;
        public Point GrabPoint;
        public async Task follow()
        {
            if (FollowParent == null) return;
            Point raw = FollowParent.PointToClient(MousePosition);
            GrabPoint = new Point(Location.X - raw.X, Location.Y - raw.Y);

            Point origin = Location;
            while (MouseButtons == MouseButtons.Left)
            {
                await Task.Delay(Settings.FollowPeriod);
                raw = FollowParent.PointToClient(MousePosition);
                Location = new Point(raw.X + GrabPoint.X, raw.Y + GrabPoint.Y);

                TriggerUpdate();
            }

            Point final = Location;
            if (Owner != null && origin != Location)
            {
                Owner.Snap();
                Owner.CurrentSnap._Undo += () => Location = origin;
                Owner.CurrentSnap._Redo += () => Location = final;
            }
        }

        public void Rename()
        {
            RenameDialog d = new RenameDialog();
            d.Value = NodeName;
            d.Alignment = Alignment;

            if (d.ShowDialog() == DialogResult.OK)
            {
                Point c = Center;

                if (Owner != null)
                {
                    string before = NodeName;
                    BondAlign b_ali = Alignment;

                    string after = d.Value;
                    BondAlign a_ali = d.Alignment;

                    Owner.Snap();
                    Owner.CurrentSnap._Undo += () =>
                    {
                        NodeName = before;
                        Alignment = b_ali;
                        Center = c;
                    };
                    Owner.CurrentSnap._Redo += () =>
                    {
                        NodeName = after;
                        Alignment = a_ali;
                        Center = c;
                    };
                }

                NodeName = d.Value;
                Alignment = d.Alignment;

                Center = c;
            }

            d.Dispose();
        }

        public NodePersist Persist()
        {
            NodePersist to = new NodePersist();

            to.Name = CharDatabase.ParseToFormat(NodeName);
            to.ID = ID;

            to.Pos = Location;
            to.Align = Alignment;

            return to;
        }

        public void Import(NodePersist from)
        {
            NodeName = CharDatabase.ParseToNormal(from.Name);
            ID = from.ID;

            Location = from.Pos;
            Alignment = from.Align;
        }
    }
}