using System;
using System.CodeDom;
using UnityEngine;

namespace HughGeneric
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Singleton Generic�� �̿��ϴ� Manager���� Awiat�� �������� ����!
        /// </summary>
        private static T instance = null;
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
            //Scene�� T type�� Manager�� �ߺ��Ǿ� �����Ǵ°� ������ ������ �κ�
            var tTypeObjects = FindObjectsOfType<T>();
            if (tTypeObjects.Length == 1)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }
}
