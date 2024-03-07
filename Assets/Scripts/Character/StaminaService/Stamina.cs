using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamina
{
    #region Stamina Settings
    private float maxStamina;
    private float currentStamina;
    public Stamina(float maxStamina)
    {
        MaxStamina = maxStamina;
        CurrentStamina = maxStamina;
    }
    public float CurrentStamina
    {
        get
        {
            return currentStamina;
        }
        set
        {
            if (value <= 0)
            {
                currentStamina = 0;
            }
            else if (value >= maxStamina)
            {
                currentStamina = maxStamina;
            }
            else
            {
                currentStamina = value;
            }
        }
    }

    public float MaxStamina { get => maxStamina; private set => maxStamina = value; }


    #endregion
}
