using UnityEngine;

public static class Defines
{
    public enum UnitType
    {
        Tank,
        Aircraft,
    }

    public enum AbilityType
    {
        ArtilleryStrike,
    }

    public static class Layers
    {
        public static int groundMask = LayerMask.GetMask("Ground");
        public static int buildingsMask = LayerMask.GetMask("Buildings");
        public static int unitsMask = LayerMask.GetMask("Units");
        public static int monsterMask = LayerMask.GetMask("Monster");
        public static int projectileColliderMask = LayerMask.GetMask("ProjectileCollider");
        public static int projectileColliderLayer = LayerMask.NameToLayer("ProjectileCollider");
        public static int deploymentZoneMask = LayerMask.GetMask("DeploymentZone");

        public static int Combine(params int[] layersMasks)
        {
            int combinedMask = 0;
            foreach (int layerMask in layersMasks)
                combinedMask |= layerMask;
            return combinedMask;
        }
    }
}
