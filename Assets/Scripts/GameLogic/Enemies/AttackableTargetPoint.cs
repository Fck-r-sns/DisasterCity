using UnityEngine;

public class AttackableTargetPoint : MonoBehaviour
{
    GameObject _marker;

    private void Start()
    {
        _marker = transform.GetChild(0).gameObject;
        _marker.SetActive(false);
    }

    public void SetMarkerEnabled(bool enabled)
    {
        _marker.SetActive(enabled);
    }
}
