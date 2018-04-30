using UnityEngine;
using System.Collections;

public class SoundSettings : MonoBehaviour {

    public void MainMenuButton() {
        if (MainMenuCamera.camIsMoving == false) {
            StartCoroutine(MainMenuCamera.CoroutineUtil.MoveCameraTwice(Camera.main.transform, MainMenuCamera.SettingsCamPos, MainMenuCamera.OTableCamPos, MainMenuCamera.MainMenuCamPos,
                                                                        MainMenuCamera.SettingsCamRot, MainMenuCamera.OTableCamRot, MainMenuCamera.MainMenuCamRot, 1));
        }//if
    }
}
