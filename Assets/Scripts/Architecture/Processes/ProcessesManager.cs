using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessesManager : MonoBehaviour
{
    public event Action<Process> onProcessStarted;
    public event Action<Process> onProcessUpdated;
    public event Action<Process> onProcessFinished;
    public event Action<Process> onProcessTerminated;

    Dictionary<int, Process> _activeProcesses = new Dictionary<int, Process>();
    List<Process> _newProcesses = new List<Process>();
    List<Process> _terminatedProcesses = new List<Process>();
    List<Process> _finishedProcesses = new List<Process>();

    public void StartProcess(Process p)
    {
        _newProcesses.Add(p);
    }

    public void TerminateProcess(Process p)
    {
        _terminatedProcesses.Add(p);
    }

    void Update()
    {
        foreach (var p in _newProcesses)
        {
            _activeProcesses.Add(p.id, p);
            p.Start();
            if (onProcessStarted != null)
                onProcessStarted(p);
        }
        _newProcesses.Clear();

        foreach (var p in _terminatedProcesses)
        {
            _activeProcesses.Remove(p.id);
            p.Terminate();
            if (onProcessTerminated != null)
                onProcessTerminated(p);
        }
        _terminatedProcesses.Clear();

        _finishedProcesses.Clear();

        foreach (var kv in _activeProcesses)
        {
            Process p = kv.Value;
            bool finished = p.Update(Time.deltaTime);
            if (onProcessUpdated != null)
                onProcessUpdated(p);
            if (finished)
                _finishedProcesses.Add(p);
        }

        foreach (var p in _finishedProcesses)
        {
            _activeProcesses.Remove(p.id);
            p.Finish();
            if (onProcessFinished != null)
                onProcessFinished(p);
        }
    }
}
