using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortalScript : MonoBehaviour {
    GameObject canvas;
    bool canvasEnabled = false;

    void Start() {
        canvas = GameObject.FindGameObjectWithTag("ExitMenu");
        canvas.gameObject.SetActive(canvasEnabled);
    }

    void OnTriggerStay(Collider other) {
        if ((other.gameObject.tag == "Player") && (Input.GetKeyUp(KeyCode.E))) {
            GameController.isPaused = true;
            canvasEnabled = true;
            canvas.gameObject.SetActive(canvasEnabled);
        }//if
    }//OnTriggerStay

    public void CloseUI() {
        canvasEnabled = false;
        canvas = GameObject.FindGameObjectWithTag("ExitMenu");
        canvas.gameObject.SetActive(canvasEnabled);
        GameController.isPaused = false;
    }

    public void NextLevel() {
        Debug.Log("Loading next level...");
        GameController.lvlCount++;
        GameController.mapIsDone = false;
        GameController.isPaused = false;
        SceneManager.LoadScene(1);
    }
}//ExitPortalScript
