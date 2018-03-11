using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    ProcessesManager _processesManager;
    [SerializeField]
    UnitsManager _unitsManager;
    [SerializeField]
    BuildingsManager _buildingsManager;
    [SerializeField]
    EnemiesManager _enemiesManager;

    static Game _instance;

    public static ProcessesManager processesManager { get { return _instance._processesManager; } }
    public static UnitsManager unitsManager { get { return _instance._unitsManager; } }
    public static BuildingsManager buildingsManager { get { return _instance._buildingsManager; } }
    public static EnemiesManager enemiesManager { get { return _instance._enemiesManager; } }

    void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
    }
}
