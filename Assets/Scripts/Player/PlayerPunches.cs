using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPunches : MonoBehaviour
{
    public float attackDamage = 40;

    private void OnTriggerEnter(Collider other) {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null) {
            enemyHealth.TakeDamage(attackDamage);
        }
    }
}
