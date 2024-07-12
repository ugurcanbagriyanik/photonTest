using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AddressableHelper
{
    private static Dictionary<object, Object> _referenceMap = new Dictionary<object, Object>();

    public static T GetFromReference<T>(AssetReference assetReference) where T : Object
    {
        if (assetReference == null) return null;

        var loadOp = Addressables.LoadAssetAsync<T>(assetReference);
		
        if(!loadOp.IsDone)
        {
            loadOp.WaitForCompletion();
        }

        return loadOp.Result;
    }
	
    public static T GetAsset<T>(this AssetReferenceT<T> assetReference) where T : Object
    {
        if (assetReference == null)
            return null;
		
        if(_referenceMap.ContainsKey(assetReference))
        {
            return _referenceMap[assetReference] as T;
        }
		
        if(!AddressableResourceExists<T>(assetReference))
            return null;

        var loadOp = Addressables.LoadAssetAsync<T>(assetReference);
		
        if(!loadOp.IsDone)
        {
            loadOp.WaitForCompletion();
        }

        _referenceMap.Add(assetReference, loadOp.Result);

        return loadOp.Result;
    }

    public static bool Exists<T>(this AssetReference key) where T : Object
    {
        return AddressableResourceExists<T>(key);
    }
	
    private static bool AddressableResourceExists<T>(AssetReference key)
    {
        var op = Addressables.LoadResourceLocationsAsync(key);

        if (!op.IsDone) op.WaitForCompletion();

        return op.Result.Count>0;
    }
}