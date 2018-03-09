using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionFrame : Graphic, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    RectTransform _frame;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _frame.gameObject.SetActive(true);
        Game.instance.unitsManager.BeginFrameSelection();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        float x = eventData.position.x >= eventData.pressPosition.x ? eventData.pressPosition.x : eventData.position.x;
        float y = eventData.position.y >= eventData.pressPosition.y ? eventData.pressPosition.y : eventData.position.y;
        _frame.anchoredPosition = new Vector2(x, y);
        _frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Abs(eventData.position.x - eventData.pressPosition.x));
        _frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Abs(eventData.position.y - eventData.pressPosition.y));

        Rect rect = _frame.rect;
        rect.x = x;
        rect.y = y;
        Game.instance.unitsManager.UpdateFrameSelection(rect);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        _frame.gameObject.SetActive(false);
        Game.instance.unitsManager.EndFrameSelection();
    }
}
