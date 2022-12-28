using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class Building : MonoBehaviour
    {
        protected static Building selectedBuilding;
        protected int nextUpdate;
        
        public int level;
        public Building inputConnection;
        public List<Building> outputConnections;
        public int maxConnections;
        
        public TextMeshPro buildingText;

        public Building()
        {
            nextUpdate = 1;
            level = 0;
            maxConnections = 0;
            inputConnection = null;
            outputConnections = new List<Building>();
        }

        protected void CreateBuildingText(Color textColor)
        {
            GameObject textGameObject = Instantiate(BuildingSystem.instance.buildingTextObject, gameObject.transform);
            buildingText = textGameObject.GetComponent<TextMeshPro>();
            buildingText.faceColor = textColor;
        }
        public void UpdateBuildingText()
        {
            buildingText.text = outputConnections.Count + "/" + maxConnections;
        }
    }

    public class ResidentialBuilding : Building
    {
        public int stockpile;

        void Start()
        {
            CreateBuildingText(Color.grey);
            Upgrade();
        }
        void Update()
        {
            GenerateIncome();
        }
        
        void OnMouseDown()
        {
            if (stockpile >= 0)
            {
                CollectIncome();
            }

            if (selectedBuilding is CommercialBuilding && !inputConnection)
            {
                inputConnection = selectedBuilding;
                buildingText.faceColor = Color.green;
                
                selectedBuilding.outputConnections.Add(this);
                selectedBuilding.UpdateBuildingText();
                selectedBuilding.buildingText.outlineColor = Color.black;
                selectedBuilding = null;
            }
            else if (this == selectedBuilding)
            {
                selectedBuilding = null;
                buildingText.outlineColor = Color.black;
            }
        }
        
        public void Upgrade()
        {
            level += 1;
            maxConnections = level;
            UpdateBuildingText();
        }
        public void GenerateIncome()
        {
            if (Time.time >= nextUpdate && inputConnection)
            {
                nextUpdate = Mathf.FloorToInt(Time.time) + 1;
                stockpile += level;
            }
            buildingText.text = stockpile.ToString();
        }

        public void CollectIncome()
        {
            BuildingSystem.instance.money += stockpile;
            stockpile = 0;
        }
        
    }

    public class CommercialBuilding : Building
    {
        void Start()
        {
            CreateBuildingText(Color.grey);
            Upgrade();
        }

        void OnMouseDown()
        {
            if (selectedBuilding is null && inputConnection && outputConnections.Count < maxConnections)
            {
                buildingText.outlineColor = Color.red;
                selectedBuilding = this;
                return;
            }

            if (selectedBuilding is IndustrialBuilding && !inputConnection)
            {
                inputConnection = selectedBuilding;
                buildingText.faceColor = Color.blue;
                
                selectedBuilding.outputConnections.Add(this);
                selectedBuilding.UpdateBuildingText();
                selectedBuilding.buildingText.outlineColor = Color.black;
                selectedBuilding = null;
                // isActive = true;
            }
            else if (this == selectedBuilding)
            {
                selectedBuilding = null;
                buildingText.outlineColor = Color.black;
            }
        }

        public void Upgrade()
        {
            level += 1;
            maxConnections = level * 2;
            UpdateBuildingText();
        }
    }

    public class IndustrialBuilding : Building
    {
        void Start()
        {
            CreateBuildingText(Color.yellow);
            Upgrade();
        }
        void OnMouseDown()
        {
            if (selectedBuilding is null && outputConnections.Count < maxConnections)
            {
                selectedBuilding = this;
                buildingText.outlineColor = Color.red;
            }
            else if (this == selectedBuilding)
            {
                selectedBuilding = null;
                buildingText.outlineColor = Color.black;
            }
        }
        public void Upgrade()
        {  
            level += 1;
            maxConnections = level + 1;
            UpdateBuildingText();
        }
    }
}