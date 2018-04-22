using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract void Deploy(Vector3 position, float radius, float damage);
}
