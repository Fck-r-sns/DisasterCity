using System;
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
    [SerializeField]
    TechTreeManager _techTreeManager;
    [SerializeField]
    GameObject _uiTechTree;

    public event Action<bool> onSetPaused;

    private static Game _instance;
    private bool _isPaused;
    private bool _isPausedBeforeTechTreeMode;
    private bool _isTechTreeMode;

    public static Game instance { get { return _instance; } }
    public static ProcessesManager processesManager { get { return _instance._processesManager; } }
    public static UnitsManager unitsManager { get { return _instance._unitsManager; } }
    public static BuildingsManager buildingsManager { get { return _instance._buildingsManager; } }
    public static EnemiesManager enemiesManager { get { return _instance._enemiesManager; } }
    public static TechTreeManager techTreeManager { get { return _instance._techTreeManager; } }

    public static bool isPaused { get { return _instance._isPaused; } }

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SetPaused(!_isPaused);
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleTechTree();
    }

    private void SetPaused(bool isPaused)
    {
        _isPaused = isPaused;
        Time.timeScale = _isPaused ? 0f : 1f;
        if (onSetPaused != null)
            onSetPaused(_isPaused);
    }

    private void SetTechTreeVisible(bool visible)
    {
        _uiTechTree.gameObject.SetActive(visible);
        _isTechTreeMode = visible;
    }

    private void ToggleTechTree()
    {
        SetTechTreeVisible(!_isTechTreeMode);
    }
}
