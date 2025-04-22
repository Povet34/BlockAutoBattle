using UnityEngine;
using System.Collections.Generic;

public class PlayerCardManager : MonoBehaviour
{
    private Player player; // Player 참조

    [Header("Card Deck")]
    private List<ConstructCard> cardDeck; // 플레이어의 카드덱
    private List<ConstructCard> hand; // 현재 핸드에 있는 카드들
    private List<ConstructCard> graveyard; // 묘지로 간 카드들

    public void Initialize(Player player, StartingDeckData startingDeck)
    {
        this.player = player;
        InitializeDeck(startingDeck);
    }

    private void InitializeDeck(StartingDeckData startingDeck)
    {
        // 카드덱 초기화
        cardDeck = new List<ConstructCard>();
        hand = new List<ConstructCard>();
        graveyard = new List<ConstructCard>();

        // StartingDeckData를 기반으로 ConstructCard 생성
        foreach (var cardData in startingDeck.constructCardDatas)
        {
            ConstructCard newCard = new GameObject(cardData.name).AddComponent<ConstructCard>();
            newCard.Initialize(cardData);
            cardDeck.Add(newCard);
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

    public bool UseCard(ConstructCard card, PlayerStats playerStats)
    {
        // 카드 사용 시 코스트 소모
        if (hand.Contains(card) && playerStats.UseCost(card.GetCost()))
        {
            hand.Remove(card);
            graveyard.Add(card);
            return true;
        }

        return false; // 코스트 부족 또는 카드가 핸드에 없음
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
