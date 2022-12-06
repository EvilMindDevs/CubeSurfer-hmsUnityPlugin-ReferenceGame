using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseClickDetector : MonoBehaviour, IPointerDownHandler

{
    public bool startTextClicked = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        
        Debug.Log("Mouse Down: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

}
