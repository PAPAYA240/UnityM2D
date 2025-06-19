using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Resource_Manager : MonoBehaviour
{
	public Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
	public Dictionary<string, SkeletonDataAsset> skeletons = new Dictionary<string, SkeletonDataAsset>();

    public void Init()
    {
    }

    // 자료 로드
    public T Load<T>(string path) where T : Object
    {
        if(typeof(T) == typeof(Sprite))
        {
            if (sprites.TryGetValue(path, out Sprite _sprite))
                return _sprite as T;

            Sprite spriteObj = Resources.Load<Sprite>(path);
            sprites.Add(path, spriteObj); 
            return spriteObj as T;
        }

        else if(typeof(T) == typeof(SkeletonDataAsset))
        {
            if(skeletons.TryGetValue(path, out SkeletonDataAsset _skeletonData))
                return _skeletonData as T; 

            SkeletonDataAsset skeletonObj = Resources.Load<SkeletonDataAsset>(path);  
            skeletons.Add(path, skeletonObj);
            return skeletonObj as T;
        }
        return Resources.Load<T>(path);
	}

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"{path}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Instantiate(prefab, parent);
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }


}
