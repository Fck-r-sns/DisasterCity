using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiOverlay : Graphic, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform _canvas;
    [SerializeField]
    private RectTransform _selectionFrame;
    [SerializeField]
    private RectTransform _attackableTargetPointer;
    [SerializeField]
    private RectTransform _selectedTargetPointer;

    private AttackableTargetPoint _curAttackableTargetPoint;
    private AttackableTargetPoint _selectedTargetPoint;

    protected override void Start()
    {
        base.Start();

        Game.unitsManager.onUnitsSelectionChanged += OnUnitsSelectionChanged;
        Game.unitsManager.onCurAttackableTargetPointChanged += SetCurAttackableTargetPoint;
        Game.unitsManager.onAttackableTargetPointSelected += SetSelectedTargetPoint;
    }

    private void Update()
    {
        if (_attackableTargetPointer.gameObject.activeSelf)
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(_curAttackableTargetPoint.transform.position);
            float x = pos.x / _canvas.localScale.x;
            float y = pos.y / _canvas.localScale.y;
            _attackableTargetPointer.anchoredPosition = new Vector2(x, y);
        }

        if (_selectedTargetPointer.gameObject.activeSelf)
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(_selectedTargetPoint.transform.position);
            float x = pos.x / _canvas.localScale.x;
            float y = pos.y / _canvas.localScale.y;
            _selectedTargetPointer.anchoredPosition = new Vector2(x, y);
        }
    }

    private void OnUnitsSelectionChanged(int selectedUnitsCount)
    {
        SetSelectedTargetPoint(null);

        if (selectedUnitsCount == 0)
            SetCurAttackableTargetPoint(null);
    }

    private void SetCurAttackableTargetPoint(AttackableTargetPoint point)
    {
        _curAttackableTargetPoint = point;
        _attackableTargetPointer.gameObject.SetActive(_curAttackableTargetPoint != null);
    }

    private void SetSelectedTargetPoint(AttackableTargetPoint point)
    {
        _selectedTargetPoint = point;
        _selectedTargetPointer.gameObject.SetActive(point != null);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _selectionFrame.gameObject.SetActive(true);
        Game.unitsManager.BeginFrameSelection();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        float x1 = eventData.pressPosition.x;
        float y1 = eventData.pressPosition.y;
        float x2 = eventData.position.x;
        float y2 = eventData.position.y;

        // Logic
        float x = Mathf.Min(x1, x2);
        float y = Mathf.Min(y1, y2);
        float width = Mathf.Abs(x2 - x1);
        float height = Mathf.Abs(y2 - y1);
        Game.unitsManager.UpdateFrameSelection(new Rect(x, y, width, height));

        // Visual
        x /= _canvas.localScale.x;
        y /= _canvas.localScale.y;
        width /= _canvas.localScale.x;
        height /= _canvas.localScale.y;
        _selectionFrame.anchoredPosition = new Vector2(x, y);
        _selectionFrame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        _selectionFrame.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _selectionFrame.gameObject.SetActive(false);
        Game.unitsManager.EndFrameSelection();
    }
}
