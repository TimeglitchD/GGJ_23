using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro winName;
    private Vector2 MouseOffset;

    private void Start()
    {
        winName.text = gameObject.name;
    }

    private void OnMouseDown()
    {
        MouseOffset = GetMousePosition() - (Vector2)transform.position;
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePosition() - MouseOffset;
    }

    private Vector2 GetMousePosition()
    {
       
        Vector2 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        return _mousePosition;
    }
}
