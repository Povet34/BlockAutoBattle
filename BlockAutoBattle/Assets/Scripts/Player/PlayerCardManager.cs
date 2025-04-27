using UnityEngine;
using System.Collections.Generic;

public class PlayerCardManager : MonoBehaviour
{
    private Player player; // Player 참조

    [Header("Card Deck")]
    [SerializeField] private ConstructCard cardPrefab; // ConstructCard Prefab
    [SerializeField] private CardCanvas cardCanvasPrefab; // CardCanvas Prefab

    private CardCanvas cardCanvas; // 동적으로 생성된 CardCanvas
    private List<ConstructCardData> cardDeckData; // 원본 카드 데이터 리스트
    private List<ConstructCard> hand; // 현재 핸드에 있는 카드들
    private List<ConstructCard> graveyard; // 묘지로 간 카드들

    public void Initialize(Player player, StartingDeckData startingDeck)
    {
        this.player = player;

        // CardCanvas 프리팹을 동적으로 생성
        if (cardCanvas == null)
        {
            cardCanvas = Instantiate(cardCanvasPrefab, transform);
            if (cardCanvas == null)
            {
                Debug.LogError("CardCanvas 프리팹에 CardCanvas 컴포넌트가 없습니다.");
                return;
            }
        }

        // rechargeButton 초기화
        cardCanvas.Init(() => { player.onRecharge?.Invoke(); }, 
        (active) => {player.onRechargable?.Invoke(active); });

        // Player의 onRecharge 이벤트 구독
        player.onRecharge += RechargeCards;

        InitializeDeck(startingDeck);
    }

    private void InitializeDeck(StartingDeckData startingDeck)
    {
        // 카드덱 초기화
        cardDeckData = new List<ConstructCardData>(startingDeck.constructCardDatas);
        hand = new List<ConstructCard>();
        graveyard = new List<ConstructCard>();

        DrawCards(); // 초기 핸드 드로우
    }

    private void DrawCards()
    {
        // 카드덱에서 5장의 카드를 핸드로 드로우
        ClearHand();

        int cardCount = Mathf.Min(5, cardDeckData.Count); // 최대 5장만 드로우
        float radius = 500f; // 원호의 반지름
        float maxAngle = 30f; // 원호의 최대 각도 (양쪽으로 15도씩 펼침)
        float angleStep = cardCount > 1 ? maxAngle * 2 / (cardCount - 1) : 0; // 카드 간의 각도 간격
        float startAngle = -maxAngle; // 시작 각도

        for (int i = 0; i < cardCount; i++)
        {
            // 카드 데이터 가져오기
            ConstructCardData cardData = cardDeckData[0];
            cardDeckData.RemoveAt(0);

            // ConstructCard 생성 및 초기화
            ConstructCard newCard = CreateCard(cardData);

            // 각도 계산
            float angle = startAngle + (i * angleStep);
            float radian = angle * Mathf.Deg2Rad;

            // 위치 계산 (원호의 중심에서 각도를 기준으로 배치)
            Vector3 position = new Vector3(Mathf.Sin(radian) * radius, 0, Mathf.Cos(radian) * radius);
            position.z = 0; // UI는 2D이므로 Z축은 0으로 설정

            // 카드 위치 및 회전 설정
            RectTransform cardRect = newCard.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                cardRect.anchoredPosition = position;
                cardRect.localRotation = Quaternion.Euler(0, 0, -angle); // 카드가 원호를 따라 기울어지도록 설정
            }

            // 카드의 부모를 HandArea로 설정
            newCard.transform.SetParent(cardCanvas.GetHandArea(), false);

            hand.Add(newCard);
        }
    }

    private ConstructCard CreateCard(ConstructCardData cardData)
    {
        // Prefab을 기반으로 ConstructCard 생성
        ConstructCard newCard = Instantiate(cardPrefab, cardCanvas.GetHandArea());
        newCard.Initialize(cardData, cardCanvas, player);
        return newCard;
    }

    private void ClearHand()
    {
        // 핸드에 있는 기존 카드를 모두 제거
        foreach (var card in hand)
        {
            Destroy(card.gameObject);
        }
        hand.Clear();
    }

    public void RechargeCards()
    {
        // 핸드의 카드들을 묘지로 이동
        foreach (var card in hand)
        {
            card.transform.SetParent(cardCanvas.GetGraveyardArea(), false);
            graveyard.Add(card);
        }
        ClearHand();

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
            card.transform.SetParent(cardCanvas.GetGraveyardArea(), false); // 카드의 부모를 GraveyardArea로 설정
            Destroy(card.gameObject); // UI 카드 제거
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
