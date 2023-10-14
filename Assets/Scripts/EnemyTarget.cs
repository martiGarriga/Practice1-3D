using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public float m_LifeEnemy;
    bool m_RecibedDamage;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (m_RecibedDamage)
            m_LifeEnemy--;
    }
}
