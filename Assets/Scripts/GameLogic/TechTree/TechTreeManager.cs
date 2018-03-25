using System;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeManager : MonoBehaviour
{
    private Dictionary<TechTreeNodeId, TechTreeNode> _nodes = new Dictionary<TechTreeNodeId, TechTreeNode>();
    private TechTreeNodeId? _curResearchingNodeId;

    public event Action<TechTreeNodeId> onResearchStarted;
    public event Action<TechTreeNodeId> onResearchFinished;

    public TechTreeNode GetNode(TechTreeNodeId id)
    {
        return _nodes[id];
    }

    public void StartResearching(TechTreeNodeId nodeId)
    {
        if (_curResearchingNodeId.HasValue)
            throw new Exception("Failed to start research " + nodeId + ": TechTreeManager already has research: " + _curResearchingNodeId.Value);

        TechTreeNode node = _nodes[nodeId];
        Process p = node.CreateResearchProcess();
        p.onStarted += () => OnResearchStarted(nodeId);
        p.onFinished += () => OnResearchFinished(nodeId);
        Game.processesManager.StartProcess(p);
    }

    private void OnResearchStarted(TechTreeNodeId nodeId)
    {
        _curResearchingNodeId = nodeId;
        if (onResearchStarted != null)
            onResearchStarted(nodeId);
    }

    private void OnResearchFinished(TechTreeNodeId nodeId)
    {
        _curResearchingNodeId = null;
        if (onResearchFinished != null)
            onResearchFinished(nodeId);
    }

    private void Awake()
    {
        AddNode(new TechTreeNode(TechTreeNodeId.City, "Disaster city", "The city itself", 0f));
        AddNode(new TechTreeNode(TechTreeNodeId.TankBase, "Army base", "Provides tanks", 10f));
        AddNode(new TechTreeNode(TechTreeNodeId.TankBaseUpgradeProductionTime, "Army facility", "Decreases tanks' production time", 30f));
        AddNode(new TechTreeNode(TechTreeNodeId.TankBaseUpgradeCapacity, "Army depot", "Increases max capacity of army base", 30f));
        AddNode(new TechTreeNode(TechTreeNodeId.TankBaseUpgradeDamage, "Army research centre", "Increases tanks' damage", 30f));
        AddNode(new TechTreeNode(TechTreeNodeId.AviaBase, "Avia base", "Provides aircrafts", 20f));
        AddNode(new TechTreeNode(TechTreeNodeId.AviaBaseUpgradeProductionTime, "Avia facility", "Decreases aircrafts' production time", 30f));
        AddNode(new TechTreeNode(TechTreeNodeId.AviaBaseUpgradeCapacity, "Avia depot", "Increases max capacity of avia base", 30f));
        AddNode(new TechTreeNode(TechTreeNodeId.AviaBaseUpgradeDamage, "Avia research centre", "Increases aircrafts' damage", 30f));
        AddNode(new TechTreeNode(TechTreeNodeId.ArtilleryBase, "Artillery base", "Provides artillery strike", 30f));
        AddNode(new TechTreeNode(TechTreeNodeId.ArtilleryBaseUpgradeDelayTime, "Artillery aiming centre", "Descreases delay before artillery strike", 40f));
        AddNode(new TechTreeNode(TechTreeNodeId.ArtilleryBaseUpgradeReloadTime, "Artillery logistics", "Descreases artillery strike's reload time", 50f));
        AddNode(new TechTreeNode(TechTreeNodeId.ArtilleryBaseUpgradeDamage, "Artillery ammo facility", "Increases artillery strike's damage", 40f));

        ConnectNodes(TechTreeNodeId.City, TechTreeNodeId.TankBase);
        ConnectNodes(TechTreeNodeId.TankBase, TechTreeNodeId.TankBaseUpgradeProductionTime);
        ConnectNodes(TechTreeNodeId.TankBase, TechTreeNodeId.TankBaseUpgradeCapacity);
        ConnectNodes(TechTreeNodeId.TankBase, TechTreeNodeId.TankBaseUpgradeDamage);
        ConnectNodes(TechTreeNodeId.City, TechTreeNodeId.AviaBase);
        ConnectNodes(TechTreeNodeId.AviaBase, TechTreeNodeId.AviaBaseUpgradeProductionTime);
        ConnectNodes(TechTreeNodeId.AviaBase, TechTreeNodeId.AviaBaseUpgradeCapacity);
        ConnectNodes(TechTreeNodeId.AviaBase, TechTreeNodeId.AviaBaseUpgradeDamage);
        ConnectNodes(TechTreeNodeId.City, TechTreeNodeId.ArtilleryBase);
        ConnectNodes(TechTreeNodeId.ArtilleryBase, TechTreeNodeId.ArtilleryBaseUpgradeDelayTime);
        ConnectNodes(TechTreeNodeId.ArtilleryBase, TechTreeNodeId.ArtilleryBaseUpgradeReloadTime);
        ConnectNodes(TechTreeNodeId.ArtilleryBase, TechTreeNodeId.ArtilleryBaseUpgradeDamage);

        _nodes[TechTreeNodeId.City].SetInitialState(TechTreeNodeState.Researched);
    }

    private void AddNode(TechTreeNode node)
    {
        _nodes.Add(node.id, node);
    }

    private void ConnectNodes(TechTreeNodeId parentId, TechTreeNodeId childId)
    {
        TechTreeNode parent = _nodes[parentId];
        TechTreeNode child = _nodes[childId];
        parent.AddChild(child);
        child.AddParent(parent);
    }
}
