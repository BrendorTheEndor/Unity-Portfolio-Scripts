using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// A script that manages player score throughout scenes, displays the score, and handles scene loading
public class GameSession : MonoBehaviour {

    [SerializeField] int playerScore = 0;
    [SerializeField] Text scoreText;

    private void Awake() {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if(numGameSessions > 1) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        scoreText.text = playerScore.ToString();
    }

    private void Update() {
        if(SceneManager.GetActiveScene().buildIndex == 0) {
            Destroy(gameObject);
        }
    }

    public void AddScore(int scoreToAdd) {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();
    }

    public void ProcessPlayerDeath() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
