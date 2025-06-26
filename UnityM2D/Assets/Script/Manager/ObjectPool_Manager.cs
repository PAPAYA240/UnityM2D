using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class ObjectPool_Manager : MonoBehaviour
{
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    // 인스턴스를 원본 prefab 키로 매핑
    private Dictionary<GameObject, GameObject> prefabMap = new Dictionary<GameObject, GameObject>();


    public void CreatePool<T>(GameObject prefab, int count, Transform _transform = null) where T : Component
    {
        if (poolDictionary.ContainsKey(prefab))
            return;

        var objectQueue = new Queue<GameObject>();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, _transform);
            obj.GetOrAddComponent<T>();
            obj.SetActive(false);
            objectQueue.Enqueue(obj);

            // 매핑 추가
            prefabMap[obj] = prefab;
        }

        poolDictionary[prefab] = objectQueue;
        Debug.Log($"Pool for {prefab.name} created ({count})");
    }

    public GameObject GetObjectKey(GameObject prefab, Vector3 pos, Quaternion rot, Transform _transform = null)
    {
        if (!poolDictionary.TryGetValue(prefab, out var queue))
        {
            Debug.LogWarning($"Pool for {prefab.name} not found!");
            return Instantiate(prefab, pos, rot, _transform);
        }

        GameObject obj;
        if (queue.Count == 0)
        {
            obj = Instantiate(prefab, _transform);
            Debug.LogWarning($"Pool empty for {prefab.name}, instantiating extra.");
            prefabMap[obj] = prefab; // 매핑 추가
        }
        else
        {
            obj = queue.Dequeue();
        }

        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;
        obj.SetActive(false);

        if (prefabMap.TryGetValue(obj, out var prefab))
        {
            poolDictionary[prefab].Enqueue(obj);
        }
        else
        {
            Destroy(obj); 
        }
    }
}