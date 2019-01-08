using UnityEngine;
using System.Collections;

namespace easy_keystates {
    public enum ButtonType
    {
        BUTTON_NULL,
        BUTTON_LEFT,
        BUTTON_RIGHT,
        BUTTON_UP,
        BUTTON_DOWN,
        BUTTON_SPACE,
        BUTTON_SHIFT,
        BUTTON_ENTER,
        BUTTON_BACKSPACE,
        BUTTON_ESCAPE,
        BUTTON_LEFT_MOUSE,
        BUTTON_RIGHT_MOUSE,
        BUTTON_1,
        BUTTON_F1,
        BUTTON_Z,
        BUTTON_R,
        BUTTON_COMMAND,
        BUTTON_TILDE,
        //
        BUTTON_COUNT
    }

    public class AppKeyStates
    {

        public GameButton[] gameButtons;
        public Vector3 mouseInWorldSpace;
        public Vector3 mouseInScreenSpace;

        public AppKeyStates()
        {
            this.gameButtons = new GameButton[(int)ButtonType.BUTTON_COUNT];
            for(int i = 0; i < gameButtons.Length; ++i) {
                this.gameButtons[i] = new GameButton();
            }
        }

        public bool wasPressed(ButtonType type)
        {
            return (gameButtons[(int)type].isDown && gameButtons[(int)type].transitionCount != 0);
        }

        public bool wasReleased(ButtonType type) {
            return ((!gameButtons[(int)type].isDown) && gameButtons[(int)type].transitionCount != 0);

            }
            
        public bool isDown(ButtonType type) {
            return gameButtons[(int)type].isDown;
        }



    private void ProcessGameKey(ButtonType type, bool isDown, bool repeated)
    {
        GameButton button = gameButtons[(int)type];
        button.isDown = isDown;
        if (!repeated)
        {
            button.transitionCount++;
        }
    }


    public class GameButton
    {
        public bool isDown;
        public int transitionCount;
    }

        void ClearGameButtons() {
            for (int i = 0; i < gameButtons.Length; ++i)
            {
                GameButton butt = this.gameButtons[i];
                butt.isDown = false;
                butt.transitionCount = 0;
            }
        }
        public void ProcessKeyStates(GameObject cameraInScene)
        {
            //Save state of last frame game buttons 
            bool mouseWasDown = isDown(ButtonType.BUTTON_LEFT_MOUSE);
            bool mouseWasDownRight = isDown(ButtonType.BUTTON_RIGHT_MOUSE);
            bool leftArrowWasDown = isDown(ButtonType.BUTTON_LEFT);
            bool rightArrowWasDown = isDown(ButtonType.BUTTON_RIGHT);
            bool upArrowWasDown = isDown(ButtonType.BUTTON_UP);
            bool downArrowWasDown = isDown(ButtonType.BUTTON_DOWN);
            bool shiftWasDown = isDown(ButtonType.BUTTON_SHIFT);
            bool commandWasDown = isDown(ButtonType.BUTTON_COMMAND);
            bool spaceWasDown = isDown(ButtonType.BUTTON_SPACE);
            /////
            ClearGameButtons();

            //assert(state->gameButtons[BUTTON_LEFT_MOUSE].transitionCount == 0);
            //ask player for new input

            //bool leftArrowIsDown = Input.GetButton("Left");
            //bool rightArrowIsDown = Input.GetButton("Right");
            //bool upArrowIsDown = Input.GetButton("Up");
            //bool downArrowIsDown = Input.GetButton("Down");


            //ProcessGameKey(ButtonType.BUTTON_LEFT, leftArrowIsDown, leftArrowWasDown == leftArrowIsDown);
            //ProcessGameKey(ButtonType.BUTTON_RIGHT, rightArrowIsDown, rightArrowWasDown == rightArrowIsDown);
            //ProcessGameKey(ButtonType.BUTTON_DOWN, downArrowIsDown, downArrowWasDown == downArrowIsDown);
            //ProcessGameKey(ButtonType.BUTTON_UP, upArrowIsDown, upArrowWasDown == upArrowIsDown);

            Camera cam = cameraInScene.GetComponent<Camera>();
            Vector2 mousePos = Input.mousePosition;
            Vector3 mouseP_withZ = new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane);
            this.mouseInWorldSpace = cam.ScreenToWorldPoint(mouseP_withZ);
            //Debug.Log(this.mouseInWorldSpace.ToString());
            this.mouseInScreenSpace = mouseP_withZ;
            bool leftMouseDown = Input.GetMouseButton(0);
            //if (leftMouseDown) { 
            //    Debug.Log("is Down: " + leftMouseDown);
            //}
            ProcessGameKey(ButtonType.BUTTON_LEFT_MOUSE, leftMouseDown, leftMouseDown == mouseWasDown);

            //if(wasReleased(ButtonType.BUTTON_LEFT_MOUSE)) {
            //    Debug.Log("heya");
            //}
            bool rightMouseDown = Input.GetMouseButton(1);
        ProcessGameKey(ButtonType.BUTTON_RIGHT_MOUSE, rightMouseDown, rightMouseDown == mouseWasDownRight);
        
    }


}
}
