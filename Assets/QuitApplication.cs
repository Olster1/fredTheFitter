using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuitApplication : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        Button but = gameObject.GetComponent<Button>();
        but.onClick.AddListener(() => quitGame());
    }

    public void quitGame() {
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {

    }
}


