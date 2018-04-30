using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreatePlayer : MonoBehaviour {
    public float RotationSpeed = 1.0f;
    public Transform ttPoint;
    private List<PlayerClassContainer> ModelContainer = new List<PlayerClassContainer>();

    public GameObject MiniatureCanvas;

    public Button MiniButton;

    public Text classDescription;

    public Text charDescription;
    public Text PerkPlaceholder;
    public Text[] Perks = new Text[6];
    public Text[] PDescs = new Text[6];

    //Perk Text
    string[] CharacterPerks = new string[6];
    string[] CharacterPDesc = new string[6];

    void Start() {
        ShowPerkText(false);

        //Get the loaded playerlist from resources
        PlayerList pList = PlayerList.GetPlayerList();

        //Check everything is ready
        if (pList != null && ttPoint != null) {
            //Check the list isn't empty
            if (pList.PlayerPrefabs != null && pList.PlayerPrefabs.Count > 0) {
                //For each item
                for (int i = 0; i < pList.PlayerPrefabs.Count; i++) {
                    //Check it exists
                    if (pList.PlayerPrefabs[i] != null) {
                        //Check is has a model
                        if (pList.PlayerPrefabs[i].PlayerModel != null) {
                            //Creating the new model, at potision of transform
                            GameObject temp = GameObject.Instantiate(pList.PlayerPrefabs[i].PlayerModel, ttPoint.position, ttPoint.rotation) as GameObject;

                            //Create new container
                            PlayerClassContainer pCC = new PlayerClassContainer();
                            //Set it up
                            pCC.Class = pList.PlayerPrefabs[i].Class;
                            pCC.PlayerModel = temp;

                            if (temp.GetComponent<CameraOrbit>() != null) {
                                temp.GetComponent<CameraOrbit>().enabled = false;
                            }

                            //Add to our list
                            ModelContainer.Add(pCC);

                            //Deactivate it for now
                            temp.SetActive(false);
                        }//if
                    }//if
                }//for
            }//if
        }//if
    }//Start

    void Update() {
        if (MiniButton != null) {
            MiniButton.gameObject.SetActive(PlayerStats.playerClass != PlayerStats.PlayerClass.None);
        }

        //For each model container
        for (int i = 0; i < ModelContainer.Count; i++) {
            //If it exists
            if (ModelContainer[i] != null) {
                //If its model exists
                if (ModelContainer[i].PlayerModel != null) {
                    //If it's active
                    if (ModelContainer[i].PlayerModel.activeSelf) {
                        //Get the rotation
                        Quaternion Rot = ModelContainer[i].PlayerModel.transform.rotation;
                        //Get the XYZ euler angles
                        Vector3 EA = Rot.eulerAngles;

                        //Update values
                        EA.y += RotationSpeed * Time.deltaTime;

                        //Pass it back
                        Rot.eulerAngles = EA;
                        //Update the rotation
                        ModelContainer[i].PlayerModel.transform.rotation = Rot;
                    }
                }
            }
        }
    }//Update

    public void SetCharacterPerkText(string choice) {
        CharacterPerks = MenuText.SetCharacterPerks(choice);
        CharacterPDesc = MenuText.SetCharacterPDesc(choice);
        classDescription.text = MenuText.SetCharacterDesc(choice);
        for(int x = 0; x < 6; x++) {
            Perks[x].text = CharacterPerks[x];
            PDescs[x].text = CharacterPDesc[x];
        }
    }

    public void ShowPerkText(bool ToF) {
        PerkPlaceholder.gameObject.SetActive(!ToF);

        for (int x = 0; x < 6; x++) {
            Perks[x].gameObject.SetActive(ToF);
            PDescs[x].gameObject.SetActive(ToF);
        }
    }

    public void FighterButton() {
        ChangeSelection(PlayerStats.PlayerClass.Fighter);
        ShowPerkText(true);
    }

    public void RangerButton() {
        ChangeSelection(PlayerStats.PlayerClass.Ranger);
        ShowPerkText(true);
    }

    public void RogueButton() {
        ChangeSelection(PlayerStats.PlayerClass.Rogue);
        ShowPerkText(true);
    }

    public void WizardButton() {
        ChangeSelection(PlayerStats.PlayerClass.Wizard);
        ShowPerkText(true);
    }

    public void MainMenuButton() {
        MiniatureCanvas.gameObject.SetActive(false);
        ShowPerkText(false);
        if (MainMenuCamera.camIsMoving == false) {
            StartCoroutine(MainMenuCamera.CoroutineUtil.MoveCamera(Camera.main.transform, MainMenuCamera.PlayerCreationCamPos, MainMenuCamera.MainMenuCamPos, MainMenuCamera.PlayerCreationCamRot, MainMenuCamera.MainMenuCamRot, 1));
        }//if
    }

    public void MiniatureButton() {
        StartCoroutine(MainMenuCamera.CoroutineUtil.WaitForRealSecs(2));
        MiniatureCanvas.gameObject.SetActive(true);

        if (MainMenuCamera.camIsMoving == false) {
            StartCoroutine(MainMenuCamera.CoroutineUtil.MoveCamera(Camera.main.transform, MainMenuCamera.PlayerCreationCamPos, MainMenuCamera.MiniCamPos, MainMenuCamera.PlayerCreationCamRot, MainMenuCamera.MiniCamRot, 1));
        }//if
    }

    private void ChangeSelection(PlayerStats.PlayerClass Chosen) {
        //Update chosen
        PlayerStats.playerClass = Chosen;

        // Update all the text
        SetCharacterPerkText(Chosen.ToString());

        //For each model container
        for (int i = 0; i < ModelContainer.Count; i++) {
            //If it exists
            if (ModelContainer[i] != null) {
                //If its model exists
                if (ModelContainer[i].PlayerModel != null) {
                    //If it is the chosen one
                    if (ModelContainer[i].Class == Chosen) {
                        //Activate it
                        ModelContainer[i].PlayerModel.SetActive(true);
                        //Reset rotations
                        ModelContainer[i].PlayerModel.transform.rotation = new Quaternion();
                    } else {
                        //Otherwise, deactivate it
                        ModelContainer[i].PlayerModel.SetActive(false);
                    }
                }
            }
        }
    }
}
