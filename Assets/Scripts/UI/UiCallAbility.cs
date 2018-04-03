using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiCallAbility : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Defines.AbilityType _abilityType;
    [SerializeField]
    Text _rechargeTime;

    private void Start()
    {

    }

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
