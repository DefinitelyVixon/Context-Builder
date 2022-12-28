using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts
{
    public class PlaceableObject : MonoBehaviour
    {
        [SerializeField] public BuildingType buildingType;
        
        public Vector3[] localVertexPositions;
        public Vector3[] globalVertexPositions;
        public Vector3Int Size { get; private set; }

        private void Start()
        {
            InitializeLocalColliderVertexPositions();
            InitializeGlobalColliderVertexPositions();
            CalculateCellSize();
        }

        public virtual void Place()
        {
            Destroy(gameObject.GetComponent<FollowCursor>());
        }

        public void AssignBuildingScript()
        {
            if (buildingType == BuildingType.Residential)
            {
                gameObject.AddComponent<ResidentialBuilding>();
            }
            else if (buildingType == BuildingType.Commercial)
            {
                gameObject.AddComponent<CommercialBuilding>();
            }
            else if (buildingType == BuildingType.Industrial)
            {
                gameObject.AddComponent<IndustrialBuilding>();
            }
            else
            {
                throw new Exception("Something went wrong.");
            }
        }
        
        private void InitializeLocalColliderVertexPositions()
        {
            BoxCollider bc = gameObject.GetComponent<BoxCollider>();
            Vector3 bcSize = bc.size;
            Vector3 bcCenter = bc.center;

            localVertexPositions = new Vector3[4];
            localVertexPositions[0] = bcCenter + new Vector3(-bcSize.x, -bcSize.y, -bcSize.z) * 0.5f;
            localVertexPositions[1] = bcCenter + new Vector3(bcSize.x, -bcSize.y, -bcSize.z) * 0.5f;
            localVertexPositions[2] = bcCenter + new Vector3(bcSize.x, -bcSize.y, bcSize.z) * 0.5f;
            localVertexPositions[3] = bcCenter + new Vector3(-bcSize.x, -bcSize.y, bcSize.z) * 0.5f;
        }

        private void InitializeGlobalColliderVertexPositions()
        {
            globalVertexPositions = new Vector3[localVertexPositions.Length];

            for (int i = 0; i < localVertexPositions.Length; i++)
            {
                Vector3 worldPos = gameObject.transform.TransformPoint(localVertexPositions[i]);
                globalVertexPositions[i] = BuildingSystem.instance.gridLayout.WorldToCell(worldPos);
            }
        }

        private void CalculateCellSize()
        {
            Vector3Int[] intVector = new Vector3Int[globalVertexPositions.Length];
            for (int i = 0; i < globalVertexPositions.Length; i++)
            {
                intVector[i] = Vector3Int.FloorToInt(globalVertexPositions[i]);
            }

            Size = new Vector3Int(
                Math.Abs((intVector[0] - intVector[1]).x),
                Math.Abs((intVector[0] - intVector[3]).y),
                1);
        }

        public Vector3 GetStartPosition()
        {
            return transform.TransformPoint(localVertexPositions[0]);
        }
    }
    public enum BuildingType
    {
        Residential,
        Commercial,
        Industrial
    }
}