﻿public class TanksProvider : UnitsProvider
{
    protected override void OnResearchFinished(TechTreeNodeId nodeId)
    {
        switch (nodeId)
        {
            case TechTreeNodeId.TankBase:
                Enable();
                break;
        }
    }
}