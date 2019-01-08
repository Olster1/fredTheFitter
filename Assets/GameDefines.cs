using System;
using Levels;
namespace game_defines
{
    public enum StartMode { 
        OVERWORLD,
        BOARD,
        CREDITS,
        START_SCREEN
    }
    public static class GameDefines
    {

        public static readonly float FADE_TIMER_INTERVAL = 0.3f;
        public static readonly int XP_PER_LINE = 100;
        public static readonly LevelType START_LEVEL = LevelType.LEVEL_16;
        public static readonly StartMode START_MODE = StartMode.BOARD;

        //#define DEVELOPER_MODE 1
        //#define DEMO_MODE 0
        //#define APP_TITLE "Feoh the Fitter"
        //#define MOVE_INTERVAL 1.0f

        public static readonly float SCENE_TRANSITION_TIME = 0.3f;
        //#define SCENE_MUSIC_TRANSITION_TIME (SCENE_TRANSITION_TIME + 0.2f) //extra lee way so they overlap 
        //#define CHEAT_MODE 1
        //#define START_MENU_MODE MENU_MODE
        //#define XP_PER_LINE 100
        //#define GO_TO_NEXT_GROUP_AUTO 0
        //#define UI_BUTTON_COLOR COLOR_YELLOW
        //#define CAN_ALTER_SHAPE_DIAGONAL 0 //this is if you can move a block to a position only situated diagonally 
        //#define OPENGL_BACKEND 1
        //#define RENDER_BACKEND OPENGL_BACKEND
        //#define RENDER_HANDNESS -1 //Right hand handess -> z going into the screen. 
    }
}
