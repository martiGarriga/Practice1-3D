using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract bool CanPick();
    public virtual void Pick()
    {
        GameObject.Destroy(gameObject);
    }
}
