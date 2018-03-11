using UnityEngine;
using UnityEngine.UI;

public class UiStats : MonoBehaviour
{
    const float TimeUpdatePeriod = 1f;

    [SerializeField]
    Text _time;
    [SerializeField]
    Text _unitsCreated;
    [SerializeField]
    Text _unitsLost;
    [SerializeField]
    Text _buildingsLost;

    float _lastTimeUpdateTime = -float.MaxValue;

    void Start()
    {
        Game.unitsManager.onUnitsCreatedCountChanged += SetUnitsCreatedCount;
        Game.unitsManager.onUnitsLostCountChanged += SetUnitsLostCount;
        Game.buildingsManager.onBuildingsLostCountChanged += SetBuildingsLostCount;

        SetUnitsCreatedCount(0);
        SetUnitsLostCount(0);
        SetBuildingsLostCount(0);
    }

    void Update()
    {
        if (Time.time >= _lastTimeUpdateTime + TimeUpdatePeriod)
        {
            _lastTimeUpdateTime = Time.time;
            _time.text = "Time: " + Mathf.CeilToInt(Time.time);
        }
    }

    void SetUnitsCreatedCount(int count)
    {
        _unitsCreated.text = "Units created: " + count;
    }

    void SetUnitsLostCount(int count)
    {
        _unitsLost.text = "Units lost: " + count;
    }

    void SetBuildingsLostCount(int count)
    {
        _buildingsLost.text = "Buldings lost: " + count;
    }
}
