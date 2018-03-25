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

    private static Game _instance;
    private bool _isPaused;
    private bool _isPausedBeforeTechTreeMode;
    private bool _isTechTreeMode;

    public static ProcessesManager processesManager { get { return _instance._processesManager; } }
    public static UnitsManager unitsManager { get { return _instance._unitsManager; } }
    public static BuildingsManager buildingsManager { get { return _instance._buildingsManager; } }
    public static EnemiesManager enemiesManager { get { return _instance._enemiesManager; } }
    public static TechTreeManager techTreeManager { get { return _instance._techTreeManager; } }

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        if (!_isTechTreeMode && Input.GetKeyDown(KeyCode.Space))
            SetPaused(!_isPaused);
        if (!_isTechTreeMode && Input.GetKeyDown(KeyCode.M))
            ShowTechTree();
        else if (_isTechTreeMode && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M)))
            HideTechTree();
    }

    private void SetPaused(bool isPaused)
    {
        _isPaused = isPaused;
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    private void ShowTechTree()
    {
        _isPausedBeforeTechTreeMode = _isPaused;
        SetPaused(true);
        _uiTechTree.gameObject.SetActive(true);
        _isTechTreeMode = true;
    }

    private void HideTechTree()
    {
        _uiTechTree.gameObject.SetActive(false);
        SetPaused(_isPausedBeforeTechTreeMode);
        _isTechTreeMode = false;
    }
}
