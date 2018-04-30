using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    GameObject[] _explosionsPrefabs;

    float _speed;
    float _damage;
    float _explosionScale = 1f;

    public void SetUp(float speed, float damage)
    {
        _speed = speed;
        _damage = damage;
    }

    public void SetExplosionScale(float scale)
    {
        _explosionScale = scale;
    }

    void Awake()
    {
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        transform.Translate(0f, Time.deltaTime * _speed, 0f, Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        GameObject explosion = Instantiate(_explosionsPrefabs[Random.Range(0, _explosionsPrefabs.Length)], pos, rot);
        explosion.transform.localScale *= _explosionScale;
        Destroy(gameObject);
        Destroy(explosion, 1f);

        var damageReceiver = contact.otherCollider.GetComponent<DragonDamageReceiver>();
        if (damageReceiver != null)
            damageReceiver.ReceiveDamage(_damage);
    }
}
