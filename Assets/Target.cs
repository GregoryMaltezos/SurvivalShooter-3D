using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public bool isAlive = true;
    public Animator animator;
    public float health = 50f;
    public float deathAnimationDelay = 1.0f; // Adjust this value to match the animation duration
    public float corpseDelay = 2.0f; // Adjust this value to control the corpse's duration
    public AudioSource deathSound; // Reference to the AudioSource for death sound
    public AudioClip deathSoundClip; // Death sound clip

    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        if (deathSound == null)
        {
            deathSound = gameObject.AddComponent<AudioSource>();
            deathSound.playOnAwake = false;
        }
    }


    public void TakeDamage(float amount)
    {

        if (health <= 0f)
        {
            return; // Don't apply damage if health is already zero
        }

        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.IncrementZombiesKilled();
        }
        // Play death sound
        if (deathSoundClip != null)
        {
            deathSound.PlayOneShot(deathSoundClip);
        }

        StartCoroutine(DestroyAfterDelay());

        // Access the EnemyController script and inform it that the enemy is dead
        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.isAlive = false;
        }
    
    animator.SetTrigger("Die");

        // Disable physics components to prevent movement
        rb.isKinematic = true;
        col.enabled = false;

        // Set velocity and angular velocity to zero
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;        
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(deathAnimationDelay); // Wait for the animation to complete

        // Keep the corpse around for a specified duration
        yield return new WaitForSeconds(corpseDelay);

        Destroy(gameObject);
    }
}