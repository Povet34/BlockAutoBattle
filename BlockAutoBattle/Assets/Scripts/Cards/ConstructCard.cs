using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConstructCard : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Components")]
    [SerializeField] private Image cardBackground; // ī�� ��� �̹���
    [SerializeField] private Image cardIcon; // ī�� ������ �̹���
    [SerializeField] private Text cardNameText; // ī�� �̸� �ؽ�Ʈ
    [SerializeField] private Text cardDescriptionText; // ī�� ���� �ؽ�Ʈ
    [SerializeField] private Text cardCostText; // ī�� �ڽ�Ʈ �ؽ�Ʈ

    private TetrisBlockData tetrisBlockData;
    private int cost;
    private string description;

    public void Initialize(ConstructCardData data)
    {
        tetrisBlockData = data.tetrisBlockData;
        cost = data.cost;

        // UI ������Ʈ
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (!tetrisBlockData) return;

        cardNameText.text = tetrisBlockData.blockName;
        cardDescriptionText.text = description;
        cardCostText.text = cost.ToString();

        // ī�� ��� �� ������ ���� (�ʿ� ��)
        if (cardBackground != null)
        {
            cardBackground.color = tetrisBlockData.blockColor;
        }

        if (cardIcon != null && tetrisBlockData != null)
        {
            // ������ ���� ���� (��: Resources.Load �Ǵ� ScriptableObject���� ������ ����)
        }
    }

    // Ŭ�� �̺�Ʈ ó��
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"ī�� {tetrisBlockData.blockName} Ŭ����.");
        // Ŭ�� �� ���� �߰� (��: ī�� ����, ī�� ��� ��)
    }

    // �巡�� ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"ī�� {tetrisBlockData.blockName} �巡�� ����.");
        // �巡�� ���� �� ���� �߰�
    }

    // �巡�� ��
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // ī�� ��ġ�� ���콺 ��ġ�� �̵�
    }

    // �巡�� ����
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"ī�� {tetrisBlockData.blockName} �巡�� ����.");
        // �巡�� ���� �� ���� �߰� (��: ī�� ��ġ, ���� ��ġ�� ���� ��)
    }

    public int GetCost()
    {
        return cost; // ī���� �ڽ�Ʈ ��ȯ
    }
}