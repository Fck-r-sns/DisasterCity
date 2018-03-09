using UnityEngine;

public class UnitComponent : MonoBehaviour
{
    protected Unit unit { get; private set; }

    void Awake()
    {
        unit = GetComponent<Unit>();
    }
}
