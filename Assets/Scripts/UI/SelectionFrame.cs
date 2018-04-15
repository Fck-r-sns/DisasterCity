using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionFrame : Graphic, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    RectTransform _canvas;
    [SerializeField]
    RectTransform _frame;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _frame.gameObject.SetActive(true);
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
        _frame.anchoredPosition = new Vector2(x, y);
        _frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        _frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _frame.gameObject.SetActive(false);
        Game.unitsManager.EndFrameSelection();
    }
}
