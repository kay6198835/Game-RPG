using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INegativeReceiver
{
    void TakeDamage(float amountDamage, Vector2 attackPosition);
    //void MakeSlow(float amountPer);
}

public interface INegativeReceiver<TCore> where TCore : ICore
{
    void InvicisibleHurtbox(bool isInvis);
}
