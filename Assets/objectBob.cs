using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectBob : MonoBehaviour
{
    private float tAt;
    public bool isCos;
    private Transform objTrans;
    public bool expandWidth;
    private Vector3 startPos;
    private Vector3 startScale;
    // Start is called before the first frame update
    void Start()
    {

        objTrans = gameObject.GetComponent<Transform>();
        startPos = new Vector3(objTrans.position.x, objTrans.position.y, objTrans.position.z);
        startScale = new Vector3(objTrans.localScale.x, objTrans.localScale.y, objTrans.localScale.z);
        tAt = Random.Range(0, 2 * Mathf.PI);
    }

    // Update is called once per frame
    void Update()
    {
        tAt += Time.deltaTime;
        float scale = 0.4f;

        if (isCos) {
            scale *= Mathf.Cos(2*tAt);
        } else {
            scale *= Mathf.Sin(tAt);
        }
        Vector3 pos = new Vector3(0, 0, 0);
        Vector3 newScale = new Vector3(0, 0 ,0);
        if (expandWidth) {  
            newScale.x += scale;
        } else {
            pos.y = 0.5f*scale;
            newScale.y += scale;
        }
        objTrans.position = startPos + pos;
        objTrans.localScale = startScale + newScale;


    }
}
