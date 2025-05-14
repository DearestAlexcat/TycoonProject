using UnityEngine;
using System.Collections.Generic;

namespace IdleTycoon
{
    interface IPoolObject<T> where T : Component
    {
        Pooler<T> Pooler { get; set; }
    }

    public class Pooler<T> where T : Component
    {
        protected Stack<T> m_FreeInstances = new Stack<T>();
        protected T m_Original;
        protected Transform m_parent;
        public System.Action<T> FreeCallback;

        public Pooler(T original, int initialSize, Transform parent = null)
        {
            m_Original = original;
            m_FreeInstances = new Stack<T>(initialSize);
            m_parent = parent;

            for (int i = 0; i < initialSize; ++i)
            {
                T obj = Object.Instantiate(m_Original, m_parent);
                obj.gameObject.SetActive(false);
                //obj.name = Time.realtimeSinceStartup.ToString();

                m_FreeInstances.Push(obj);
            }
        }

        private T Get()
        {
            T item = m_FreeInstances.Count > 0 ? m_FreeInstances.Pop() : Object.Instantiate(m_Original, m_parent);

            if (item is IPoolObject<T> link)
                link.Pooler = this;

            return item;
        }

        public T Get(Vector3 position)
        {
            T item = Get();
            item.gameObject.transform.position = position;

            item.gameObject.SetActive(true);

            if (item is IPoolObject<T> link)
                link.Pooler = this;

            return item;
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            T item = Get();
            item.gameObject.transform.position = position;
            item.gameObject.transform.rotation = rotation;

            item.gameObject.SetActive(true);

            if (item is IPoolObject<T> link)
                link.Pooler = this;

            return item;
        }

        public void Clear()
        {
            int count = m_FreeInstances.Count;

            for (int i = 0; i < count; i++)
            {
                Object.Destroy(m_FreeInstances.Pop().gameObject);
            }
        }

        public void Free(T obj)
        {
            obj.gameObject.SetActive(false);
            m_FreeInstances.Push(obj);
            FreeCallback?.Invoke(obj);
        }
    }
}