using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Cost Management")]
    public int maxCost = 10; // 최대 코스트
    private int currentCost; // 현재 코스트

    [Header("Recharge Settings")]
    public float rechargeTime = 10f; // 재충전 시간 (초)
    private float rechargeTimer; // 재충전 타이머

    private Player player; // Player 참조

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
        // 재충전 타이머 업데이트
        rechargeTimer -= Time.deltaTime;
        if (rechargeTimer <= 0f)
        {
            Recharge();
            rechargeTimer = rechargeTime; // 타이머 초기화
        }
    }

    private void Recharge()
    {
        // 코스트 재충전
        currentCost = maxCost;

        // Player를 통해 카드 재충전 호출
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

        return false; // 코스트 부족
    }

    public int GetCurrentCost()
    {
        return currentCost;
    }
}
