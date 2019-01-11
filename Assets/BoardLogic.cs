using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using easy_timer;
using easy_lex;
using easy_keystates;
using easy_transition;
using game_defines;
using Fitris;
using Levels;
using ExtraShapeNameSpace;

public class BoardLogic : MonoBehaviour
{

    [HideInInspector]
    public int boardWidth;
    [HideInInspector]
    public int boardHeight;

    private AppKeyStates gameKeyStates;
    public int cameraScale;

    private GameObject cameraInScene;

    private TransitionState transitionState;
    private GameObject[] spritesForGrid;
    private GameObject[] heartSpriteObjects;

    public int groupId;

    private ResourceManager resManager;

    public string levelName;
    private Text textForBoard;

    [TextArea(20, 20)]
    public string levelData;

    GameObject xpBarObject;
    GameObject xpBarBackGroundObject;

    [HideInInspector]
    public bool isFreestyle;

    [HideInInspector]
    public float paramsDt;

    private FitrisShape currentShape;

    private GlowingLine[] glowingLines;
    int glowingLinesCount;

    public ExtraShape[] extraShapesAvailable;

    private int extraShapeCount;
    private ExtraShape[] extraShapes;

    int experiencePoints;

    int maxExperiencePoints;
    [HideInInspector]
    public bool createShape;
    public LevelType currentLevelType;

    private Timer levelNameTimer;

    [HideInInspector]
    public int currentHotIndex;
    [HideInInspector]
    public bool letGo;

    [HideInInspector]
    public bool wasHoldingShape;

    [HideInInspector]
    public float accumHoldTime;

    public int[] startOffsets;

    public int[] shapeSizes;

    public bool isMirrorLevel;

    public int lifePointsMax;
    [HideInInspector]
    public int lifePoints;


    private BoardValue[] board;

    public static int MAX_SHAPE_COUNT = 16;

    void Awake()
    {

    }

    public enum GlowingLineType
    {
        GREEN_LINE,
        RED_LINE,
    }

    public enum GridOrder {
        GRID,
        GLOW,
        PREV,
        CURRENT,
        //
        GRID_ORDER_COUNT
    }


    public class GlowingLine
    {
        public GlowingLineType type;
        public int yAt;
        public Timer timer;
        public bool isDead;

        public GlowingLine(int yAt, GlowingLineType type)
        {
            timer = new Timer(1.0f);
            isDead = false;
            this.type = type;
            this.yAt = yAt;
        }
    }








    public class TransitionDataStartOrEndGame {
        public TransitionState.TransitionDataLevel levelData;
        public BoardLogic board;
    }

    void TransitionCallbackForLevel(System.Object data_)
    {
        TransitionState.TransitionDataLevel trans = (TransitionState.TransitionDataLevel)data_;
        BoardLogic board = trans.board;

        board.ResetBoard(true, false);


    }

    //void transitionCallbackForStartOrEndGame(System.Object data_)
    //{
    //    TransitionDataStartOrEndGame trans = (TransitionDataStartOrEndGame)data_;
    //    transitionCallbackForLevel(trans.levelData);
    //}

    //void transitionCallbackForBackToOverworld(void* data_)
    //{
    //    TransitionDataStartOrEndGame* trans = (TransitionDataStartOrEndGame*)data_;
    //    FrameParams *params = trans->params;
    //    for (int i = 0; i < LEVEL_COUNT; ++i)
    //    {
    //        params->levelsData[i].angle = 0;
    //        params->levelsData[i].dA = 0;
    //    }
    //    params->bgTex = findTextureAsset("blue_grass.png");
    //    trans->info->gameMode = trans->newMode;
    //    trans->info->lastMode = trans->lastMode;
    //    trans->info->menuCursorAt = 0;
    //    assert(parentChannelVolumes_[AUDIO_FLAG_MENU] == 0);
    //    setParentChannelVolume(AUDIO_FLAG_MENU, 1, SCENE_MUSIC_TRANSITION_TIME);
    //    setSoundType(AUDIO_FLAG_MENU);
    //}

    //void transitionCallbackForSettingsScreen(void* data_)
    //{
    //    TransitionDataStartOrEndGame* trans = (TransitionDataStartOrEndGame*)data_;
    //    FrameParams *params = trans->params;
    //    trans->info->gameMode = trans->newMode;
    //    trans->info->lastMode = trans->lastMode;
    //    trans->info->menuCursorAt = 0;
    //    setParentChannelVolume(AUDIO_FLAG_MENU, 1, SCENE_MUSIC_TRANSITION_TIME);
    //    setSoundType(AUDIO_FLAG_MENU);
    //}

        private void SetNextLevelTransition(LevelType nextLevel) {
            TransitionState.TransitionDataLevel data = new TransitionState.TransitionDataLevel();
            data.levelType = nextLevel;
            data.lastLevel = currentLevelType;
            data.board = null;
            this.transitionState.SetTransition_(transitionState.NextLevelTransition, data);
            
        }
    private void SetLevelTransition(LevelType levelType)
    {
        TransitionState.TransitionDataLevel data = new TransitionState.TransitionDataLevel();
        data.levelType = levelType;
        data.board = this;
        this.transitionState.SetTransition_(TransitionCallbackForLevel, data);
    }

    //void setBackToOverworldTransition(FrameParams*params)
    //{
    //    TransitionDataStartOrEndGame* data = (TransitionDataStartOrEndGame*)calloc(sizeof(TransitionDataStartOrEndGame), 1);
    //    data->info = &params->menuInfo;
    //    data->lastMode = params->menuInfo.gameMode;
    //    data->newMode = OVERWORLD_MODE;
    //    data->params = params;
    //    setParentChannelVolume(AUDIO_FLAG_MAIN, 0, SCENE_MUSIC_TRANSITION_TIME);
    //    setTransition_(&params->transitionState, transitionCallbackForBackToOverworld, data);

