using System.Collections;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using LoginResult = PlayFab.ClientModels.LoginResult;
using PlayFabError = PlayFab.PlayFabError;
using Random = UnityEngine.Random;
using SystemInfo = UnityEngine.Device.SystemInfo;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public Player playerPrefab;
    public Worker workerPrefab;
    public Interactable woodPrefab;

    public int maxResource = 10;
    public string resourceProp = "resourceCount";

    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Login();
    }


    private void Login()
    {
        var request = new LoginWithCustomIDRequest()
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request,OnSuccess,OnError);
        
        void OnSuccess(LoginResult obj)
        {
            Debug.Log("Login Succesfully");
        }
        
        void OnError(PlayFabError obj)
        {
            Debug.Log("Login Error");
            Debug.Log(obj);
        }
        
    }



    public void CreatePlayerAssets()
    {
        var p= PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f,0f,0f), Quaternion.identity, 0).GetComponent<Player>();
        for (int i = 0; i < 3; i++)
        {
            var w = PhotonNetwork.Instantiate(workerPrefab.name, new Vector3(0f,0f,0f), Quaternion.identity, 0).GetComponent<Worker>();
            p.workers.Add(w);
            w.owner = p;
            
        }
        p.SelectColor();

    }




    public IEnumerator ResourceLifecycle()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(1,5));
            var customProps = PhotonNetwork.CurrentRoom.CustomProperties;
            if (maxResource> (int)customProps[resourceProp])
            {
                var randomLocation = new Vector3(Random.Range(-25,25),0,Random.Range(-10,10));
                PhotonNetwork.Instantiate(woodPrefab.name, randomLocation, Quaternion.identity);
                customProps[resourceProp]= (int)customProps[resourceProp]+1;
                PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
            }
        }
    }
}
