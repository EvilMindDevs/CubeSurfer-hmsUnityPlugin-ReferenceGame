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
    private Action<int, string> OnGameFinished { get; set; }

    void Start()
    {
    }
    void OnTriggerEnter(Collider other)
    {
        string gameResult = "Defeat";
        bool isGameOver = false;

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
                gameResult = HitObstacle(other);
                isGameOver = true;
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
            OnGameFinished?.Invoke(int.TryParse(scoreText.text, out int score) ? score : 0, gameResult);
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
            scoreText.text = "SCORE: " + FindObjectOfType<GameManager>().gemCount.ToString();
        }
        else
        {
            scoreText.text = ("SCORE: " + ((other.GetComponent<MultiplierHandler>().MultiplierValue - 1) * FindObjectOfType<GameManager>().gemCount).ToString());
        }
        gameResult = "Victory";
        OnGameFinished += GameOver;
        return gameResult;
    }
    // Handle the trigger event when the player hits an obstacle.
    string HitObstacle(Collider other)
    {
        string gameResult;
        if (gameObject.tag.Equals("Player"))
        {
            character.GetComponent<Animation>().CrossFade("Defeat");
            other.enabled = false;
            transform.GetComponent<BoxCollider>().enabled = false;
            transform.GetComponent<Rigidbody>().useGravity = false;
            defeatMenu.SetActive(true);
            AudioManager.Instance.PlaySound(defeat, transform.position);
            OnGameFinished += GameOver;

        }
        gameResult = "Defeat";
        return gameResult;
    }
    string FinishLine(Collider other)
    {
        string gameResult;
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

        // Calculate the new transform vector
        Vector3 newTransform = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        // Iterate over all children of the cube
        for (int i = 0; i < transformChildren.Count; i++)
        {
            // Enable the BoxCollider and use gravity on the child game object
            Transform otherTransform = transformChildren[i];
            otherTransform.GetComponent<BoxCollider>().enabled = true;
            otherTransform.GetComponent<Rigidbody>().useGravity = true;

            DecideAddCube(otherTransform, newTransform);
        }

        // Destroy the original game object
        Destroy(other.gameObject);
    }
    public void DecideAddCube(Transform otherTransform, Vector3 newTransform)
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
    public void GameOver(int score, string result)
    {
        OnGameFinished += GameManager.Instance.OnGameFinished;
        OnGameFinished -= GameOver;
        OnGameFinished.Invoke(score, result);
        KitManager.Instance.EndGameAnalytics(score, result);
    }

    
}


