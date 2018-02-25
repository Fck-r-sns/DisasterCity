using System.Collections;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    const float CrushingSpeed = 10f;
    const float ShakingMagnitude = 1f;

    public void ReceiveDamage()
    {
        StartCoroutine(CrushingProcess());
    }

    IEnumerator CrushingProcess()
    {
        float startX = transform.position.x;
        float startZ = transform.position.z;
        while (transform.position.y > -100f)
        {
            transform.position = new Vector3(
                startX + Random.Range(0f, ShakingMagnitude),
                transform.position.y - Time.deltaTime * CrushingSpeed,
                startZ + Random.Range(0f, ShakingMagnitude));
            yield return null;
        }
        Destroy(gameObject, 1f);
    }
}
