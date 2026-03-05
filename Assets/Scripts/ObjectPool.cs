using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly Func<T> createFunc;
    private readonly Action<T> onGet;
    private readonly Action<T> onRelease;
    private readonly Action<T> onDestroyPoolObject;
    private readonly bool collectionCheck;
    private readonly int defaultCapacity;
    private readonly int maxSize;

    private readonly Queue<T> pool = new Queue<T>();
    private int activeCount = 0;

    public ObjectPool(
        Func<T> createFunc,
        Action<T> onGet = null,
        Action<T> onRelease = null,
        Action<T> onDestroyPoolObject = null,
        bool collectionCheck = true,
        int defaultCapacity = 10,
        int maxSize = 10000)
    {
        this.createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        this.onGet = onGet;
        this.onRelease = onRelease;
        this.onDestroyPoolObject = onDestroyPoolObject;
        this.collectionCheck = collectionCheck;
        this.defaultCapacity = Mathf.Max(defaultCapacity, 1);
        this.maxSize = Mathf.Max(maxSize, this.defaultCapacity);

        // 预热
        for (int i = 0; i < this.defaultCapacity; i++)
        {
            T obj = createFunc();
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        T obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = createFunc();
        }

        activeCount++;
        onGet?.Invoke(obj);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Release(T obj)
    {
        if (collectionCheck && pool.Count > 0 && pool.Contains(obj))
        {
            Debug.LogWarning("试图重复释放同一个对象到池中，已忽略");
            return;
        }

        activeCount--;

        if (pool.Count < maxSize)
        {
            onRelease?.Invoke(obj);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
        else
        {
            onDestroyPoolObject?.Invoke(obj);
            if (obj != null) UnityEngine.Object.Destroy(obj.gameObject);
        }
    }

    public void Clear()
    {
        while (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            if (obj != null)
            {
                onDestroyPoolObject?.Invoke(obj);
                UnityEngine.Object.Destroy(obj.gameObject);
            }
        }
        activeCount = 0;
    }

    public int CountInactive => pool.Count;
    public int CountActive => activeCount;
    public int CountAll => activeCount + pool.Count;
}