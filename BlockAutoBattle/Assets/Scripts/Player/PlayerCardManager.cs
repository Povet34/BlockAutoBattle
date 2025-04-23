using UnityEngine;
using System.Collections.Generic;

public class PlayerCardManager : MonoBehaviour
{
    private Player player; // Player ����

    [Header("Card Deck")]
    [SerializeField] private GameObject cardPrefab; // ConstructCard Prefab
    [SerializeField] private Transform handCardArea; // �ڵ� ī�尡 ��ġ�� �θ� Transform

    private List<ConstructCardData> cardDeckData; // ���� ī�� ������ ����Ʈ
    private List<ConstructCard> hand; // ���� �ڵ忡 �ִ� ī���
    private List<ConstructCard> graveyard; // ������ �� ī���

    public void Initialize(Player player, StartingDeckData startingDeck)
    {
        this.player = player;

        // "HandDeckArea"��� �̸��� ���� ������Ʈ�� ã�� handCardArea�� �Ҵ�
        if (handCardArea == null)
        {
            GameObject handDeckAreaObject = GameObject.Find("HandDeckArea");
            if (handDeckAreaObject != null)
            {
                handCardArea = handDeckAreaObject.transform;
            }
            else
            {
                Debug.LogError("HandDeckArea ������Ʈ�� ã�� �� �����ϴ�. ���� 'HandDeckArea'��� �̸��� ������Ʈ�� �־�� �մϴ�.");
                return;
            }
        }

        InitializeDeck(startingDeck);
    }

    private void InitializeDeck(StartingDeckData startingDeck)
    {
        // ī�嵦 �ʱ�ȭ
        cardDeckData = new List<ConstructCardData>(startingDeck.constructCardDatas);
        hand = new List<ConstructCard>();
        graveyard = new List<ConstructCard>();

        DrawCards(); // �ʱ� �ڵ� ��ο�
    }

    private void DrawCards()
    {
        // ī�嵦���� 5���� ī�带 �ڵ�� ��ο�
        ClearHand();

        int cardCount = Mathf.Min(5, cardDeckData.Count); // �ִ� 5�常 ��ο�
        float radius = 500f; // ��ȣ�� ������
        float maxAngle = 30f; // ��ȣ�� �ִ� ���� (�������� 15���� ��ħ)
        float angleStep = cardCount > 1 ? maxAngle * 2 / (cardCount - 1) : 0; // ī�� ���� ���� ����
        float startAngle = -maxAngle; // ���� ����

        for (int i = 0; i < cardCount; i++)
        {
            // ī�� ������ ��������
            ConstructCardData cardData = cardDeckData[0];
            cardDeckData.RemoveAt(0);

            // ConstructCard ���� �� �ʱ�ȭ
            ConstructCard newCard = CreateCard(cardData);

            // ���� ���
            float angle = startAngle + (i * angleStep);
            float radian = angle * Mathf.Deg2Rad;

            // ��ġ ��� (��ȣ�� �߽ɿ��� ������ �������� ��ġ)
            Vector3 position = new Vector3(Mathf.Sin(radian) * radius, 0, Mathf.Cos(radian) * radius);
            position.z = 0; // UI�� 2D�̹Ƿ� Z���� 0���� ����

            // ī�� ��ġ �� ȸ�� ����
            RectTransform cardRect = newCard.GetComponent<RectTransform>();
            if (cardRect != null)
            {
                cardRect.anchoredPosition = position;
                cardRect.localRotation = Quaternion.Euler(0, 0, -angle); // ī�尡 ��ȣ�� ���� ���������� ����
            }

            hand.Add(newCard);
        }
    }

    private ConstructCard CreateCard(ConstructCardData cardData)
    {
        // Prefab�� ������� ConstructCard ����
        GameObject cardObject = Instantiate(cardPrefab, handCardArea);
        ConstructCard newCard = cardObject.GetComponent<ConstructCard>();
        newCard.Initialize(cardData);
        return newCard;
    }

    private void ClearHand()
    {
        // �ڵ忡 �ִ� ���� ī�带 ��� ����
        foreach (var card in hand)
        {
            Destroy(card.gameObject);
        }
        hand.Clear();
    }

    public void RechargeCards()
    {
        // �ڵ��� ī����� ������ �̵�
        graveyard.AddRange(hand);
        ClearHand();

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
            Destroy(card.gameObject); // UI ī�� ����
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
