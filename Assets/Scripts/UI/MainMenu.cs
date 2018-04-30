using UnityEngine;

public class MainMenu : MonoBehaviour 
{
	public GameObject mainMenu;
	public GameObject MiniatureCanvas;

	void Start () 
	{
		MiniatureCanvas.gameObject.SetActive(false);
	}

	public void StartButton()
	{ 
		if(MainMenuCamera.camIsMoving == false)
		{
			StartCoroutine(MainMenuCamera.CoroutineUtil.MoveCamera(Camera.main.transform,MainMenuCamera.MainMenuCamPos,MainMenuCamera.PlayerCreationCamPos,MainMenuCamera.MainMenuCamRot, MainMenuCamera.PlayerCreationCamRot, 1));
		}//if
	}

	public void SettingsButton()
	{
		if(MainMenuCamera.camIsMoving == false)
		{
			//StartCoroutine(MainMenuCamera.CoroutineUtil.MoveCamera(Camera.main.transform,MainMenuCamera.MainMenuCamPos,MainMenuCamera.SettingsCamPos,MainMenuCamera.MainMenuCamRot, MainMenuCamera.SettingsCamRot, 1));
			StartCoroutine(MainMenuCamera.CoroutineUtil.MoveCameraTwice(Camera.main.transform,MainMenuCamera.MainMenuCamPos,MainMenuCamera.OTableCamPos, MainMenuCamera.SettingsCamPos, MainMenuCamera.MainMenuCamRot,MainMenuCamera.OTableCamRot, MainMenuCamera.SettingsCamRot, 1));
		}//if
	}

	public void ControlsButton()
	{
		if(MainMenuCamera.camIsMoving == false)
		{
			StartCoroutine(MainMenuCamera.CoroutineUtil.MoveCameraTwice(Camera.main.transform,MainMenuCamera.MainMenuCamPos,MainMenuCamera.ControlsMidCamPos,MainMenuCamera.ControlsCamPos,MainMenuCamera.MainMenuCamRot,MainMenuCamera.ControlsMidCamRot, MainMenuCamera.ControlsCamRot, 1));
		}//if
	}
}
