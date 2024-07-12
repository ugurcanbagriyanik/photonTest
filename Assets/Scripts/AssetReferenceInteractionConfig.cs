using System;
using UnityEngine.AddressableAssets;

[Serializable]
public class AssetReferenceInteractionConfig : AssetReferenceT<Interaction>
{
    public AssetReferenceInteractionConfig(string guid) : base(guid)
    {
        
    }

    public static implicit operator Interaction(AssetReferenceInteractionConfig rI) => rI.GetAsset();
}