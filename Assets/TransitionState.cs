using System;
using UnityEngine;
using easy_timer;
using game_defines;
using Levels;
using System.IO;
//using UnityEngine.
using json_array;
using UnityEngine.Assertions;
namespace easy_transition
{
    public delegate void transition_callback(System.Object obj);



    public class TransitionState : MonoBehaviour
    {
        [HideInInspector]
        public bool transitioning;
        [HideInInspector]
        public SceneTransition currentTransition;

        [HideInInspector]
        public SaveState[] saveStates;

        [HideInInspector]
        public LevelGroup[] levelGroups;

        [HideInInspector]
        public LevelType lastLevelType;
        public LevelType currentLevelType;
        public GameObject[] levelObjects;

        [HideInInspector]
        public bool playFoundSound;
        [HideInInspector]
        public bool canPlayFoundSound;

        public AudioClip foundSound;

        [HideInInspector]
        public RectTransform leftTrans;
        [HideInInspector]
        public RectTransform rightTrans;

        public GameObject boardParent;
        public GameObject overworldParent;

        [HideInInspector]
        public GameObject currentBoardActive;

        public AudioClip transitionSound;
        GameObject canvasObj;

        [HideInInspector]
        public ClickLevel[] overworldStars;


        public void DeleteSaveFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "saveFile.ftf");
            File.Delete(filePath);
        }

        public void SaveGameState()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "saveFile.ftf");
            string stringToWrite = JsonHelper.ToJson(saveStates, true);
            //Debug.Log(stringToWrite);
            File.WriteAllText(filePath, stringToWrite);
        }

        private void LoadSaveFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "saveFile.ftf");

            for (int i = 0; i < saveStates.Length; ++i)
            {
                saveStates[i] = new SaveState((LevelType)i, LevelState.LEVEL_STATE_LOCKED);
            }
            saveStates[(int)LevelType.LEVEL_0].state = LevelState.LEVEL_STATE_UNLOCKED;

            if (File.Exists(filePath))
            {
                // Read the json from the file into a string
                string dataAsJson = File.ReadAllText(filePath);
                Debug.Log(dataAsJson);
                saveStates = JsonHelper.FromJson<SaveState>(dataAsJson);

            }
        }

        public void Awake()
        {
            levelObjects = new GameObject[(int)LevelType.LEVEL_COUNT];
            saveStates = new SaveState[(int)LevelType.LEVEL_COUNT];
            levelGroups = new LevelGroup[(int)LevelType.LEVEL_COUNT];
            for (int i = 0; i < levelGroups.Length; ++i)
            {
                levelGroups[i] = new LevelGroup();
            }
            LoadSaveFile();

            overworldStars = new ClickLevel[(int)LevelType.LEVEL_COUNT];

            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            GameObject obj = GameObject.Find("TransitionCanvas/LeftPanel");
            GameObject obj1 = GameObject.Find("TransitionCanvas/RightPanel");
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj1);
            leftTrans = obj.GetComponent<RectTransform>();
            rightTrans = obj1.GetComponent<RectTransform>();
            currentLevelType = GameDefines.START_LEVEL;
            if (GameDefines.START_MODE == StartMode.OVERWORLD)
            {
                canPlayFoundSound = true;
            }
            //RectTransform canvasScaler = obj.GetComponent<Canvas>();
            //Vector2 refResolution = canvasScaler.referenceResolution();
        }

        public SceneTransition SetTransition_(transition_callback callback, System.Object data)
        {
            SceneTransition trans = new SceneTransition();

            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(this.transitionSound, 1.0f);

            trans.timer.initTimer(GameDefines.SCENE_TRANSITION_TIME);
            trans.data = data;
            trans.callback = callback;
            trans.direction = true;

            trans.next = this.currentTransition;

            this.currentTransition = trans;

            return trans;
        }



        public void Update()
        {
            if (playFoundSound && canPlayFoundSound)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.PlayOneShot(foundSound, 1.0f);
                playFoundSound = false;
                canPlayFoundSound = false;
            }
            SceneTransition trans = this.currentTransition;
            if (trans != null)
            {
                TimerReturnInfo timeInfo = trans.timer.updateTimer(Time.deltaTime);
                float canvasRelWidth = 1000;  //TODO: we could query for this. 
                float halfScreen = canvasRelWidth;

                //this should really be a smoothstep00 and somehow find out when we reach halfway to do the close interval. This could be another timer. 
                float tValue = timeInfo.canonicalVal;
                if (!trans.direction)
                {
                    tValue = 1.0f - timeInfo.canonicalVal;
                }
                float transWidth = Mathf.Lerp(0, halfScreen, tValue);
                //These are the black rects to make it look like a shutter opening and closing. 
                this.rightTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transWidth);
                this.leftTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, transWidth);

                if (timeInfo.finished)
                {
                    if (trans.direction)
                    {
                        //Assert.IsNotNull(trans.data);
                        trans.callback(trans.data);

                        trans.direction = false;
                        trans.timer.turnTimerOn();
                    }
                    else
                    {
                        //finished the transition
                        this.currentTransition = trans.next;
                    }
                }

            }
        }

        public class TransitionDataLevel
        {
            public LevelType levelType;
            public LevelType lastLevel;
            public BoardLogic board;
        }

        public class OverworldTransLevelData
        {
            public LevelType currentLvl;
            public OverworldTransLevelData(LevelType t)
            {
                currentLvl = t;
            }
        }

        public void BackToWorldTransition(System.Object obj)
        {
            canPlayFoundSound = true;
            OverworldTransLevelData nxtLevelData = (OverworldTransLevelData)obj;
            this.currentLevelType = nxtLevelData.currentLvl;

            levelObjects[(int)nxtLevelData.currentLvl].SetActive(false);
            this.boardParent.SetActive(false);
            this.overworldParent.SetActive(true);
            for (int i = 0; i < overworldStars.Length; ++i)
            {
                if (overworldStars[i] != null)
                {
                    overworldStars[i].Renew();
                }
            }

        }

        public void NextLevelTransition(System.Object obj)
        {
            TransitionDataLevel nxtLevelData = (TransitionDataLevel)obj;

            levelObjects[(int)nxtLevelData.lastLevel].SetActive(false);

            GameObject gameObj = levelObjects[(int)nxtLevelData.levelType];
            gameObj.SetActive(true);
            gameObj.GetComponent<BoardLogic>().ResetBoard(true, true);


        }

        public class SceneTransition
        {
            public transition_callback callback;
            public System.Object data;

            public Timer timer;
            public bool direction; //true for in //false for out


            public SceneTransition next;

            public SceneTransition()
            {
                timer = new Timer(0.5f);
            }

        }

    }
}
