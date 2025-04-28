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
    private CardCanvas cardCanvas; // �������� ������ CardCanvas

    public Action<bool> onRechargable;
    public Action onRecharge;

    private void OnDestroy()
    {
        onRecharge -= RechargeCards;
    }

    private void Awake()
    {
        // �� ������Ʈ �ʱ�ȭ
        playerCardDeck = GetComponent<PlayerCardDeck>();
        playerStats = GetComponent<PlayerStats>();
        blockPlacer = FindAnyObjectByType<BlockPlacer>();

        onRecharge += RechargeCards;

        if (playerCardDeck == null || playerStats == null || blockPlacer == null)
        {
            Debug.LogError("Player�� ���� ��Ұ� ����� �������� �ʾҽ��ϴ�.");
            return;
        }

        // CardCanvas �ʱ�ȭ
        if (cardCanvas == null)
        {
            cardCanvas = Instantiate(cardCanvasPrefab, transform);
            cardCanvas.Init(() => { onRecharge?.Invoke(); }, (active) => { onRechargable?.Invoke(active); });
        }
    }

    public void Init(PlayerData data)
    {
        characterType = data.characterType;

        // ������ ����
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
        // PlayerStats���� ȣ���Ͽ� ī�� ������ ����
        playerCardDeck.RechargeCards();
    }

    public bool UseCard(ConstructCard card, int cost)
    {
        // ī�� ��� ������ �߾ӿ��� ����
        if (playerCardDeck != null && playerStats != null && blockPlacer != null)
        {
            if (playerCardDeck.UseCard(card, playerStats))
            {
                // ��Ʈ �� ����
                blockPlacer.CreateGhostBlock(card.GetCardData().tetrisBlockData);
                return true;
            }
        }

        return false;
    }
}
