using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBehaviour : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f;
    private float initialHealth;
    private float damage = 0;
    public GameObject healthBarPrefab;
    private GameObject healthBarUI;
    private Slider healthBarSlider;
    private float lerpSpeed = 0.1f;
    private float lerpTimer;
    [SerializeField] private float chipSpeed = 2f;
    [Header("Player Only Settings")]
    public Image frontHealthBar;
    public Image backHealthBar;
    // Start is called before the first frame update
    void Start()
    {
        initialHealth = health;
    }

    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetInitialHealth()
    {
        return initialHealth;
    }

    public void SetHealth(float healthAmount)
    {
        health += healthAmount;
        if (health >= initialHealth)
        {
            health = initialHealth;
        }
        UpdateHealthBar();
        if (gameObject.CompareTag("Player"))
        {
            lerpTimer = 0f;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        AIController aiController = gameObject.GetComponent<AIController>();
        if (aiController != null)
        {
            
           
        }
        if (gameObject.CompareTag("Enemy") || gameObject.CompareTag("TargetWall")) 
        {
            UpdateHealthBar();
        }

        if (health <= 0)
        {
            if (gameObject.CompareTag("Player"))
            {
                Debug.Log("You died!");
            }
            else
            {
                
                Destroy(gameObject);
            }
        }
        if (gameObject.CompareTag("Player"))
        {
            
            lerpTimer = 0;
        }
    }

    private void UpdateHealthBar()
    {
        if (!gameObject.CompareTag("Player"))
        {
           
            if (healthBarUI == null)
            {
               
                healthBarUI = Instantiate(healthBarPrefab, transform.position, Quaternion.identity) as GameObject;
                healthBarUI.transform.SetParent(transform);
                healthBarUI.transform.localPosition = new Vector3(0, 1, 0); // Set the position to be above the enemy's head
                if (gameObject.CompareTag("TargetWall"))
                {
                    healthBarUI.transform.localPosition = new Vector3(0f, 0.18f, -0.9f);
                    healthBarUI.transform.rotation = Quaternion.Euler(0, 0, 0);

                }
                healthBarSlider = healthBarUI.transform.GetChild(0).GetComponent<Slider>();
                healthBarSlider.maxValue = initialHealth;
                healthBarSlider.value = health;
                healthBarUI.GetComponent<Canvas>().worldCamera = GameObject.FindWithTag("Player").GetComponentInChildren<Camera>();
            }
            else
            {
                healthBarSlider.value = health;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

        if (gameObject.CompareTag("Player")) 
        {
            health = Mathf.Clamp(health, 0, initialHealth);
            UpdateHealthUI();
        }
        if (healthBarUI != null && gameObject.CompareTag("Enemy"))
        {
            healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, health, lerpSpeed * Time.deltaTime);
            healthBarUI.transform.LookAt(healthBarUI.transform.position + GameObject.FindWithTag("Player").transform.rotation * Vector3.forward,
            GameObject.FindWithTag("Player").transform.rotation * Vector3.up);
        }
        if (healthBarUI != null && gameObject.CompareTag("TargetWall"))
        {
            healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, health, lerpSpeed * Time.deltaTime);
            healthBarUI.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
    private void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / initialHealth;
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if(fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

}