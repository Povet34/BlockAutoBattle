using UnityEngine;

public class GameMngr : MonoBehaviour
{
    [SerializeField] Player playerPrefab;

    //테스트임. 시작전에 데이터들도 먼가 so로 관리해야할듯
    [SerializeField] PlayerData playerData;

    private void Awake()
    {
        var player = Instantiate(playerPrefab);
        player.Init(playerData);
    }
}
