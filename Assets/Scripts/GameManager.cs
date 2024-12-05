using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject menuPanel;
    public GameObject playPanel;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI levelText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject ballPrefab;
    public GameObject playerPrefab;
    public GameObject[] levels;
    GameObject ballInstance;
    GameObject playerInstance;
    GameObject levelInstance;

    public int lives;
    public int levelNumber;
    public int totalBricks;

    public AudioSource audioSource;
    public AudioClip menuMusic;
    public AudioClip playMusic;
    public AudioClip gameOverMusic;
    public AudioClip winMusic;

    // Position for breakout and resetting ball.
    Vector3 initialPosition = new Vector3(0, 5.3f, 0.3f);
    Vector3 resetPosition = new Vector3(0, 1, 0.3f);

    private bool isLevelTransitioning = false;


    public enum GameState
    {
        Menu,
        Init,
        Play,
        LoadLevel,
        Reset,
        GameOver,
        Win,
    }

    public GameState currentState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        ChangeState(GameState.Menu);
    }

    public void ChangeState(GameState newState, float delay = 0)
    {
        // Prevent from calling load level twice or loading level on win screen.
        if (isLevelTransitioning && newState == GameState.LoadLevel) return;
        StartCoroutine(ChangeStateCoroutine(newState, delay));
    }

    private IEnumerator ChangeStateCoroutine(GameState newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }

    public void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                menuPanel.SetActive(false);
                audioSource.Stop();
                break;
            case GameState.Init:
                break;
            case GameState.Play:
                break;
            case GameState.LoadLevel:
                break;
            case GameState.Reset:
                gameOverPanel.SetActive(false);
                break;
            case GameState.GameOver:
                gameOverPanel.SetActive(false);
                audioSource.Stop();
                break;
            case GameState.Win:
                winPanel.SetActive(false);
                audioSource.Stop();
                break;

        }
    }

    public void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                menuPanel.SetActive(true);
                audioSource.clip = menuMusic;
                audioSource.Play();
                break;
            case GameState.Init:
                audioSource.clip = playMusic;
                audioSource.Play();
                lives = 3;
                levelNumber = 0;
                SpawnPlayer();
                ChangeState(GameState.LoadLevel);
                break;
            case GameState.Play:
                playPanel.SetActive(true);
                UpdateLivesText();
                UpdateLevelText();
                break;
            case GameState.LoadLevel:
                if (ballInstance != null)
                {
                    Destroy(ballInstance);
                }
                
                isLevelTransitioning = true;
                levelNumber++;
                if (levelNumber > 3)
                {
                    isLevelTransitioning = false;
                    ChangeState(GameState.Win);
                    return;
                }
                UpdateLevelText();
                LoadLevel(levelNumber);
                CountBricks();
                SpawnBall(initialPosition);
                ChangeState(GameState.Play, 4f);
                break;
            case GameState.Reset:
                lives--;
                if (lives <= 0)
                {
                    ChangeState(GameState.GameOver);
                    break;
                }
                else
                {
                    SpawnBall(resetPosition);
                    ChangeState(GameState.Play);
                    break;
                }
            case GameState.GameOver:
                playPanel.SetActive(false);
                ClearLevel();
                gameOverPanel.SetActive(true);
                audioSource.clip = gameOverMusic;
                audioSource.Play();
                break;
            case GameState.Win:
                playPanel.SetActive(false);
                ClearLevel();
                winPanel.SetActive(true);
                audioSource.clip = winMusic;
                audioSource.Play();
                break;
        }
    }

    public void ClearLevel()
    {
        Destroy(levelInstance);
        Destroy(ballInstance);
        Destroy(playerInstance);
    }

    public void CountBricks()
    {
        totalBricks = 0;
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
        totalBricks = bricks.Length;
    }

    public void LoadLevel(int levelNumber)
    {
        if (levelNumber < 1)
        {
            levelNumber = 1;
        }
        if (levelInstance != null)
        {
            Destroy(levelInstance);
        }
        switch (levelNumber)
        {
            case 1:
                levelInstance = Instantiate(levels[0].gameObject, levels[0].gameObject.transform.position, Quaternion.identity);
                break;
            case 2:
                levelInstance = Instantiate(levels[1].gameObject, levels[1].gameObject.transform.position, Quaternion.identity);
                break;
            case 3:
                levelInstance = Instantiate(levels[2].gameObject, levels[2].gameObject.transform.position, Quaternion.identity);
                break;
        }
        isLevelTransitioning = false;
    }

    public void UpdateLivesText()
    {
       livesText.text = "Balls: " + lives;
    }

    public void UpdateLevelText()
    {
       levelText.text = "Level: " + levelNumber;
    }

    public void SpawnBall(Vector3 position)
    {
        ballInstance = Instantiate(ballPrefab, position, Quaternion.identity);
    }

    public void SpawnPlayer()
    {
        playerInstance = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.identity);
    }

    public void OnPlayClicked()
    {
        ChangeState(GameState.Init);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    } 
}
