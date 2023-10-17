using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Puntuation : MonoBehaviour
{
    private int m_points = 0;

    public Text m_PointsText;
    private void OnEnable()
    {
        ShootingGalery.OnTimeOut += RestartPoints;
    }
    private void OnDisable()
    {
        ShootingGalery.OnTimeOut -= RestartPoints;

    }
    private void Update()
    {
        m_PointsText.text = m_points.ToString();
    }
    public void PlusPoints()
    {
         m_points=m_points + 100;
    }
    public void RestartPoints()
    {
        m_points = m_points = 0;
    }
}
