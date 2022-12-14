using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int poolSize;
}

public class ObjectPooler : MonoBehaviour
{
    public List<GameObject> PooledObjects { get; private set; }
    
    [SerializeField] private List<ObjectPoolItem> itemsToPool;
    
    private void Start()
    {
        PooledObjects = new List<GameObject>();
        foreach (var item in itemsToPool)
        {
            for (var i = 0; i < item.poolSize; i++)
            {
                var obj = Instantiate(item.objectToPool);
                
                obj.SetActive(false);
                PooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(string itemTag, int index) 
    {
        return PooledObjects[index];
    }
}