using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public Player.CharacterType characterType; // 플레이어 타입 (Player1, Player2)
    public string playerName;
    public StartingDeckData startingDeckData;

    public int startingCost;
    public int reChargeTime;
}
