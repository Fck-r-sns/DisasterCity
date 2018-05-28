using UnityEngine;

public class TankAttack : Attack
{
    [SerializeField]
    private Transform _towerPivot;
    [SerializeField]
    private Transform _gunPivot;

    void Update()
    {
        if (Game.isPaused)
            return;

        if (_target != null)
        {
            Vector3 towerDirection = _target.transform.position - transform.position;
            towerDirection.y = 0f;
            _towerPivot.rotation = Quaternion.LookRotation(towerDirection);

            Vector3 gunDirection = transform.forward;
            float angle = -Mathf.Rad2Deg * Mathf.Atan((_target.transform.position.y - transform.position.y) / towerDirection.magnitude);
            _gunPivot.localEulerAngles = new Vector3(angle, 0f, 0f);

            if (IsLoaded() && IsTargetInLineOfSight())
                Shoot();
        }
    }
}
