using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health
{
    #region Health Settings
    private float maxHp;

    private float currentHp;

    public Health(float maxHealth) 
    {
        MaxHP = maxHealth;
        currentHp = MaxHP;
    }

    public float CurrentHP
    {
        get
        {
            return currentHp;
        }
        set
        {
            if (value <= 0)
            {
                currentHp = 0;
            }
            else if (value >= maxHp)
            {
                currentHp = maxHp;
            }
            else
            {
                currentHp = value;
            }
        }
    }



    public float MaxHP { get => maxHp; private set => maxHp = value; }
    #endregion
}
