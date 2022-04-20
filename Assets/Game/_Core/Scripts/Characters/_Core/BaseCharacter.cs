using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains all the common logic for a character
/// </summary>
public abstract class BaseCharacter : MonoBehaviour, IDestroyable
{
    // Components
    protected CharacterMovementComponent characterMovementComponent;
    protected HealthComponent healthComponent;

    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        // Setup components
        characterMovementComponent = GetComponent<CharacterMovementComponent>();
        healthComponent = GetComponent<HealthComponent>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
      
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    // FixedUpdate is called every fixed framerate frame
    protected virtual void FixedUpdate()
    {

    }

    /// <summary>
    /// Handle character's destruction
    /// </summary>
    public virtual void HandleDestruction()
    {
        Destroy(gameObject);
    }
}