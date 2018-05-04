using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerCreation : MonoBehaviour {
    public float RotationSpeed = 1.0f;
    public Transform ttPoint;
    private List<PlayerClassContainer> ModelContainer = new List<PlayerClassContainer>();

    public GUISkin myGUI;
    private string playerName;
    private string characterDescription;
    public TextAsset fighterDescription;
    public TextAsset rangerDescription;
    public TextAsset rogueDescription;
    public TextAsset wizardDescription;
    public UnityEngine.UI.InputField Name;
    public UnityEngine.UI.InputField Description;
    public UnityEngine.UI.Button StartButton;

    void Start() {
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
        if (Name != null) {
            SetPlayerName(Name.text);
        }
        if (Description != null) {
            if (!Description.enabled) {
                Description.enabled = true;
            }
            characterDescription = Description.text;
        }

        if (StartButton != null) {
            StartButton.gameObject.SetActive(PlayerStats.playerClass != PlayerStats.PlayerClass.None);
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
    }

    public void SetPlayerName(string Name) {
      PlayerStats.PlayerName = Name;
    }

    public void SetFighter() {
        ChangeSelection(PlayerStats.PlayerClass.Fighter);
        characterDescription = fighterDescription.text;
    }

    public void SetRanger() {
        ChangeSelection(PlayerStats.PlayerClass.Ranger);
        characterDescription = rangerDescription.text;
    }

    public void SetRogue() {
        ChangeSelection(PlayerStats.PlayerClass.Rogue);
        characterDescription = rogueDescription.text;
    }

    public void SetWizard() {
        ChangeSelection(PlayerStats.PlayerClass.Wizard);
        characterDescription = wizardDescription.text;
    }

    private void ChangeSelection(PlayerStats.PlayerClass Chosen) {
        if (Description != null) {
            Description.text = "";
            Description.text += characterDescription;
            Description.MoveTextEnd(true);
            Description.textComponent.text = characterDescription;
        }//if

        //Update chosen
        PlayerStats.playerClass = Chosen;

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

    public void LoadNextLevel(int LevelID) {
        SceneManager.LoadScene(LevelID);
    }
}
