using UnityEngine;

public class Player : MonoBehaviour
{
    public enum CharacterType //�̰� ĳ���Ͱ� ���������� �����Ѵ�.
    {
        TetrisBlocker,
    }

    public PlayerCardManager playerCardManager { get; private set; }
    public PlayerStats playerStats { get; private set; }

    public CharacterType characterType;

    private void Awake()
    {
        // �� ������Ʈ �ʱ�ȭ
        playerCardManager = GetComponent<PlayerCardManager>();
        playerStats = GetComponent<PlayerStats>();

        if (playerCardManager == null || playerStats == null)
        {
            Debug.LogError("Player�� ���� ��Ұ� ����� �������� �ʾҽ��ϴ�.");
            return;
        }
    }

    public void Init(PlayerData data)
    {
        characterType = data.characterType;

        // ������ ����
        playerCardManager.Initialize(this, data.startingDeckData);
        playerStats.Initialize(this);
    }

    public void RechargeCards()
    {
        // PlayerStats���� ȣ���Ͽ� ī�� ������ ����
        playerCardManager.RechargeCards();
    }

    public bool UseCard(ConstructCard card, int cost)
    {
        // ī�� ��� ������ �߾ӿ��� ����
        if (playerCardManager != null && playerStats != null)
        {
            return playerCardManager.UseCard(card, playerStats);
        }

        return false;
    }
}
