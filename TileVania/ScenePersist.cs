using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script is placed on a parent object for objects that you
// do not want to respawn on level reload, such as coin pickups.
public class ScenePersist : MonoBehaviour {

    int startingSceneIndex;

    // Singleton pattern used often to allow objects to persist through scene loading without duplicates
    private void Awake() {
        int numScenePersists = FindObjectsOfType<ScenePersist>().Length;

        if(numScenePersists > 1) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    // Object destroys itself when a new scene is loaded
    void Update() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(currentSceneIndex != startingSceneIndex) {
            Destroy(gameObject);
        }
    }
}
