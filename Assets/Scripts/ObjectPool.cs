using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour, IPollable
{
    //public bool AutoExpand { get; set; }

    private readonly T _prefab;
    private readonly Transform _container;
    private List<T> _pool;

    public ObjectPool(T prefab, int count)
    {
        _prefab = prefab;
        _container = null;

        CreatePool(count);

    }
    public ObjectPool(T prefab, int count, Transform container)
    {
        _prefab = prefab;
        _container = container;

        CreatePool(count);
    }
    public void DestroyAllToPool()
    {
		foreach (var item in _pool)
		{
            item.DestroyToPool();
		}
    }

    private void CreatePool(int count)
    {
        _pool = new List<T>();

        for (int i = 0; i < count; i++)
            CreateObject();
    }

    private T CreateObject(bool isActiveByDefault = false)
    {
        var createObject = Object.Instantiate(_prefab, _container);
        createObject.gameObject.SetActive(isActiveByDefault);
        createObject.IsDestroyedToPool = !isActiveByDefault;
        if (isActiveByDefault)
            createObject.InitAsNew();
        _pool.Add(createObject);
        return createObject;

    }

    private bool HasFreeElement(out T element)
    {
		for (int i = 0; i < _pool.Count; i++)
		{
            var item = _pool[i];
            if (item == null)
			{
                item = CreateObject();
                _pool[i] = item;
            }
            if (item.IsDestroyedToPool)
            {
                item.IsDestroyedToPool = false;
                element = item;
                item.gameObject.SetActive(true);
                item.InitAsNew();
                return true;
            }
        }
        element = null;
        return false;
    }
    public T GetFreeElement()
    {
        if (HasFreeElement(out var element))
            return element;
		return CreateObject(true);
        //if (AutoExpand)
        //{
        //    return CreateObject(true);
        //}

        //throw new System.Exception($"There is not elements in pool of type { typeof(T)}");
    }
    public T GetFreeElement(Vector3 position)
	{
        var el = GetFreeElement();
        el.transform.position = position;
        return el;
    }
    public T GetFreeElement(Vector3 position, Quaternion rotation)
    {
        var el = GetFreeElement(position);
        el.transform.rotation = rotation;
        return el;
    }
}

public static class ObjectPoolUtils
{
    public static void DestroyToPool<T>(this T obj) where T : MonoBehaviour, IPollable
    {
        obj.IsDestroyedToPool = true;
        obj.gameObject.SetActive(false);
        obj.StopAllCoroutines();
    }
}

public interface IPollable
{
    public bool IsDestroyedToPool { get; set; }
    public void InitAsNew();
}
