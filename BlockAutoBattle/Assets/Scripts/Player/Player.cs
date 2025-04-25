using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum CharacterType //이건 캐릭터가 무엇인지를 결정한다.
    {
        TetrisBlocker,
    }

    public PlayerCardManager playerCardManager { get; private set; }
    public PlayerStats playerStats { get; private set; }
    public BlockPlacer blockPlacer { get; private set; }

    public CharacterType characterType;


    /// <summary>
    /// 카드와 코스트를 Recharing 할 수 있음을 알림 
    /// </summary>
    public Action onRechargable;

    /// <summary>
    /// 카드와 코스트를 Recharing 함.
    /// </summary>
    public Action onRecharge;

    private void OnEnable()
    {
        onRecharge += RechargeCards;
    }


    private void Awake()
    {
        // 각 컴포넌트 초기화
        playerCardManager = GetComponent<PlayerCardManager>();
        playerStats = GetComponent<PlayerStats>();
        blockPlacer = FindAnyObjectByType<BlockPlacer>();

        if (playerCardManager == null || playerStats == null || blockPlacer == null)
        {
            Debug.LogError("Player의 구성 요소가 제대로 설정되지 않았습니다.");
            return;
        }
    }

    public void Init(PlayerData data)
    {
        characterType = data.characterType;

        // 의존성 주입
        playerCardManager.Initialize(this, data.startingDeckData);
        playerStats.Initialize(new PlayerStats.Data(this));
        blockPlacer.Initialize(this);
    }

    public void RechargeCards()
    {
        // PlayerStats에서 호출하여 카드 재충전 수행
        playerCardManager.RechargeCards();
    }

    public bool UseCard(ConstructCard card, int cost)
    {
        // 카드 사용 로직을 중앙에서 관리
        if (playerCardManager != null && playerStats != null && blockPlacer != null)
        {
            if (playerCardManager.UseCard(card, playerStats))
            {
                // 고스트 블럭 생성
                blockPlacer.CreateGhostBlock(card.GetCardData().tetrisBlockData);
                return true;
            }
        }

        return false;
    }
}
