using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ConstructCard : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] private Image cardBackground; // ī�� ��� �̹���
    [SerializeField] private Image cardImage; // ī�� �̹���
    [SerializeField] private TextMeshProUGUI cardCostText; // ī�� �ڽ�Ʈ �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI cardDescriptionText; // ī�� ���� �ؽ�Ʈ

    private ConstructCardData cardData; // ConstructCardData ����

    private Vector3 originalPosition; // ī���� ���� ��ġ
    private Quaternion originalRotation; // ī���� ���� ȸ��
    private Transform originalParent; // ī���� ���� �θ� Transform

    private CardCanvas cardCanvas; // UI ĵ���� ����
    private RectTransform rectTransform; // ī���� RectTransform
    private Player player; // Player ����

    public void Initialize(ConstructCardData cardData, CardCanvas cardCanvas, Player player)
    {
        this.cardData = cardData; // ConstructCardData ����
        this.cardCanvas = cardCanvas; // CardCanvas ����
        this.player = player; // Player ����

        // UI ������Ʈ
        UpdateUI();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void UpdateUI()
    {
        if (cardData == null || cardData.tetrisBlockData == null) return;

        // ī�� ���� �� �ڽ�Ʈ �ؽ�Ʈ ������Ʈ
        cardDescriptionText.text = cardData.description;
        cardCostText.text = cardData.cost.ToString();

        // ī�� ��� ���� ������Ʈ
        if (cardBackground != null)
        {
            cardBackground.color = cardData.tetrisBlockData.blockColor;
        }

        // ī�� �̹��� ������Ʈ
        if (cardImage != null)
        {
            cardImage.sprite = cardData.cardMainImage;
            cardImage.enabled = true;
        }
    }

    // ConstructCardData ��ȯ
    public ConstructCardData GetCardData()
    {
        return cardData;
    }

    // ���콺�� ī�� ���� �ö��� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalRotation = rectTransform.localRotation;
        originalParent = transform.parent;

        // ī�尡 �ٸ� ī��� ���� ������ ����
        transform.SetAsLastSibling();

        // ī���� ȸ���� ���������� ����
        rectTransform.localRotation = Quaternion.identity;

        // ī���� ��ġ�� �ణ ���� �̵�
        rectTransform.anchoredPosition += new Vector2(0, 50f);
    }

    // ���콺�� ī�忡�� ����� ��
    public void OnPointerExit(PointerEventData eventData)
    {
        // ī���� ��ġ�� ȸ���� ���� ���·� ����
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localRotation = originalRotation;
    }

    // ���콺�� Ŭ������ ��
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"ī�� {cardData.tetrisBlockData.blockName} Ŭ����.");
        // Ŭ�� �� ���� �߰� (��: ī�� ����, ī�� ��� ��)
    }

    // �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // �巡�� �߿��� ī�尡 ĵ������ �ֻ����� �̵�
        transform.SetParent(cardCanvas.transform, true);
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {
        // ���콺 ��ġ�� ī�� �̵�
        rectTransform.anchoredPosition += eventData.delta / cardCanvas.GetCanvasScale();
    }

    // �巡�� ����
    public void OnEndDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� ��ġ Ȯ��
        if (cardCanvas.IsInConstructPanel(eventData.position))
        {
            Debug.Log($"ī�� {cardData.tetrisBlockData.blockName}�� ConstructPanel�� �������ϴ�.");

            // Player�� UseCard ȣ��
            if (player.UseCard(this, cardData.cost))
            {
                Debug.Log("ī�� ��� ���� �� ��Ʈ �� ����!");
            }
            else
            {
                Debug.Log("ī�� ��� ����: �ڽ�Ʈ ���� �Ǵ� �ٸ� ����.");
                ResetCardPosition();
            }
        }
        else
        {
            // ī���� ��ġ�� ���� ���·� ����
            ResetCardPosition();
        }
    }

    private void ResetCardPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        transform.SetParent(originalParent, true);
    }

    public int GetCost()
    {
        return cardData != null ? cardData.cost : 0; // ī���� �ڽ�Ʈ ��ȯ
    }
}
