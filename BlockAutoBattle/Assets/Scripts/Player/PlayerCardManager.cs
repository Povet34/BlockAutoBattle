using UnityEngine;
using System.Collections.Generic;

public class PlayerCardManager : MonoBehaviour
{
    private Player player; // Player 참조

    [Header("Card Deck")]
    public List<TetrisBlockData> allAvailableCards; // 모든 가능한 카드 데이터
    private List<TetrisBlockData> cardDeck; // 플레이어의 카드덱
    private List<TetrisBlockData> hand; // 현재 핸드에 있는 카드들
    private List<TetrisBlockData> graveyard; // 묘지로 간 카드들

    public void Initialize(Player player)
    {
        this.player = player;
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        // 카드덱 초기화: 모든 가능한 카드 중 랜덤으로 20개 선택
        cardDeck = new List<TetrisBlockData>();
        hand = new List<TetrisBlockData>();
        graveyard = new List<TetrisBlockData>();

        for (int i = 0; i < 20; i++)
        {
            int randomIndex = Random.Range(0, allAvailableCards.Count);
            cardDeck.Add(allAvailableCards[randomIndex]);
        }

        DrawCards(); // 초기 핸드 드로우
    }

    private void DrawCards()
    {
        // 카드덱에서 5장의 카드를 핸드로 드로우
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
        // 핸드의 카드들을 묘지로 이동
        graveyard.AddRange(hand);
        hand.Clear();

        // 카드덱에서 새로운 카드 드로우
        DrawCards();
    }

    public bool UseCard(TetrisBlockData card, int cost, PlayerStats playerStats)
    {
        // 카드 사용 시 코스트 소모
        if (hand.Contains(card) && playerStats.UseCost(cost))
        {
            hand.Remove(card);
            graveyard.Add(card);
            return true;
        }

        return false; // 코스트 부족 또는 카드가 핸드에 없음
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
