using System.Collections;
using UnityEngine;

public class CullLight : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private float cullAreaRadius = 5f;
    [SerializeField] private float activeAreaRadius = 2f;

    [Header("Light Settings")]
    public float maxIntensity = 5;

    private bool isDetectedPlayer;
    private Light pointLight;
    private GameObject player;
    private Coroutine startCullLight;

    private void Start()
    {
        pointLight = GetComponent<Light>();
        ActiveLight(false, 0);
    }

    private void ActiveLight(bool active, float intensity)
    {
        pointLight.enabled = active;
        pointLight.intensity = intensity;
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, cullAreaRadius);
        // bool newDetectedPlayer = false;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                isDetectedPlayer = true;
                player = collider.gameObject;
                break;
            }
        }

        if (isDetectedPlayer)
        {
            if (!pointLight.enabled)
            {
                ActiveLight(true, 0);
                startCullLight = StartCoroutine(UpdateCullLight());
            }
        }
        else
        {
            if (pointLight.enabled)
            {
                ActiveLight(false, 0);
            }

            if (startCullLight != null)
            {
                StopCoroutine(startCullLight);
            }
        }
    }

    private IEnumerator UpdateCullLight()
    {
        while (isDetectedPlayer && player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            // Player is inside the active area
            if (distance <= activeAreaRadius)
            {
                print("Active Area");
                pointLight.intensity = maxIntensity;
            }
            // Player is inside the cull area
            else
            {
                print("Cull Area");
                float currentDistanceToActiveArea = distance - cullAreaRadius;
                float distanceBetweenCullToActiveArea = activeAreaRadius - cullAreaRadius;
                float percentage = (currentDistanceToActiveArea / distanceBetweenCullToActiveArea) * 100f;
                percentage = Mathf.Clamp(percentage, 0f, 100f);
                pointLight.intensity = percentage;
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, cullAreaRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activeAreaRadius);
    }
}
