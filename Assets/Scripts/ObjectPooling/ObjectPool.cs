using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LumosLabs.Raindrops
{
	/// <summary>
	/// A C# class that creates and manages instances of the <see cref="_prefab"/>
	/// Use <see cref="Get"/> to return an instance (previous created or fresh one).
	///
	/// To retire a child, use the <see cref="Release"/> method.
	/// </summary>
    public class ObjectPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _poolContainer;
        private readonly Stack<GameObject> _unusedObjects = new Stack<GameObject>();
        private readonly List<GameObject> _inUseObjects = new List<GameObject>();

        public ObjectPool(GameObject prefab, Transform poolContainer, int initialAmount = 20)
        {
            _prefab = prefab;
            _poolContainer = poolContainer;
            
            for (var i = 0; i < initialAmount; i++)
            {
                CreateInstance(false);
            }
        }

        public void Reset()
        {
	        foreach(var obj in _unusedObjects)
	        {
		        Object.Destroy(obj);
	        }
	        foreach (var obj in _inUseObjects)
	        {
		        Object.Destroy(obj);
	        }
        }

        private GameObject CreateInstance(bool initiallyActive)
        {
            GameObject newObject = Object.Instantiate(_prefab, _poolContainer);
            newObject.SetActive(initiallyActive);
            _unusedObjects.Push(newObject);
            return newObject;
        }

        public GameObject Release(GameObject child)
        {
	        if (child == null)
	        {
		        return null;
	        }

	        if (_inUseObjects.Contains(child))
	        {
		        // Reset the game object
		        child.transform.SetParent(_poolContainer, false);
		        child.transform.localPosition = Vector3.zero;
		        child.transform.localRotation = Quaternion.identity;
		        child.transform.localScale = Vector3.one;
		        child.SetActive(false);

		        // Update pools
		        _inUseObjects.Remove(child);
		        _unusedObjects.Push(child);
	        }

	        return child;
        }
        
        public GameObject Get(bool enable = true)
        {
            GameObject newObject = null;
            if (_unusedObjects.Count > 0)
            {
	            newObject = _unusedObjects.Pop();
	            if (enable)
	            {
		            newObject.SetActive(true);
	            }
            }
            else
            {
	            newObject = CreateInstance(enable);
            }
            _inUseObjects.Add(newObject);
            return newObject;
        }
    }
}