using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public float forwardSpeed = 10f;
  public float sideSpeed = 10f;
  public bool isMoving = false;
  public GameObject MainMenu;
  public GameObject defeatMenu;
  public GameObject victoryMenu;


  public void StartSpeedBoost()
  {
    StartCoroutine(SpeedBoost());
  }

  public IEnumerator SpeedBoost()
  {
    forwardSpeed = 15;
    yield return new WaitForSeconds(2);
    forwardSpeed = 10;
  }

  void Update()
  {
    if (!isMoving)
    {

      bool continueGame = Input.GetMouseButtonDown(0);
      if(AdsManager.Instance){
        continueGame = continueGame || AdsManager.Instance.isAdRewarded;
      }

      bool conditionMovementPlayer =  continueGame && !MainMenu && 
                                      !defeatMenu.activeInHierarchy && 
                                      !victoryMenu.activeInHierarchy;

      if (conditionMovementPlayer)
      {
        isMoving = true;
      }

      return;
    }

    //Player'ın hareket etmesini sağlayan kod.
    transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime); 

    //player position not range of -2 to 2 on x axis
    if (transform.position.x < -3f)
    {
      transform.position = new Vector3(-3f, transform.position.y, transform.position.z);
    }
    if (transform.position.x > 1.5f)
    {
      transform.position = new Vector3(1.5f, transform.position.y, transform.position.z);
    }



  }
}
