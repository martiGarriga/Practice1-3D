using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShootingGalery : MonoBehaviour
{

    bool m_Activated = false;
    bool m_PlayerNear = false;
    public GameObject m_Points;
    public GameObject m_Restart;
    public float m_MaxDistance;
    public float m_Time;
    public float m_Timer;
    public static Action OnTimeOut;

    public Text m_TimeHud;
    public GameObject m_TimerActivate;

    private void OnEnable()
    {
        PlayerController.OnRestart += StartShooting;
    }
    private void OnDisable()
    {
        PlayerController.OnRestart -= StartShooting;
    }
    void Update()
    {
        m_TimeHud.text = m_Timer.ToString();
        PlayerNear();
        
        if(m_Activated && m_PlayerNear)
        {
            m_Timer += Time.deltaTime;
            m_Restart.SetActive(false);
            if(m_Timer >= m_Time)
            {
                m_Timer = 0.0f;
                m_Activated = false;
                m_Restart.SetActive(true);
                OnTimeOut?.Invoke();
            }
        }
    }
    void PlayerNear()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_ShootingGaleryPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition,l_ShootingGaleryPosition);
        if(l_Distance<=m_MaxDistance)
        {
            m_Points.SetActive(true);
            m_TimerActivate.SetActive(true);
            m_PlayerNear = true;
            
        }
        else
        {
            m_Points.SetActive(false);
            m_TimerActivate.SetActive(false);
            m_PlayerNear = false;
            m_Timer = 0.0f;
            m_Activated = false;
            m_Restart.SetActive(true);
            OnTimeOut?.Invoke();
        }
    }
    void StartShooting()
    {
        m_Activated = true; 
    }
}
