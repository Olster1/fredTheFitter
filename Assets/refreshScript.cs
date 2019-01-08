using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using easy_transition;
using Levels;

public class refreshScript : MonoBehaviour
{
    private TransitionState transitionState;

    // Start is called before the first frame update
    void Start()
    {
        Button but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(() => refreshLevel());
        GameObject canvasObj = GameObject.Find("TransitionCanvas");
        transitionState = canvasObj.GetComponent<TransitionState>();

    }

    void refreshLevel() {
        transitionState.SetTransition_(refreshLevel_, null);
    }


    void refreshLevel_(System.Object obj) {
        GameObject brdObj = transitionState.levelObjects[(int)transitionState.currentLevelType];
        BoardLogic brd = brdObj.GetComponent<BoardLogic>();
        brd.ResetBoard(true, false);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
