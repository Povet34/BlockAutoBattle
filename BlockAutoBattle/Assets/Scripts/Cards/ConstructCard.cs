using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ConstructCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    private Canvas canvas; // UI ĵ���� ����
    private RectTransform rectTransform; // ī���� RectTransform

    public void Initialize(ConstructCardData data)
    {
        cardData = data; // ConstructCardData ����

        // UI ������Ʈ
        UpdateUI();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
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
        transform.SetParent(canvas.transform, true);
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {
        // ���콺 ��ġ�� ī�� �̵�
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // �巡�� ����
    public void OnEndDrag(PointerEventData eventData)
    {
        // ī���� ��ġ�� ���� ���·� ����
        rectTransform.anchoredPosition = originalPosition;

        // ī���� �θ� ���� �θ�� ����
        transform.SetParent(originalParent, true);
    }

    public int GetCost()
    {
        return cardData != null ? cardData.cost : 0; // ī���� �ڽ�Ʈ ��ȯ
    }
}
