using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public float currentHealth;
    public float maxHealth = 100f;

    private Animator anim;
    private Image healthImage;
    private void Awake() 
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        healthImage = GameObject.Find("HealthOrb").GetComponent<Image>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            TakeDamage(10);
            print(currentHealth);
        }
    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;
        healthImage.fillAmount = currentHealth / maxHealth;
        if ( currentHealth <= 0f) {
            anim.SetBool("Death", true);
        }
    }
    public void UpdateHealth() {
        healthImage.fillAmount = currentHealth / maxHealth;
    }
    
}
