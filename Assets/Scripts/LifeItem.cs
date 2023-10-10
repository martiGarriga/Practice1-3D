using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeItem : Item
{
    public int m_LifeCount;
    public override bool CanPick()
    {
        return GameController.GetGameController().m_Player.CanPickLife();
    }
    public override void Pick()
    {
        GameController.GetGameController().m_Player.AddLife(m_LifeCount);
        GameObject.Destroy(gameObject);   
    }
}
