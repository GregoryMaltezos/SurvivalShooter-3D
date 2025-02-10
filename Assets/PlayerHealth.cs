using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Image deathPanel; // Reference to the UI panel for the death effect
    public float fadeDuration = 2f; // Duration for fading effect
    public Canvas uiCanvas; // Reference to the UI canvas
    private Color transparentColor = new Color(0f, 0f, 0f, 0f); // Transparent color
    private Color blackColor = new Color(0f, 0f, 0f, 1f); // Fully opaque black color

    private Coroutine rotationCoroutine;

    public int maxHealth = 100;
    public int currentHealth;
    public float regenRate = 5f; // Adjust this to control regeneration speed
    private bool canFire = true;
    public Slider healthBar;

    public Transform playerModel; // Reference to the player's model Transform

    private float lastHitTime;
    private bool isDead = false;

    private Rigidbody playerRigidbody; // Reference to the player's Rigidbody component

    public AudioClip hitSoundClip; // Sound when the player gets hit
    public AudioClip deathSoundClip; // Sound when the player dies

    private AudioSource hitAudioSource;
    private AudioSource deathAudioSource;

    private void Start()
    {
        currentHealth = maxHealth;
        lastHitTime = Time.time;
        playerRigidbody = GetComponent<Rigidbody>(); // Getting the Rigidbody component
        UpdateHealthBar();

        hitAudioSource = gameObject.AddComponent<AudioSource>();
        deathAudioSource = gameObject.AddComponent<AudioSource>();

        // Set up the properties for hit sound
        hitAudioSource.clip = hitSoundClip;

        // Set up the properties for death sound
        deathAudioSource.clip = deathSoundClip;
    }

    private void Update()
    {
        if (!isDead && Time.time - lastHitTime > regenRate && currentHealth < maxHealth)
        {
            currentHealth += 1; // Increase health over time
            UpdateHealthBar();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        lastHitTime = Time.time;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Play hit sound when the player gets hit
            if (hitAudioSource != null && hitSoundClip != null)
            {
                hitAudioSource.PlayOneShot(hitSoundClip);
            }
        }
    }

    void Die()
    {
        if (!isDead)
        {
            isDead = true;

            // Rotate the player model to lie down
            if (playerModel != null)
            {
                // Cancel any ongoing rotation coroutine
                if (rotationCoroutine != null)
                {
                    StopCoroutine(rotationCoroutine);
                }
                rotationCoroutine = StartCoroutine(RotatePlayerModel());
            }
            if (deathAudioSource != null && deathSoundClip != null)
            {
                deathAudioSource.PlayOneShot(deathSoundClip);
            }

            // Disable any other scripts or components affecting the player's rotation
            FirstPersonController controller = GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.enabled = false; // Disable the FirstPersonController script
            }

            // Disable firing upon death
            canFire = false;
            uiCanvas.enabled = false;
            // Start a coroutine to stop the rotation after a delay
            StartCoroutine(FadeToBlackAndReload());
        }
    }

    IEnumerator RotatePlayerModel()
    {
        Quaternion startRotation = playerModel.rotation;
        Quaternion endRotation = Quaternion.Euler(-90f, 0f, 0f);
        float rotationDuration = 1.5f; // Adjust this duration as needed
        float timer = 0f;

        while (timer < rotationDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / rotationDuration;

            // Lerp the rotation gradually
            playerModel.rotation = Quaternion.Lerp(startRotation, endRotation, normalizedTime);
            yield return null;
        }

        // Unfreeze the x rotation after the rotation completes
        Rigidbody playerRigidbody = GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;
        }

        // Start the coroutine to stop rotation after a delay
        StartCoroutine(StopRotationAfterDelay());
    }


    IEnumerator FadeToBlackAndReload()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / fadeDuration;

            // Lerp the alpha value of the panel color from transparent to black
            deathPanel.color = Color.Lerp(transparentColor, blackColor, normalizedTime);
            yield return null;
        }

        // Wait for a delay after fading to black
        yield return new WaitForSeconds(2f); // Adjust this delay as needed

        // Reload the scene with a new seed or reset the game
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }


    IEnumerator StopRotationAfterDelay()
    {
        yield return new WaitForSeconds(1.5f); // Wait for half a second

        // Stop the player's rotation
        Rigidbody playerRigidbody = GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;
        }

        // You can also disable player control or perform other necessary actions upon death
    }
    public bool IsDead()
    {
        return isDead;
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }
}
