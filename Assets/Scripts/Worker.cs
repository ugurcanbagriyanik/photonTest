
using System;
using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class Worker : MonoBehaviourPun
{
    public Player owner;
    public Renderer teamColorRenderer;
    public PhotonView photonView;
    private Tween movementTween=null;

    private void Start()
    {
        if (!photonView.IsMine && photonView.Owner.CustomProperties.TryGetValue(Player.ColorProp, out var colorIndex))
        {
            teamColorRenderer.material.color = Player.Colors[(int)colorIndex];
        }
        
    }

    private void OnMouseDown()
    {
        if (photonView.IsMine)
        {
            owner.SelectWorker(this);
        }
    }




    public void Move(Vector3 target)
    {
        if (movementTween is { active: true })
        {
            movementTween.Kill();
            
        }
        movementTween = transform.DOMove(target, 10, false).SetSpeedBased(true);
    }


    public void OnInteract(AssetReferenceInteractionConfig interaction,Interactable interactable)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RemoveInteractedObject",RpcTarget.MasterClient,PhotonView.Get(interactable).ViewID);
            var interactionSource = interaction.GetAsset();
            PlayfabHelper.TryIncrementValue(interactionSource.storageKey,1);
            if (owner.photonView.Owner.CustomProperties.TryGetValue(interactionSource.storageKey, out var value))
            {
                owner.photonView.Owner.CustomProperties[interactionSource.storageKey] = (int)value + 1;
                Debug.Log((int)value+1);
                return;
            }
        }
    }



    [PunRPC]

    public void RemoveInteractedObject(int viewId)
    {
        PhotonNetwork.Destroy(PhotonView.Find(viewId));
    }
    
    



    public void Toggle(bool on)
    {
        if (on)
        {
            Debug.Log(this.photonView.ViewID + "  selected");
        }
        else
        {
            Debug.Log(this.photonView.ViewID + "  unselected");
        }
    }
    
    
    
    
    
    
    
}
