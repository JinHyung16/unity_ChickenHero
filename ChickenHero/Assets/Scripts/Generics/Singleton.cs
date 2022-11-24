using System;
using UnityEngine;

namespace HughGeneric
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Singleton Generic�� �̿��ϴ� Manager���� Awiat�� �������� ����!
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
