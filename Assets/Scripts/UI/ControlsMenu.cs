using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour {
    public Text ButtonDescriptions;

    public TextAsset FighterBDes;
    public TextAsset RangerBDes;
    public TextAsset RogueBDes;
    public TextAsset WizardBDes;

    public void SetButtonDescription(string text) {
        ButtonDescriptions.text = text;
    }

    public void FighterButton() {
        SetButtonDescription(FighterBDes.text);
    }

    public void RangerButton() {
        SetButtonDescription(RangerBDes.text);
    }

    public void RogueButton() {
        SetButtonDescription(RogueBDes.text);
    }

    public void WizardButton() {
        SetButtonDescription(WizardBDes.text);
    }

    public void MainMenuButton() {
        if (MainMenuCamera.camIsMoving == false) {
            StartCoroutine(MainMenuCamera.CoroutineUtil.MoveCameraTwice(Camera.main.transform, MainMenuCamera.ControlsCamPos, MainMenuCamera.ControlsMidCamPos, MainMenuCamera.MainMenuCamPos,
                                                                        MainMenuCamera.ControlsCamRot, MainMenuCamera.ControlsMidCamRot, MainMenuCamera.MainMenuCamRot, 1));
        }//if
    }
}
