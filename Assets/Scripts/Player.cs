using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Hashtable= ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class Player : MonoBehaviourPun
{
    public string id;
    public List<Worker> workers = new List<Worker>();
    public Vector3 baseCoords;
    public Worker selectedWorker = null;
    public const string ColorProp = "pc";
    public PhotonView photonView;

    public static Color[] Colors = new Color[] { Color.red, Color.blue, Color.yellow, Color.green ,Color.magenta,  };
    public static Vector3[] BasePositions = new Vector3[] { new Vector3(0,0,0),new Vector3(-25,0, 10), new Vector3(-25,0,-5), new Vector3(25,0,10), new Vector3(25,0,-5),new Vector3(10,0,10)};
    /// <summary>
    /// Color this player selected. Defaults to grey.
    /// </summary>
    public Color MyColor = Color.grey;

    public bool ColorPicked = false;



    public void SelectWorker(Worker worker=null)
    {
        if (selectedWorker)
        {
            if (worker==selectedWorker)
            {
                worker = null;
            }
            selectedWorker.Toggle(false);
            selectedWorker = null;
        }

        if (worker == null )
        {
            InputManager.OnGroundSelected.RemoveListener(OnGroundSelected);
            return;
        }
        
        
        selectedWorker = worker;
        worker.Toggle(true);
        DOTween.Sequence().AppendInterval(0.1f).AppendCallback((() =>
        {
            InputManager.OnGroundSelected.AddListener(OnGroundSelected);
        }));

    }

    private void OnGroundSelected(Vector3 target)
    {
        if (selectedWorker)
        {
            selectedWorker.Move(target);
            SelectWorker();
        }
    }

    private void Start()
    {
        if (photonView.Owner.CustomProperties.TryGetValue(ColorProp, out var property))
        {
            int picked = (int)property;
            MyColor=Colors[picked];
            ColorPicked = true;
            StartCoroutine(SetWorkersColor());
        }
    }


    /// <summary>
    /// Resets the color locally. In this class and the PhotonNetwork.player instance.
    /// </summary>
    public void Reset()
    {
        this.MyColor = Color.grey;
        ColorPicked = false;

        // colors are select per room. to reset, we have to clean the locally cached property in PhotonPlayer, too
        Hashtable colorProp = new Hashtable();
        colorProp.Add(ColorProp, null);
        PhotonNetwork.LocalPlayer.SetCustomProperties(colorProp);
    }
    
    
    
    public void SelectColor()
    {
        if (ColorPicked)
        {
            StartCoroutine(SetWorkersColor());
            return;
        }

        HashSet<int> takenColors = new HashSet<int>();

        // check which colors the OTHERS picked. we pick one of the remaining colors.
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(ColorProp, out var property))
            {
                int picked = (int)property;
                Debug.Log("Taken color index: " + picked);
                takenColors.Add(picked);
            }
            else
            {
                // a player joined earlier but didn't set a color yet. as that player has a lower ID, it should select a color before we do.
                // we will wait to avoid clashes when 2 players join soon after another. we don't want a color picked twice!
                if (player.ActorNumber < PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    Debug.Log("Can't select a color yet. This player has to pick one first: " + player);
                    return;
                }
            }
        }

        //Debug.Log("Taken colors: " + takenColors.Count);

        if (takenColors.Count == Colors.Length)
        {
            Debug.LogWarning("No color available! All picked. Colors length should match MaxPlayers of the room.");
            return;
        }

        // go through the list of available colors and check each if it's taken or not
        // pick the first color that's not taken
        for (int index = 0; index < Colors.Length; index++)
        {
            if (!takenColors.Contains(index))
            {
                Color color = Colors[index];
                this.MyColor = color;
                transform.position = BasePositions[index];

                // this stores the picked color in the server and makes it known to the others (network sync)
                Hashtable colorProp = new Hashtable();
                colorProp.Add(ColorProp, index);
                PhotonNetwork.LocalPlayer.SetCustomProperties(colorProp); // this goes to the server asap.

                Debug.Log("Selected my color: " + this.MyColor);
                ColorPicked = true;
                StartCoroutine(SetWorkersColor());
                StartCoroutine(SetWorkersPositions());
                break; // one color selected. break this loop.
            }
        }
    }

    public IEnumerator SetWorkersColor()
    {
        yield return new WaitUntil((() => ColorPicked && workers.Count > 0));

        foreach (var w in workers)
        {
            w.teamColorRenderer.material.color = MyColor;
        }
    }
    public IEnumerator SetWorkersPositions()
    {
        yield return new WaitUntil((() => ColorPicked && workers.Count > 0));

        foreach (var w in workers)
        {
            var seed = Random.onUnitSphere * 3;
            seed -= seed.y * Vector3.up;
            w.Move(transform.position + seed );
        }
    }
}
