using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField]
    DragonController _monster;

    public DragonController GetMonster()
    {
        return _monster;
    }
}
