using System;
using UnityEngine;
using UnityEngine.Assertions;
using easy_keystates;
using easy_lex;

namespace Overworld
{
    public class MoveOverWorldCamera : MonoBehaviour
    {
        private GameObject cameraInScene;
        private AppKeyStates keyStates;
        private Vector3 lastMouseP;
        private Vector3 camVel;
        private Transform camTransform;

        public void Start()
        {
            keyStates = new AppKeyStates();
            cameraInScene = GameObject.Find("OverworldCam");
            camTransform = cameraInScene.GetComponent<Transform>();
        }

        public void Update()
        {

            keyStates.ProcessKeyStates(cameraInScene);
            float ySpace = 1.0f;
            float xSpace = ySpace;

            if (keyStates.wasPressed(ButtonType.BUTTON_LEFT_MOUSE))
            {
                lastMouseP = keyStates.mouseInWorldSpace;
            }

            Vector3 accel = new Vector3(0, 0, 0);
            if (keyStates.isDown(ButtonType.BUTTON_LEFT_MOUSE))
            {
                Vector3 diffVec = keyStates.mouseInWorldSpace - lastMouseP;
                diffVec = Vector3.Normalize(diffVec);
                accel = 600.0f * diffVec;
                // error_printFloat2("diff: ", accel.xy.E);
                accel.x *= -1; //inverse the pull direction. Since y is down for mouseP, y is already flipped 
            }

            camVel = Time.deltaTime * accel + camVel;
            camVel = camVel - 0.6f * camVel;
            camTransform.position += Time.deltaTime * camVel;

            lastMouseP = keyStates.mouseInWorldSpace;
                //float factor = 10.0f;
                //Vector3 newPos = new Vector3();
                //newPos.x = ((int)((camTransform.position.x + 0.5f) * factor)) / factor;
                //newPos.y = ((int)((camTransform.position.y + 0.5f) * factor)) / factor;
                //camTransform.position = newPos;
        }
    }
}
