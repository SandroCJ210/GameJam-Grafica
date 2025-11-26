using UnityEngine;

[CreateAssetMenu(menuName = "GameConfig/MageDifficulty")]

public class MageDifficultyConfig : ScriptableObject
{
    public float boldness;
    public float mistakeChanceEyesClosed;
    public float goodCard_Player_EyesOpen;
    public float goodCard_Player_EyesClosed;
    public float goodCard_Wizard_EyesOpen;
    public float goodCard_Wizard_EyesClosed;
}


