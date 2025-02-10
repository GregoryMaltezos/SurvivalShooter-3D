using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool isAlive = true;
    public Transform player; // Reference to the player's transform
    public float initialMovementSpeed = 3.0f; // Initial enemy movement speed
    public float maxMovementSpeed = 6.8f; // Maximum movement speed cap
    public float speedIncreaseRate = 0.05f; // Speed increase rate (5% every minute)
    private float currentMovementSpeed;
    public float attackRange = 1.5f; // Adjust the attack range
    public int damage = 10; // Adjust the damage value
    public float attackCooldown = 2.0f; // Adjust the attack cooldown

    private Animator animator;
    private bool isAttacking = false;
    private float lastAttackTime = 0;
    private Rigidbody rb; // Reference to the Rigidbody component
    private float timePassed = 0;


    public AudioSource zombieSound;
    public AudioClip zombieSoundClip;
    private bool soundPlaying = false;
    private void Start()
    {
        // Find and store a reference to the player's transform
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        StartCoroutine(PlayZombieSoundWithDelay());
    }
    private IEnumerator PlayZombieSoundWithDelay()
    {
        while (isAlive)
        {
            if (!soundPlaying)
            {
                soundPlaying = true;
                zombieSound.PlayOneShot(zombieSoundClip);
                yield return new WaitForSeconds(Random.Range(3f, 7f)); // Adjust the range as needed
                soundPlaying = false;
            }
            yield return null;
        }
    }
    private void Update()
    {
        if (isAlive && player != null)
        {
            // Calculate the direction vector from the enemy to the player
            Vector3 direction = player.position - transform.position;

            // Keep the direction's Y component at 0 to prevent vertical movement
            direction.y = 0;

            // Rotate the enemy to face the player smoothly
            if (!isAttacking)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 3.0f);
            }

            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the player is within the attack range and not currently attacking
            if (distanceToPlayer < attackRange && !isAttacking && Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
            }

            // Move the enemy in the direction of the player, but only if not attacking
            if (!isAttacking)
            {
                float speedMultiplier = 1.0f + (timePassed / 60.0f); // Increase speed over time
                currentMovementSpeed = initialMovementSpeed * speedMultiplier;

                // Cap the movement speed
                currentMovementSpeed = Mathf.Clamp(currentMovementSpeed, initialMovementSpeed, maxMovementSpeed);

                transform.Translate(direction.normalized * currentMovementSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                // If the enemy is not alive, stop its movement
                StopMoving();
            }
        }
    }

    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetBool("IsAttacking", true); // Set the "IsAttacking" parameter to true

        // Stop the enemy's movement during the attack animation
        StopMoving();

        // Implement damage dealing logic here, e.g., reduce player's health
        player.GetComponent<PlayerHealth>().TakeDamage(damage);
        Invoke("EndAttack", 0.5f); // Adjust the delay as needed
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false); // Set the "IsAttacking" parameter to false

        // Resume the enemy's movement after the attack animation ends
        ResumeMoving();
    }

    private void StopMoving()
    {
        // Stop the enemy's movement by freezing its Rigidbody velocity
        rb.velocity = Vector3.zero;
    }

 
    private void ResumeMoving()
    {
        // Resume the enemy's movement by enabling its Rigidbody velocity
        rb.velocity = Vector3.zero; // Reset the velocity to zero or set it to a specific value for movement.
    }
}
