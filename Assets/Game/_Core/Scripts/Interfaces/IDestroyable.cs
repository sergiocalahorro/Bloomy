using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface implemented by every Gameobject that can be destroyed
/// </summary>
public interface IDestroyable
{
    public abstract void HandleDestruction();
}