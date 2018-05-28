using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiCallAbility : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Defines.AbilityType _abilityType;
    [SerializeField]
    Text _loadState;

    private AbilityProvider _abilityProvider;

    private void Start()
    {
        _abilityProvider = Game.unitsManager.GetAbilityProvider(_abilityType);
        gameObject.SetActive(_abilityProvider.isActivated);
        _abilityProvider.onActivated += () =>
        {
            UpdateLoadStateOutput();
            gameObject.SetActive(true);
        };
        _abilityProvider.onLoadStateChanged += isLoaded => UpdateLoadStateOutput();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_abilityProvider.isLoaded)
            Game.instance.SetGameMode(Game.Mode.CallAbility, _abilityType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Todo: show description
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Todo: hide description
    }

    private void UpdateLoadStateOutput()
    {
        _loadState.text = _abilityProvider.isLoaded ? "Ready" : "Reloading...";
    }
}
