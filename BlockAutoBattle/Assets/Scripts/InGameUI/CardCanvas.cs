using System;
using UnityEngine;
using UnityEngine.UI;

public class CardCanvas : MonoBehaviour
{
    [SerializeField] RectTransform constructPanel;
    [SerializeField] RectTransform playerCardPanel;
    [SerializeField] RectTransform battlePanel;

    [SerializeField] RectTransform deckArea;
    [SerializeField] RectTransform handArea;
    [SerializeField] RectTransform graveyardArea;

    [SerializeField] Button rechargeButton;

    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    public void Init(Action onRecharge, Action<bool> onRechargable)
    {
        rechargeButton.onClick.AddListener(() => onRecharge?.Invoke());
        onRechargable += SetRechargeButtonInteractable;
    }

    public void SetRechargeButtonInteractable(bool interactable)
    {
        rechargeButton.interactable = interactable;
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
