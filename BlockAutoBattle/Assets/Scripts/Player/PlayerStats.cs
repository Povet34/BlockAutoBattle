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
    public int maxCost = 10; // 최대 코스트
    private int currentCost; // 현재 코스트

    [Header("Recharge Settings")]
    public float rechargeTime = 10f; // 재충전 시간 (초)
    private float rechargeTimer; // 재충전 타이머

    private bool isRecargable;

    private Player player; // Player 참조

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
        if (!isRecargable)
        {
            rechargeTimer -= Time.deltaTime;
            if (rechargeTimer <= 0f)
            {
                isRecargable = true; // 재충전 가능 상태로 변경
                rechargeTimer = rechargeTime; // 타이머 초기화
            }
        }

        player?.onRechargable?.Invoke(isRecargable);
    }

    private void Recharge()
    {
        currentCost = maxCost;
        isRecargable = false;
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
