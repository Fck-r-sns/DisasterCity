using UnityEngine;
using UnityEngine.EventSystems;

public class AttackableTargetCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField]
    private AttackableTarget _target;

    public AttackableTarget target { get { return _target; } }

    public void OnPointerDown(PointerEventData eventData)
    {
        DebugUtilities.Print(gameObject.GetInstanceID(), "OnPointerDown");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DebugUtilities.Print(gameObject.GetInstanceID(), "OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DebugUtilities.Print(gameObject.GetInstanceID(), "OnPointerExit");
    }
}
