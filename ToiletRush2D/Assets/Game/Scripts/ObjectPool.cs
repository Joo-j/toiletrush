using System.Collections.Generic;
using UnityEngine;

namespace toilet
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance;

        public enum ObjectType
        {
            Unknown = -1,
            Line = 0,
        }

        [SerializeField] public List<GameObject> _poolObjList;
        [SerializeField] public List<int> _maxCountList;
        [SerializeField] private Transform _poolLayer;

        private Dictionary<ObjectType, Queue<GameObject>> _pool;

        private void Awake()
        {
            Instance = this;
            _pool = new Dictionary<ObjectType, Queue<GameObject>>();
            CreatePoolList();
        }

        private void CreatePoolList()
        {
            var poolType = (ObjectType)0;
            GameObject go = null;
            for (var i = 0; i < _poolObjList.Count; ++i)
            {
                poolType = (ObjectType)i;
                _pool.Add(poolType, new Queue<GameObject>());

                for (var count = 0; count < _maxCountList[i]; ++count)
                {
                    go = CreatePoolObj(poolType);
                    go.transform.SetParent(_poolLayer);
                    go.SetActive(false);
                    go.transform.localPosition = Vector3.zero;
                    _pool[poolType].Enqueue(go);
                }
            }
        }

        private GameObject CreatePoolObj(ObjectType poolType)
        {
            var go = Instantiate(_poolObjList[(int)poolType]);

            return go;
        }

        public static T Get<T>(ObjectType type)
        {
            var curPool = Instance._pool[type];
            GameObject go = null;

            while (go == null)
            {
                if (curPool.Count == 0)
                    go = Instance.CreatePoolObj(type);
                else
                    go = curPool.Dequeue();
            }

            go.SetActive(true);
            go.transform.SetParent(null);

            return go.GetComponent<T>();
        }

        public static void Return(ObjectType type, GameObject go)
        {
            var curPool = Instance._pool;
            if (curPool.Count > (int)type)
            {
                if (curPool[type].Count < Instance._maxCountList[(int)type])
                {
                    go.SetActive(false);
                    go.transform.SetParent(Instance.transform);
                    curPool[type].Enqueue(go);
                }
                else
                {
                    Destroy(go);
                }
            }
        }
    }
}