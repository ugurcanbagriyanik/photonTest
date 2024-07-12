using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTag : MonoBehaviour
{
    public Image colorTag;
    public TMP_Text username;

    

    public void Init(string user, Color color )
    {
        colorTag.color = color;
        username.text = user;
    }
}
