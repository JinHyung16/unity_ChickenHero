using System;
using UnityEngine;

namespace HughGeneric
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Singleton Generic을 이용하는 Manager들은 Awiat를 선언하지 말자!
        /// </summary>
        private static T instance;
        public static T GetInstance
        {
            get
            {
                if (instance == null)
                {
                    return null;
                }
                return instance;
            }
        }
        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}
