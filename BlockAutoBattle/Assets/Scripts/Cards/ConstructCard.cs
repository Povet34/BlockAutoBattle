using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ConstructCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] private Image cardBackground; // 카드 배경 이미지
    [SerializeField] private Image cardImage; // 카드 이미지
    [SerializeField] private TextMeshProUGUI cardCostText; // 카드 코스트 텍스트
    [SerializeField] private TextMeshProUGUI cardDescriptionText; // 카드 설명 텍스트

    private ConstructCardData cardData; // ConstructCardData 저장

    private Vector3 originalPosition; // 카드의 원래 위치
    private Quaternion originalRotation; // 카드의 원래 회전
    private Transform originalParent; // 카드의 원래 부모 Transform

    private Canvas canvas; // UI 캔버스 참조
    private RectTransform rectTransform; // 카드의 RectTransform

    public void Initialize(ConstructCardData data)
    {
        cardData = data; // ConstructCardData 저장

        // UI 업데이트
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

        // 카드 설명 및 코스트 텍스트 업데이트
        cardDescriptionText.text = cardData.description;
        cardCostText.text = cardData.cost.ToString();

        // 카드 배경 색상 업데이트
        if (cardBackground != null)
        {
            cardBackground.color = cardData.tetrisBlockData.blockColor;
        }

        // 카드 이미지 업데이트
        if (cardImage != null)
        {
            cardImage.sprite = cardData.cardMainImage;
            cardImage.enabled = true;
        }
    }

    // ConstructCardData 반환
    public ConstructCardData GetCardData()
    {
        return cardData;
    }

    // 마우스가 카드 위로 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalRotation = rectTransform.localRotation;
        originalParent = transform.parent;

        // 카드가 다른 카드들 위로 오도록 설정
        transform.SetAsLastSibling();

        // 카드의 회전을 정방향으로 설정
        rectTransform.localRotation = Quaternion.identity;

        // 카드의 위치를 약간 위로 이동
        rectTransform.anchoredPosition += new Vector2(0, 50f);
    }

    // 마우스가 카드에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        // 카드의 위치와 회전을 원래 상태로 복귀
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localRotation = originalRotation;
    }

    // 마우스를 클릭했을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"카드 {cardData.tetrisBlockData.blockName} 클릭됨.");
        // 클릭 시 동작 추가 (예: 카드 선택, 카드 사용 등)
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;

        // 드래그 중에는 카드가 캔버스의 최상위로 이동
        transform.SetParent(canvas.transform, true);
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 위치로 카드 이동
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        // 카드의 위치를 원래 상태로 복귀
        rectTransform.anchoredPosition = originalPosition;

        // 카드의 부모를 원래 부모로 복귀
        transform.SetParent(originalParent, true);
    }

    public int GetCost()
    {
        return cardData != null ? cardData.cost : 0; // 카드의 코스트 반환
    }
}
