using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoNextLevel : MonoBehaviour
{
    public Puntuation m_Points;
    int m_points;

    public Animation m_AnimationDoor;
    public AnimationClip m_DoorOpen;
    private void Update()
    {
        m_points = m_Points.m_points;
        ActivateAnimationDoor();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SceneManager.LoadSceneAsync("Level2Scene");
        }
    }
    public void ActivateAnimationDoor()
    {
        if (m_points == 1000)
        {
            m_AnimationDoor.CrossFade(m_DoorOpen.name, 0.1f);
        }
    }
}
