using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanksProvider : UnitsProvider
{
    protected override void OnResearchFinished(TechTreeNodeId nodeId)
    {
        switch (nodeId)
        {
            case TechTreeNodeId.TankBase:
                Enable
        }
    }
}
