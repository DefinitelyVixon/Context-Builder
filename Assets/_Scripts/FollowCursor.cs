using UnityEngine;

namespace _Scripts
{
    public class FollowCursor : MonoBehaviour
    {
        private LayerMask hitMask;
        private Camera mainCamera;
        private Vector3 objPosition;
    
        private void Awake()
        {
            mainCamera = Camera.main;
            hitMask = LayerMask.GetMask("Default", "TransparentFX", "Water");
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
}
