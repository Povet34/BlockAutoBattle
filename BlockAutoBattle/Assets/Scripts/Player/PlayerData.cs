using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public Player.CharacterType characterType; // �÷��̾� Ÿ�� (Player1, Player2)
    public string playerName;
    public StartingDeckData startingDeckData;

    public int startingCost;
    public int reChargeTime;
}
