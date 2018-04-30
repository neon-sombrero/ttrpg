using UnityEngine;
using System.Collections;

public class PassValues : MonoBehaviour {
    public UnityEngine.UI.Text[] Scores;
    public UnityEngine.UI.Text[] Kills;
    public UnityEngine.UI.Text[] Levels;


    // Update is called once per frame
    void Update() {
        if (Scores != null) {
            foreach (UnityEngine.UI.Text Score in Scores) {
                if (Score != null) {
                    Score.text = GameController.ScoreAmount.ToString();
                }
            }
        }

        if (Kills != null) {
            foreach (UnityEngine.UI.Text Kill in Kills) {
                if (Kill != null) {
                    Kill.text = GameController.NumKills.ToString();
                }
            }
        }

        if (Levels != null) {
            foreach (UnityEngine.UI.Text Level in Levels) {
                if (Level != null) {
                    Level.text = GameController.lvlCount.ToString();
                }
            }
        }
    }
}
