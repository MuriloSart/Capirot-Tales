using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public SpriteRenderer sr;
    void Start()
    {
        int randomColor = Random.Range(0, 3);

        if (randomColor == 0)
            sr.color = Color.red;
        else if(randomColor == 1)
            sr.color = Color.green;
        else
            sr.color = Color.blue;
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
