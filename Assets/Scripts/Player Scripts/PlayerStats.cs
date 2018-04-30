using UnityEngine;

public class PlayerStats
{
    [System.Serializable]
    public enum PlayerClass
    {
        None,
        Fighter,
        Rogue,
        Ranger,
        Wizard
    }

    public static PlayerClass playerClass = PlayerClass.None;

    //Private Variables
    //public static int STR = 0;
    //private int DEX = 0;
    //private int CON = 0;
    //private int INT = 0;
    //private int WIS = 0;
    //private int CHA = 0;

    // Use this for initialization
    void Start ()
    {
        switch (playerClass)
        {           
            case PlayerClass.Fighter:
                //STR = 15;
                //DEX = 12;
                //CON = 16;
                //INT = 8;
                //WIS = 10;
                //CHA = 8;
                break;

            case PlayerClass.Rogue:
                //STR = 10;
                //DEX = 16;
                //CON = 12;
                //INT = 12;
                //WIS = 10;
                //CHA = 10;
                break;

            case PlayerClass.Ranger:
                //STR = 10;
                //DEX = 10;
                //CON = 10;
                //INT = 10;
                //WIS = 10;
                //CHA = 10;
                break;

            case PlayerClass.Wizard:
                //STR = 10;
                //DEX = 10;
                //CON = 10;
                //INT = 10;
                //WIS = 10;
                //CHA = 10;
                break;

            default:
                break;
        }//switch

	}//start
	
}
