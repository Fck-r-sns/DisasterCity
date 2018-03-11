using System;
using UnityEngine;

public enum TechTreeNodeId
{
    City,
    TankBase,
    TankBaseUpgradeTime,
    TankBaseUpgradeCapacity,
    TankBaseUpgradePower,
    AviaBase,
    AviaBaseUpgradeTime,
    AviaBaseUpgradeCapacity,
    AviaBaseUpgradePower,
    ScienceCentre,
    SecretLab,
}

public class TechTreeNode
{
    public TechTreeNodeId id { get; private set; }
    public string name { get; private set; }
    public string description { get; private set; }
    public float researchTime { get; private set; }
    public bool isResearched { get; private set; }

    TechTreeNode[] _parents;
    TechTreeNode[] _children;

    public TechTreeNode(TechTreeNodeId id, string name, string description, float researchTime)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.researchTime = researchTime;
    }

    public void SetParents(TechTreeNode[] parents)
    {
        _parents = parents ?? new TechTreeNode[0];
    }

    public void SetChildren(TechTreeNode[] children)
    {
        _children = children ?? new TechTreeNode[0];
    }

    public bool isAvailable()
    {
        bool isAvailable = true;
        foreach (var parent in _parents)
            isAvailable |= parent.isResearched;
        return isAvailable;
    }

    public Process StartResearching()
    {
        Process p = new TimeProcess(name, researchTime);
        p.onFinished += () => isResearched = true;
        Game.processesManager.StartProcess(p);
        return p;
    }
}