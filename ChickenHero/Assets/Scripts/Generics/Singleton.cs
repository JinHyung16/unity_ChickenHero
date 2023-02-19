using System;
using System.CodeDom;
using UnityEngine;

namespace HughGeneric
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Singleton Generic을 이용하는 Manager들은 Awiat를 선언하지 말자!
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
            //Scene에 T type의 Manager가 중복되어 생성되는걸 막고자 수정한 부분
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
