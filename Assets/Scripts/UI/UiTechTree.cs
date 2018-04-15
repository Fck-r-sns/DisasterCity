using UnityEngine;

public class UiTechTree : MonoBehaviour
{
    private void Start()
    {
        Game.instance.onGameModeChanged += mode => gameObject.SetActive(mode == Game.Mode.TechTree);
        gameObject.SetActive(false);
    }
}
