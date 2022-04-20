using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface implemented by every Gameobject that can interact and/or be interacted with
/// </summary>
public interface IInteractable
{
    public abstract void Interact();
}