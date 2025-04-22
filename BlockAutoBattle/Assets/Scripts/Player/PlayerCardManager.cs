using UnityEngine;
using System.Collections.Generic;

public class PlayerCardManager : MonoBehaviour
{
    private Player player; // Player ����

    [Header("Card Deck")]
    private List<ConstructCard> cardDeck; // �÷��̾��� ī�嵦
    private List<ConstructCard> hand; // ���� �ڵ忡 �ִ� ī���
    private List<ConstructCard> graveyard; // ������ �� ī���

    public void Initialize(Player player, StartingDeckData startingDeck)
    {
        this.player = player;
        InitializeDeck(startingDeck);
    }

    private void InitializeDeck(StartingDeckData startingDeck)
    {
        // ī�嵦 �ʱ�ȭ
        cardDeck = new List<ConstructCard>();
        hand = new List<ConstructCard>();
        graveyard = new List<ConstructCard>();

        // StartingDeckData�� ������� ConstructCard ����
        foreach (var cardData in startingDeck.constructCardDatas)
        {
            ConstructCard newCard = new GameObject(cardData.name).AddComponent<ConstructCard>();
            newCard.Initialize(cardData);
            cardDeck.Add(newCard);
        }

        DrawCards(); // �ʱ� �ڵ� ��ο�
    }

    private void DrawCards()
    {
        // ī�嵦���� 5���� ī�带 �ڵ�� ��ο�
        hand.Clear();
        for (int i = 0; i < 5; i++)
        {
            if (cardDeck.Count > 0)
            {
                hand.Add(cardDeck[0]);
                cardDeck.RemoveAt(0);
            }
        }
    }

    public void RechargeCards()
    {
        // �ڵ��� ī����� ������ �̵�
        graveyard.AddRange(hand);
        hand.Clear();

        // ī�嵦���� ���ο� ī�� ��ο�
        DrawCards();
    }

    public bool UseCard(ConstructCard card, PlayerStats playerStats)
    {
        // ī�� ��� �� �ڽ�Ʈ �Ҹ�
        if (hand.Contains(card) && playerStats.UseCost(card.GetCost()))
        {
            hand.Remove(card);
            graveyard.Add(card);
            return true;
        }

        return false; // �ڽ�Ʈ ���� �Ǵ� ī�尡 �ڵ忡 ����
    }

    public List<ConstructCard> GetHand()
    {
        return hand;
    }

    public List<ConstructCard> GetGraveyard()
    {
        return graveyard;
    }
}