    //}

    //void setToLoadScreenTransition(FrameParams*params)
    //{
    //    TransitionDataStartOrEndGame* data = (TransitionDataStartOrEndGame*)calloc(sizeof(TransitionDataStartOrEndGame), 1);
    //    data->info = &params->menuInfo;
    //    data->lastMode = params->menuInfo.gameMode;
    //    data->newMode = SETTINGS_MODE;
    //    updateSaveStateDetails(params->menuInfo.saveStateDetails, arrayCount(params->menuInfo.saveStateDetails));
    //    data->params = params;
    //    setParentChannelVolume(AUDIO_FLAG_MAIN, 0, SCENE_MUSIC_TRANSITION_TIME);
    //    setTransition_(&params->transitionState, transitionCallbackForSettingsScreen, data);

    //}

    //void setStartOrEndGameTransition(FrameParams*params, LevelType levelType, GameMode newMode)
    //{
    //    TransitionDataStartOrEndGame* data = (TransitionDataStartOrEndGame*)calloc(sizeof(TransitionDataStartOrEndGame), 1);
    //    data->levelData.params = params;
    //    data->levelData.levelType = levelType;

    //    data->info = &params->menuInfo;
    //    data->lastMode = params->menuInfo.gameMode;
    //    data->newMode = newMode;

    //    setParentChannelVolume(AUDIO_FLAG_MENU, 0, SCENE_MUSIC_TRANSITION_TIME);
    //    //this one is different to just setLevelTransition since it changes game mode as well. 
    //    setTransition_(&params->transitionState, transitionCallbackForStartOrEndGame, data);
    //}


    private Texture GetBoardTex(BoardValue boardVal, BoardState boardState, BoardValType type)
    {
        Texture tex = null;
        if (boardState != BoardState.BOARD_NULL)
        {
            switch (boardState)
            {
                case BoardState.BOARD_STATIC:
                    {
                        if (type == BoardValType.BOARD_VAL_OLD)
                        {
                            tex = resManager.metalTex;
                            Assert.IsNotNull(tex);
                        }
                        else if (type == BoardValType.BOARD_VAL_DYNAMIC)
                        {
                            tex = resManager.woodTex;
                            Assert.IsNotNull(tex);
                        }
                        else if (type == BoardValType.BOARD_VAL_ALWAYS)
                        {
                            tex = resManager.stoneTex;
                            Assert.IsNotNull(tex);
                        }
                        else
                        {
                            Assert.IsTrue(false);
                        }
                        Assert.IsNotNull(tex);
                    }
                    break;
                case BoardState.BOARD_EXPLOSIVE:
                    {
                        tex = resManager.explosiveTex;
                    }
                    break;
                case BoardState.BOARD_SHAPE:
                    {
                        switch (type)
                        {
                            case BoardValType.BOARD_VAL_SHAPE0:
                                {
                                    tex = resManager.alienSprites[0];
                                }
                                break;
                            case BoardValType.BOARD_VAL_SHAPE1:
                                {
                                    tex = resManager.alienSprites[1];
                                }
                                break;
                            case BoardValType.BOARD_VAL_SHAPE2:
                                {
                                    tex = resManager.alienSprites[2];
                                }
                                break;
                            case BoardValType.BOARD_VAL_SHAPE3:
                                {
                                    tex = resManager.alienSprites[3];
                                }
                                break;
                            case BoardValType.BOARD_VAL_SHAPE4:
                                {
                                    tex = resManager.alienSprites[4];
                                }
                                break;
                            default:
                                {
                                    Assert.IsTrue(false);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    {
                        Assert.IsTrue(false);
                    }
                    break;
            }
            Assert.IsNotNull(tex);
        }
        return tex;
    }

    public BoardState GetBoardState(Vector2 pos)
    {
        BoardState result = BoardState.BOARD_INVALID;
        if (pos.x >= 0 && pos.x < this.boardWidth && pos.y >= 0 && pos.y < this.boardHeight)
        {
            BoardValue val = this.board[this.boardWidth * (int)pos.y + (int)pos.x];
            result = val.state;
            Assert.IsTrue(result != BoardState.BOARD_INVALID);
        }

        return result;
    }

    public void PlayExplosiveSound()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(resManager.explosiveSound, 1.0f);
    }

    public void PlaySuccessSound() {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(resManager.successSound, 1.0f);
    }

    public void playMoveSound()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(resManager.moveSound, 1.0f);
    }

    public void PlaySolidfySound()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(resManager.solidfySound, 1.0f);
    }

