using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;
 
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                //? 오브젝트 없다면 그냥 쓰지 말라고 상정하고 만듬
                // GameObject가 Class이름과 동일해야 함
                GameObject go = GameObject.Find(typeof(T).Name);

                // 오브젝트 없다면 그냥 쓰지 말라고 하자
                if (go != null)
                {
                    m_instance = go.GetComponent<T>();
                }

                // // 오브젝트가 없다면 GameObject를 새로 만듬
                // if (go == null)
                // {
                //     go = new GameObject(typeof(T).Name);
                //     m_instance = go.AddComponent<T>();
                // }
                // // 오브젝트가 있다면 GameObject에서 컴포넌트를 가져온다
                // else
                // {
                //     m_instance = go.GetComponent<T>();
                // }
            }
            return m_instance;
        }
    }
}