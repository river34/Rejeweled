using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : View
{
	[SerializeField]
	Text scoreText;
	[SerializeField]
	View addViewTemplate;

    int score = 0;
	Vector2 disappearPoint = Vector2.up * 1000;

    public void IncreaseScore(int tileCount)
    {
        if (tileCount < 2) return;

		int addScore = (tileCount - 2) * 10;
		score += addScore;
        UpdateScore();

		GameObject addViewObject = Instantiate<GameObject>(addViewTemplate.gameObject, addViewTemplate.transform.parent);
		addViewObject.SetActive(true);
		View addView = addViewObject.GetComponent<View>();
		addView.SetText("+" + addScore);
		addView.MoveToAndDestroy(disappearPoint, ANIM_DELAY * 4);
    }

    public void UpdateScore()
    {
		scoreText.text = "" + score;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScore();
    }
}
