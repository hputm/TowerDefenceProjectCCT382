using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health bar component for visualizing the health of enemies and buildings
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("Health Bar References")]
    [Tooltip("Slider component for the health bar")]
    public Slider slider;
    
    [Tooltip("Image component for the health bar fill")]
    public Image fillImage;
    
    [Header("Color Settings")]
    [Tooltip("Color when health is high")]
    public Color highHealthColor = Color.green;
    
    [Tooltip("Color when health is low")]
    public Color lowHealthColor = Color.red;
    
    [Header("Visibility Settings")]
    [Tooltip("Should the health bar be visible at full health?")]
    public bool hideAtFullHealth = true;
    
    [Tooltip("Delay before hiding the health bar when at full health")]
    public float hideDelay = 1f;
    
    private float hideTimer = 0f;
    private IDamageable target;
    private Canvas canvas;
    
    void Start()
    {
        // Get components if not assigned
        if (slider == null)
            slider = GetComponent<Slider>();
            
        if (canvas == null)
            canvas = GetComponent<Canvas>();
            
        // Initially hide the health bar if needed
        if (hideAtFullHealth && canvas != null)
        {
            canvas.enabled = false;
        }
    }
    
    void Update()
    {
        // Handle visibility based on health and settings
        if (hideAtFullHealth && target != null && canvas != null)
        {
            if (target.GetCurrentHealth() >= target.GetMaxHealth())
            {
                hideTimer += Time.deltaTime;
                if (hideTimer >= hideDelay)
                {
                    canvas.enabled = false;
                }
            }
            else
            {
                canvas.enabled = true;
                hideTimer = 0f;
            }
        }
    }
    
    /// <summary>
    /// Initialize the health bar with a target
    /// </summary>
    /// <param name="target">The damageable object to track</param>
    public void Initialize(IDamageable target)
    {
        this.target = target;
        
        // Set initial values
        if (slider != null && target != null)
        {
            slider.maxValue = target.GetMaxHealth();
            slider.value = target.GetCurrentHealth();
            UpdateFillImage();
        }
        
        // Subscribe to health change events
        Enemy enemy = target as Enemy;
        if (enemy != null)
        {
            enemy.onHealthChanged += OnHealthChanged;
            enemy.onEnemyDeath += OnTargetDeath;
        }
        
        BuildingBase building = target as BuildingBase;
        if (building != null)
        {
            building.onHealthChanged += OnHealthChanged;
            building.onBuildingDestroyed += OnTargetDeath;
        }
    }
    
    /// <summary>
    /// Called when the target's health changes
    /// </summary>
    /// <param name="newHealth">New health value</param>
    private void OnHealthChanged(float newHealth)
    {
        if (slider != null)
        {
            slider.value = newHealth;
            UpdateFillImage();
        }
    }
    
    /// <summary>
    /// Called when the target dies/gets destroyed
    /// </summary>
    private void OnTargetDeath()
    {
        // Unsubscribe from events
        Enemy enemy = target as Enemy;
        if (enemy != null)
        {
            enemy.onHealthChanged -= OnHealthChanged;
            enemy.onEnemyDeath -= OnTargetDeath;
        }
        
        BuildingBase building = target as BuildingBase;
        if (building != null)
        {
            building.onHealthChanged -= OnHealthChanged;
            building.onBuildingDestroyed -= OnTargetDeath;
        }
        
        // Destroy the health bar
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Update the fill image color based on health percentage
    /// </summary>
    private void UpdateFillImage()
    {
        if (fillImage == null || target == null) return;
        
        float healthPercentage = target.GetCurrentHealth() / target.GetMaxHealth();
        fillImage.color = Color.Lerp(lowHealthColor, highHealthColor, healthPercentage);
    }
    
    /// <summary>
    /// Set the world position of the health bar
    /// </summary>
    /// <param name="position">World position</param>
    public void SetWorldPosition(Vector3 position)
    {
        transform.position = position;
    }
}