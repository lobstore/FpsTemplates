using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
   public Dictionary<string, object> Data;
   public Vector3 Position;
   public GameData() { 
        Data = new Dictionary<string, object>();
    Position = Vector3.zero;
    }
}
