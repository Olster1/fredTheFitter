using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using easy_transition;

public class BackToMapBtn : MonoBehaviour
{
    private TransitionState transitionState;
    // Start is called before the first frame update
    void Start()
    {
        Button but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(() => backToMap());
        GameObject canvasObj = GameObject.Find("TransitionCanvas");
        transitionState = canvasObj.GetComponent<TransitionState>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void backToMap() {
        TransitionState.OverworldTransLevelData data = new TransitionState.OverworldTransLevelData(transitionState.currentLevelType);
        transitionState.SetTransition_(transitionState.BackToWorldTransition, data);

    }
}

