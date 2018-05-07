using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiTechTreeNode : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private TechTreeNodeId _id;
    [SerializeField]
    private GameObject _connectionLine;
    [SerializeField]
    private GameObject _readyIcon;
    [SerializeField]
    private GameObject _clockIcon;

    private TechTreeNode _node;
    private bool _techTreeIsBusy;

    private void Start()
    {
        _node = Game.techTreeManager.GetNode(_id);
        transform.Find("Name").GetComponent<Text>().text = _node.name;
        _node.onStateChanged += OnNodeStateChanged;
        OnNodeStateChanged(_node.state);

        Game.techTreeManager.onResearchStarted += nodeId => _techTreeIsBusy = true;
        Game.techTreeManager.onResearchFinished += nodeId => _techTreeIsBusy = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_techTreeIsBusy && _node.state == TechTreeNodeState.Available)
            Game.techTreeManager.StartResearching(_id);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Todo: show decription
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Todo: hide decription
    }

    private void OnNodeStateChanged(TechTreeNodeState state)
    {
        gameObject.SetActive(state != TechTreeNodeState.Hidden);
        if (_connectionLine != null)
            _connectionLine.SetActive(state != TechTreeNodeState.Hidden);
        if (_readyIcon != null)
            _readyIcon.SetActive(state == TechTreeNodeState.Researched);
        if (_clockIcon != null)
            _clockIcon.SetActive(state == TechTreeNodeState.InProcess);
        // Todo: visually show research states
    }
}
