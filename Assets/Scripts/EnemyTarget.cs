using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTarget : MonoBehaviour
{
    public float m_LifeEnemy;

    private void OnEnable()
    {
        PlayerController.OnRestart += RestartPractice;
        ShootingGalery.OnTimeOut += DefusePractice;
    }
    
    public void DefusePractice()
    {
        gameObject.SetActive(false);   
    }
    public void RestartPractice()
    {
        gameObject.SetActive(true);
    }
}
