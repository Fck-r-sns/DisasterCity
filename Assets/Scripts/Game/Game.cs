using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    UnitsManager _unitsManager;
    [SerializeField]
    BuildingsManager _buildingsManager;
    [SerializeField]
    EnemiesManager _enemiesManager;

    UnitsManager unitsManager { get { return _unitsManager; } }
    BuildingsManager buildingsManager { get { return _buildingsManager; } }
    EnemiesManager enemiesManager { get { return _enemiesManager; } }

    void Start()
    {
        Unit
    }

    void Update()
    {

    }
}
