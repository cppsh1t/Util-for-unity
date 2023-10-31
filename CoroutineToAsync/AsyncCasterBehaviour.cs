using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityUtil.CoroutineToAsync
{
    public class AsyncCasterBehaviour : MonoBehaviour
    {
        internal static AsyncCasterBehaviour instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}

