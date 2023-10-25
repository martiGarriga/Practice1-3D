using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : MonoBehaviour
{
    // Start is called before the first frame update
    public Animation m_AnimationDoorKey;
    public AnimationClip m_DoorOpenKey;
    public int m_DistancetoPlayer = 10;
    public GameObject m_Key;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool l_HaveKey = GameController.GetGameController().m_Player.m_HaveKey;
        if (l_HaveKey)
            GameObject.Destroy(m_Key);
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_DoorPosition = transform.position;
        float l_Distance = Vector3.Distance(l_PlayerPosition, l_DoorPosition);
        if(l_Distance <= m_DistancetoPlayer && l_HaveKey)
        {
            ActivateDoorAnimationKey();
        }

    }

    void ActivateDoorAnimationKey()
    {
        m_AnimationDoorKey.CrossFade(m_DoorOpenKey.name, 0.1f);
    }
}
