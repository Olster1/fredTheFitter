using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCloud : MonoBehaviour
{
    private Transform trans;
    // Start is called before the first frame update
    void Start()
    {
        trans = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        trans.position = new Vector3(Time.deltaTime * 0.1f, 0, 0) + trans.position;

        if(trans.position.x > 17.0f) {
            trans.position.Set(-50.0f, trans.position.y, trans.position.z);
        }
    }
}
