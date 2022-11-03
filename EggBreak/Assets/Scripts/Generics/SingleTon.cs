using UnityEngine;

namespace HughLibrary.Generics
{
    public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T GetInstance
        {
            get
            {
                if (instance == null)
                {
                    GameObject tObj = GameObject.Find(typeof(T).Name);
                    if (tObj == null)
                    {
                        tObj = new GameObject(typeof(T).Name);
                        instance = tObj.AddComponent<T>();
                    }
                    else
                    {
                        instance = tObj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
