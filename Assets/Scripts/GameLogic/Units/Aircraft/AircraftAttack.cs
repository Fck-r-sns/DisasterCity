using UnityEngine;

public class AircraftAttack : Attack
{
    private const bool DrawDebugRays = false;

    [SerializeField]
    private Transform _gunPivot;
    [SerializeField]
    private float _maxGunLatitude;
    [SerializeField]
    private float _maxGunLongitude;

    private void Update()
    {
        if (_target != null)
        {
            Vector3 directionToTarget = _target.transform.position - transform.position;
            _gunPivot.rotation = ClampRotationAroundXAxis(Quaternion.LookRotation(directionToTarget));

            float angleY = _gunPivot.localEulerAngles.y;
            if (angleY >= 180f)
                angleY -= 360f;
            _gunPivot.localEulerAngles = new Vector3(
                _gunPivot.localEulerAngles.x,
                Mathf.Clamp(angleY, -_maxGunLongitude, _maxGunLongitude),
                _gunPivot.localEulerAngles.z);

            if (IsLoaded() && IsTargetInLineOfSight())
                Shoot();
        }

        if (DrawDebugRays)
        {
            Debug.DrawRay(_shootingPoint.transform.position, _shootingPoint.transform.forward * 500f, Color.red);
            Transform t = _gunPivot.parent;
            Debug.DrawRay(t.position, t.forward * 500f);
            int totalRays = 5;
            for (int i = 0; i < totalRays; i++)
            {
                float rate = (float)i / (totalRays - 1);
                float x = Mathf.Lerp(0, _maxGunLatitude, rate);
                float y = Mathf.Lerp(-_maxGunLongitude, _maxGunLongitude, rate);
                Debug.DrawRay(t.position, Quaternion.AngleAxis(y, t.up) * t.forward * 500f);
                Debug.DrawRay(t.position, Quaternion.AngleAxis(_maxGunLatitude, t.right) * Quaternion.AngleAxis(y, t.up) * t.forward * 500f);
                Debug.DrawRay(t.position, Quaternion.AngleAxis(x, t.right) * Quaternion.AngleAxis(-_maxGunLongitude, t.up) * t.forward * 500f);
                Debug.DrawRay(t.position, Quaternion.AngleAxis(x, t.right) * Quaternion.AngleAxis(+_maxGunLongitude, t.up) * t.forward * 500f);
            }
        }
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, 0, _maxGunLatitude);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        q.x *= q.w;
        q.y *= q.w;
        q.z *= q.w;

        return q;
    }
}
