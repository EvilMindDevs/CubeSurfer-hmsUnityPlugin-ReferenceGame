using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class GameManager : Singleton<GameManager>
{
  public GameObject mainMenu;
  public GameObject startPrompt;
  public GameObject gemCounter;
  public GameObject progressBar;
  public GameObject gameButtons;
  public GameObject pauseMenu;
  public GameObject HMSKitScreen;
  public GameObject defeatMenu;
  public Button ShowRewardedCountinueGameAdButton;
  public Button ShowRewardedDoubleScoreAdButton;
  public GameObject Cube;
  public TMP_Text EndGameScore;
  [SerializeField] static bool restartGame = false;
  public int gemCount = 0;
  [SerializeField] TMP_Text gemCountText;
  float startTime;
  public float elapsedTime { get; private set; }
  public Action OnGameStarted { get; set; }
  public static Action<string> OnGameFinished { get; set; }
  public delegate void DestroyDelegate(GameObject obj, float t);

  new void Awake()
  {
    if (Instance) Destroy(gameObject);
        base.Awake();
    
    OnGameStarted += StartGame;
    OnGameFinished += GameOver;
    //lock potrait mode
    Screen.orientation = ScreenOrientation.Portrait;

  }

  void Start()
  {
    startTime = Time.time;
    AudioListener.volume = 0.5f;

    Time.timeScale = 1;

    if (restartGame)
    {
      OnGameStarted.Invoke();
    }

  }

  void Update()
  {
    gemCountText.text = (gemCount.ToString());
    elapsedTime = Time.time - startTime;
    if (!startPrompt.activeInHierarchy)
    {
      return;
    }
    if (Input.GetMouseButtonDown(0))
    {
      startPrompt.SetActive(false);
      gemCounter.SetActive(true);
      progressBar.SetActive(true);
      gameButtons.SetActive(true);
    }
  }

  public void StartGame()
  {
    Destroy(mainMenu);
    startPrompt.SetActive(true);
    startTime = Time.time;
  }

  public void PauseGame()
  {
    Time.timeScale = 0;
    pauseMenu.SetActive(true);

  }

  public void ResumeGame()
  {
    Time.timeScale = 1;
    pauseMenu.SetActive(false);

  }

  public void RestartGame()
  {
    restartGame = true;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void MainMenu()
  {
    restartGame = false;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void LoginHMS(){
    HMSKitScreen.SetActive(true);
    mainMenu.SetActive(false);
  }

  public void CloseMHSLoginMenu(){
    HMSKitScreen.SetActive(false);
    mainMenu.SetActive(true);

  }
  public void GameOver(string result)
  {
    FindObjectOfType<PlayerMovement>().isMoving = false;
    gemCounter.SetActive(false);
    progressBar.SetActive(false);
    gameButtons.SetActive(false);
    if(AdsManager.Instance){
      AdsManager.Instance.ShowInstertitialAd();
      AdsManager.Instance.isAdRewarded = false;
    }
  }

  public void ContinueGameAfterReward(){
        // Use a lambda expression to avoid the overhead of calling a function multiple times
        Action<Transform> decideAddCube = (cubeTransform) =>
          { PlayerController.Instance.DecideAddCube(cubeTransform); };
        // Use a delegate to avoid the overhead of calling a function multiple times
        DestroyDelegate destroyDelegate = Destroy;

        //Run main thread task
        StartCoroutine("PauseGame");


        ShowRewardedCountinueGameAdButton.interactable = false;
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<BoxCollider>().enabled = true;
        player.GetComponent<Rigidbody>().useGravity = true;
        var transform = player.transform;

        float count =5;
        for (int i = 0; i < count; i++)
        {
            GameObject cube = Instantiate(Cube, transform.position, Quaternion.identity, transform);
            cube.GetComponent<BoxCollider>().enabled = true;
            cube.GetComponent<Rigidbody>().useGravity = true;
            cube.transform.position = new Vector3(player.transform.position.x, player.transform.position.y-1, player.transform.position.z);
            decideAddCube.Invoke(cube.transform);
            destroyDelegate.Invoke(cube, 1f);
        }

        var aJ = GameObject.Find("Aj");
        aJ.GetComponent<Animation>().CrossFade("Idle");
        gemCounter.SetActive(true);
        progressBar.SetActive(true);
        gameButtons.SetActive(true);
        defeatMenu.SetActive(false);
        pauseMenu.SetActive(true);
        AdsManager.Instance.RewardAdCompleted -= GameManager.Instance.ContinueGameAfterReward;
  }

  public void DoubleScoreAfterReward(){
    ShowRewardedDoubleScoreAdButton.interactable = false;
    var score = PlayerController.score * 2;
    EndGameScore.text = $"SCORE: {score}";
    KitManager.Instance.EndGameAnalytics("Double Score Reward After Victory");
    AdsManager.Instance.RewardAdCompleted -= GameManager.Instance.DoubleScoreAfterReward;
  }

}



