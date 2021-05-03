using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [HideInInspector] public float currentHealth;
    Animator anim;
    public float maxHealth = 100f;
    [SerializeField] private Image EnemyHealthBar;
    [SerializeField] private SphereCollider targetCollider;
    private void Awake() {
        currentHealth = maxHealth;
        targetCollider =GetComponentInChildren<SphereCollider>();
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damageAmount) {
        
        
        currentHealth -= damageAmount;
        EnemyHealthBar.fillAmount = currentHealth / maxHealth;

        if (currentHealth > 0) {
            anim.SetTrigger("Hit");
        }
        if (currentHealth <= 0) {
            
            Canvas canvas = EnemyHealthBar.gameObject.GetComponentInParent<Canvas>(); //Enemy öldüğünde can barının yok olmasını sağladık.
            
            if(targetCollider.gameObject.activeInHierarchy) {
                targetCollider.gameObject.SetActive(false);
            }
            if (canvas.gameObject.activeInHierarchy) {
                canvas.gameObject.SetActive(false); //null referance bugg ı için
            }
        }
    }
}
