using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum CharacterType
    {
        TetrisBlocker,
    }

    public PlayerCardDeck playerCardDeck { get; private set; }
    public PlayerStats playerStats { get; private set; }
    public BlockPlacer blockPlacer { get; private set; }

    public CharacterType characterType;

    [SerializeField] private CardCanvas cardCanvasPrefab; // CardCanvas Prefab
    private CardCanvas cardCanvas; // 동적으로 생성된 CardCanvas

    public Action<bool> onRechargable;
    public Action onRecharge;

    private void OnDestroy()
    {
        onRecharge -= RechargeCards;
    }

    private void Awake()
    {
        // 각 컴포넌트 초기화
        playerCardDeck = GetComponent<PlayerCardDeck>();
        playerStats = GetComponent<PlayerStats>();
        blockPlacer = FindAnyObjectByType<BlockPlacer>();

        onRecharge += RechargeCards;

        if (playerCardDeck == null || playerStats == null || blockPlacer == null)
        {
            Debug.LogError("Player의 구성 요소가 제대로 설정되지 않았습니다.");
            return;
        }

        // CardCanvas 초기화
        if (cardCanvas == null)
        {
            cardCanvas = Instantiate(cardCanvasPrefab, transform);
            cardCanvas.Init(() => { onRecharge?.Invoke(); }, (active) => { onRechargable?.Invoke(active); });
        }
    }

    public void Init(PlayerData data)
    {
        characterType = data.characterType;

        // 의존성 주입
        playerCardDeck.Initialize(this, data.startingDeckData);
        playerStats.Initialize(new PlayerStats.Data(this));
        blockPlacer.Initialize(this);
    }

    public CardCanvas GetCardCanvas()
    {
        return cardCanvas;
    }

    public void RechargeCards()
    {
        // PlayerStats에서 호출하여 카드 재충전 수행
        playerCardDeck.RechargeCards();
    }

    public bool UseCard(ConstructCard card, int cost)
    {
        // 카드 사용 로직을 중앙에서 관리
        if (playerCardDeck != null && playerStats != null && blockPlacer != null)
        {
            if (playerCardDeck.UseCard(card, playerStats))
            {
                // 고스트 블럭 생성
                blockPlacer.CreateGhostBlock(card.GetCardData().tetrisBlockData);
                return true;
            }
        }

        return false;
    }
}
