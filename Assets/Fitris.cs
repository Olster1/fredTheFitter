using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using easy_timer;
using easy_keystates;

namespace Fitris {
    public enum BoardState
    {
        BOARD_NULL, //0
        BOARD_STATIC, //1
        BOARD_SHAPE, //2
        BOARD_EXPLOSIVE, //3
        BOARD_INVALID, //For out of bounds  //4
    };

    public enum BoardValType
    {
        BOARD_VAL_NULL,
        BOARD_VAL_OLD,
        BOARD_VAL_ALWAYS,
        BOARD_VAL_SHAPE0,
        BOARD_VAL_SHAPE1,
        BOARD_VAL_SHAPE2,
        BOARD_VAL_SHAPE3,
        BOARD_VAL_SHAPE4,
        BOARD_VAL_GLOW,
        BOARD_VAL_DYNAMIC, //this is for the obstacles windmill 
        BOARD_VAL_TRANSIENT, //this isn't used for anything, just to make it so we aren't using the other ones. 
    };

    public class BoardValue {
        public BoardValType type;
        public BoardValType prevType;
        public BoardState state;
        public BoardState prevState;

        public Vector4 color;

        public Timer fadeTimer;
        public bool valid;

        public BoardValue() {
            fadeTimer = new Timer(0);
            this.ClearBoardValue();
        }

        public void ClearBoardValue() {
            fadeTimer.turnTimerOff();

            this.type = BoardValType.BOARD_VAL_NULL;
            this.state = BoardState.BOARD_NULL;
            this.prevState = BoardState.BOARD_NULL;

            this.color = Color.white;
        }

    }

    public class FitrisBlock
    {
        public int id;
        public Vector2 pos;
        public BoardValType type; //this is used to keep the images consistent;

        public FitrisBlock() {
            pos = new Vector2(0, 0);
            id = 0; 
        }

        public void Copy(FitrisBlock block) {
            block.id = this.id;
            block.pos.x = this.pos.x;
            block.pos.y = this.pos.y;
            block.type = this.type;
        }
    };



    public class FitrisShape {
        public FitrisBlock[] blocks;
        public int count;
        readonly bool valid;

        public Timer moveTimer;
        public float moveTime;
        private AppKeyStates keyStates;

        public bool wasHitByExplosive;

        public FitrisShape(int size, AppKeyStates gameKeyStates)
        {
            this.keyStates = gameKeyStates;
            this.moveTimer = new Timer(1);
            this.moveTimer.value = 0;
            this.blocks = new FitrisBlock[BoardLogic.MAX_SHAPE_COUNT];
            for(int i = 0;  i < this.blocks.Length; ++i) {
                this.blocks[i] = new FitrisBlock();
            }
            this.count = size;
            this.wasHitByExplosive = false;
        }

        public void RenewShape() {
            this.moveTimer.value = 0;
            this.wasHitByExplosive = false;
    }


        public enum MoveType
        {
            MOVE_LEFT,
            MOVE_RIGHT,
            MOVE_DOWN
        };

        public Vector2 GetMoveVec(MoveType moveType)
        {
            Vector2 moveVec;
            if (moveType == MoveType.MOVE_LEFT)
            {
                moveVec = new Vector2(-1, 0);
            }
            else if (moveType == MoveType.MOVE_RIGHT)
            {
                moveVec = new Vector2(1, 0);
            }
            else if (moveType == MoveType.MOVE_DOWN)
            {
                moveVec = new Vector2(0, -1);
            }
            else
            {
                moveVec = new Vector2(0, 0);
                Assert.IsTrue(false);
            }
            return moveVec;
        }

