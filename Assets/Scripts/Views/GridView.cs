using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridView : View, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    int row = 6;
    [SerializeField]
    int col = 6;
    [SerializeField]
    View cellViewTemplate;
    [SerializeField]
    View tileViewTemplate;
    [SerializeField]
    Sprite[] tileSprites;
    [SerializeField]
    GameView gameView;

    Grid grid;
	Cell activeCell1, activeCell2;
	Cell possibleCell1, possibleCell2, warningCell;
    int gridWidth = 600, gridHeight = 600;
    float gridOffsetX = 100, gridOffsetY = 100;
    float cellWidth = 100, cellHeight = 100;
    View[,] cellViewArray;
    View[,] tileViewArray;
    int startX, startY, endX, endY;
    bool canMove = false;
	IEnumerator timerCoroutine = null;

	const float TIMEOUT_DELAY = 5;

    public void InitGrid()
    {
        CreateGrid();
        FillTiles();
		Invoke("CheckPossibleMove", ANIM_DELAY);
		Invoke("CheckMatch", ANIM_DELAY);
		StartTimeout();
    }

    public void ResetGrid()
    {
        RemoveAllTiles();
		FillTiles();
		ResetTimeout();
		StartTimeout();
		Invoke("CheckPossibleMove", ANIM_DELAY);
		Invoke("CheckMatch", ANIM_DELAY);
    }

    public void CreateGrid()
    {
        grid = new Grid(row, col);
        cellWidth = gridWidth / col;
        cellHeight = gridHeight / row;

        cellViewArray = new View[row, col];
        tileViewArray = new View[row, col];

        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < col; x++)
            {
                grid.CellArray[x, y] = new Cell(x, y);

                GameObject cellObject = Instantiate<GameObject>(cellViewTemplate.gameObject, cellViewTemplate.transform.parent);
                cellObject.SetActive(true);
                View cellView = cellObject.GetComponent<View>();
                cellView.SetSize(x, y, cellWidth, cellHeight);
				cellView.SetColorFade ();
                cellViewArray[x, y] = cellView;
            }
        }

		Vector3[] corners = GetCornors();
        gridOffsetX = corners[0].x;
        gridOffsetY = corners[0].y;
    }

    public void RemoveAllTiles()
	{
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < col; x++)
            {
                grid.CellArray[x, y].Tile = null;

                if (tileViewArray[x, y] != null)
                {
                    //Destroy(tileViewArray[x, y].gameObject);
					tileViewArray[x, y].FadeOut(ANIM_DELAY);
                    tileViewArray[x, y] = null;
                }
            }
        }
    }

    public void FillTiles()
    {
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < col; x++)
            {
                if (grid.CellArray[x, y].Tile == null)
                {
                    grid.CellArray[x, y].Tile = new Tile(Random.Range(0, tileSprites.Length));

                    GameObject tileObject = Instantiate<GameObject>(tileViewTemplate.gameObject, tileViewTemplate.transform.parent);
                    tileObject.SetActive(true);
                    View tileView = tileObject.GetComponent<View>();
                    tileView.SetImage(ref tileSprites[grid.CellArray[x, y].Tile.Color]);

                    // tileView.SetSize(x, y, cellWidth, cellHeight);
                    tileView.SetSize(x, row, cellWidth, cellHeight);
                    tileView.MoveTo(new Vector2(x * cellWidth, y * cellHeight));

                    tileViewArray[x, y] = tileView;
                }
            }
        }
    }

    public List<List<Cell>> GetMatches()
    {
        List<Cell> block;
        List<List<Cell>> matches = new List<List<Cell>>();

        // check horizontal
        for (int y = 0; y < row; y++)
        {
            block = new List<Cell>();
            for (int x = 0; x < col - 2; x++)
            {
                if (grid.CellArray[x, y].Tile == null) continue;
                if (grid.CellArray[x + 1, y].Tile == null) continue;
                if (grid.CellArray[x + 2, y].Tile == null) continue;

                if (grid.CellArray[x, y].Tile.Color == grid.CellArray[x + 1, y].Tile.Color && grid.CellArray[x, y].Tile.Color == grid.CellArray[x + 2, y].Tile.Color)
                {
                    if (!block.Contains(grid.CellArray[x, y]))
                    {
                        if (block.Count > 0)
                        {
                            matches.Add(block);
                            block = new List<Cell>();
                        }
                    }

                    if (!block.Contains(grid.CellArray[x, y]))
                    {
                        block.Add(grid.CellArray[x, y]);
                    }
                    if (!block.Contains(grid.CellArray[x + 1, y]))
                    {
                        block.Add(grid.CellArray[x + 1, y]);
                    }
                    if (!block.Contains(grid.CellArray[x + 2, y]))
                    {
                        block.Add(grid.CellArray[x + 2, y]);
                    }
                }
            }
            if (block.Count > 0)
            {
                matches.Add(block);
            }
        }

        // check vertical
        for (int x = 0; x < col; x++)
        {
            block = new List<Cell>();
            for (int y = 0; y < row - 2; y++)
            {
                if (grid.CellArray[x, y].Tile == null) continue;
                if (grid.CellArray[x, y + 1].Tile == null) continue;
                if (grid.CellArray[x, y + 2].Tile == null) continue;

                if (grid.CellArray[x, y].Tile.Color == grid.CellArray[x, y + 1].Tile.Color && grid.CellArray[x, y].Tile.Color == grid.CellArray[x, y + 2].Tile.Color)
                {
                    if (!block.Contains(grid.CellArray[x, y]))
                    {
                        if (block.Count > 0)
                        {
                            matches.Add(block);
                            block = new List<Cell>();
                        }
                    }

                    if (!block.Contains(grid.CellArray[x, y]))
                    {
                        block.Add(grid.CellArray[x, y]);
                    }
                    if (!block.Contains(grid.CellArray[x, y + 1]))
                    {
                        block.Add(grid.CellArray[x, y + 1]);
                    }
                    if (!block.Contains(grid.CellArray[x, y + 2]))
                    {
                        block.Add(grid.CellArray[x, y + 2]);
                    }
                }
            }
            if (block.Count > 0)
            {
                matches.Add(block);
            }
        }

        return matches;
    }

	public List<Cell> GetPossibleMove()
	{
		List<Cell> block;
		List<Cell> move = new List<Cell>();

		// check horizontal
		for (int y = 0; y < row; y++)
		{
			for (int x = 0; x < col - 2; x++)
			{
				if (grid.CellArray[x, y].Tile == null) continue;
				if (grid.CellArray[x + 1, y].Tile == null) continue;

				if (grid.CellArray[x, y].Tile.Color == grid.CellArray[x + 1, y].Tile.Color)
				{
					block = new List<Cell>();
					block.Add(grid.CellArray[x, y]);
					block.Add(grid.CellArray[x + 1, y]);

					if (block.Count >= 2)
					{
						if (block[0].X > 0 && block[0].Y < row - 1 && grid.CellArray[(block[0].X - 1), (block[0].Y + 1)].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[(block[0].X - 1), (block[0].Y + 1)].Tile.Color)
						{
							Debug.Log("100 / 011");
							move.Add(grid.CellArray[(block[0].X - 1), (block[0].Y + 1)]);
							move.Add(grid.CellArray[(block[0].X - 1), (block[0].Y)]);
							return move;
						}

						if (block[0].X > 0 && block[0].Y > 0 && grid.CellArray[(block[0].X - 1), (block[0].Y - 1)].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[(block[0].X - 1), (block[0].Y - 1)].Tile.Color)
						{
							Debug.Log("011 / 100");
							move.Add(grid.CellArray[(block[0].X - 1), (block[0].Y - 1)]);
							move.Add(grid.CellArray[(block[0].X - 1), (block[0].Y)]);
							return move;
						}

						if (block[1].X < col - 1 && block[1].Y < row - 1 && grid.CellArray[(block[1].X + 1), (block[1].Y + 1)].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[(block[1].X + 1), (block[1].Y + 1)].Tile.Color)
						{
							Debug.Log("001 / 110");
							move.Add(grid.CellArray[(block[1].X + 1), (block[1].Y + 1)]);
							move.Add(grid.CellArray[(block[1].X + 1), (block[1].Y)]);
							return move;
						}

						if (block[1].X < col - 1 && block[1].Y > 0 && grid.CellArray[(block[1].X + 1), (block[1].Y - 1)].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[(block[1].X + 1), (block[1].Y - 1)].Tile.Color)
						{
							Debug.Log("110 / 001");
							move.Add(grid.CellArray[(block[1].X + 1), (block[1].Y - 1)]);
							move.Add(grid.CellArray[(block[1].X + 1), (block[1].Y)]);
							return move;
						}
					}
				}

				if (x > col - 3) continue;
				if (grid.CellArray[x + 2, y].Tile == null) continue;

				if (grid.CellArray[x, y].Tile.Color == grid.CellArray[x + 2, y].Tile.Color)
				{
					block = new List<Cell>();
					block.Add(grid.CellArray[x, y]);
					block.Add(grid.CellArray[x + 2, y]);

					if (block.Count >= 2)
					{
						if (y < col - 1 && grid.CellArray[x + 1, y + 1].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[x + 1, y + 1].Tile.Color)
						{
							Debug.Log("010 / 101");
							move.Add(grid.CellArray[x + 1, y + 1]);
							move.Add(grid.CellArray[x + 1, y]);
							return move;
						}

						if (y > 0 && grid.CellArray[x + 1, y - 1].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[x + 1, y - 1].Tile.Color)
						{
							Debug.Log("101 / 010");
							move.Add(grid.CellArray[x + 1, y - 1]);
							move.Add(grid.CellArray[x + 1, y]);
							return move;
						}
					}
				}
			}
		}

		// check vertical
		for (int x = 0; x < col; x++)
		{
			block = new List<Cell>();
			for (int y = 0; y < row - 2; y++)
			{
				if (grid.CellArray[x, y].Tile == null) continue;
				if (grid.CellArray[x, y + 1].Tile == null) continue;

				if (grid.CellArray[x, y].Tile.Color == grid.CellArray[x, y + 1].Tile.Color)
				{
					block = new List<Cell>();
					block.Add(grid.CellArray[x, y]);
					block.Add(grid.CellArray[x, y + 1]);

					if (block.Count >= 2)
					{
						if (block[0].X > 0 && block[0].Y > 0 && grid.CellArray[(block[0].X - 1), (block[0].Y - 1)].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[(block[0].X - 1), (block[0].Y - 1)].Tile.Color)
						{
							Debug.Log("010 / 010 / 100");
							move.Add(grid.CellArray[(block[0].X - 1), (block[0].Y - 1)]);
							move.Add(grid.CellArray[(block[0].X), (block[0].Y - 1)]);
							return move;
						}

						if (block[0].X < col - 1 && block[0].Y > 0 && grid.CellArray[(block[0].X + 1), (block[0].Y - 1)].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[(block[0].X + 1), (block[0].Y - 1)].Tile.Color)
						{
							Debug.Log("010 / 010 / 001");
							move.Add(grid.CellArray[(block[0].X + 1), (block[0].Y - 1)]);
							move.Add(grid.CellArray[(block[0].X), (block[0].Y - 1)]);
							return move;
						}

						if (block[1].X > 0 && block[1].Y < row - 1 && grid.CellArray[(block[1].X - 1), (block[1].Y + 1)].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[(block[1].X - 1), (block[1].Y + 1)].Tile.Color)
						{
							Debug.Log("100 / 010 / 010");
							move.Add(grid.CellArray[(block[1].X - 1), (block[1].Y + 1)]);
							move.Add(grid.CellArray[(block[1].X), (block[1].Y + 1)]);
							return move;
						}

						if (block[1].X < col - 1 && block[1].Y < row - 1 && grid.CellArray[(block[1].X + 1), (block[1].Y + 1)].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[(block[1].X + 1), (block[1].Y + 1)].Tile.Color)
						{
							Debug.Log("001 / 010 / 010");
							move.Add(grid.CellArray[(block[1].X + 1), (block[1].Y + 1)]);
							move.Add(grid.CellArray[(block[1].X), (block[1].Y + 1)]);
							return move;
						}
					}
				}

				if (y > row - 3) continue;
				if (grid.CellArray[x, y + 2].Tile == null) continue;

				if (grid.CellArray[x, y].Tile.Color == grid.CellArray[x, y + 2].Tile.Color)
				{

					block = new List<Cell>();
					block.Add(grid.CellArray[x, y]);
					block.Add(grid.CellArray[x, y + 2]);

					if (x < col - 1 && grid.CellArray[x + 1, y + 1].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[x + 1, y + 1].Tile.Color)
					{
						Debug.Log("10 / 01 / 10");
						move.Add(grid.CellArray[x + 1, y + 1]);
						move.Add(grid.CellArray[x , y + 1]);
						return move;
					}

					if (x > 0 && grid.CellArray[x - 1, y + 1].Tile != null && grid.CellArray[x, y].Tile.Color == grid.CellArray[x - 1, y + 1].Tile.Color)
					{
						Debug.Log("01 / 10 / 01");
						move.Add(grid.CellArray[x - 1, y + 1]);
						move.Add(grid.CellArray[x, y + 1]);
						return move;
					}
				}
			}
		}

		return null;
	}

    public void RemoveMatches(List<List<Cell>> _matches)
    {
        foreach (List<Cell> block in _matches)
        {
            gameView.IncreaseScore(block.Count);

            foreach (Cell cell in block)
            {
                grid.CellArray[cell.X, cell.Y].Tile = null;

                if (tileViewArray[cell.X, cell.Y] != null)
                {
                    Destroy(tileViewArray[cell.X, cell.Y].gameObject);
                    tileViewArray[cell.X, cell.Y] = null;
                }
            }
        }
    }

    public void ApplyGravity()
    {
        for (int x = 0; x < col; x++)
        {
            for (int y = 1; y < row; y++)
            {
                int tempY = y;
                while (tempY > 0 && grid.CellArray[x, tempY - 1].Tile == null && grid.CellArray[x, tempY].Tile != null)
                {
                    grid.CellArray[x, tempY - 1].Tile = grid.CellArray[x, tempY].Tile;
                    grid.CellArray[x, tempY].Tile = null;

                    tileViewArray[x, tempY - 1] = tileViewArray[x, tempY];
					tileViewArray[x, tempY] = null;

                    // tileViewArray[x, tempY - 1].SetSize(x, tempY - 1, cellWidth, cellHeight);

                    tempY -= 1;
                }

				if (grid.CellArray [x, tempY].Tile != null)
				{
					tileViewArray[x, tempY].MoveTo(new Vector2(x * cellWidth, (tempY) * cellHeight));
				}
            }
        }
    }

    public void OnBeginDrag(PointerEventData _eventData)
	{
		OnCellUp();

        startX = (int)Mathf.Floor((_eventData.position.x - gridOffsetX) / cellWidth);
        startY = (int)Mathf.Floor((_eventData.position.y - gridOffsetY) / cellHeight);

        if (startX >= 0 && startX < col && startY >= 0 && startY < row)
		{
			if (canMove)
            {
                activeCell1 = grid.CellArray[startX, startY];
				cellViewArray[activeCell1.X, activeCell1.Y].SetColorSuccess();
            }
			else
			{
				warningCell = grid.CellArray[startX, startY];
				cellViewArray[warningCell.X, warningCell.Y].SetColorWarning();
			}
        }
    }

    public void OnDrag(PointerEventData _eventData)
    {
        if (activeCell1 != null && activeCell2 == null)
        {
            endX = (int)Mathf.Floor((_eventData.position.x - gridOffsetX) / cellWidth);
            endY = (int)Mathf.Floor((_eventData.position.y - gridOffsetY) / cellHeight);

            if (endX >= 0 && endX < col && endY >= 0 && endY < row)
            {
                if (Mathf.Abs(endX - startX) == 1 && Mathf.Abs(endY - startY) == 0 || Mathf.Abs(endX - startX) == 0 && Mathf.Abs(endY - startY) == 1)
                {
                    canMove = false;
                    activeCell2 = grid.CellArray[endX, endY];
                    SwapTiles();
                    // CheckMatch();
					Invoke("CheckMatch", ANIM_DELAY);
                }
            }

        }
    }

	public void OnEndDrag(PointerEventData _eventData)
	{
		if (activeCell1 != null)
		{
			cellViewArray[activeCell1.X, activeCell1.Y].SetColorFade();
		}
	}

	public void OnCellUp()
	{
		if (activeCell1 != null)
		{
			cellViewArray[activeCell1.X, activeCell1.Y].SetColorFade();
			activeCell1 = null;
		}
		if (activeCell2 != null)
		{
			cellViewArray[activeCell2.X, activeCell2.Y].SetColorFade();
			activeCell2 = null;
		}
		if (possibleCell1 != null)
		{
			cellViewArray[possibleCell1.X, possibleCell1.Y].SetColorFade();
			possibleCell1 = null;
		}
		if (possibleCell2 != null)
		{
			cellViewArray[possibleCell2.X, possibleCell2.Y].SetColorFade();
			possibleCell2 = null;
		}
		if (warningCell != null)
		{
			cellViewArray[warningCell.X, warningCell.Y].SetColorFade();
			warningCell = null;
		}
	}

    public void SwapTiles()
    {
        if (activeCell1 != null && activeCell2 != null)
        {
            if (activeCell1.Tile != null && activeCell2.Tile != null)
            {
                Tile temp = activeCell1.Tile;
                activeCell1.Tile = activeCell2.Tile;
                activeCell2.Tile = temp;

                View tempView = tileViewArray[activeCell1.X, activeCell1.Y];
                tileViewArray[activeCell1.X, activeCell1.Y] = tileViewArray[activeCell2.X, activeCell2.Y];
                tileViewArray[activeCell2.X, activeCell2.Y] = tempView;

                // tileViewArray [activeCell1.X, activeCell1.Y].SetSize (activeCell1.X, activeCell1.Y, cellWidth, cellHeight);
                // tileViewArray [activeCell2.X, activeCell2.Y].SetSize (activeCell2.X, activeCell2.Y, cellWidth, cellHeight);
                tileViewArray[activeCell1.X, activeCell1.Y].MoveTo(new Vector2(activeCell1.X * cellWidth, activeCell1.Y * cellHeight));
                tileViewArray[activeCell2.X, activeCell2.Y].MoveTo(new Vector2(activeCell2.X * cellWidth, activeCell2.Y * cellHeight));
            }
        }
    }

    public void CheckMatch()
    {
        List<List<Cell>> matches = GetMatches();
        Debug.Log("CheckMatch : " + matches.Count);

        if (matches.Count > 0)
        {
            RemoveMatches(matches);
			Invoke("ApplyGravity", ANIM_DELAY);
			Invoke("OnCellUp", ANIM_DELAY * 2);
			Invoke("FillTiles", ANIM_DELAY * 2);
			Invoke("CheckPossibleMove", ANIM_DELAY * 3);
			Invoke("CheckMatch", ANIM_DELAY * 3);
        }
        else
        {
            SwapTiles();
			OnCellUp();
            canMove = true;
        }
    }

	public void CheckPossibleMove()
	{
		List<Cell> move = GetPossibleMove();

		if(move != null && move.Count == 2)
		{
			StartTimeout();
		}
		else
		{
			Invoke("RemoveAllTiles", ANIM_DELAY);
			Invoke("FillTiles", ANIM_DELAY);
			Invoke("ResetTimeout", ANIM_DELAY);
			Invoke("StartTimeout", ANIM_DELAY);
			Invoke("CheckPossibleMove", ANIM_DELAY * 2);
			Invoke("CheckMatch", ANIM_DELAY * 2);
		}
	}

	public void StartTimeout()
	{
		if (timerCoroutine != null) return;

		timerCoroutine = Timeout (TIMEOUT_DELAY);

		StartCoroutine(timerCoroutine);
	}

	public void ResetTimeout()
	{
		if (timerCoroutine != null)
		{
			StopCoroutine(timerCoroutine);
			timerCoroutine = null;
		}
	}

	IEnumerator Timeout(float _delay = TIMEOUT_DELAY)
	{
		yield return new WaitForSeconds(_delay);

		List<Cell> move = GetPossibleMove();

		if(move != null && move.Count == 2)
		{
			possibleCell1 = move[0];
			possibleCell2 = move[1];

			cellViewArray[possibleCell1.X, possibleCell1.Y].SetColorPossible();
			cellViewArray[possibleCell2.X, possibleCell2.Y].SetColorPossible();
		}

		timerCoroutine = null;
	}

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
			Application.Quit();
        }
    }
}
