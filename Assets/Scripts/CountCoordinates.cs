using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountCoordinates : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}
