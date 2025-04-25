using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum CharacterType //�̰� ĳ���Ͱ� ���������� �����Ѵ�.
    {
        TetrisBlocker,
    }

    public PlayerCardManager playerCardManager { get; private set; }
    public PlayerStats playerStats { get; private set; }
    public BlockPlacer blockPlacer { get; private set; }

    public CharacterType characterType;


    /// <summary>
    /// ī��� �ڽ�Ʈ�� Recharing �� �� ������ �˸� 
    /// </summary>
    public Action onRechargable;

    /// <summary>
    /// ī��� �ڽ�Ʈ�� Recharing ��.
    /// </summary>
    public Action onRecharge;

    private void OnEnable()
    {
        onRecharge += RechargeCards;
    }


    private void Awake()
    {
        // �� ������Ʈ �ʱ�ȭ
        playerCardManager = GetComponent<PlayerCardManager>();
        playerStats = GetComponent<PlayerStats>();
        blockPlacer = FindAnyObjectByType<BlockPlacer>();

        if (playerCardManager == null || playerStats == null || blockPlacer == null)
        {
            Debug.LogError("Player�� ���� ��Ұ� ����� �������� �ʾҽ��ϴ�.");
            return;
        }
    }

    public void Init(PlayerData data)
    {
        characterType = data.characterType;

        // ������ ����
        playerCardManager.Initialize(this, data.startingDeckData);
        playerStats.Initialize(new PlayerStats.Data(this));
        blockPlacer.Initialize(this);
    }

    public void RechargeCards()
    {
        // PlayerStats���� ȣ���Ͽ� ī�� ������ ����
        playerCardManager.RechargeCards();
    }

    public bool UseCard(ConstructCard card, int cost)
    {
        // ī�� ��� ������ �߾ӿ��� ����
        if (playerCardManager != null && playerStats != null && blockPlacer != null)
        {
            if (playerCardManager.UseCard(card, playerStats))
            {
                // ��Ʈ �� ����
                blockPlacer.CreateGhostBlock(card.GetCardData().tetrisBlockData);
                return true;
            }
        }

        return false;
    }
}
