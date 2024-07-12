using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public AssetReferenceInteractionConfig interactionType;


    private void OnTriggerEnter(Collider other)
    {
        var interactor = other.GetComponent<Interactor>();
        if (interactor && interactor.availableInteractions.Exists(l=>l.AssetGUID == interactionType.AssetGUID))
        {
            interactor.OnInteract.Invoke(interactionType,this);
        }
    }
}
