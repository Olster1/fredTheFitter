using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using easy_transition;
using game_defines;
using easy_timer;
using Levels;


public class BackToMapBtn : MonoBehaviour
{
    private TransitionState transitionState;
    private Timer lerpTimer;
    private Transform camTrans;
    private Vector3 startP;
    // Start is called before the first frame update
    void Start()
    {
        Button but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(() => backToMap());
        GameObject canvasObj = GameObject.Find("TransitionCanvas");
        transitionState = canvasObj.GetComponent<TransitionState>();
        lerpTimer = new Timer(0.5f);
        lerpTimer.turnTimerOff();
        camTrans = transitionState.overworldCamera.GetComponent<Camera>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (lerpTimer.isOn())
        {
            TimerReturnInfo info = lerpTimer.updateTimer(Time.deltaTime);
            Vector3 newP = Vector3.Lerp(startP, transitionState.camPosForAttention, info.canonicalVal);
            camTrans.position = new Vector3(newP.x, newP.y, -10.0f);
            if (info.finished)
            {
                lerpTimer.turnTimerOff();
            }

        }
    }

    void backToMap() {
        if (transitionState.overworldParent.activeSelf)
        {
            startP = camTrans.position;
            lerpTimer.turnTimerOn();

        }
        else
        {
            int groupId = transitionState.levelObjects[(int)transitionState.currentLevelType].GetComponent<BoardLogic>().groupId;
            LevelGroup group = transitionState.levelGroups[groupId];
            TransitionState.OverworldTransLevelData data = new TransitionState.OverworldTransLevelData(transitionState.currentLevelType, group.GetPos(transitionState));
            transitionState.SetTransition_(transitionState.BackToWorldTransition, data);
        }



    }
}

