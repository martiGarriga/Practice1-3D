using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public float m_LifeEnemy;
    public void HitByPlayer()
    {
        gameObject.SetActive(false);   
    }
}
