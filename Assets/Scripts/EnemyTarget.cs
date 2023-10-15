using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public float m_LifeEnemy;
    Animation DisapearEnemy;
    private void Start()
    {
        DisapearEnemy = GetComponent<Animation>();
    }
    public void HitByPlayer()
    {
        DisapearEnemy.Play();
        gameObject.SetActive(false);
    }
}
