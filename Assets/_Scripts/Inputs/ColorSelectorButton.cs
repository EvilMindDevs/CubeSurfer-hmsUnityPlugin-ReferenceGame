using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorSelectorButton : MonoBehaviour, IPointerDownHandler
{
  public Color color;
  public Material material;


  public void OnPointerDown(PointerEventData eventData)
  {
    material.SetColor("_Color", color);
  }
  
  void Update()
  {
    transform.GetComponent<Image>().color = color;

  }
}
