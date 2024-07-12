using System.Collections;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerTagUIController : MonoBehaviourPunCallbacks
{
    public static PlayerTagUIController Instance;
    public PlayerTag playerTagPrefab;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        OnPlayerEnteredRoom(PhotonNetwork.LocalPlayer);
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {

        if (!playerTagPrefab)
        {
            playerTagPrefab = Addressables.LoadAssetAsync<GameObject>("PlayerTag").WaitForCompletion()
                .GetComponent<PlayerTag>();
        }

        StartCoroutine(AddToList());

        IEnumerator AddToList()
        {
            yield return new WaitUntil((() => newPlayer.CustomProperties.Keys.Contains(Player.ColorProp)));
            
            var pt = Instantiate(playerTagPrefab, transform);
            pt.Init(newPlayer.UserId,Player.Colors[(int)newPlayer.CustomProperties[Player.ColorProp]]);
        }

        
    }
}
