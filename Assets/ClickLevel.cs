using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;
using Levels;
using UnityEngine.Assertions;
using easy_transition;
using easy_timer;
using UnityEngine.EventSystems;

public class ClickLevel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public LevelType levelToLoad;
    private ParticleSystem sys;
    private TransitionState transitionState;
    private SaveState saveState;
    private Button but;
    public Timer scaleTimer;
    private Text overlayText;

    // Start is called before the first frame update
    void Start()
    {
        but = gameObject.GetComponent<Button>();
        sys = gameObject.GetComponent<ParticleSystem>();
        but.onClick.AddListener(() => LoadNewScene());
        GameObject canvasObj = GameObject.Find("TransitionCanvas");
        transitionState = canvasObj.GetComponent<TransitionState>();

        //Image img = gameObject.GetComponent<Image>();

        saveState = transitionState.saveStates[(int)levelToLoad];
        scaleTimer = new Timer(1);
        transitionState.overworldStars[(int)levelToLoad] = this;
        this.Renew();

        overlayText = GameObject.Find("OverlayText").GetComponent<Text>();

    }


    public void Renew() {
        if (saveState.state != LevelState.LEVEL_STATE_UNLOCKED)
        {
            scaleTimer.turnTimerOff();
        }
        else
        {
            sys.Play();
            scaleTimer.turnTimerOn();
            transitionState.playFoundSound = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject board = transitionState.levelObjects[(int)levelToLoad];
        BoardLogic bor = board.GetComponent<BoardLogic>();

        overlayText.text = bor.levelName;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject board = transitionState.levelObjects[(int)levelToLoad];
        BoardLogic bor = board.GetComponent<BoardLogic>();
        overlayText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (saveState.state == LevelState.LEVEL_STATE_LOCKED)
        {
            but.interactable = false;
        }
        else
        {
            but.interactable = true;
        }

        if (scaleTimer.isOn()) {
            TimerReturnInfo info = scaleTimer.updateTimer(Time.deltaTime);
            float newScale = Mathf.Lerp(1, 2, Mathf.Sin(Mathf.PI*info.canonicalVal));
            but.transform.localScale = new Vector3(newScale, newScale, 1);
            sys.transform.localScale = new Vector3(newScale, newScale, 1); 
            if (info.finished) {
                scaleTimer.turnTimerOff();
            }
        }

        if (saveState.state == LevelState.LEVEL_STATE_COMPLETED) {
            but.image.color = Color.green;
        } else if(saveState.state == LevelState.LEVEL_STATE_LOCKED) {
            but.image.color = Color.grey;
        } else if(saveState.state == LevelState.LEVEL_STATE_UNLOCKED) {
            but.image.color = Color.white;
        }
    }


    public void loadAndUnloadScene(System.Object obj) {
        LoadLevelData sceneToLoad = (LoadLevelData)obj;

        //transitionState.currentBoardActive = ;
        transitionState.boardParent.SetActive(true);
        transitionState.overworldParent.SetActive(false);
        GameObject board = transitionState.levelObjects[(int)sceneToLoad.type];
        transitionState.currentLevelType = sceneToLoad.type;
        board.SetActive(true);
        BoardLogic bor = board.GetComponent<BoardLogic>();
        bor.ResetBoard(true, true);
        //SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        //SceneManager.UnloadScene("Scenes/Overworld");
    }

    class LoadLevelData {
        public LevelType type;

        public LoadLevelData(LevelType t) {
            this.type = t;
        }

    }

    public void LoadNewScene() {
        sys.Play();
        LoadLevelData a = new LoadLevelData(levelToLoad);
        transitionState.SetTransition_(loadAndUnloadScene, a);
    }


}
