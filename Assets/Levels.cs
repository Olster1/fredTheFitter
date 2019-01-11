using System;
using easy_timer;
using UnityEngine;
using UnityEngine.Assertions;
using easy_transition;
namespace Levels
{
    [Serializable]
    public enum LevelState {
        LEVEL_STATE_NULL,
        LEVEL_STATE_COMPLETED,
        LEVEL_STATE_UNLOCKED,
        LEVEL_STATE_LOCKED,
    }

    [Serializable]
    public class SaveState
    {
        public LevelType type;
        public LevelState state;

        public SaveState(LevelType type, LevelState state)
        {
            this.state = state;
            this.type = type;
        }
    }

    public class LevelCountFromFile
    {
        int completedCount;
        bool valid;
    }

    public enum LevelType
    {
        LEVEL_NULL,
        LEVEL_0,
        LEVEL_1,
        LEVEL_2,
        LEVEL_3,
        LEVEL_4,
        LEVEL_5,
        LEVEL_6,
        LEVEL_7,
        LEVEL_8,
        LEVEL_9,
        LEVEL_10,
        LEVEL_11,
        LEVEL_12,
        LEVEL_13,
        LEVEL_14,
        LEVEL_15,
        LEVEL_16,
        LEVEL_17,
        LEVEL_18,
        LEVEL_19,
        LEVEL_20,
        LEVEL_21,
        LEVEL_22,
        LEVEL_23,
        LEVEL_24,
        LEVEL_25,
        LEVEL_26,
        LEVEL_27,
        LEVEL_28,
        LEVEL_29,
        LEVEL_30,
        LEVEL_31,
        LEVEL_32,
        LEVEL_33,
        LEVEL_34,
        LEVEL_35,
        LEVEL_36,
        LEVEL_37,
        LEVEL_38,
        LEVEL_39,
        LEVEL_40,
        LEVEL_41,
        LEVEL_42,
        LEVEL_43,
        LEVEL_44,
        LEVEL_45,
        LEVEL_46,
        LEVEL_47,
        LEVEL_48,
        LEVEL_49,
        LEVEL_50,
        LEVEL_COUNT
    }

     
    public class LevelGroup
    {
        public LevelType[] groups;
        public int count;

        public LevelGroup() {
            groups = new LevelType[22]; //22 is just the max number of levels that can be in a group;
            count = 0;


        }


        public Vector3 GetPos(TransitionState state) {
            Vector3 currentAverage = new Vector3(0, 0, 0);
            for(int i = 0; i < count; ++i) {
                LevelType t = groups[i];
                Vector3 posToAdd = state.levelPosInOverworld[(int)t];
                currentAverage += posToAdd;
            }
            Vector3 average = currentAverage / count;
            average.z = -10;
            return average;
        }

        public void AddLevelToGroup(LevelType type) {
            Assert.IsTrue(count < groups.Length);
            groups[count++] = type;

        }
    }






}
