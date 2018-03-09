using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    UnitsManager _unitsManager;
    [SerializeField]
    BuildingsManager _buildingsManager;
    [SerializeField]
    EnemiesManager _enemiesManager;

    static Game _instance;

    public static Game instance { get { return _instance; } }
    public UnitsManager unitsManager { get { return _unitsManager; } }
    public BuildingsManager buildingsManager { get { return _buildingsManager; } }
    public EnemiesManager enemiesManager { get { return _enemiesManager; } }

    void Start()
    {
        _instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
    }
}
