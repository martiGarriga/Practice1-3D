using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingGalery : MonoBehaviour
{

    bool m_Activated = false;
    public GameObject m_Points;
    public float m_MaxDistance;
    public float m_Time;
    float m_Timer;

    void Update()
    {
        PlayerNear();
        
        if(m_Activated)
        {
            m_Timer += Time.deltaTime;
            if(m_Timer >= m_Time)
            {
                m_Timer = 0.0f;
                m_Activated = false;
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
            m_Activated = true;
        }
        else
        {
            m_Points.SetActive(false);
            m_Activated = false;
        }
    }
}
