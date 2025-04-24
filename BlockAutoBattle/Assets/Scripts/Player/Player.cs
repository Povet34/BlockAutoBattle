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
        playerStats.Initialize(this);
        blockPlacer.Initialize(this); // BlockPlacer 초기화
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
