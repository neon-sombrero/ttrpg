using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniatureScript : MonoBehaviour {
    public GameObject MiniatureCanvas;

    public void StartQuestButton() {
        SceneManager.LoadScene(1);
    }
}
