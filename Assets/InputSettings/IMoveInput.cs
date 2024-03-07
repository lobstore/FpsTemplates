using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IMoveInput
{
    public UnityEvent OnJumped { get; }
    public UnityEvent<bool> IsSprinting { get; }
    UnityEvent<bool> IsCrouching { get; }
}