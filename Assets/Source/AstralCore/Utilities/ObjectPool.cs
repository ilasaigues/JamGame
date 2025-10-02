using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstralCore
{
    public class ObjectPool : MonoBehaviour
    {
        private readonly Queue<GameObject> pool = new();
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool expandable;

        [SerializeField][Range(1, 100)] private int initialSize = 10;

        private BaseObjectPoolWrapper _poolWrapper;

        void Awake()
        {
            for (int i = 0; i < initialSize; i++)
            {
                pool.Enqueue(CreateNewInstance());
            }
        }

        private GameObject CreateNewInstance()
        {
            var obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            return obj;
        }

        public GameObject Get()
        {
            if (pool.Count == 0)
            {
                if (expandable) pool.Enqueue(CreateNewInstance());
                else return null;
            }

            var obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pool.Enqueue(obj);
        }

        public ObjectPoolWrapper<T> GetWrapper<T>() where T : MonoBehaviour
        {
            if (prefab != null && prefab.GetComponent<T>() != null)
            {
                _poolWrapper ??= new ObjectPoolWrapper<T>(this);
                return _poolWrapper as ObjectPoolWrapper<T>;
            }
            else
            {
                throw new TypeAccessException($"{typeof(T).FullName} is not a component in the provided prefab");
            }
        }


        public abstract class BaseObjectPoolWrapper
        {
            public readonly ObjectPool basePool;
            public BaseObjectPoolWrapper(ObjectPool basePool)
            {
                if (basePool == null) throw new ArgumentNullException(nameof(basePool));
                this.basePool = basePool;
            }
        }

        public class ObjectPoolWrapper<T> : BaseObjectPoolWrapper where T : MonoBehaviour
        {
            public ObjectPoolWrapper(ObjectPool basePool) : base(basePool) { }

            public T Get()
            {
                return basePool.Get().GetComponent<T>();
            }

            public void Return(T obj)
            {
                basePool.Return(obj.gameObject);
            }
        }
    }
}
