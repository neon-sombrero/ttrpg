using UnityEngine;
using System.Collections;

public class GameController {
    public static int lvlCount = 1;
    public static int NumKills = 0;
    public static int ScoreAmount = 0;

    public static bool isPaused = false;
    public static bool mapIsDone = false;

    CursorLockMode wantedMode;

    public static void PauseGame(bool shouldPause) {
        isPaused = shouldPause;

        if (shouldPause == true) {
            Debug.Log("Paused");
            Time.timeScale = 0;
        }//if
        else {
            Debug.Log("Un-paused");
            Time.timeScale = 1;
        }//else
    }//PauseGame

    void Start() {
        // Might need to be looked at
        wantedMode = CursorLockMode.Confined;
        Cursor.lockState = wantedMode;

        Time.timeScale = 1;
    }//Start
}//GameController
