using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using easy_timer;
using Fitris;

namespace ExtraShapeNameSpace
{
    public enum ExtraShapeType
    {
        EXTRA_SHAPE_NULL,
        EXTRA_SHAPE_WINDMILL,
        EXTRA_SHAPE_MAGNET,
    }

    [System.Serializable]
    public class ExtraShape
    {
        ExtraShapeType type;

        public int id; //this is to link extra shape info with the location in the board text file 

        [HideInInspector]
        public Vector2 pos;
        public Vector2 growDir;

        [HideInInspector]
        public bool active; //this is for the lag period. 

        public Timer timer;

        public Timer lagTimer;
        private float lagPeriod;
        //private float movePeriod;

        bool justFlipped;

        bool isOut; //going out or in

        public int perpSize;

        public bool isBomb;

        private int count;

        public bool timeAffected; //this is if it slows down when the player as grabbed the block 
        [HideInInspector]
        public bool tryingToBegin;

        public int max;

        public ExtraShape(Vector2 pos, float showPeriod, float lagPeriod)
        {
            this.timer = new Timer(showPeriod);
            this.lagTimer = new Timer(lagPeriod);
            this.growDir = new Vector2();

            this.isOut = true;
            this.count = 0;
            this.active = true;
            this.pos = pos;
            this.tryingToBegin = false;

            this.type = ExtraShapeType.EXTRA_SHAPE_WINDMILL;
        }

        public void CopyShape(ExtraShape newShape) {
            newShape.type = this.type;

            newShape.id = this.id;

            newShape.pos.x = this.pos.x;
            newShape.pos.y = this.pos.y;

            newShape.growDir.x = this.growDir.x;
            newShape.growDir.y = this.growDir.y;

            newShape.active = this.active; //this is for the lag period. 
            //Debug.Log(newShape.timer.period);
            //Debug.Log(this.timer.period);
            newShape.timer.period = this.timer.period;
            newShape.lagTimer.period = this.lagTimer.period;
            newShape.lagPeriod = this.lagTimer.period; 

            newShape.perpSize = this.perpSize;

            newShape.isBomb = this.isBomb;

            newShape.count = this.count;

            newShape.timeAffected = this.timeAffected; //this is if it slows down when the player as grabbed the block 

            newShape.max = this.max;
        }

        public void updateWindmillSide(BoardLogic board)
        {
            bool repeat = true;
            bool repeatedYet = false;
            while (repeat)
            { //this is for when we get blocked 
                repeat = false;
                Assert.IsTrue(this.count <= this.max);
                BoardState stateToSet;
                BoardState staticState = (this.isBomb) ? BoardState.BOARD_EXPLOSIVE : BoardState.BOARD_STATIC;

                int addend = 0;
                if (this.isOut)
                {
                    addend = 1;
                    stateToSet = staticState;
                }
                else
                {
                    stateToSet = BoardState.BOARD_NULL;
                    addend = -1;
                }

                bool wasJustFlipped = this.justFlipped;
                if (!this.justFlipped)
                {
                    this.count += addend;
                }
                else
                {
                    this.justFlipped = false;
                }

                if (this.count != 0)
                {
                    BoardState toBoardState = BoardState.BOARD_INVALID;
                    Assert.IsTrue(this.count >= 0 && this.count <= this.max);
                    Vector2 shift = new Vector2(this.growDir.x * (this.count - 1), this.growDir.y * (this.count - 1));
                    Vector2 newPos = this.pos + shift;

                    toBoardState = board.GetBoardState(newPos);

                    bool settingBlock = (toBoardState == BoardState.BOARD_NULL && stateToSet == staticState);
                    bool isInBounds = board.InBoardBounds(newPos);
                    bool blocked = (toBoardState != BoardState.BOARD_NULL && stateToSet == staticState);
                    if (blocked || !isInBounds)
                    {
                        Assert.IsTrue(this.isOut);
                        if (this.count == 1)
                        {
                            Assert.IsTrue(stateToSet != BoardState.BOARD_NULL);
                            this.isOut = true;
                            if (this.tryingToBegin)
                            { //on the second attempt after the shape has moved
                                for (int i = 0; i < this.perpSize; ++i)
                                {
                                    this.growDir = Vector2.Perpendicular(this.growDir);
                                }
                            }
                            this.count = 0;
                            this.tryingToBegin = true;
                        }
                        else
                        {
                            this.isOut = false;
                            repeat = true;
                            this.justFlipped = true;
                            this.count--;
                        }
                    }
                    else
                    {
                        Assert.IsTrue(this.count > 0);
                        Assert.IsTrue(stateToSet != BoardState.BOARD_INVALID);

                        Assert.IsTrue(isInBounds);
                        if (settingBlock || stateToSet == BoardState.BOARD_NULL)
                        {
                            if (stateToSet == BoardState.BOARD_NULL) { Assert.IsTrue(toBoardState == staticState); }
                            board.SetBoardState(newPos, stateToSet, BoardValType.BOARD_VAL_DYNAMIC);

                        }
                    }

                    Assert.IsTrue(this.max > 0);

                    if (this.count == this.max)
                    {
                        if (!wasJustFlipped)
                        {
                            this.isOut = false;
                            this.justFlipped = true;
                        }
                        else
                        {
                            Assert.IsTrue(!this.isOut);
                        }
                    }
                }
                else
                {
                    Assert.IsTrue(this.count == 0);
                    Assert.IsTrue(!this.isOut);
                    this.isOut = true;
                    for (int i = 0; i < this.perpSize; ++i)
                    {
                        this.growDir = Vector2.Perpendicular(this.growDir);
                    }
                    this.lagTimer.period = this.lagPeriod; //we change from the begin period to the lag period
                    if (!Equals(this.lagTimer.period, 0.0f))
                    {
                        active = false; //we lag for a bit. 
                    }
                }

                if (this.count == 0 && !repeatedYet)
                {
                    repeat = true;
                    repeatedYet = true;
                }
            }
        }
    }
}
