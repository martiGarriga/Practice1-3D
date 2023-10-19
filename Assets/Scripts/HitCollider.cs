using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    const int m_HeadLifePoints=50;
    const int m_HelixLifePoints=15;
    const int m_BodyLifePoints=25;
    public enum THitColliderType
    {
        HEAD=0,
        HELIX,
        BODY,
    }
    public THitColliderType m_HitColliderType;
    public Enemy m_Enemy;

    public void Hit()
    {
        int l_LifePoints = m_HeadLifePoints;
        if(m_HitColliderType == THitColliderType.BODY)
        {
            l_LifePoints = m_BodyLifePoints;
        }
        else if(m_HitColliderType == THitColliderType.HELIX)
        {
            l_LifePoints = m_HelixLifePoints;
        }
        m_Enemy.Hit(l_LifePoints);
    }
}
