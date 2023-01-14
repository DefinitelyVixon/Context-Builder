using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class BuildingUI : MonoBehaviour
    {
        public static BuildingUI instance;

        public Button buildButton;
        public Button backButton;
        public Button residentialBuildingButton;
        public Button commercialBuildingButton;
        public Button industrialBuildingButton;

        private bool[][] buttonStates;
        private int buttonStatesIndex;

        public GameObject moneyTextObject;
        private TextMeshProUGUI moneyText;

        public GameObject baseBuildingPopupObject;

        void Awake()
        {
            instance = this;
            moneyText = moneyTextObject.GetComponent<TextMeshProUGUI>();
            buttonStates = new[] // buildButton backButton residentialBB commercialBB industrialBB
            {
                new[] {true, false, false, false, false}, // Initial UI (only build button active)
                new[] {false, true, true, true, true}, // Expanded UI (building buttons active) 
                new[] {false, true, false, false, false} // Build Mode UI (only back button active)
            };
            buttonStatesIndex = 0;
        }

        private void Update()
        {
            moneyText.text = "Money: " + BuildingSystem.money + "$";
        }

        private void SetButtonStates()
        {
            buildButton.gameObject.SetActive(buttonStates[buttonStatesIndex][0]);
            backButton.gameObject.SetActive(buttonStates[buttonStatesIndex][1]);
            residentialBuildingButton.gameObject.SetActive(buttonStates[buttonStatesIndex][2]);
            commercialBuildingButton.gameObject.SetActive(buttonStates[buttonStatesIndex][3]);
            industrialBuildingButton.gameObject.SetActive(buttonStates[buttonStatesIndex][4]);
        }

        public void ExpandBuildingUI()
        {
            buttonStatesIndex = 1;
            SetButtonStates();
        }

        public void EnterBuildModeUI()
        {
            buttonStatesIndex = 2;
            SetButtonStates();
        }

        public void PreviousBuildingUI()
        {
            buttonStatesIndex -= 1;
            SetButtonStates();
        }
    }
}