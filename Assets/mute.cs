using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class mute : MonoBehaviour
{
    public Sprite onSpeaker;
    public Sprite offSpeaker;
    private Button but;
    // Use this for initialization
    void Start()
    {
        but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(() => muteSound());
    }

    void muteSound() {
        Sprite newSprite = offSpeaker;
        if(AudioListener.pause) {
            newSprite = onSpeaker;
        }
        but.GetComponent<Image>().sprite = newSprite;
        AudioListener.pause = !AudioListener.pause;
    }
    // Update is called once per frame
    void Update()
    {

    }
}


