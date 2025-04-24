using UnityEngine;

public class CardCanvas : MonoBehaviour
{
    [SerializeField] RectTransform constructPanel;
    [SerializeField] RectTransform playerCardPanel;
    [SerializeField] RectTransform battlePanel;

    [SerializeField] RectTransform deckArea;
    [SerializeField] RectTransform handArea;
    [SerializeField] RectTransform graveyardArea;

    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    public RectTransform GetHandArea()
    {
        return handArea;
    }

    public RectTransform GetDeckArea()
    {
        return deckArea;
    }

    public RectTransform GetGraveyardArea()
    {
        return graveyardArea;
    }

    public bool IsInConstructPanel(Vector3 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(constructPanel, position);
    }

    public bool IsInBattleCardPanel(Vector3 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(playerCardPanel, position);
    }

    public float GetCanvasScale()
    {
        return canvas.scaleFactor;
    }
}
