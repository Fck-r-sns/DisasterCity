using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiCallReserves : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Text _valueOutput;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Todo: call reserves
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Todo: show description
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Todo: hide description
    }
}
