using UnityEngine;

public class DragonDamageReceiver : MonoBehaviour
{
    [SerializeField]
    DragonController.BodyPart _bodyPart;

    DragonController _dragonController;

    public void Start()
    {
        Transform t = transform;
        while (_dragonController == null && t != null)
        {
            _dragonController = t.GetComponent<DragonController>();
            t = t.parent;
        }
    }

    public void ReceiveDamage(float damage)
    {
        _dragonController.ReceiveDamage(_bodyPart, damage);
    }
}
