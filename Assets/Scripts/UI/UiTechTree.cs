using UnityEngine;

public class UiTechTree : MonoBehaviour
{
    private void Start()
    {
        Game.instance.onGameModeChanged += (mode, context) => gameObject.SetActive(mode == Game.Mode.TechTree);
        gameObject.SetActive(false);
    }
}
