using System.Collections.Generic;
using UnityEngine;

public class UiProcesses : MonoBehaviour
{
    [SerializeField]
    Transform _widgetsList;
    [SerializeField]
    UiProcessWidget _processWidgetPrefab;

    Dictionary<int, UiProcessWidget> _widgets = new Dictionary<int, UiProcessWidget>();

    void Start()
    {
        Game.processesManager.onProcessStarted += OnProcessStarted;
        Game.processesManager.onProcessUpdated += OnProcessUpdated;
        Game.processesManager.onProcessFinished += OnProcessFinished;
        Game.processesManager.onProcessTerminated += OnProcessTerminated;
    }

    void OnProcessStarted(Process p)
    {
        UiProcessWidget widget = Instantiate(_processWidgetPrefab);
        widget.SetName(p.name);
        widget.SetProgress(p.progress);
        widget.transform.SetParent(_widgetsList);
        widget.transform.SetAsLastSibling();
        _widgets.Add(p.id, widget);
    }

    void OnProcessUpdated(Process p)
    {
        _widgets[p.id].SetProgress(p.progress);
    }

    void OnProcessFinished(Process p)
    {
        Destroy(_widgets[p.id].gameObject);
        _widgets.Remove(p.id);
    }

    void OnProcessTerminated(Process p)
    {
        Destroy(_widgets[p.id]);
        _widgets.Remove(p.id);
    }
}
