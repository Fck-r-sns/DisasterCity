using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    static int idGenerator;

    [SerializeField]
    Transform _selectionFrame;

    public event Action<bool> onSelectionChanged;
    public event Action onDestroyed;

    public int id { get; private set; }
    public bool isSelected { get; private set; }
    public bool isDestroyed { get; private set; }

    void Awake()
    {
        id = ++idGenerator;
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
