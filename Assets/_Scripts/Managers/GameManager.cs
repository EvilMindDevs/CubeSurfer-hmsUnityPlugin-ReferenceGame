using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using HuaweiMobileServices.Analytics;
using HuaweiMobileServices.Utils;
using UnityEngine.UI;
using System.Net.Mail;


public class GameManager : Singleton<GameManager>
{
  public GameObject mainMenu;
  public GameObject startPrompt;
  public GameObject gemCounter;
  public GameObject progressBar;
  public GameObject gameButtons;
  public GameObject pauseMenu;
  public GameObject HMSKitScreen;
  [SerializeField] static bool restartGame = false;
  public int gemCount = 0;
  [SerializeField] TMP_Text gemCountText;

  new void Awake()
  {
    if (Instance) Destroy(gameObject);
        base.Awake();
      
    }

  void Start()
  {
    
    AudioListener.volume = 0.5f;

    Time.timeScale = 1;

    if (restartGame)
    {
      StartGame();
    }

  }

  void Update()
  {
    gemCountText.text = (gemCount.ToString());

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
    KitManager.Instance.StartGameAnalytics();
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
}
