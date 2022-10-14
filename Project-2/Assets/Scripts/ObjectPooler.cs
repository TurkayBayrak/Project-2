using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int poolSize;
    public bool shouldExpand = true;

    public bool hasParent;
    public Transform parentTransform;
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    public List<GameObject> pooledObjects;

    public List<ObjectPoolItem> itemsToPool;

    private void Awake() 
    {
        if(!instance)
            instance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach (var item in itemsToPool)
        {
            for (var i = 0; i < item.poolSize; i++)
            {
                var obj = Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(string itemTag) 
    {
        for (var i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].CompareTag(itemTag))
                return pooledObjects[i];
        }
        
        foreach (var item in itemsToPool)
        {
            if (!item.objectToPool.CompareTag(itemTag)) continue;
            if (!item.shouldExpand) continue;
            var obj = (GameObject)Instantiate(item.objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
            return obj;
        }
        return null;
    }
}
