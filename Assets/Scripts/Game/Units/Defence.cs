using UnityEngine;

public class Defence : UnitComponent
{
    [SerializeField]
    GameObject[] _explosionsPrefabs;

    public void ReceiveAttack()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;
        GameControl.instance.UnregisterUnit(this);
        GameObject explosion = Instantiate(_explosionsPrefabs[Random.Range(0, _explosionsPrefabs.Length)], transform.position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * 10f;
        Destroy(gameObject);
        Destroy(_movementPointer.gameObject);
        Destroy(explosion, 1f);
    }
}
