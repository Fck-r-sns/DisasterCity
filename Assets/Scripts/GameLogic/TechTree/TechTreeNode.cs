using System;
using System.Collections.Generic;

public enum TechTreeNodeId
{
    City,
    TankBase,
    TankBaseUpgradeProductionTime,
    TankBaseUpgradeCapacity,
    TankBaseUpgradeDamage,
    AviaBase,
    AviaBaseUpgradeProductionTime,
    AviaBaseUpgradeCapacity,
    AviaBaseUpgradeDamage,
    ArtilleryBase,
    ArtilleryBaseUpgradeReloadTime,
    ArtilleryBaseUpgradeDelayTime,
    ArtilleryBaseUpgradeDamage,
}

public enum TechTreeNodeState
{
    Hidden,
    Available,
    InProcess,
    Researched,
}

public class TechTreeNode
{
    public event Action<TechTreeNodeState> onStateChanged;

    public TechTreeNodeId id { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }
    public float researchTime { get; private set; }
    public TechTreeNodeState state { get; private set; }
    public List<TechTreeNode> children { get { return _children; } }

    private List<TechTreeNode> _parents = new List<TechTreeNode>();
    private List<TechTreeNode> _children = new List<TechTreeNode>();

    public TechTreeNode(TechTreeNodeId id, string name, string description, float researchTime)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.researchTime = researchTime;
    }

    public void AddParent(TechTreeNode parent)
    {
        _parents.Add(parent);
        parent.onStateChanged += parentState =>
        {
            if (parentState == TechTreeNodeState.Researched)
                TryChangeState(TechTreeNodeState.Hidden, TechTreeNodeState.Available);
        };
    }

    public void AddChild(TechTreeNode child)
    {
        _children.Add(child);
    }

    public void SetInitialState(TechTreeNodeState initialState)
    {
        TryChangeState(TechTreeNodeState.Hidden, initialState);
    }

    public Process CreateResearchProcess()
    {
        if (state != TechTreeNodeState.Available)
            throw new Exception("Can not create research process, wrong node state: " + name + ", " + state);

        Process p = new TimeProcess(name, researchTime);
        p.onStarted += () => TryChangeState(TechTreeNodeState.Available, TechTreeNodeState.InProcess);
        p.onFinished += () => TryChangeState(TechTreeNodeState.InProcess, TechTreeNodeState.Researched);
        p.onTerminated += () => TryChangeState(TechTreeNodeState.InProcess, TechTreeNodeState.Available);
        return p;
    }

    private void TryChangeState(TechTreeNodeState fromState, TechTreeNodeState toState)
    {
        if (state != fromState)
            return;

        state = toState;
        if (onStateChanged != null)
            onStateChanged(toState);
    }
}