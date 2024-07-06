using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineSingleton : MonoBehaviour
{
    public static CinemachineSingleton instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
