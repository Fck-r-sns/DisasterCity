using System.Collections;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    static int IdGenerator;

    const float CrushingSpeed = 10f;
    const float ShakingMagnitude = 25f;

    public int id { get; private set; }
    public bool isDestroyed { get; private set; }

    void Start()
    {
        id = ++IdGenerator;
        Game.instance.buildingsManager.RegisterBuilding(this);
    }

    public void ReceiveAttack()
    {
        if (isDestroyed)
            return;

        DisableRigidbodies(transform);
        isDestroyed = true;
        Game.instance.buildingsManager.UnregisterBuilding(this);
        StartCoroutine(CrushingProcess());
    }

    void DisableRigidbodies(Transform transform)
    {
        var rb = transform.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
        foreach (Transform child in transform)
            DisableRigidbodies(child);
    }

    IEnumerator CrushingProcess()
    {
        float startX = transform.position.x;
        float startZ = transform.position.z;
        while (transform.position.y > -100f)
        {
            transform.position = new Vector3(
                startX + Random.Range(0f, Time.deltaTime * ShakingMagnitude),
                transform.position.y - Time.deltaTime * CrushingSpeed,
                startZ + Random.Range(0f, Time.deltaTime * ShakingMagnitude));
            yield return null;
        }
        Destroy(gameObject, 1f);
    }
}
