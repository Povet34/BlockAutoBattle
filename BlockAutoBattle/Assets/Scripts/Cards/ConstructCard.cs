using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConstructCard : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] private Image cardBackground; // 카드 배경 이미지
    [SerializeField] private Image cardIcon; // 카드 아이콘 이미지
    [SerializeField] private Text cardNameText; // 카드 이름 텍스트
    [SerializeField] private Text cardDescriptionText; // 카드 설명 텍스트
    [SerializeField] private Text cardCostText; // 카드 코스트 텍스트

    private TetrisBlockData tetrisBlockData;
    private int cost;
    private string description;

    public void Initialize(ConstructCardData data)
    {
        tetrisBlockData = data.tetrisBlockData;
        cost = data.cost;

        // UI 업데이트
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (!tetrisBlockData) return;

        cardNameText.text = tetrisBlockData.blockName;
        cardDescriptionText.text = description;
        cardCostText.text = cost.ToString();

        // 카드 배경 및 아이콘 설정 (필요 시)
        if (cardBackground != null)
        {
            cardBackground.color = tetrisBlockData.blockColor;
        }

        if (cardIcon != null && tetrisBlockData != null)
        {
            // 아이콘 설정 로직 (예: Resources.Load 또는 ScriptableObject에서 아이콘 참조)
        }
    }

    // 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"카드 {tetrisBlockData.blockName} 클릭됨.");
        // 클릭 시 동작 추가 (예: 카드 선택, 카드 사용 등)
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"카드 {tetrisBlockData.blockName} 드래그 시작.");
        // 드래그 시작 시 동작 추가
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // 카드 위치를 마우스 위치로 이동
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"카드 {tetrisBlockData.blockName} 드래그 종료.");
        // 드래그 종료 시 동작 추가 (예: 카드 배치, 원래 위치로 복귀 등)
    }

    public int GetCost()
    {
        return cost; // 카드의 코스트 반환
    }
}