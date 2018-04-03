public class AircraftsProvider : UnitsProvider
{
    protected override void OnResearchFinished(TechTreeNodeId nodeId)
    {
        switch (nodeId)
        {
            case TechTreeNodeId.AviaBase:
                Enable();
                break;
        }
    }
}
