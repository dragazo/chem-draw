namespace Chemipad
{
    public enum TargetMode
    {
        Closest,
        All
    }

    public enum BondType
    {
        Single,
        Double,
        Triple,
        Wedge,
        Dash,
        Thick,
        Semi,
        Wavy
    }

    public enum BondAlign
    {
        Left,
        Center,
        Right
    }

    public enum MirrorMode
    {
        Vertical,
        Horizontal
    }

    public enum ActionMode
    {
        None = 0,
        Snap = 1,
        Log = 2,
        Update = 4
    }
}
