using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game_defines;

public class HideOnStartupBoard : MonoBehaviour
{
    bool started;
    // Start is called before the first frame update
    void Start()
    {
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        //this isn't ideal maybe we can just reference all the 
        //levels at the end so we can just turn off the reference dependency!
        if (started && GameDefines.START_MODE != StartMode.BOARD)
        {
            started = false;
            gameObject.SetActive(false);
        }
    }
}
