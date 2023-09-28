using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanScript : MonoBehaviour
{
    private Vector2 initialTouchPos;
    private Vector2 finalTouchPos;
    private float moveAngle = 0;



    private void OnMouseDown()
    {
        initialTouchPos = Input.mousePosition;
        Debug.Log(initialTouchPos);
    }
}
