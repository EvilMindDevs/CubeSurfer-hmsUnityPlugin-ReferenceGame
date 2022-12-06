using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
  private Slider slider;
  public GameObject player;

  void Start()
  {
    slider = GetComponent<Slider>();
  }
  void Update()
  {
    slider.value = player.transform.position.z / 443;
  }


}
