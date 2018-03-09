using UnityEngine;

public class UnitComponent : MonoBehaviour
{
    protected Unit _unit;

    void Start()
    {
        _unit = GetComponent<Unit>();
    }
}
