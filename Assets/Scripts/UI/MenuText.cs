using System;

public class MenuText {
    public static string[] fighterPerks = new string[6] { "First Strike", "Bloodlust", "Reckless Swipe", "Scent of Blood", "Vanguard", "Juggernaut" };
    public static string[] fighterPDesc = new string[6] {
        "Once per battle you will always strike the first blow.",
        "Any damage you deal this turn heals you for that amount.",
        "You critically strike on a roll of a 19 or 20, but you critically miss on a 1 or a 2.",
        "If you score a critical hit against an opponent you will gain 'Rage' until the end of the battle, even if 'Rage' is on cooldown.",
        "At the expense of heavy armor, you gain an extra 2 movement squares but lose 10 hitpoints.",
        "Only heavy armor for you. You gain an extra 20 hitpoints but lose one square of movement."
    };

    public static string[] rangerPerks = new string[6] { "Double Shot", "Steady Aim", "Bear Trap", "Composite Bow", "", "" };
    public static string[] rangerPDesc = new string[6] {
        "Once per battle you unleash a second arrow which always finds its mark dealing half of your basic attack.",
        "You are more accurate with your attacks, but lose one square of movement.",
        "Place a bear trap at your feet. Enemies that walk over the trap are damaged and slow.",
        "You basic ranged attacks do more damage, but your melee becomes very weak.",
        "",
        ""
    };

    public static string[] roguePerks = new string[6] { "Booby Trap", "Poison Tipped", "Smoke Bomb", "Heavy Pockets", "Poison Darts", "Blade Runner" };
    public static string[] roguePDesc = new string[6] {
        "Place a trap at your feet which does massive damage to enemies when activated again.",
        "All basic attacks do a small amount of extra damage per turn.",
        "You can now find smoke bombs in chests. When used, this grants you stealth until you attack an ememy or leave the room.",
        "You can carry more potions than normal, and your keen senses seem to find more gold than usual.",
        "Swap your ranged throwing knives for poison darts. Poison darts do less damage but slow enemies significantly.",
        "No ranged attacks for you, you want to be beside your foe when they fall. You gain on extra 3 movement squares."
    };

    public static string[] wizardPerks = new string[6] { "Fire Mage", "Ice Mage", "", "", "", "" };
    public static string[] wizardPDesc = new string[6] {
        "",
        "",
        "",
        "",
        "",
        ""
    };

    public static string fighterDesc = "The Fighter class is the most resilient to damage and once per level can 'Rage', dealing double damage. " + Environment.NewLine + Environment.NewLine +
        "The Fighter can also use their brute strength and kick down doors, however this will alert all enemies in the room.";

    public static string rangerDesc = "The Ranger class is based around dealing damage at a distance. You can use melee attacks but they are far weaker than your ranged. " + Environment.NewLine + Environment.NewLine +
        "Rangers have 'Tracker's sense' which allows them to track enemy movements and the 'Headshot' ability lets you critically hit one foe per level if unnoticed.";

    public static string rogueDesc = "The Rogue class has a unique 'Stealth' ability where once per level you can roll for absoulte stealth, meaning that only line of sight can trigger an enemy. " +
        "While stealthed you can confidently sneak up behind an enemy and critically strike dealing max damage." + Environment.NewLine +
        "The Rogue also has ranged thrown weapons and can pick locks on doors and chests.";

    public static string wizardDesc = "The wizard class is the most frail of the classes but packs the biggest punch. " +
        "You use a variety of different attacks including area of effect abilites to hit multiple enemies. " + Environment.NewLine +
        "The Wizard can also 'skip' a level, teleporting them to the start of the next level.This can only be done once before incuring a level long cooldown. " +
        "The wizard can also unlock doors and chests with magic.";

    public static string[] SetCharacterPerks(string choice) {
        switch (choice) {
            case ("Fighter"):
                return fighterPerks;
            case ("Ranger"):
                return rangerPerks;
            case ("Rogue"):
                return roguePerks;
            case ("Wizard"):
                return wizardPerks;
            default:
                return null;
        }
    }

    public static string[] SetCharacterPDesc(string choice) {
        switch (choice) {
            case ("Fighter"):
                return fighterPDesc;
            case ("Ranger"):
                return rangerPDesc;
            case ("Rogue"):
                return roguePDesc;
            case ("Wizard"):
                return wizardPDesc;
            default:
                return null;
        }
    }

    public static string SetCharacterDesc(string choice) {
        switch (choice) {
            case ("Fighter"):
                return fighterDesc;
            case ("Ranger"):
                return rangerDesc;
            case ("Rogue"):
                return rogueDesc;
            case ("Wizard"):
                return wizardDesc;
            default:
                return null;
        }
    }
}