        public bool canShapeMove(BoardLogic board, MoveType moveType)
        {
            bool result = false;
            bool valid = true;
            int leftMostPos = board.boardWidth;
            int rightMostPos = 0;
            int bottomMostPos = board.boardHeight; ;
            Vector2 moveVec = GetMoveVec(moveType);
            for (int i = 0; i < this.count; ++i)
            {
                Vector2 pos = this.blocks[i].pos;
                BoardState state = board.GetBoardState(pos + moveVec);
                if (!(state == BoardState.BOARD_NULL || state == BoardState.BOARD_SHAPE || state == BoardState.BOARD_EXPLOSIVE))
                {
                    valid = false;
                    break;
                }
                if (this.blocks[i].pos.x < leftMostPos)
                {
                    leftMostPos = (int)this.blocks[i].pos.x;
                }
                if (this.blocks[i].pos.x > rightMostPos)
                {
                    rightMostPos = (int)this.blocks[i].pos.x;
                }
                if (this.blocks[i].pos.y < bottomMostPos)
                {
                    bottomMostPos = (int)this.blocks[i].pos.y;
                }
            }
            if (this.count > 0)
            {
                //Check shape won't move off the board//
                if (moveType == MoveType.MOVE_LEFT)
                {
                    if (leftMostPos > 0 && valid)
                    {
                        result = true;
                    }
                }
                else if (moveType == MoveType.MOVE_RIGHT)
                {
                    Assert.IsTrue(rightMostPos < board.boardWidth);
                    if (rightMostPos < (board.boardWidth - 1) && valid) {
                        result = true;
                    }
                }
                else if (moveType == MoveType.MOVE_DOWN)
                {
                    if (bottomMostPos > 0 && valid)
                    {
                        Assert.IsTrue(bottomMostPos < board.boardHeight);
                        result = true;
                    }
                }
            }
            return result;
        }
            public bool isInShape(Vector2 pos)
            {
                bool result = false;
                for (int i = 0; i < this.count; ++i)
                {
                    Vector2 shapePos = this.blocks[i].pos;
                    if ((int)pos.x == (int)shapePos.x && (int)pos.y == (int)shapePos.y)
                    {
                        result = true;
                        break;
                    }
                }
                return result;
            }

        public struct QueryShapeInfo {
            public bool result;
            public int index;
        };
        

        QueryShapeInfo IsRepeatedInShape(Vector2 pos, int index)
        {
            QueryShapeInfo result = new QueryShapeInfo();
            for (int i = 0; i < this.count; ++i)
            {
                Vector2 shapePos = this.blocks[i].pos;
                if (i != index && (int)pos.x == (int)shapePos.x && (int)pos.y == (int)shapePos.y)
                {
                    result.result = true;
                    result.index = i;
                    Assert.IsTrue(i != index);
                    break;
                }
            }
            return result;
        }


