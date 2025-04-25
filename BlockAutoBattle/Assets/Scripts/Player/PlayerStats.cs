using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public class Data
    {
        public Player player;

        public Data(Player player)
        {
            this.player = player;
        }
    }

    [Header("Cost Management")]
    public int maxCost = 10; // �ִ� �ڽ�Ʈ
    private int currentCost; // ���� �ڽ�Ʈ

    [Header("Recharge Settings")]
    public float rechargeTime = 10f; // ������ �ð� (��)
    private float rechargeTimer; // ������ Ÿ�̸�

    private Player player; // Player ����

    public void Initialize(Data data)
    {
        player = data.player;
        currentCost = maxCost;
        rechargeTimer = rechargeTime;

        player.onRecharge += Recharge;
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
            // onRechargable �̺�Ʈ �߻�
            player?.onRechargable?.Invoke();
            rechargeTimer = rechargeTime; // Ÿ�̸� �ʱ�ȭ
        }
    }

    private void Recharge()
    {
        currentCost = maxCost;
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
