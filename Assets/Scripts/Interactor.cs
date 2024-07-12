using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Interactor : MonoBehaviour
{
    public List<AssetReferenceInteractionConfig> availableInteractions;
    public UnityEvent<AssetReferenceInteractionConfig,Interactable> OnInteract = new UnityEvent<AssetReferenceInteractionConfig,Interactable>();
}
