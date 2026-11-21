using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INegativeReceiver
{
    void TakeDamage(float amountDamage,Vector2 attackPosition);
    void InvicisibleHurtbox(bool isInvis);
    //void MakeSlow(float amountPer);
    
}
