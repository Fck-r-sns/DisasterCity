using UnityEngine;

public class UiTechTree : MonoBehaviour
{
    private void Start()
    {
        Game.instance.onTechTreeActivationChanged += gameObject.SetActive;
        gameObject.SetActive(false);
    }
}
