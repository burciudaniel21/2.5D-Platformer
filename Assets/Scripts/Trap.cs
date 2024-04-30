using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int trapDamage = 1;
    [SerializeField] private float damageInterval = .1f; // Adjust this interval as needed

    private bool isPlayerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            StartCoroutine(DealDamageOverTime(other.GetComponent<HealthSystem>()));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private IEnumerator DealDamageOverTime(HealthSystem healthSystem)
    {
        while (isPlayerInside && !healthSystem.isDead)
        {
            healthSystem.Damage(trapDamage);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