        public int FindBlockById(int id)
        {
            int result = -1;
            bool found = false;
            for (int i = 0; i < this.count; ++i)
            {
                if (this.blocks[i].id == id)
                {
                    result = i;
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found);

            return result;
        }

        bool MoveShape(BoardLogic board, MoveType moveType)
        {
            bool result = canShapeMove(board, moveType);
            if (result)
            {
                Vector2 moveVec = GetMoveVec(moveType);

                Assert.IsTrue(!this.wasHitByExplosive);
                // CHECK FOR EXPLOSIVES HIT
                int idsHitCount = 0;
                int[] idsHit = new int[BoardLogic.MAX_SHAPE_COUNT];

                for (int i = 0; i < this.count; ++i)
                {
                    Vector2 oldPos = this.blocks[i].pos;
                    Vector2 newPos = oldPos + moveVec;

                    BoardValue val = board.GetBoardValue(oldPos);
                    val.color = Color.white;

                    BoardValue newVal = board.GetBoardValue(newPos);
                    newVal.color = Color.white;

                    BoardState state = board.GetBoardState(newPos);
                    if (state == BoardState.BOARD_EXPLOSIVE)
                    {
                        Assert.IsTrue(board.lifePointsMax > 0);
                        board.lifePoints -= 1;
                        this.wasHitByExplosive = true;
                        board.PlayExplosiveSound();
                        //remove from shape
                        Assert.IsTrue(idsHitCount < idsHit.Length);
                        idsHit[idsHitCount++] = this.blocks[i].id;
                        board.SetBoardState(oldPos, BoardState.BOARD_NULL, BoardValType.BOARD_VAL_NULL);    //this is the shape
                        board.SetBoardState(newPos, BoardState.BOARD_NULL, BoardValType.BOARD_VAL_TRANSIENT);//this is the bomb position
                    }
                }

                //NOTE: we have this since our findBlockById wants to search the original shape with the 
                //full count so we can't change it in the loop
                int newShapeCount = this.count;
                for (int hitIndex = 0; hitIndex < idsHitCount; ++hitIndex)
                {
                    int id = idsHit[hitIndex];
                    int blockIndex = this.FindBlockById(id);

                    this.blocks[--newShapeCount].Copy(this.blocks[blockIndex]);
                }
                this.count = newShapeCount;

                for (int i = 0; i < this.count; ++i)
                {
                    Vector2 oldPos = this.blocks[i].pos;
                    Vector2 newPos = oldPos + moveVec;

                    // printf("boardState: %d, index: %d\n", getBoardState(params, oldPos), i);
                    Assert.IsTrue(board.GetBoardState(oldPos) == BoardState.BOARD_SHAPE);

                    BoardState newPosState = board.GetBoardState(newPos);
                    Assert.IsTrue(newPosState == BoardState.BOARD_SHAPE || newPosState == BoardState.BOARD_NULL);

                    QueryShapeInfo info = this.IsRepeatedInShape(oldPos, i);
                    if (!info.result)
                    { //dind't just get set by the block in shape before. 
                        board.SetBoardState(oldPos, BoardState.BOARD_NULL, BoardValType.BOARD_VAL_NULL);
                    }
                    board.SetBoardState(newPos, BoardState.BOARD_SHAPE, this.blocks[i].type);
                    this.blocks[i].pos = newPos;
                }
                board.playMoveSound();
            }
            return result;
        }

        public void solidfyShape(BoardLogic board)
        {
            for (int i = 0; i < this.count; ++i)
            {
                Vector2 pos = this.blocks[i].pos;
                BoardValue val = board.GetBoardValue(pos);
                if (val.state == BoardState.BOARD_SHAPE)
                {
                    board.SetBoardState(pos, BoardState.BOARD_STATIC, BoardValType.BOARD_VAL_OLD);
                }
                val.color = Color.white;

            }
            board.PlaySolidfySound();
        }

        /* we were using the below code to do a flood fill to see if the shape was connected. But realised I could just see if the new 
        position had any neibours of BORAD_SHAPE. 
        */
        class VisitedQueue
        {
            public Vector2 pos;
            public VisitedQueue next;
            public VisitedQueue prev;
        }


    private void addToQueryList(VisitedQueue sentinel, Vector2 pos, bool[] boardArray, BoardLogic board)
        {
            if ((int)pos.x >= 0 && (int)pos.x < board.boardWidth && (int)pos.y >= 0 && (int)pos.y < board.boardHeight)
            {
                int boardIndex = (((int)pos.y) * board.boardWidth) + (int)pos.x;
                bool visited = boardArray[boardIndex];

                if (!visited && board.GetBoardState(pos) == BoardState.BOARD_SHAPE)
                {
                    boardArray[boardIndex] = true;
                    VisitedQueue queue = new VisitedQueue();
                    queue.pos = pos;

                    //add to the search queue
                    Assert.IsTrue(sentinel.prev.next == sentinel);
                    queue.prev = sentinel.prev;
                    queue.next = sentinel;
                    sentinel.prev.next = queue;
                    sentinel.prev = queue;
                }
            }
        }
        /* end the queue stuff for the flood fill.*/

        class IslandInfo
        {
            public int count;
            public Vector2[] poses;
            public IslandInfo() {
                this.poses = new Vector2[BoardLogic.MAX_SHAPE_COUNT];
            }
        }

        IslandInfo GetShapeIslandCount(Vector2 startPos, BoardLogic board)
    {
        IslandInfo info = new IslandInfo();

            //NOTE: This is since shape can be blown apart we want to still be able to move a block on their 
            //own island. So the count isn't the shapeCount but only a portion in this count. So we first search 
            //how big the island is and match against that. 

            bool[] boardArray = new bool[board.boardWidth*board.boardHeight]; 

        VisitedQueue sentinel = new VisitedQueue();
        sentinel.next = sentinel.prev = sentinel;
    
        addToQueryList(sentinel, startPos, boardArray, board);

        VisitedQueue queryAt = sentinel.next;
        Assert.IsTrue(queryAt != sentinel);
        while (queryAt != sentinel)
        {
            Vector2 pos = queryAt.pos;

            Assert.IsTrue(info.count < info.poses.Length);
            info.poses[info.count++] = pos;
            
            addToQueryList(sentinel, new Vector2(1, 0) + pos, boardArray, board);
            addToQueryList(sentinel, new Vector2(-1, 0) + pos, boardArray, board);
            addToQueryList(sentinel, new Vector2(0, 1) + pos, boardArray, board);
            addToQueryList(sentinel, new Vector2(0, -1) + pos, boardArray, board);

    
            queryAt = queryAt.next;
        }

        if (info.count > this.count)
        {
            //printf("Count At; %d\n", info.count);
            //printf("Shape Count: %d\n", shape->count);
        }
        Assert.IsTrue(info.count <= this.count);

        return info;
    }

    public bool shapeStillConnected(int currentHotIndex, Vector2 boardPosAt, BoardLogic board)
    {
        bool result = true;

        for (int i = 0; i < this.count; ++i)
        {
            Vector2 pos = this.blocks[i].pos;
            if ((int)boardPosAt.x == (int)pos.x && (int)boardPosAt.y == (int)pos.y)
            {
                result = false;
                break;
            }

            BoardState state = board.GetBoardState(boardPosAt);
            if (state != BoardState.BOARD_NULL)
            {
                result = false;
                break;
            }
        }
        if (result)
        {
            Vector2 oldPos = this.blocks[currentHotIndex].pos;

            BoardValue oldVal = board.GetBoardValue(oldPos);
            Assert.IsTrue(oldVal.state == BoardState.BOARD_SHAPE);

            IslandInfo mainIslandInfo = this.GetShapeIslandCount(oldPos, board);
            Assert.IsTrue(mainIslandInfo.count >= 1);

            if (mainIslandInfo.count <= 1)
            {
                //There is an isolated block
                result = false;
            }
            else
            {
                Vector2 idPos = mainIslandInfo.poses[1]; //won't be that starting pos since the first position will dissapear if it is correct. 
                //temporaialy set the board state to where the shape was to be null, so this can't act as a bridge in the flood fill
                oldVal.state = BoardState.BOARD_NULL;

                //set where the board will be to a valid position
                BoardValue newVal = board.GetBoardValue(boardPosAt);
                Assert.IsTrue(newVal.state == BoardState.BOARD_NULL);
                newVal.state = BoardState.BOARD_SHAPE;
                ////   This code isn't needed anymore. Just used for the assert below. 
                IslandInfo islandInfo = this.GetShapeIslandCount(boardPosAt, board);

                //See if the new pos is part of the same island
                bool found = false;
                for (int index = 0; index < islandInfo.count; ++index)
                {
                    Vector2 srchPos = islandInfo.poses[index];
                    if ((int)srchPos.x == (int)idPos.x && (int)srchPos.y == (int)idPos.y)
                    {
                        found = true;
                        break;
                    }
                }
                ////

                IslandInfo mainIslandInfo_after = this.GetShapeIslandCount(idPos, board);
                if (mainIslandInfo_after.count < mainIslandInfo.count)
                {
                    result = false;
                }
                else
                {
                    Assert.IsTrue(found);
                }
                //set the state back to being a shape. 
                newVal.state = BoardState.BOARD_NULL;
                oldVal.state = BoardState.BOARD_SHAPE;
            }
        }

        return result;
    }

    public void ResetMouseUI(BoardLogic board)
    {
        if (board.currentHotIndex >= 0) {
            board.letGo = true;
            Vector2 pos = this.blocks[board.currentHotIndex].pos;
            BoardValue val = board.GetBoardValue(pos);
            val.color = Color.white;
        }

        board.currentHotIndex = -1; //reset hot ui   
    }

    //void changeMenuStateCallback(void* data)
    //{
    //    FrameParams *params = (FrameParams*)data;
    //    resetMouseUI(params);
    //}

    public void UpdateShapeMoveTime(BoardLogic board)
    {
        if (keyStates.wasReleased(ButtonType.BUTTON_LEFT_MOUSE))
        {
                //Debug.Log("released");
            this.ResetMouseUI(board);
        }
        bool isHoldingShape = board.currentHotIndex >= 0;
        this.moveTime = board.paramsDt;

        if (isHoldingShape)
        {
            if (!board.wasHoldingShape) {
                board.accumHoldTime = this.moveTimer.period - this.moveTimer.value;
                Assert.IsTrue(board.accumHoldTime >= 0);
            }
            this.moveTime = 0.0f;
            Assert.IsTrue(!board.letGo);
        } 
        board.wasHoldingShape = isHoldingShape;

        if (board.letGo) {
            Assert.IsTrue(!isHoldingShape);
            this.moveTime = board.accumHoldTime;
            board.accumHoldTime = 0;
            board.letGo = false;

        }
    }

    public int GetTotalNumberOfShapeBlocks(BoardLogic board)
    {
        int result = 0;
        for (int i = 0; i < board.shapeSizes.Length; ++i) {
            result += board.shapeSizes[i];
        }

        return result;
    }

    bool HasMirrorPartner(BoardLogic board, int hotBlockIndex, int mirrorIndexAt)
    {
        bool result = false;
        if (hotBlockIndex < mirrorIndexAt)
        {
            result = mirrorIndexAt < this.GetTotalNumberOfShapeBlocks(board);
        }
        else
        {
            Assert.IsTrue(hotBlockIndex != mirrorIndexAt);
            result = mirrorIndexAt < board.shapeSizes[0];
        }
        return result;
    }

    bool IsMirrorPartnerIndex(BoardLogic board, int currentHotIndex, int i, bool isCurrentHotIndex/*for the assert*/)
    {
        bool result = false;

        int mirrorOffsetCount = board.shapeSizes[0];
        int mirrorIndexAt = currentHotIndex < mirrorOffsetCount ? (currentHotIndex + mirrorOffsetCount) : (currentHotIndex - mirrorOffsetCount);

        if (board.isMirrorLevel && currentHotIndex >= 0 && HasMirrorPartner(board, currentHotIndex, mirrorIndexAt)) {
            if (currentHotIndex < mirrorOffsetCount)
            {
                Assert.IsTrue(currentHotIndex + mirrorOffsetCount < GetTotalNumberOfShapeBlocks(board));
                if ((currentHotIndex + mirrorOffsetCount) == i)
                {
                    if (isCurrentHotIndex)
                    {
                        Assert.IsTrue(keyStates.isDown(ButtonType.BUTTON_LEFT_MOUSE));
                    }
                    //this would be if you grabbed the 'lower shape' and you want to hightlight the one above
                    result = true;
                }
            }
            else
            {
                Assert.IsTrue(currentHotIndex - mirrorOffsetCount >= 0);
                if ((currentHotIndex - mirrorOffsetCount) == i)
                {
                    if (isCurrentHotIndex)
                    {
                        Assert.IsTrue(keyStates.isDown(ButtonType.BUTTON_LEFT_MOUSE));
                    }
                    //this would be if you grabbed the 'higher shape' 
                    result = true;
                }
            }
        }
        return result;
    }
    public class TwoColors {
        public Vector4 color1;
        public Vector4 color2;
            public TwoColors(Vector4 color1, Vector4 color2) {
                this.color1 = color1;
                this.color2 = color2;
            }

            public TwoColors()
            {
                this.color1 = new Vector4();
                this.color2 = new Vector4();
            }
        }


/*
NOTE: This is since alien blocks are different colors and so when the hover & 
grab color are the same they don't show up on some. For example yellow hover on a yellow block.
*/
public TwoColors GetAlienHoverColor(int indexAt)
    {
        TwoColors result = new TwoColors();

        BoardValType type = this.blocks[indexAt].type;
        switch (type)
        {
            case BoardValType.BOARD_VAL_SHAPE0:
                { //green
                    result.color1 = Color.yellow;
                    result.color2 = Color.green;
                }
                break;
            case BoardValType.BOARD_VAL_SHAPE1:
                { //yellow
                    result.color1 = Color.green;
                    result.color2 = Color.green;
                }
                break;
            case BoardValType.BOARD_VAL_SHAPE2:
                { //blue
                    result.color1 = Color.yellow;
                    result.color2 = Color.green;
                }
                break;
            case BoardValType.BOARD_VAL_SHAPE3:
                { //pink
                    result.color1 = Color.yellow;
                    result.color2 = Color.green;
                }
                break;
            case BoardValType.BOARD_VAL_SHAPE4:
                { //beige
                    result.color1 = Color.yellow;
                    result.color2 = Color.green;
                }
                break;
                default:
                {
                    Assert.IsTrue(false);
                } break;
        }
        return result;
    }

    public void UpdateAndRenderShape(BoardLogic board)
    {
        Assert.IsTrue(!this.wasHitByExplosive);
        Assert.IsTrue(!board.createShape);


        bool isHoldingShape = board.currentHotIndex >= 0;

        bool turnSolid = false;

        TimerReturnInfo timerInfo = this.moveTimer.updateTimer(this.moveTime);
        if (timerInfo.finished)
        {
            this.moveTimer.turnTimerOn();
            this.moveTimer.value = timerInfo.residue;
            if (!this.MoveShape(board, MoveType.MOVE_DOWN))
            {
                turnSolid = true;
            }
        }
        this.wasHitByExplosive = false;
        if (turnSolid)
        {
            solidfyShape(board);
            board.createShape = true; 
            this.wasHitByExplosive = false;
            ResetMouseUI(board);
        }
        else
        {

            int hotBlockIndex = -1;
            for (int i = 0; i < this.count; ++i)
            {
                //NOTE: Render the hover indications & check for hot player block 
                Vector2 pos = this.blocks[i].pos;
                TwoColors alienColors = GetAlienHoverColor(i);

                Vector4 color = Color.white;
                Bounds blockBounds = new Bounds(new Vector3(pos.x, pos.y, keyStates.mouseInWorldSpace.z), new Vector3(1, 1, 1));

                if (blockBounds.Contains(keyStates.mouseInWorldSpace))
                {
                    hotBlockIndex = i;
                    if (board.currentHotIndex < 0) {
                        color = alienColors.color1;
                        }
                 }

            if (hotBlockIndex >= 0 && board.currentHotIndex < 0) {
                if (IsMirrorPartnerIndex(board, hotBlockIndex, i, false))
                {
                    color = alienColors.color1;
                }
            }

            if (board.currentHotIndex == i) {
                Assert.IsTrue(keyStates.isDown(ButtonType.BUTTON_LEFT_MOUSE));
                color = alienColors.color2;
            }

            if (IsMirrorPartnerIndex(board, board.currentHotIndex, i, true))
            {
                color = alienColors.color2;
            }

            BoardValue val = board.GetBoardValue(pos);
            Assert.IsTrue(val.valid);
            val.color = color;
            
        }

        //NOTE: Have to do this afterwards since we the block can be before for the mirrorHotIndex
        if(board.currentHotIndex< 0 && hotBlockIndex >= 0 && board.isMirrorLevel) {
            int mirrorOffsetCount = board.shapeSizes[0];
            int mirrorIndexAt = hotBlockIndex < mirrorOffsetCount ? (hotBlockIndex + mirrorOffsetCount) : (hotBlockIndex - mirrorOffsetCount);

            if(HasMirrorPartner(board, hotBlockIndex, mirrorIndexAt)) { //same size
                Assert.IsTrue(mirrorIndexAt >= 0 && mirrorIndexAt< GetTotalNumberOfShapeBlocks(board));

                Vector2 pos = this.blocks[mirrorIndexAt].pos;
                BoardValue val = board.GetBoardValue(pos);
                Assert.IsTrue(val.valid);
                val.color = GetAlienHoverColor(hotBlockIndex).color1;
            } else {
                // printf("%s\n", "no mirror partner");
                Assert.IsTrue(board.shapeSizes[0] != board.shapeSizes[1]);
            }
        }

        if(keyStates.wasPressed(ButtonType.BUTTON_LEFT_MOUSE) && hotBlockIndex >= 0) {
            board.currentHotIndex = hotBlockIndex;

        }
        
        if(board.currentHotIndex >= 0) {
            //We are holding onto a block
            Vector2 boardPosAt = keyStates.mouseInWorldSpace;
            boardPosAt.x = (int) (Mathf.Clamp(boardPosAt.x, 0, board.boardWidth - 1) + 0.5f);
            boardPosAt.y = (int) (Mathf.Clamp(boardPosAt.y, 0, board.boardHeight -1) + 0.5f);

            int hotIndex = board.currentHotIndex;
            bool okToMove = shapeStillConnected(hotIndex, boardPosAt, board);

            Vector2 boardPosAtMirror = new Vector2(0, 0); //not used unless is a mirror level
            int mirrorIndex = -1;
            bool isEvenSize = true;
            if(board.isMirrorLevel) {
                int mirrorOffsetCount = board.shapeSizes[0];
                mirrorIndex = hotIndex<mirrorOffsetCount? (hotIndex + mirrorOffsetCount) : (hotIndex - mirrorOffsetCount);
                if(HasMirrorPartner(board, board.currentHotIndex, mirrorIndex)) { //
                    Vector2 mirrorOffset = boardPosAt - this.blocks[hotIndex].pos;
                    boardPosAtMirror = this.blocks[mirrorIndex].pos + mirrorOffset;

                    okToMove &= shapeStillConnected(mirrorIndex, boardPosAtMirror, board);
                } else {
                    Assert.IsTrue(board.shapeSizes[0] != board.shapeSizes[1]);
                    isEvenSize = false;
                }
            }
            
            int mirCount = (board.isMirrorLevel && isEvenSize) ? 2 : 1;
            int[] hotIndexes = new int[] { hotIndex, mirrorIndex };
            Vector2[] newPoses = new Vector2[] { boardPosAt, boardPosAtMirror };

            if(okToMove) {
                //play the sound once
                board.playArrangeSound();
                for(int m = 0; m<mirCount; ++m) {
                    int thisHotIndex = hotIndexes[m];

                    Vector2 oldPos = this.blocks[thisHotIndex].pos;
                    Vector2 newPos = newPoses[m];

                    Assert.IsTrue(board.GetBoardState(oldPos) == BoardState.BOARD_SHAPE);
                    Assert.IsTrue(board.GetBoardState(newPos) == BoardState.BOARD_NULL);
                    board.SetBoardState(oldPos, BoardState.BOARD_NULL, BoardValType.BOARD_VAL_NULL);
                    board.SetBoardState(newPos, BoardState.BOARD_SHAPE, this.blocks[thisHotIndex].type);
                    this.blocks[thisHotIndex].pos = newPos;
                    Assert.IsTrue(board.GetBoardState(newPos) == BoardState.BOARD_SHAPE);
                }
            }
        }

    }
}
    
    }

}
