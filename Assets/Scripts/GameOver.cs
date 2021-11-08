using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    // Start is called before the first frame update
    void Start()
    {
        Guard.OnGuardHasSpottedPlayer += DisplayGameOver;
        FindObjectOfType<Player>().OnGoalReached += DisplayGameWin;
    }

    void DisplayGameOver() {
        OnGameOver(gameLoseUI);
    }

    void DisplayGameWin() {
        OnGameOver(gameWinUI);
    }

    void OnGameOver(GameObject gameoverUI) {
        gameoverUI.SetActive(true);
        gameIsOver = true;
        FindObjectOfType<Player>().OnGoalReached -= DisplayGameWin;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene(0);
            }
        }
    }
}
