using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using HmsPlugin;

public class PlayerController : StaticInstance<PlayerController>
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
    public static int score { get; set; }
    Action<string> OnGameFinished { get; set; }
    bool isGameOver = false;
    void Start()
    {
        if (GameManager.Instance != null)
            OnGameFinished += GameManager.OnGameFinished;
        
    }
    void OnTriggerEnter(Collider other)
    {
        string gameResult = "Defeat";
        

        // Call the appropriate function based on the tag of the object that was triggered.
        switch (other.tag)
        {
            case "Cubes":
                CollectCube(other);
                break;

            case "Gem":
                CollectGem(other);
                break;

            case "SpeedBoost":
                ActivateSpeedBoost(other);
                break;

            case "FinishLine":
                gameResult = FinishLine(other);
                isGameOver = true;
                break;

            case "Obstacle":
            case "SpinningObstacle":
            case "Lava":
                HitObstacle(other);
                break;

            case "Multiplier":
                gameResult = MultiplierScore(other);
                isGameOver = true;
                break;

            default:
                break;
        }

        // If the game is over, invoke the OnGameFinished event.
        if (isGameOver)
        {
            OnGameFinished?.Invoke(gameResult);
        }            
    }
    // Handle the trigger event when the player collides with a cube.
    void CollectCube(Collider other)
    {
        AudioManager.Instance.PlaySound(collectCube, transform.position);
        other.enabled = false;
        AddCube(other);
    }
    // Handle the trigger event when the player collides with a gem.
    void CollectGem(Collider other)
    {
        AudioManager.Instance.PlaySound(collectGem, transform.position);
        FindObjectOfType<GameManager>().gemCount++;
        Destroy(other.gameObject);
    }
    // Handle the trigger event when the player collides with a speed boost.
    void ActivateSpeedBoost(Collider other)
    {
        other.enabled = false;
        AudioManager.Instance.PlaySound(speedBoost, transform.position);
        FindObjectOfType<PlayerMovement>().StartSpeedBoost();
    }
    string MultiplierScore(Collider other)
    {
        character.GetComponent<Animation>().CrossFade("Victory");
        victoryMenu.SetActive(true);
        AudioManager.Instance.PlaySound(victory, transform.position);
        string gameResult;
        if (other.GetComponent<MultiplierHandler>().MultiplierValue - 1 <= 0)
        {
            score = CalculateScore();
            scoreText.text = $"SCORE: {score}";
        }
        else
        {
            score =  CalculateScore(other.GetComponent<MultiplierHandler>().MultiplierValue - 1);
            scoreText.text = $"SCORE: {score}";
        }
        gameResult = "Victory";
        OnGameFinished += GameOver;
        return gameResult;
    }
    // Handle the trigger event when the player hits an obstacle.
    void HitObstacle(Collider other)
    {
        if (gameObject.tag.Equals("Player"))
        {
            character.GetComponent<Animation>().CrossFade("Defeat");
            other.enabled = false;
            transform.GetComponent<BoxCollider>().enabled = false;
            transform.GetComponent<Rigidbody>().useGravity = false;
            defeatMenu.SetActive(true);
            AudioManager.Instance.PlaySound(defeat, transform.position);
            OnGameFinished += GameOver;
            isGameOver = true;
        }
    }
    string FinishLine(Collider other)
    {
        string gameResult;
        score = CalculateScore(other.GetComponent<MultiplierHandler>().MultiplierValue);
        if (gameObject.tag.Equals("Player"))
        {
            character.GetComponent<Animation>().CrossFade("Victory");
            victoryMenu.SetActive(true);
            scoreText.text = $"SCORE: {score}";
        }
        if (gameObject.tag.Equals("PlayerCube"))
        {
            character.GetComponent<Animation>().CrossFade("Victory");
            transform.parent.GetComponent<PlayerController>().victoryMenu.SetActive(true);
            transform.parent.GetComponent<PlayerController>().scoreText.text = $"SCORE: {score}";
        }
        AudioManager.Instance.PlaySound(victory, transform.position);
        gameResult = "Victory";
        OnGameFinished += GameOver;
        return gameResult;
    }
    void AddCube(Collider other)
    {
        List<Transform> transformChildren = new List<Transform>();

        // Get the transform of all children belonging to the cube
        for (int i = 0; i < other.transform.childCount; i++)
        {
            transformChildren.Add(other.transform.GetChild(i));
        }
        // Iterate over all children of the cube
        for (int i = 0; i < transformChildren.Count; i++)
        {
            // Enable the BoxCollider and use gravity on the child game object
            Transform otherTransform = transformChildren[i];
            otherTransform.GetComponent<BoxCollider>().enabled = true;
            otherTransform.GetComponent<Rigidbody>().useGravity = true;

            DecideAddCube(otherTransform);
        }

        // Destroy the original game object
        Destroy(other.gameObject);
    }
    public void DecideAddCube(Transform otherTransform)
    {
        // Determine if the parent object is tagged as "Player"
        Transform parentTransform = transform.tag == "Player" ? transform : transform.parent;

        // Add the cube underneath the parent object
        GameObject childCube = Instantiate(otherTransform.gameObject, parentTransform.position, Quaternion.identity, parentTransform);
        parentTransform.position = new Vector3(parentTransform.position.x, parentTransform.position.y + 1, parentTransform.position.z);
        childCube.transform.position = new Vector3(transform.position.x, 1, transform.position.z);

        // Get the Animator component on the child object and set the "collected" bool to true
        Animator animator = childCube.transform.Find("+1").GetComponent<Animator>();
        animator?.SetBool("collected", true);
    }
    void GameOver(string result)
    {
       KitManager.Instance.EndGameAnalytics(result);
    }

    int CalculateScore(int multiplier = 1)
    {
        var score = multiplier * FindObjectOfType<GameManager>().gemCount;
        var doubleScoreIsReady = Convert.ToBoolean(PlayerPrefs.GetInt("DoubleScore", 0));
        if(doubleScoreIsReady){
            score *= 2;
            PlayerPrefs.SetInt("DoubleScore", 0);
        }
        return score;
    }
        

    
}