    public void playArrangeSound()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(resManager.arrangeSound, 1.0f);

    }

    public BoardValue GetBoardValue(Vector2 pos)
    {
        BoardValue result = new BoardValue();
        if (pos.x >= 0 && pos.x < this.boardWidth && pos.y >= 0 && pos.y < this.boardHeight)
        {
            BoardValue val = this.board[this.boardWidth * (int)pos.y + (int)pos.x];
            result = val;
            result.valid = true;
        }
        else
        {
            result.valid = false;
        }

        return result;
    }

    public bool InBoardBounds(Vector2 pos)
    {
        bool result = false;
        if (pos.x >= 0 && pos.x < this.boardWidth && pos.y >= 0 && pos.y < this.boardHeight)
        {
            result = true;
        }
        return result;
    }

    public void SetBoardState(Vector2 pos, BoardState state, BoardValType type)
    {
        if (pos.x >= 0 && pos.x < this.boardWidth && pos.y >= 0 && pos.y < this.boardHeight)
        {
            BoardValue val = this.board[this.boardWidth * (int)pos.y + (int)pos.x];
            val.prevState = val.state;
            val.prevType = val.type;
            val.state = state;
            val.type = type;
            val.fadeTimer.initTimer(GameDefines.FADE_TIMER_INTERVAL);
        }
        else
        {
            Assert.IsTrue(false);
        }
    }

    private Renderer SetTextureForGridObject(GameObject obj, Texture tex) {
        Assert.IsNotNull(obj);
        Renderer gridRender = obj.GetComponent<Renderer>();
        gridRender.material.SetTexture("_MainTex", tex);
        return gridRender;
    }

    private void SetTextureForGridObjectAndColor(GameObject obj, Texture tex, Vector4 color) {
        Renderer rend = SetTextureForGridObject(obj, tex);
        rend.material.SetColor("_Color", color);
    }

    void SetupAndRenderXpBar(bool setup) {

        float barHeight = 0.4f;
        Assert.IsTrue(maxExperiencePoints > 0);
        float ratioXp = Mathf.Clamp01((float)experiencePoints / (float)maxExperiencePoints);
        float startXp = -0.5f; //move back half a square
        float halfXp = 0.5f * boardWidth;
        float xpWidth = ratioXp * boardWidth;
        Vector3 xpPos = new Vector3(startXp + 0.5f * xpWidth, -2 * barHeight, -2);

        if (setup)
        {
            xpBarObject = Instantiate(resManager.quadMesh, xpPos, Quaternion.identity, gameObject.transform);
            xpBarBackGroundObject = Instantiate(resManager.quadMesh, new Vector3(startXp + halfXp, -2 * barHeight, -2), Quaternion.identity, gameObject.transform);
            //SetTextureForGridObject(xpBarBackGroundObject, EmptyBar);
            Vector3 scale = new Vector3(boardWidth, barHeight, 1);
            xpBarBackGroundObject.transform.localScale = 1.1f * scale;
        }
        xpBarObject.transform.localScale = new Vector3(xpWidth, barHeight, 1);
        xpBarObject.transform.position = xpPos;
    }

    void RenderXPBarAndHearts() {
        if (lifePointsMax > 0)
        {
            for (int heartIndex = 0; heartIndex < lifePointsMax; ++heartIndex)
            {
                 GameObject heartObject = heartSpriteObjects[heartIndex];

                Texture heartTex = null;
                if (lifePoints <= heartIndex)
                {
                    heartTex = resManager.heartEmptyTex;
                }
                else
                {
                    heartTex = resManager.heartFullTex;
                }

                SetTextureForGridObject(heartObject, heartTex);


            }
        }
        SetupAndRenderXpBar(false);

    }
    void InitXPBarAndHearts()
    {
        heartSpriteObjects = new GameObject[lifePointsMax];
        if (lifePointsMax > 0) {
            float heartDim = 0.6f;
            float across = lifePointsMax * heartDim / 2;
            float heartY = boardHeight;
            float xAt = 0.5f * boardWidth - across;
            for (int heartIndex = 0; heartIndex < lifePointsMax; ++heartIndex) {


                GameObject heartObject = Instantiate(resManager.spriteForGridPrefab, new Vector3(xAt, heartY, -2), Quaternion.identity, gameObject.transform);
                heartObject.transform.localScale = new Vector3(heartDim, heartDim, 1);
                Assert.IsNotNull(heartObject);

                heartSpriteObjects[heartIndex] = heartObject;
                heartObject.SetActive(true);
                SetTextureForGridObject(heartObject, resManager.heartFullTex);

                xAt += heartDim;
            }
        }

        if (isFreestyle) {

        } else {
            SetupAndRenderXpBar(true);
        }
    }


    public Vector2 ParseGetBoardDim(string str)
    {

        Vector2 result = new Vector2(0, 0);
        int xAt = 0;
        int yAt = 0;
        int at = 0;
        at = Lexer.LexEatWhiteSpace(str, at);
        while (at < str.Length)
        {
            at = Lexer.LexEatWhiteSpaceExceptNewLine(str, at);
            if (at < str.Length)
            {
                switch (str[at])
                {
                    case '\r':
                    case '\n':
                    {
                        if (xAt > result.x)
                        {
                            result.x = xAt;
                        }
                        xAt = 0;
                        yAt++;
                        at = Lexer.LexEatWhiteSpace(str, at);
                    }
                    break;
                    default:
                    {
                        //Without this ist seems to only cause a bug on the last line of the baord
                        if (!(str[at] == '/' || str[at] == ')'))
                        {
                            xAt++;
                        }
                        at++;
                        if (at >= str.Length)
                        {
                            Assert.IsTrue(at == str.Length);
                            yAt++;
                        }
                    }
                    break;
                }
            }
        }
        result.y = yAt;
        return result;
    }

    private ExtraShape AddExtraShape()
    {
        Assert.IsTrue(extraShapeCount < extraShapes.Length);
        int indexAt = extraShapeCount++;
        extraShapes[indexAt] = new ExtraShape(new Vector2(), 1, 0);
        ExtraShape shape = extraShapes[indexAt];
        Assert.IsNotNull(shape.timer);
        //if(currentLevelType == LevelType.LEVEL_20) {
        //    Debug.Log("Length is " + extraShapeCount);
        //}
        return shape;
    }

    public void AddGlowingLine(int yAt, GlowingLineType type)
    {
        Assert.IsNotNull(glowingLines);
        Assert.IsTrue(glowingLinesCount < glowingLines.Length);
        int indexAt = glowingLinesCount++;
        GlowingLine line = glowingLines[indexAt] = new GlowingLine(yAt, type);

        for(int xAt = 0; xAt < boardWidth; xAt++) {
            GameObject obj = GetSpriteObjectForGrid(xAt, yAt, GridOrder.GLOW);
            Assert.IsNotNull(obj);
            obj.SetActive(true);

            Texture winTexture = null;
            if(type == GlowingLineType.RED_LINE) {
                winTexture = resManager.redWinTex;
            } else if(type == GlowingLineType.GREEN_LINE) {
                winTexture = resManager.greenWinTex;
            }
            SetTextureForGridObject(obj, winTexture);
            obj.SetActive(true);

        }
    }

    //finds a shape from an id. Used for level loading 
    ExtraShape FindExtraShape(ExtraShape[] shapes, int count, int id)
    {
        ExtraShape result = null;
        for (int i = 0; i < count; ++i)
        {

            ExtraShape shape = shapes[i];
            if (shape.id == id)
            {
                result = shape;
                break;
            }
        }
        Assert.IsNotNull(result);
        return result;
    }

    class SquareProperty {
        public char id;
        public int shapeId;
    }

    void CreateLevelFromString() {

        int propertyCount = 0;
        SquareProperty[] squareProperties = new SquareProperty[64];

        extraShapeCount = 0;

        int at = 0;
        at = Lexer.LexEatWhiteSpace(levelData, at);
        int xAt = 0;
        int yAt = boardHeight - 1;
        //NOTE: Parse the board. Since it is individual numbers we are concerned with we can use the standard tokensizer. 
        bool justNewLine = false;
        //Debug.Log(levelData);
        while (at < levelData.Length)
        {
            at = Lexer.LexEatWhiteSpaceExceptNewLine(levelData, at);

            switch (levelData[at])
            {
                case '\r':
                case '\n':
                    {
                        xAt = 0;
                        yAt--;
                        Assert.IsTrue(!justNewLine);

                        justNewLine = true;
                        at = Lexer.LexEatWhiteSpace(levelData, at);
                        Assert.IsTrue(levelData[at] != '\r' && levelData[at] != '\n');
                    }
                    break;
                case '0':
                    {
                        xAt++;
                        at++;
                        justNewLine = false;
                    }
                    break;
                case '/':
                    {
                        Assert.IsTrue(yAt >= 0);
                        AddGlowingLine(yAt, GlowingLineType.GREEN_LINE);
                        at++;
                        justNewLine = false;
                        maxExperiencePoints += GameDefines.XP_PER_LINE;
                    }
                    break;
                case ')':
                    {
                        Assert.IsTrue(yAt >= 0);
                        AddGlowingLine(yAt, GlowingLineType.RED_LINE);
                        at++;
                        justNewLine = false;
                        maxExperiencePoints += GameDefines.XP_PER_LINE;
                    }
                    break;
                case 'a':
                    {
                        SetBoardState(new Vector2(xAt, yAt), BoardState.BOARD_STATIC, BoardValType.BOARD_VAL_ALWAYS);
                        xAt++;
                        at++;
                        justNewLine = false;
                    }
                    break;
                case 'b':
                    {
                        Assert.IsTrue(false);
                        // setBoardState(params, v2(xAt, yAt), BOARD_STATIC, BOARD_VAL_GLOW);    
                        // xAt++;
                        // at++;
                        // justNewLine = false;
                    }
                    break;
                case '!':
                    {
                        if (lifePointsMax == 0)
                        {
                            Debug.Log("LEVEL TYPE IS: " + currentLevelType);
                        }
                        Assert.IsTrue(lifePointsMax > 0);
                        SetBoardState(new Vector2(xAt, yAt), BoardState.BOARD_EXPLOSIVE, BoardValType.BOARD_VAL_ALWAYS);
                        at++;
                        xAt++;
                        justNewLine = false;
                    }
                    break;
                default:
                    {
                        int shapeIdToLookFor = -1;
                        if (Lexer.LexIsNumeric(levelData[at]))
                        {
                            shapeIdToLookFor = (int)((levelData[at]) - 48);
                            Assert.IsTrue(shapeIdToLookFor >= 0);
                        }
                        else
                        {
                            SquareProperty prop = null;
                            for (int propIndex = 0; propIndex < propertyCount; ++propIndex)
                            {
                                SquareProperty tempProp = squareProperties[propIndex];
                                if (tempProp.id == levelData[at])
                                {
                                    prop = tempProp;
                                }
                            }
                            if (prop != null)
                            {
                                shapeIdToLookFor = prop.shapeId;
                                //add other information for different square info 
                            }
                            else
                            {
                                //printf("the id %c didn't match a square property\n", *at);
                            }
                        }

                        if (shapeIdToLookFor >= 0)
                        {
                            ExtraShape shapeOnStack = FindExtraShape(extraShapesAvailable, extraShapesAvailable.Length, shapeIdToLookFor);
                            Assert.IsNotNull(shapeOnStack);
                            ExtraShape shp = AddExtraShape();
                            shapeOnStack.CopyShape(shp);
                            shp.pos.x = xAt;
                            shp.pos.y = yAt;
                        }
                        xAt++;
                        at++;
                        justNewLine = false;
                    } break;
            }
        }
    }

    private void AllocateBoardValues() {
        this.board = new BoardValue[this.boardWidth * this.boardHeight];
        for (int boardY = 0; boardY < this.boardHeight; ++boardY)
        {
            for (int boardX = 0; boardX < this.boardWidth; ++boardX)
            {
                this.board[boardY * this.boardWidth + boardX] = new BoardValue();
            }
        }

    }

    public void ResetBoard(bool clearSprites, bool displayName)
    {
        this.currentShape.RenewShape();

        this.currentShape.ResetMouseUI(this);

        this.experiencePoints = 0;
        this.currentHotIndex = -1;
        this.maxExperiencePoints = 0;
        this.glowingLinesCount = 0;
        this.accumHoldTime = 0;
        this.letGo = false;

        this.createShape = true;

        if (displayName)
        {
            levelNameTimer.turnTimerOn();
        }

        if (clearSprites)
        {
            for (int boardY = 0; boardY < this.boardHeight; ++boardY)
            {
                for (int boardX = 0; boardX < this.boardWidth; ++boardX)
                {
                    BoardValue val = this.board[boardY * this.boardWidth + boardX];
                    val.ClearBoardValue();

                    for (int i = 0; i < (int)GridOrder.GRID_ORDER_COUNT; ++i)
                    {
                        if (i != 0)
                        {
                            GameObject gridObj = GetSpriteObjectForGrid(boardX, boardY, (GridOrder)i);
                            gridObj.SetActive(false);
                            SetTextureForGridObject(gridObj, null);

                        }
                    }

                }
            }

        }
        CreateLevelFromString();
        this.lifePoints = this.lifePointsMax;

       
    }

    public void CreateSpriteObjectsInScene() {
        spritesForGrid = new GameObject[this.boardWidth * this.boardHeight * (int)GridOrder.GRID_ORDER_COUNT];
        for (int y = 0; y < this.boardHeight; y++)
        {
            for (int x = 0; x < this.boardWidth; x++)
            {
                for (int i = 0; i < (int)GridOrder.GRID_ORDER_COUNT; ++i)
                {
                    if (resManager.spriteForGridPrefab == null)
                    {
                        Debug.Log("prefab null");
                    }
                    GameObject newGridCell = Instantiate(resManager.spriteForGridPrefab, new Vector3(x, y, -i), Quaternion.identity, gameObject.transform);
                    Assert.IsNotNull(newGridCell);

                    GameObject gridObj = spritesForGrid[(y * this.boardWidth * (int)GridOrder.GRID_ORDER_COUNT) + (x * (int)GridOrder.GRID_ORDER_COUNT) + i] = newGridCell;
                    if (i == 0)
                    {
                        gridObj.SetActive(true);
                        SetTextureForGridObject(gridObj, resManager.gridTex);
                    }
                    else
                    {
                        gridObj.SetActive(false);
                    }
                }
            }
        }
    }

        

    // Use this for initialization
        void Start()
    {
        if (GameDefines.START_LEVEL != currentLevelType)
        {
            gameObject.SetActive(false);

        }

        GameObject canvasObj = GameObject.Find("TransitionCanvas");

        textForBoard = GameObject.Find("LevelText").GetComponent<Text>();

        resManager = canvasObj.GetComponent<ResourceManager>();

        transitionState = canvasObj.GetComponent<TransitionState>();

        transitionState.levelObjects[(int)currentLevelType] = gameObject;
        Assert.IsNotNull(transitionState.levelObjects[(int)currentLevelType]);

        transitionState.saveStates[(int)currentLevelType].type = currentLevelType;

        LevelGroup group = transitionState.levelGroups[this.groupId];
        group.AddLevelToGroup(currentLevelType);

        gameKeyStates = new AppKeyStates();
        Vector2 boardDim = ParseGetBoardDim(levelData);
        boardWidth = (int)boardDim.x;
        boardHeight = (int)boardDim.y;
        //Debug.Log(boardWidth);
        //Debug.Log(boardHeight);

        this.glowingLines = new GlowingLine[16];
        AllocateBoardValues();

        CreateSpriteObjectsInScene();

        extraShapes = new ExtraShape[16];

        this.currentShape = new FitrisShape(shapeSizes[0], gameKeyStates);
        Assert.IsNotNull(this.currentShape);
        levelNameTimer = new Timer(2.0f);
        levelNameTimer.turnTimerOn();
        this.ResetBoard(false, true);
        InitXPBarAndHearts();

        cameraInScene = GameObject.FindWithTag("MainCamera");
        //Debug.Log("camera z: " + cameraInScene.transform.position.z);

        Assert.IsNotNull(this.currentShape);

    }



    void UpdateBoardRows()
    {
        for (int boardY = 0; boardY < boardHeight; ++boardY) {
            bool isFull = true;
            for (int boardX = 0; boardX < boardWidth && isFull; ++boardX) {
                BoardValue boardVal = board[boardY *boardWidth + boardX];

                if (!(boardVal.state == BoardState.BOARD_STATIC && boardVal.type == BoardValType.BOARD_VAL_OLD))
                {
                    isFull = false;
                }
            }
            if (isFull)
            {
                for (int boardX = 0; boardX < boardWidth; ++boardX) {
                    this.SetBoardState(new Vector2(boardX, boardY), BoardState.BOARD_NULL, BoardValType.BOARD_VAL_OLD);
                }
            }
        }
    }

    public GameObject GetSpriteObjectForGrid(int boardX, int boardY, GridOrder order) {

        Assert.IsTrue((boardX >= 0 && boardX < boardWidth && boardY >= 0 && boardY < boardHeight && (int)order >= 0 && (int)order < (int)GridOrder.GRID_ORDER_COUNT));
        GameObject result = spritesForGrid[(boardY * this.boardWidth * (int)GridOrder.GRID_ORDER_COUNT) + boardX * (int)GridOrder.GRID_ORDER_COUNT + (int)order];
        return result;
    }

    public void UpdateBoardWinState()
    {
        for (int winLineIndex = 0; winLineIndex < this.glowingLinesCount; winLineIndex++) {
            GlowingLine line = this.glowingLines[winLineIndex];
            if (!line.isDead)
            {
                int boardY = line.yAt;
                bool win = true;
                for (int boardX = 0; boardX < boardWidth; ++boardX) {
                    BoardValue boardVal = board[boardY *boardWidth + boardX];

                    bool answer = true;
                    switch (line.type)
                    {
                        case GlowingLineType.GREEN_LINE:
                            {
                                if (!(boardVal.state == BoardState.BOARD_STATIC && (boardVal.type == BoardValType.BOARD_VAL_OLD || boardVal.type == BoardValType.BOARD_VAL_ALWAYS)))
                                {
                                    answer = false;
                                }
                            }
                            break;
                        case GlowingLineType.RED_LINE:
                            {
                                if (!(boardVal.state == BoardState.BOARD_STATIC && boardVal.type == BoardValType.BOARD_VAL_DYNAMIC))
                                {
                                    answer = false;
                                }
                            }
                            break;
                        default:
                            {
                                Assert.IsTrue(false);
                            } break;
                    }
                    if (!answer)
                    {
                        win = false;
                        break;
                    }

                    Assert.IsTrue(boardVal.state != BoardState.BOARD_INVALID);
                }

                if (win)
                {
                    this.experiencePoints += GameDefines.XP_PER_LINE;
                    for (int boardX = 0; boardX < boardWidth; ++boardX) {
                        BoardValue boardVal = board[boardY *boardWidth + boardX];
                        if (boardVal.state == BoardState.BOARD_STATIC && boardVal.type == BoardValType.BOARD_VAL_OLD)
                        {
                            //only get rid of the squares have been the player's shape
                            this.SetBoardState(new Vector2(boardX, boardY), BoardState.BOARD_NULL, BoardValType.BOARD_VAL_OLD);
                        }
                        else
                        {
                            // printf("state is: %d\n", boardVal->state);
                            // printf("type is: %d\n", boardVal->type);
                            // assert(!"type not recognised");
                        }
                    }
                    PlaySuccessSound();
                     
                    line.isDead = true;
                    line.timer.initTimer(0.5f);
                }

            }
        }

        bool allLinesAreDead = true;

        for (int glowLineIndex = 0; glowLineIndex < this.glowingLinesCount && allLinesAreDead; ++glowLineIndex) {
            GlowingLine glLine = glowingLines[glowLineIndex];
            allLinesAreDead &= glLine.isDead; //try break the match 
        }

        if (allLinesAreDead && !this.isFreestyle && transitionState.currentTransition == null) {
            //This is finishing a level
            int levelAsInt = (int)currentLevelType;

            transitionState.saveStates[(int)currentLevelType].state = LevelState.LEVEL_STATE_COMPLETED;

            LevelGroup nextGroup = null;
            bool completedGroup = true;
            LevelType nextLevel = LevelType.LEVEL_0;
            LevelGroup group = transitionState.levelGroups[this.groupId];
            Assert.IsTrue(group.count > 0);
            for(int gIndex = 0; gIndex < group.count; ++gIndex)
            {
                LevelType checkType = group.groups[gIndex];
                LevelState state = transitionState.saveStates[(int)checkType].state;
                completedGroup &= (state == LevelState.LEVEL_STATE_COMPLETED);
                if (!completedGroup)
                {
                    nextLevel = group.groups[gIndex];
                    Assert.IsTrue(nextLevel < LevelType.LEVEL_COUNT);
                    break;
                }
            }
            if (transitionState.currentTransition == null) {
                bool goToNextLevel = true;
                if (completedGroup)
                {
                    //This assumes groups have to be consecutive ie. have to have group 0, 1, 2, 3 can't 
                    //miss any ie. 0, 2, 4
                    int nextGroupId = groupId + 1;

                    nextGroup = transitionState.levelGroups[nextGroupId];
                    if (nextGroup.count == 0)
                    {
                        //FINSIHED THE GAME 
                        //setStartOrEndGameTransition(params, LEVEL_0, CREDITS_MODE);
                        goToNextLevel = false;
                    }
                    else
                    {
                        //Debug.Log("unclock Groups");
                        Assert.IsTrue(nextGroup.count > 0);
                        nextLevel = nextGroup.groups[0];

                        //transitionState.levelObjects[nextLevel].gr;
                        //Assert.IsTrue(trans.groupId == (int)nextGroupId);
                        //Assert.IsTrue(nextGroupId > groupId);

                        //NOTE: Unlock the next group
                        //Assert.IsTrue(listAt == nextGroupListAt);
                        for (int gIndex = 0; gIndex < nextGroup.count; ++gIndex)
                        {
                            LevelType checkType = nextGroup.groups[gIndex];
                            SaveState saveState = transitionState.saveStates[(int)checkType];

                            if (saveState.state == LevelState.LEVEL_STATE_LOCKED)
                            {
                                Debug.Log("unlocking level: " + checkType);
                                //this is since we can go back and complete levels again. 
                                saveState.state = LevelState.LEVEL_STATE_UNLOCKED;
                            }
                        }
                    }
                }

                if (goToNextLevel)
                {
                    if (completedGroup)
                    {
                        Assert.IsNotNull(nextGroup);
                        TransitionState.OverworldTransLevelData data = new TransitionState.OverworldTransLevelData(currentLevelType, nextGroup.GetPos(transitionState));
                        transitionState.SetTransition_(transitionState.BackToWorldTransition, data);
                    }
                    else
                    {

                        SetNextLevelTransition(nextLevel);
                    }

                }
            }
            transitionState.SaveGameState();
        }
    }


    GlowingLine CheckIsWinLine(int lineCheckAt)
    {
        GlowingLine result = null;
        for (int winLineIndex = 0; winLineIndex < this.glowingLinesCount; winLineIndex++)
        {

            GlowingLine line = this.glowingLines[winLineIndex];
            int line_yAt = line.yAt;
            //Debug.Log("yAt" + line_yAt);
            //Debug.Log("yAt to check" + lineCheckAt);
            if (line_yAt == lineCheckAt)
            {
                result = line;
                break;
            }
        }
        return result;
    }

    void RemoveWinLine(int lineToRemove)
    {
        bool found = false;
        for (int winLineIndex = 0; winLineIndex < this.glowingLinesCount; winLineIndex++)
        {
            GlowingLine line = glowingLines[winLineIndex];
            if (line.yAt == lineToRemove)
            {
                //NOTE: Writing the last one in the array over the one we are removing.
                int lastIndex = --this.glowingLinesCount;
                this.glowingLines[winLineIndex] = this.glowingLines[lastIndex];

                for (int xAt = 0; xAt < boardWidth; ++xAt) {
                    GameObject obj = GetSpriteObjectForGrid(xAt, line.yAt, GridOrder.GLOW);
                    SetTextureForGridObject(obj, null);
                    obj.SetActive(false);

                }


                found = true;
                break;
            }
        }
        Assert.IsTrue(found);
    }

    // Update is called once per frame
    void Update()
    {
        if(transitionState.lastLevelType != transitionState.currentLevelType) { 
            //set the board title and camera position
            textForBoard.text = this.levelName;
            transitionState.lastLevelType = transitionState.currentLevelType;
            cameraInScene.transform.position = 0.5f * new Vector3((float)boardWidth - 1, boardHeight - 1, -10);
            cameraInScene.GetComponent<Camera>().orthographicSize = cameraScale;
        }

        if(levelNameTimer.isOn()) { 
            TimerReturnInfo info = levelNameTimer.updateTimer(Time.deltaTime);

            Color newColor = Color.Lerp(Color.clear, Color.black, Mathf.Sin(Mathf.PI*info.canonicalVal));
            textForBoard.color = newColor;
            if(info.finished) {
                levelNameTimer.turnTimerOff();
            }
        } else {
            textForBoard.color = Color.clear;
        }

        if (transitionState.currentTransition == null)
        {
            Assert.IsNotNull(this.currentShape);
            paramsDt = Time.deltaTime;
            bool retryButtonPressed = false;
            float dtLeft = paramsDt;
            float oldDt = paramsDt;
            float increment = 1.0f / 480.0f;
            bool rAWasPressed = Input.GetKeyDown("r");
            // if(rAWasPressed) { printf("%s\n", "r Was pressed"); }
            while (dtLeft > 0.0f && transitionState.currentTransition == null)
            {
                gameKeyStates.ProcessKeyStates(cameraInScene);

                paramsDt = Mathf.Min(increment, dtLeft);
                Assert.IsTrue(paramsDt > 0.0f);
                //if updating a transition don't update the game logic, just render the game board. 
                bool canDie = this.lifePointsMax > 0;

                bool retryLevel = ((this.lifePoints <= 0) && canDie) || rAWasPressed || retryButtonPressed;
                if (this.createShape || retryLevel)
                {
                    if (!retryLevel)
                    {
                        this.currentShape.count = 0;
                    }
                    int totalShapeCountSoFar = 0;
                    for (int shpIndex = 0; shpIndex < this.shapeSizes.Length && !retryLevel; shpIndex++)
                    {
                        int shpSizeAt = this.shapeSizes[shpIndex];
                        int shpOffset = this.startOffsets[shpIndex];
                        for (int i = 0; i < shpSizeAt && !retryLevel; ++i)
                        {
                            int xAt = i + shpOffset;

                            int rowOffset = 0;
                            if (xAt >= boardWidth)
                            {
                                rowOffset = xAt / boardWidth;
                                xAt %= boardWidth;
                            }
                            int yAt = (boardHeight - 1) - rowOffset;
                            // assert(i == params->currentShape.count);
                            FitrisBlock block = currentShape.blocks[currentShape.count++];
                            block.pos = new Vector2(xAt, yAt);
                            block.type = (BoardValType.BOARD_VAL_SHAPE0 + i);
                            block.id = totalShapeCountSoFar;

                            Vector2 pos = new Vector2(xAt, yAt);
                            if (GetBoardState(pos) != BoardState.BOARD_NULL)
                            {
                                //at the top of the board
                                retryLevel = true;
                                break;
                                //
                            }
                            else
                            {
                                SetBoardState(pos, BoardState.BOARD_SHAPE, block.type);
                            }
                            totalShapeCountSoFar++;
                        }
                    }
                    if (retryLevel)
                    {
                        if (transitionState.currentTransition == null)
                        {
                            SetLevelTransition(currentLevelType);
                        }
                        else
                        {
                            Assert.IsTrue(false);
                            //Couldn't reset board!!!
                        }
                    }

                    currentShape.moveTimer.value = 0;
                    createShape = false;
                    // assert(params->currentShape.count > 0);

                }

                currentShape.UpdateShapeMoveTime(this);
                Assert.IsNotNull(extraShapes);
                for (int extraIndex = 0; extraIndex < extraShapeCount; ++extraIndex)
                {
                    ExtraShape extraShape = extraShapes[extraIndex];

                    float tUpdate = (extraShape.timeAffected) ? currentShape.moveTime : paramsDt;

                    while (tUpdate > 0.0f)
                    {
                        Assert.IsTrue(paramsDt >= 0);

                        float dt = Mathf.Min(increment, tUpdate);
                        if (dt > paramsDt)
                        {
                            //printf("Error: dt: %f paramsDt: %f\n", dt, params->dt);
                        }

                        float shapeDt = dt;
                        if (!extraShape.active && extraShape.lagTimer.period > 0.0f)
                        {
                            TimerReturnInfo lagInfo = extraShape.lagTimer.updateTimer(dt);
                            if (lagInfo.finished)
                            {
                                extraShape.lagTimer.turnTimerOn();
                                shapeDt = lagInfo.residue;
                                extraShape.active = true;
                            }
                        }

                        if (extraShape.active)
                        {
                            TimerReturnInfo info = extraShape.timer.updateTimer(shapeDt);
                            if (info.finished)
                            {
                               
                                extraShape.timer.turnTimerOn();
                                extraShape.updateWindmillSide(this);
                                extraShape.timer.value = info.residue;
                            }
                        }
                        tUpdate -= dt;
                        Assert.IsTrue(tUpdate >= 0.0f);
                    }
                }
                currentShape.UpdateAndRenderShape(this);

                for (int extraIndex = 0; extraIndex < extraShapeCount; ++extraIndex)
                {
                    ExtraShape extraShape = extraShapes[extraIndex];
                    if (extraShape.tryingToBegin)
                    {
                        extraShape.updateWindmillSide(this);
                    }
                    extraShape.tryingToBegin = false;
                }
                if (!this.isFreestyle)
                {
                    this.UpdateBoardWinState();
                }
                else
                {
                    this.UpdateBoardRows();
                }
                dtLeft -= paramsDt;
                Assert.IsTrue(dtLeft >= 0.0f);
            }
        }
        this.paramsDt = Time.deltaTime;

        if (levelNameTimer.isOn())
        {
            //TimerReturnInfo nameTimeInfo = levelNameTimer.updateTimer(Time.deltaTime);
            //V4 levelNameFontColor = smoothStep00V4(new Vector4(0, 0, 0, 0), nameTimeInfo.canonicalVal, Color.black);
            //float levelNameFontSize = 1.0f;
            //char* title = params->levelsData[params->currentLevelType].name;
            //float xFontAt = (resolution.x / 2) - (getBounds(title, menuMargin, params->font, levelNameFontSize, resolution).x / 2);
            //outputText(params->font, xFontAt, 0.5f * resolution.y, -1, resolution, title, menuMargin, levelNameFontColor, levelNameFontSize, true);
        }

        //outputText(params->font, 10, helpTextY, -1, resolution, "Press R to reset", menuMargin, COLOR_BLACK, 0.5f, true);

        RenderXPBarAndHearts();
        for (int boardY = 0; boardY < boardHeight; ++boardY)
        {
            GlowingLine isWinLine = CheckIsWinLine(boardY);

            bool renderWinLine = isWinLine != null;
            TimerReturnInfo winLineInfo;
            float alpha = 0.4f;
            Vector4 winColor = new Vector4(1, 1, 1, alpha);
            Texture winTex = resManager.greenWinTex;

            if (renderWinLine)
            {
                if (isWinLine.type == GlowingLineType.RED_LINE)
                {
                    //winColor = new Vector4(1, 0, 0, alpha);
                    winTex = resManager.redWinTex;
                }
                if (isWinLine.isDead)
                {
                    winLineInfo = isWinLine.timer.updateTimer(Time.deltaTime);
                    alpha = Mathf.Lerp(alpha, 0, winLineInfo.canonicalVal);

                    if (winLineInfo.finished)
                    {
                        if (isWinLine.isDead)
                        {
                            RemoveWinLine(isWinLine.yAt);
                        }
                    }
                }
            }

            for (int boardX = 0; boardX < this.boardWidth; ++boardX)
            {

                BoardValue boardVal = GetBoardValue(new Vector2(boardX, boardY));
                if (!(boardVal.prevState == BoardState.BOARD_NULL && boardVal.state == BoardState.BOARD_NULL))
                {
                    Vector4 currentColor = boardVal.color;
                    if (boardVal.fadeTimer.isOn())
                    {
                        TimerReturnInfo timeInfo = boardVal.fadeTimer.updateTimer(Time.deltaTime);

                        float lerpT = timeInfo.canonicalVal;
                        Vector4 prevColor = Vector4.Lerp(boardVal.color, Color.clear, Mathf.Clamp01(lerpT));
                        currentColor = Vector4.Lerp(Color.clear, boardVal.color, lerpT);

                        Texture prevTex = GetBoardTex(boardVal, boardVal.prevState, boardVal.prevType);
                        GameObject gridObj = GetSpriteObjectForGrid(boardX, boardY, GridOrder.PREV);
                        if (prevTex != null)
                        {
                            //This probably isn't the best way of doing things. 
                            gridObj.SetActive(true);
                            SetTextureForGridObjectAndColor(gridObj, prevTex, prevColor);

                            if (timeInfo.finished)
                            {
                                boardVal.prevState = boardVal.state;
                                boardVal.color = Color.white;
                            }
                        } else {
                            gridObj.SetActive(false);
                        }
                    }


                    Texture currentTex = GetBoardTex(boardVal, boardVal.state, boardVal.type);
                    GameObject gridObjCurrent = GetSpriteObjectForGrid(boardX, boardY, GridOrder.CURRENT);
                    if (currentTex != null)
                    {
                        //This probably isn't the best way of doing things. 
                        gridObjCurrent.SetActive(true);
                        SetTextureForGridObjectAndColor(gridObjCurrent, currentTex, currentColor);
                    }
                    else
                    {
                        gridObjCurrent.SetActive(false);
                    }
                }
                else
                {
                    Assert.IsTrue(!boardVal.fadeTimer.isOn());
                    GameObject obj= GetSpriteObjectForGrid(boardX, boardY, GridOrder.CURRENT);
                    obj.SetActive(false);
                    obj = GetSpriteObjectForGrid(boardX, boardY, GridOrder.PREV);
                    obj.SetActive(false);

                }

                if (renderWinLine)
                {
                    winColor.w = alpha;
                    GameObject gridObj = GetSpriteObjectForGrid(boardX, boardY, GridOrder.GLOW);
                    //This probably isn't the best way of doing things. 
                    gridObj.SetActive(true);
                    SetTextureForGridObjectAndColor(gridObj, winTex, winColor);

                }
            }
        }
    }
}

