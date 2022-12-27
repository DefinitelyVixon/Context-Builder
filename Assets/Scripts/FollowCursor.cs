using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 objPosition;
    private LayerMask hitMask;

    private void Awake()
    {
        hitMask = LayerMask.GetMask("Default", "Water", "TransparentFX");
        mainCamera = Camera.main;
    }

    void Update()
    {
        Ray ray = mainCamera!.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, int.MaxValue, hitMask))
        {
            objPosition = hit.point; // + new Vector3(0, 0.5f, 0);
            gameObject.transform.position = BuildingSystem.instance.SnapPositionToGrid(objPosition);
        }
    }
}