using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
  public AudioClip collectCube;
  public AudioClip collectGem;
  public AudioClip speedBoost;
  public AudioClip victory;
  public AudioClip defeat;
  public GameObject character;
  public GameObject defeatMenu;
  public GameObject victoryMenu;
  public TMP_Text scoreText;


  void Start()
  {
  }
  // Player'ın hangi objelerle temas ettiğini kontrol eden fonksiyon.
  void OnTriggerEnter(Collider other)
  {
    if (other.tag.Equals("Cubes"))
    {
      AudioManager.Instance.PlaySound(collectCube,transform.position);
      other.enabled = false;
      AddCube(other);
    }

    if (other.tag.Equals("Gem"))
    {
      AudioManager.Instance.PlaySound(collectGem,transform.position);
      FindObjectOfType<GameManager>().gemCount++;
      Destroy(other.gameObject);
    }

    if (other.tag.Equals("SpeedBoost"))
    {
      other.enabled = false;
      AudioManager.Instance.PlaySound(speedBoost,transform.position);
      FindObjectOfType<PlayerMovement>().StartSpeedBoost();

    }

    if (other.tag.Equals("FinishLine"))
    {
      GameOver();
      if (gameObject.tag.Equals("Player"))
      {
        character.GetComponent<Animation>().CrossFade("Victory");
        victoryMenu.SetActive(true);
        scoreText.text = ("SCORE: " + ((other.GetComponent<MultiplierHandler>().MultiplierValue - 1) * FindObjectOfType<GameManager>().gemCount).ToString());
      }
      if (gameObject.tag.Equals("PlayerCube"))
      {
        character.GetComponent<Animation>().CrossFade("Victory");
        transform.parent.GetComponent<PlayerController>().victoryMenu.SetActive(true);
        transform.parent.GetComponent<PlayerController>().scoreText.text = ("SCORE: " + ((other.GetComponent<MultiplierHandler>().MultiplierValue - 1) * FindObjectOfType<GameManager>().gemCount).ToString());
      }
      AudioManager.Instance.PlaySound(victory,transform.position);

    }

    if (gameObject.tag.Equals("Player"))
    {
      if(other is {tag: "Obstacle" or "SpinningObstacle" or "Lava"})
      {
        character.GetComponent<Animation>().CrossFade("Defeat");
        other.enabled = false;
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<Rigidbody>().useGravity = false;
        GameOver();
        defeatMenu.SetActive(true);
        AudioManager.Instance.PlaySound(defeat,transform.position);

   
        KitManager.Instance.EndGameAnalytics(Time.timeSinceLevelLoad,"Defeat");
        
        // int minutes = Mathf.FloorToInt(timeSpent / 60F);
        // int seconds = Mathf.FloorToInt(timeSpent - minutes * 60);
        // string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);



      }

      if (other.tag.Equals("Multiplier"))
      {
        GameOver();
        character.GetComponent<Animation>().CrossFade("Victory");
        victoryMenu.SetActive(true);
        AudioManager.Instance.PlaySound(victory,transform.position);
        if (other.GetComponent<MultiplierHandler>().MultiplierValue - 1 <= 0)
        {
          scoreText.text = "SCORE: " + FindObjectOfType<GameManager>().gemCount.ToString();
        }
        else
        {
          scoreText.text = ("SCORE: " + ((other.GetComponent<MultiplierHandler>().MultiplierValue - 1) * FindObjectOfType<GameManager>().gemCount).ToString());
        }
        KitManager.Instance.EndGameAnalytics(Time.timeSinceLevelLoad,"Victory");
      }

    }
  
  }
  void AddCube(Collider other)
  {
    HashSet<Transform> transformChildren = new(other.transform.childCount);  

    //Küplere ait bütün çocukların transfomlarını alıyoruz.
    for (int i = 0; i < other.transform.childCount; i++)
    {
      transformChildren.Add(other.transform.GetChild(i));
    } 


    foreach(var otherTransform in transformChildren){
      otherTransform.GetComponent<BoxCollider>().enabled = true;
      otherTransform.GetComponent<Rigidbody>().useGravity = true;
      if (transform.tag == "Player")
      {
        //Doğrudan player'a altına küp ekleme senaryosu
        GameObject childCube = Instantiate(otherTransform.gameObject, transform.position, Quaternion.identity, transform);
        Vector3 newTransform = new (transform.position.x, transform.position.y + 1, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position,newTransform, (Time.deltaTime * 42.0f));
        childCube.transform.position = new (transform.position.x, 1, transform.position.z);
        Animator animator = childCube.transform.Find("+1").GetComponent<Animator>();
        animator?.SetBool("collected", true);  
      }
      else
      {
        //Küp altına küp ekleme senaryosu
        GameObject childCube = Instantiate(otherTransform.gameObject, transform.position, Quaternion.identity, transform.parent);
        Vector3 newTransform  = new (transform.parent.position.x, transform.parent.position.y + 1, transform.parent.position.z);
        transform.parent.position = Vector3.MoveTowards(transform.parent.position,newTransform, (Time.deltaTime * 42.0f));
        childCube.transform.position = new (transform.position.x, 1, transform.position.z);
        Animator animator = childCube.transform.Find("+1").GetComponent<Animator>();
        animator?.SetBool("collected", true);
      }
    }

    Destroy(other.gameObject);

  }
  public void GameOver()
  {
    FindObjectOfType<PlayerMovement>().isMoving = false;
    FindObjectOfType<GameManager>().gemCounter.SetActive(false);
    FindObjectOfType<GameManager>().progressBar.SetActive(false);
    FindObjectOfType<GameManager>().gameButtons.SetActive(false);
  }

}


