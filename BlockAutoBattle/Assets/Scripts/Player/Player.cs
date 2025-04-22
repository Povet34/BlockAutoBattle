using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerCardManager playerCardManager { get; private set; }
    public PlayerStats playerStats { get; private set; }

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

        // ������ ����
        playerCardManager.Initialize(this);
        playerStats.Initialize(this);
    }

    public void RechargeCards()
    {
        // PlayerStats���� ȣ���Ͽ� ī�� ������ ����
        playerCardManager.RechargeCards();
    }

    public bool UseCard(TetrisBlockData card, int cost)
    {
        // ī�� ��� ������ �߾ӿ��� ����
        if (playerCardManager != null && playerStats != null)
        {
            return playerCardManager.UseCard(card, cost, playerStats);
        }

        return false;
    }
}
