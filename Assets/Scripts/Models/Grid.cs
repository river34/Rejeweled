public class Grid
{
    public Cell[,] CellArray;

    public Grid(int _row, int _col)
    {
        CellArray = new Cell[_col, _row];
    }
}