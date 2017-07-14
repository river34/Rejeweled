using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameView gameView;
    [SerializeField]
    View startView;

    void Start()
    {
        //
    }

	public void OnClickStart()
	{
		gameView.InitGame();
		startView.MoveUp();
	}
}
