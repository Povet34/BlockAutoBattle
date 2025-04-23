using UnityEngine;

public class GameMngr : MonoBehaviour
{
    [SerializeField] Player playerPrefab;

    //�׽�Ʈ��. �������� �����͵鵵 �հ� so�� �����ؾ��ҵ�
    [SerializeField] PlayerData playerData;

    private void Awake()
    {
        var player = Instantiate(playerPrefab);
        player.Init(playerData);
    }
}
