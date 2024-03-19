using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ShotHolesPool : MonoBehaviour
{
    private static ShotHolesPool instance;
    public static ShotHolesPool Instance { get { return instance; } }
    ObjectPool<GameObject> _pool;
    int PoolSize { get; set; }
    GameObject prefab;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance==null)
        {
        instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
