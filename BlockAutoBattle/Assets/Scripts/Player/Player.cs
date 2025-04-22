using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerCardManager playerCardManager { get; private set; }
    public PlayerStats playerStats { get; private set; }

    private void Awake()
    {
        // 각 컴포넌트 초기화
        playerCardManager = GetComponent<PlayerCardManager>();
        playerStats = GetComponent<PlayerStats>();

        if (playerCardManager == null || playerStats == null)
        {
            Debug.LogError("Player의 구성 요소가 제대로 설정되지 않았습니다.");
            return;
        }

        // 의존성 주입
        playerCardManager.Initialize(this);
        playerStats.Initialize(this);
    }

    public void RechargeCards()
    {
        // PlayerStats에서 호출하여 카드 재충전 수행
        playerCardManager.RechargeCards();
    }

    public bool UseCard(TetrisBlockData card, int cost)
    {
        // 카드 사용 로직을 중앙에서 관리
        if (playerCardManager != null && playerStats != null)
        {
            return playerCardManager.UseCard(card, cost, playerStats);
        }

        return false;
    }
}
