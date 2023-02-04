using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconToWindow : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private TMPro.TextMeshPro windowName;

    private void OnMouseEnter()
    {
        if (!window.activeSelf) windowName.text = (window.name);
        Debug.Log("OnMouseEnter");
    }

    private void OnMouseExit()
    {
        windowName.text = ("");
        Debug.Log("OnMouseEnter");
    }

    private void OnMouseDown()        
    {
        windowName.text = ("");
        window.SetActive(true);
        Debug.Log("window is opened");
    }
}
