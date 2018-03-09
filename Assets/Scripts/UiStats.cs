using UnityEngine;
using UnityEngine.UI;

public class UiStats : MonoBehaviour
{
    const float TimeUpdatePeriod = 1f;

    [SerializeField]
    Text _time;
    [SerializeField]
    Text _tanksCreated;
    [SerializeField]
    Text _tanksLost;
    [SerializeField]
    Text _buildingsLost;

    float _lastTimeUpdateTime = -float.MaxValue;

    void Start()
    {

    }

    void Update()
    {
        if (Time.time >= _lastTimeUpdateTime + TimeUpdatePeriod)
        {
            _lastTimeUpdateTime = Time.time;
            _time.text = "Time: " + Mathf.CeilToInt(Time.time);
        }
    }
}
