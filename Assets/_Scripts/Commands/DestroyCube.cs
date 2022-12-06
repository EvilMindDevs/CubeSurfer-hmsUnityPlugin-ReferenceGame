using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCube : MonoBehaviour
{

  public AudioClip dropCube;
  AudioSource audioSource;

  void Start()
  {
    audioSource = GetComponent<AudioSource>();
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Obstacle"))
    {
      other.tag = "Untagged";
      transform.parent = other.transform.parent;
    }
    if (other.CompareTag("ObstacleStack"))
    {
      PlaySoundDestroy();
      other.tag = "Untagged";

    }
    if (other.CompareTag("Multiplier"))
    {
      PlaySoundDestroy();
      other.tag = "Untagged";
      transform.parent = other.transform.parent;
    }
    if (other.CompareTag("SpinningObstacle"))
    {
      other.tag = "Untagged";
      transform.parent = other.transform.parent.parent;
      transform.GetComponent<Rigidbody>().isKinematic = true;
      transform.eulerAngles = Vector3.zero;
    }
    if (transform.tag.Equals("Lava"))
    {
      if (other.CompareTag("Cube"))
      {
        PlaySoundDestroy();
        Destroy(other.gameObject);
      }
    }
  }

  void PlaySoundDestroy()
  {
    audioSource.clip = dropCube;
    audioSource.Play();
  }
}
