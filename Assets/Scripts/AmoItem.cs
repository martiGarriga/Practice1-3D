using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmoItem : Item
{
    public int m_AmmoCount;
    public override bool CanPick()
    {
        return GameController.GetGameController().m_Player.CanPickAmmo();
    }
    public override void Pick()
    {
        GameController.GetGameController().m_Player.AddAmmo(m_AmmoCount);
    }
    
}
