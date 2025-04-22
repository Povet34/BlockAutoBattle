using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Cost Management")]
    public int maxCost = 10; // �ִ� �ڽ�Ʈ
    private int currentCost; // ���� �ڽ�Ʈ

    [Header("Recharge Settings")]
    public float rechargeTime = 10f; // ������ �ð� (��)
    private float rechargeTimer; // ������ Ÿ�̸�

    private Player player; // Player ����

    public void Initialize(Player player)
    {
        this.player = player;
        currentCost = maxCost;
        rechargeTimer = rechargeTime;
    }

    private void Update()
    {
        HandleRecharge();
    }

    private void HandleRecharge()
    {
        // ������ Ÿ�̸� ������Ʈ
        rechargeTimer -= Time.deltaTime;
        if (rechargeTimer <= 0f)
        {
            Recharge();
            rechargeTimer = rechargeTime; // Ÿ�̸� �ʱ�ȭ
        }
    }

    private void Recharge()
    {
        // �ڽ�Ʈ ������
        currentCost = maxCost;

        // Player�� ���� ī�� ������ ȣ��
        if (player != null)
        {
            player.RechargeCards();
        }
    }

    public bool UseCost(int cost)
    {
        if (currentCost >= cost)
        {
            currentCost -= cost;
            return true;
        }

        return false; // �ڽ�Ʈ ����
    }

    public int GetCurrentCost()
    {
        return currentCost;
    }
}
