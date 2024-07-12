using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IPointerClickHandler
{
    public static UnityEvent<Vector3> OnGroundSelected = new UnityEvent<Vector3>();
    

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 worldPosition = Vector3.zero;
        
        Plane plane = new Plane(Vector3.up, 0);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out var distance))
        {
            worldPosition = ray.GetPoint(distance);
        }
        OnGroundSelected.Invoke(worldPosition);
    }
}
