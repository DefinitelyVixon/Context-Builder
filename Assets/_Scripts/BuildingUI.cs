using UnityEngine;
using UnityEngine.UI;

namespace _Scripts
{
    public class BuildingUI : MonoBehaviour
    {
        public Button buildButton;
        
        void Start()
        {
            Button btn = buildButton.GetComponent<Button>();
            btn.onClick.AddListener(ExpandUI);
        }
        
        void ExpandUI()
        {
            Debug.Log("Expanded UI.");
        }
    }
}
