using UnityEngine;

public class UiTechTreeNode : MonoBehaviour
{
    [SerializeField]
    TechTreeNodeId _id;
    [SerializeField]
    UiTechTreeNode[] _parents;
    [SerializeField]
    UiTechTreeNode[] _children;

    TechTreeNode _node;


}
