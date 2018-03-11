using System.Collections.Generic;
using UnityEngine;

public class TechTree : MonoBehaviour
{
    Dictionary<TechTreeNodeId, TechTreeNode> _nodes = new Dictionary<TechTreeNodeId, TechTreeNode>();

    public TechTreeNode GetNode(TechTreeNodeId id)
    {
        return _nodes[id];
    }
}
