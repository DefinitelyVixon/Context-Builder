using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class Building : MonoBehaviour
    {
        public static Building selectedBuilding;
        public static bool isConnecting;

        public static readonly int[] BuildingCosts = {20, 30, 50};

        public int level;
        public BuildingType buildingType;
        public Color buildingColor;

        protected int nextUpdate;
        public Building inputConnection;
        public List<Building> outputConnections;
        public int maxConnections;

        public TextMeshPro floatingBuildingText;
        public BuildingPopupScript buildingPopup;

        protected Building()
        {
            nextUpdate = 1;
            level = 1;
            maxConnections = 0;
            inputConnection = null;
            outputConnections = new List<Building>();
        }

        public virtual void Upgrade()
        {
        }

        public virtual int GetUpgradedOutput()
        {
            return 0;
        }

        public void BaseUpgrade()
        {
            level += 1;
            UpdateBuildingText();
            buildingPopup.UpdateElementValues();
        }

        public void ConnectOutput()
        {
            if (inputConnection)
            {
                Debug.Log("This building already has a connection.");
                isConnecting = false;
                return;
            }

            BuildingType? targetBuildingType = selectedBuilding.buildingType switch
            {
                BuildingType.Industrial => BuildingType.Commercial,
                BuildingType.Commercial => BuildingType.Residential,
                BuildingType.Residential => null,
                _ => null
            };
            if (targetBuildingType != buildingType) // Invalid connection
            {
                Debug.Log("This building already has a connection.");
                isConnecting = false;
                return;
            }

            inputConnection = selectedBuilding;
            floatingBuildingText.faceColor = buildingColor;
            selectedBuilding.outputConnections.Add(this);
            selectedBuilding.floatingBuildingText.outlineColor = Color.black;
            selectedBuilding.buildingPopup.UpdateElementValues();

            GameObject connectionLine = new GameObject();

            LineRenderer lineRenderer = connectionLine.AddComponent<LineRenderer>();
            lineRenderer.startWidth = lineRenderer.endWidth = 0.3f;

            Vector3 sourceBuildingPosition = selectedBuilding.gameObject.transform.position;
            Vector3 targetBuildingPosition = gameObject.transform.position;
            lineRenderer.SetPositions(new[] {sourceBuildingPosition, targetBuildingPosition});

            lineRenderer.colorGradient = new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(selectedBuilding.buildingColor, 0.0f),
                    new GradientColorKey(buildingColor, 1.0f)
                }
            };

            Shader defaultShader = Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply");
            lineRenderer.material = new Material(defaultShader);

            isConnecting = false;
        }

        protected void SelectBuilding()
        {
            if (isConnecting)
            {
                ConnectOutput();
                return;
            }

            if (inputConnection || buildingType == BuildingType.Industrial)
            {
                if (!selectedBuilding) // Select
                {
                    buildingPopup.ShowPopupAction();
                }
                else if (selectedBuilding == this) // Deselect
                {
                    buildingPopup.HidePopupAction();
                }
                else if (selectedBuilding != this) // Switch selection
                {
                    selectedBuilding.buildingPopup.HidePopupAction();
                    buildingPopup.ShowPopupAction();
                }
                else
                {
                    throw new Exception("Unexpected error.");
                }
            }
        }

        protected void CreateBuildingText(Color textColor)
        {
            GameObject textGameObject = Instantiate(BuildingSystem.instance.buildingTextObject, gameObject.transform);
            if (buildingType == BuildingType.Industrial)
            {
                textGameObject.transform.Translate(-1, 0, 0);
            }

            floatingBuildingText = textGameObject.GetComponent<TextMeshPro>();
            floatingBuildingText.faceColor = textColor;
        }

        protected void CreateBuildingPopup()
        {
            GameObject infoPopup = Instantiate(
                BuildingUI.instance.baseBuildingPopupObject,
                GameObject.Find("GameUI").transform);
            buildingPopup = infoPopup.GetComponent<BuildingPopupScript>();
            buildingPopup.building = this;
        }

        public void UpdateBuildingText()
        {
            // floatingBuildingText.text = outputConnections.Count + "/" + maxConnections;
            floatingBuildingText.text = "Level " + level;
        }

        public int GetUpgradeCost()
        {
            return BuildingCosts[(int) buildingType] * level * 2 / 3;
        }
    }

    public class ResidentialBuilding : Building
    {
        public int stockpile;

        void Start()
        {
            buildingType = BuildingType.Residential;
            buildingColor = Color.green;
            maxConnections = level;
            CreateBuildingText(Color.grey);
            CreateBuildingPopup();
        }

        void Update()
        {
            GenerateIncome();
            floatingBuildingText.text = stockpile + "$";
        }

        void OnMouseDown()
        {
            if (stockpile >= 0)
            {
                CollectIncome();
            }

            SelectBuilding();
        }

        public override void Upgrade()
        {
            BuildingSystem.money -= GetUpgradeCost();
            maxConnections = GetUpgradedOutput();
            BaseUpgrade();
        }

        public override int GetUpgradedOutput()
        {
            return level + 1;
        }

        private void GenerateIncome()
        {
            if (Time.time >= nextUpdate && inputConnection)
            {
                nextUpdate = Mathf.FloorToInt(Time.time) + 1;
                stockpile += level;
            }
        }

        public void CollectIncome()
        {
            BuildingSystem.money += stockpile;
            stockpile = 0;
        }
    }

    public class CommercialBuilding : Building
    {
        void Start()
        {
            buildingType = BuildingType.Commercial;
            buildingColor = Color.blue;
            maxConnections = level * 2;

            CreateBuildingText(Color.grey);
            CreateBuildingPopup();
        }

        void OnMouseDown()
        {
            SelectBuilding();
        }

        public override void Upgrade()
        {
            BuildingSystem.money -= GetUpgradeCost();
            maxConnections = GetUpgradedOutput();
            BaseUpgrade();
        }

        public override int GetUpgradedOutput()
        {
            return (level + 1) * 2;
        }
    }

    public class IndustrialBuilding : Building
    {
        void Start()
        {
            buildingType = BuildingType.Industrial;
            buildingColor = Color.yellow;
            maxConnections = level + 1;

            CreateBuildingText(buildingColor);
            CreateBuildingPopup();
        }

        void OnMouseDown()
        {
            SelectBuilding();
        }

        public override void Upgrade()
        {
            BuildingSystem.money -= GetUpgradeCost();
            maxConnections = GetUpgradedOutput();
            BaseUpgrade();
        }

        public override int GetUpgradedOutput()
        {
            return level + 2;
        }
    }

    public enum BuildingType
    {
        Residential = 0,
        Commercial = 1,
        Industrial = 2
    }
}