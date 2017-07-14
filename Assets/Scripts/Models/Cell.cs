public class Cell
{
    public int X;
    public int Y;
    public Tile Tile;
    public bool Enabled;

    public Cell(int _x, int _y, Tile _tile = null, bool _enabled = true)
    {
        X = _x;
        Y = _y;
        Tile = _tile;
    }
}