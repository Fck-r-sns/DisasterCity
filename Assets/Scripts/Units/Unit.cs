using UnityEngine;

public class Unit : MonoBehaviour
{
    static int idGenerator;

    [SerializeField]
    Movement _movement;
    [SerializeField]
    Attack _attack;
    [SerializeField]
    Defence _defence;
    [SerializeField]
    Transform _selectionFrame;

    public int id { get; private set; }
    public bool isSelected { get; private set; }
    public bool isDestroyed { get; private set; }

    public Movement movement { get { return _movement; } }
    public Attack attack { get { return _attack; } }
    public Defence defence { get { return _defence; } }

    void Start()
    {
        id = ++idGenerator;
        _defence.onDestroyed += OnDestroyed;

        Game.unitsManager.RegisterUnit(this);
    }

    void OnDestroyed()
    {
        Destroy(gameObject);
        isDestroyed = true;

        Game.unitsManager.UnregisterUnit(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        _selectionFrame.gameObject.SetActive(isSelected);
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        _selectionFrame.gameObject.SetActive(isSelected);
    }
}
