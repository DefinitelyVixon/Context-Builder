using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    private bool isPlaced;

    public Vector3Int Size { get; private set; }

    public Vector3[] objVertices;

    private void GetColliderVertexPositionsLocal()
    {
        BoxCollider bc = gameObject.GetComponent<BoxCollider>();
        objVertices = new Vector3[4];
        Vector3 bcSize = bc.size;
        Vector3 bcCenter = bc.center;
        
        objVertices[0] = bcCenter + new Vector3(-bcSize.x, -bcSize.y, -bcSize.z) * 0.5f;
        objVertices[1] = bcCenter + new Vector3(bcSize.x, -bcSize.y, -bcSize.z) * 0.5f;
        objVertices[2] = bcCenter + new Vector3(bcSize.x, -bcSize.y, bcSize.z) * 0.5f;
        objVertices[3] = bcCenter + new Vector3(-bcSize.x, -bcSize.y, bcSize.z) * 0.5f;
        
        // for (int i = 0; i < 4; i++)
        // {
        //     objVertices[i] = bc.center + new Vector3(
        //         math.sign(i-1.5f) * bcSize.x, 
        //         -bcSize.y, 
        //         math.pow(-1,i) * bcSize.z) * 0.5f;
        // }
    }

    private void CalculateSizeInCells()
    {
        Vector3Int[] vertices = new Vector3Int[objVertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(objVertices[i]);
            vertices[i] = BuildingSystem.current.gridLayout.WorldToCell(worldPos);
        }

        Size = new Vector3Int(
            Math.Abs((vertices[0] - vertices[1]).x), 
            Math.Abs((vertices[0] - vertices[3]).y), 
            1);
    }

    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(objVertices[0]);
    }

    private void Start()
    {
        GetColliderVertexPositionsLocal();
        CalculateSizeInCells();
    }

    public virtual void Place()
    {
        ObjectDrag drag = gameObject.GetComponent<ObjectDrag>();
        Destroy(drag);
        isPlaced = true;
    }
}