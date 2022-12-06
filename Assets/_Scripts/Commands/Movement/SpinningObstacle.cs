using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningObstacle : MonoBehaviour
{
  public float rotationSpeed = 60f;

  void Start()
  {
    transform.Rotate(0, Random.Range(0, 90), 0);
  }
  void Update()
  {
    transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
  }
}
