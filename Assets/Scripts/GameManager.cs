using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject menuPanel;
    public GameObject gameOverPanel;
    public GameObject ballPrefab;
    public GameObject playerPrefab;
    public GameObject[] levels;
    GameObject ballInstance;
    GameObject playerInstance;
    GameObject levelInstance;

    public int lives;
    int levelNumber;
    // Position for breakout and resetting ball.
    Vector3 initialPosition = new Vector3(0, 5.3f, 0.3f);
    Vector3 resetPosition = new Vector3(0, 1, 0.3f);
    public enum GameState
    {
        Menu,
        Init,
        Play,
        Reset,
        GameOver
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

    public void ChangeState(GameState newState)
    {
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
                break;
            case GameState.Init:
                break;
            case GameState.Play:
                break;
            case GameState.Reset:
                break;
            case GameState.GameOver:
                gameOverPanel.SetActive(false);
                break;

        }
    }

    public void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                menuPanel.SetActive(true);
                Time.timeScale = 0;
                break;
            case GameState.Init:
                Time.timeScale = 1;
                lives = 3;
                levelNumber = 1;
                LoadLevel(levelNumber);
                SpawnPlayer();
                SpawnBall(initialPosition);
                break;
            case GameState.Play:
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
                ClearLevel();
                gameOverPanel.SetActive(true);
                Time.timeScale = 0;
                break;
        }
    }

    public void ClearLevel()
    {
        Destroy(levelInstance);
        Destroy(ballInstance);
        Destroy(playerInstance);
    }

    public void LoadLevel(int levelNumber)
    {
        switch (levelNumber)
        {
            case 1:
                levelInstance = Instantiate(levels[0].gameObject, levels[0].gameObject.transform.position, Quaternion.identity);
                break;
            
        }
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

   
}
