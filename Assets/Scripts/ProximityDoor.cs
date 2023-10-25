using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityDoor : MonoBehaviour
{
    public GameObject m_Checker;
    public Animation m_AnimationDoor;
    public AnimationClip m_DoorOpenClip;
    public AnimationClip m_DoorCloseClip;
    public float m_MinDistanceToDoor;
    bool m_Open = false;

    void Update()
    {
        CheckIfPlayerIsNear();
    }
    void CheckIfPlayerIsNear()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_DoorPosition = m_Checker.transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, l_DoorPosition);
        if(l_Distance < m_MinDistanceToDoor && !m_Open)
        {
            m_Open = true;
            ActivateAnimationOpen();
        }
        if(l_Distance > m_MinDistanceToDoor && m_Open)
        {
            m_Open = false;
            ActivateAnimationClose();
        }
    }
    void ActivateAnimationOpen()
    {
        m_AnimationDoor.CrossFade(m_DoorOpenClip.name, 0.1f);
    }
    void ActivateAnimationClose()
    {
        m_AnimationDoor.CrossFade(m_DoorCloseClip.name, 0.1f);
    }
}
