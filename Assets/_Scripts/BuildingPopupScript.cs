using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class BuildingPopupScript : MonoBehaviour
    {
        public Building building;
        public Image popupPanel;

        private TextMeshProUGUI buildingTypeText;
        private TextMeshProUGUI buildingLevelText;

        private TextMeshProUGUI connectionCountText;
        private Button connectButton;
        private TextMeshProUGUI connectButtonText;

        private TextMeshProUGUI upgradeCostText;
        private TextMeshProUGUI upgradeResultText;
        private Button upgradeButton;

        private Button closeButton;

        void Awake()
        {
            GetElementReferences();
            UpdateElementValues();
        }

        void Update()
        {
            if (building.buildingType == BuildingType.Residential && gameObject.activeSelf)
            {
                connectionCountText.text = "Stockpile:  " +
                                           ((ResidentialBuilding) building).stockpile + "$  +" +
                                           building.level + "$/s";
            }
        }

        void GetElementReferences()
        {
            Transform[] children = gameObject.GetComponentsInChildren<Transform>();

            popupPanel = children[0].GetComponent<Image>(); // 0: BuildingInfoPopupPanel
            buildingTypeText = children[1].GetComponent<TextMeshProUGUI>(); // 1: BuildingTypeText
            // 2: InfoPanel (Ignore)
            buildingLevelText = children[3].GetComponent<TextMeshProUGUI>(); // 3: LevelText
            // 4: ConnectPanel (Ignore)
            connectionCountText = children[5].GetComponent<TextMeshProUGUI>(); // 5: ConnectionCountText
            connectButton = children[6].GetComponent<Button>(); // 6: ConnectButton
            connectButtonText = children[7].GetComponent<TextMeshProUGUI>(); // 7: ConnectButton - Text
            // 8: Upgrade Panel (Ignore)
            upgradeCostText = children[9].GetComponent<TextMeshProUGUI>(); // 9: CostText
            upgradeResultText = children[10].GetComponent<TextMeshProUGUI>(); // 10: UpgradeResultText
            upgradeButton = children[11].GetComponent<Button>(); // 11: UpgradeButton
            // 12: UpgradeButton - Text (Ignore)
            closeButton = children[13].GetComponent<Button>(); // 13: CloseButton
            // 14: CloseButton - Text (Ignore)

            buildingTypeText.text = building.buildingType.ToString();
            popupPanel.color = building.buildingColor;

            upgradeButton.onClick.AddListener(UpgradeBuildingAction);
            closeButton.onClick.AddListener(HidePopupAction);
        }

        public void UpdateElementValues()
        {
            buildingLevelText.text = "Level: " + building.level;
            upgradeCostText.text = "Upgrade Cost:  " +
                                   building.GetUpgradeCost() + "$";
            if (building.buildingType == BuildingType.Residential)
            {
                connectionCountText.text = "Stockpile:   " +
                                           ((ResidentialBuilding) building).stockpile + "$ +" +
                                           building.level + "$/seconds";
                connectButtonText.text = "Collect";
                upgradeResultText.text = "Income: "
                                         + building.level + "$ -> "
                                         + building.GetUpgradedOutput() + "$";
                connectButton.onClick.AddListener(((ResidentialBuilding) building).CollectIncome);
            }
            else
            {
                connectionCountText.text = "Active Connections:   " +
                                           building.outputConnections.Count +
                                           " / " + building.maxConnections;
                upgradeResultText.text = "Output Limit: "
                                         + building.maxConnections + " -> "
                                         + building.GetUpgradedOutput();
                connectButton.onClick.AddListener(ConnectBuildingAction);
            }
        }

        void ConnectBuildingAction()
        {
            if (building.outputConnections.Count < building.maxConnections)
            {
                Building.isConnecting = true;
            }
        }

        void UpgradeBuildingAction()
        {
            if (BuildingSystem.money >= building.GetUpgradeCost())
            {
                building.Upgrade();
            }
        }

        public void ShowPopupAction()
        {
            gameObject.SetActive(true);
            Building.selectedBuilding = building;
            Building.selectedBuilding.floatingBuildingText.outlineColor = Color.red;
        }

        public void HidePopupAction()
        {
            gameObject.SetActive(false);
            Building.selectedBuilding.floatingBuildingText.outlineColor = Color.black;
            Building.selectedBuilding = null;
        }
    }
}