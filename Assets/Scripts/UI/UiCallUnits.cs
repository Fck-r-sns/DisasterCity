using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiCallUnits : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Defines.UnitType _unitType;
    [SerializeField]
    Text _unitsCount;

    private UnitsProvider _unitsProvider;

    private void Start()
    {
        _unitsProvider = Game.unitsManager.GetUnitsProvider(_unitType);
        gameObject.SetActive(_unitsProvider.isActivated);
        _unitsProvider.onActivated += () =>
        {
            UpdateUnitsCountOutput();
            gameObject.SetActive(true);
        };
        _unitsProvider.onUnitsCountChanged += count => UpdateUnitsCountOutput();
        _unitsProvider.onMaxUnitsCountChanged += count => UpdateUnitsCountOutput();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_unitsProvider.unitsCount > 0)
            Game.instance.SetGameMode(Game.Mode.CallUnits, _unitType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Todo: show description
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Todo: hide description
    }

    private void UpdateUnitsCountOutput()
    {
        _unitsCount.text = _unitsProvider.unitsCount + "/" + _unitsProvider.maxUnitsCount;
    }
}
