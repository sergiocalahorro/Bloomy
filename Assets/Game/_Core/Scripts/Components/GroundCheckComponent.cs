using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains the logic for checking if a character is grounded
/// </summary>
public class GroundCheckComponent : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float checkRadius;

    [SerializeField]
    private LayerMask layerMask;

    // Components
    private Collider2D collider;

    // Start is called before the first frame update
    private void Start()
    {
        collider = GetComponent<Collider2D>();

        // In case no LayerMask was set, throw warning and apply Platforms by default
        if (layerMask == LayerMask.GetMask("Nothing"))
        {
            layerMask = LayerMask.GetMask("Platforms");
            Debug.LogWarning("No layer mask was set, applied Platforms by default");
        }
    }

    /// <summary>
    /// Check if this component's owner is grounded
    /// </summary>
    public bool IsGrounded()
    {
        // Check if the bottom of the collider is overlapping with any Gameobjects in the layer mask
        Vector2 overlapOrigin = (Vector2)collider.transform.position;
        overlapOrigin.y -= collider.bounds.extents.y;
        return Physics2D.OverlapCircle(overlapOrigin, checkRadius, layerMask);
    }
}