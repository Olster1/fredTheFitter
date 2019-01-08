using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game_defines;

public class HideOnStartup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(GameDefines.START_MODE != StartMode.OVERWORLD) {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
