using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour {
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!GameController.isPaused) {
                GameController.isPaused = true;
            } else {
                GameController.isPaused = false;
            }

            GameController.PauseGame(GameController.isPaused);
        }

        // Dev Controls
        if (Input.GetKeyDown(KeyCode.F5)) {
            Debug.Log("Advancing a level...");
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.K)) {
            Debug.Log("Killing game...");
            SceneManager.UnloadSceneAsync(1);
        }
    }//Update
}
