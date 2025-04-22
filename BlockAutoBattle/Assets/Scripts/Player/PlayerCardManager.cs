using UnityEngine;
using System.Collections.Generic;

public class PlayerCardManager : MonoBehaviour
{
    private Player player; // Player ����

    [Header("Card Deck")]
    public List<TetrisBlockData> allAvailableCards; // ��� ������ ī�� ������
    private List<TetrisBlockData> cardDeck; // �÷��̾��� ī�嵦
    private List<TetrisBlockData> hand; // ���� �ڵ忡 �ִ� ī���
    private List<TetrisBlockData> graveyard; // ������ �� ī���

    public void Initialize(Player player)
    {
        this.player = player;
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        // ī�嵦 �ʱ�ȭ: ��� ������ ī�� �� �������� 20�� ����
        cardDeck = new List<TetrisBlockData>();
        hand = new List<TetrisBlockData>();
        graveyard = new List<TetrisBlockData>();

        for (int i = 0; i < 20; i++)
        {
            int randomIndex = Random.Range(0, allAvailableCards.Count);
            cardDeck.Add(allAvailableCards[randomIndex]);
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

    public bool UseCard(TetrisBlockData card, int cost, PlayerStats playerStats)
    {
        // ī�� ��� �� �ڽ�Ʈ �Ҹ�
        if (hand.Contains(card) && playerStats.UseCost(cost))
        {
            hand.Remove(card);
            graveyard.Add(card);
            return true;
        }

        return false; // �ڽ�Ʈ ���� �Ǵ� ī�尡 �ڵ忡 ����
    }

    public List<TetrisBlockData> GetHand()
    {
        return hand;
    }

    public List<TetrisBlockData> GetGraveyard()
    {
        return graveyard;
    }
}
