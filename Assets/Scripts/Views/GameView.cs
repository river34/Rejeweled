using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField]
    GridView gridView;
    [SerializeField]
    ScoreView scoreView;
	[SerializeField]
	GameObject overlay;

    public void InitGame()
    {
		gridView.Show();
        gridView.InitGrid();

		scoreView.Show();
		scoreView.ResetScore();
    }

    public void IncreaseScore(int tileCount)
    {
        scoreView.IncreaseScore(tileCount);
    }
}
