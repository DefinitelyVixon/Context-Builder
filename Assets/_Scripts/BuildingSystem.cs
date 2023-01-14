using UnityEngine;
using UnityEngine.Tilemaps;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


namespace _Scripts
{
    public class BuildingSystem : MonoBehaviour
    {
        public static BuildingSystem instance;

        public GridLayout gridLayout;
        private Grid grid;

        public GameObject buildingTextObject;
        public static int money = 100;

        [SerializeField] private Tilemap mainTileMap;
        [SerializeField] private TileBase buildingTile;

        public GameObject residentialPrefab;
        public GameObject commercialPrefab;
        public GameObject industrialPrefab;

        public static PlaceableObject objectToPlace;
        public static bool isRaycastBlocked;

        void Awake()
        {
            instance = this;
            grid = gridLayout.gameObject.GetComponent<Grid>();
        }

        void Update()
        {
            if (objectToPlace)
            {
                InBuildMode();
            }
        }

        public void EnterBuildMode(int buildingType)
        {
            GameObject selectedBuildingPrefab;

            if (buildingType == 0)
            {
                selectedBuildingPrefab = residentialPrefab;
            }
            else if (buildingType == 1)
            {
                selectedBuildingPrefab = commercialPrefab;
            }
            else // if (buildingType == 2)
            {
                selectedBuildingPrefab = industrialPrefab;
            }

            int selectedBuildingCost = Building.BuildingCosts[buildingType];

            if (money >= selectedBuildingCost)
            {
                objectToPlace = CreatePlaceableObject(selectedBuildingPrefab);
                BuildingUI.instance.EnterBuildModeUI();
            }
            else
            {
                Debug.Log("Not enough money.");
            }
        }

        public static void InBuildMode()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!isRaycastBlocked) // If not clicked on UI
                {
                    if (instance.CanBePlaced(objectToPlace)) // If building can be placed
                    {
                        objectToPlace.Place();
                        int buildingTypeIndex = (int) objectToPlace.buildingType;
                        money -= Building.BuildingCosts[buildingTypeIndex];
                        BuildingUI.instance.PreviousBuildingUI();
                    }
                    else // If building can't be placed
                    {
                        Destroy(objectToPlace.GetComponent<Building>().buildingPopup.gameObject);
                        Destroy(objectToPlace.gameObject);
                        BuildingUI.instance.PreviousBuildingUI();
                    }
                }
                else // If clicked on UI
                {
                    Destroy(objectToPlace.GetComponent<Building>().buildingPopup.gameObject);
                    Destroy(objectToPlace.gameObject);
                }

                objectToPlace = null;
            }
        }

        public static void BlockRaycast()
        {
            if (objectToPlace)
            {
                objectToPlace.GetComponent<FollowCursor>().enabled = false;
                isRaycastBlocked = true;
            }
        }

        public static void AllowRaycast()
        {
            if (objectToPlace)
            {
                objectToPlace.GetComponent<FollowCursor>().enabled = true;
                isRaycastBlocked = false;
            }
        }

        public Vector3 SnapPositionToGrid(Vector3 position)
        {
            Vector3Int cellPosition = gridLayout.WorldToCell(position);
            position = grid.GetCellCenterWorld(cellPosition);
            return position;
        }

        private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
        {
            TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];

            int counter = 0;
            foreach (var v in area.allPositionsWithin)
            {
                Vector3Int pos = new Vector3Int(v.x, v.y, 0);
                array[counter] = tilemap.GetTile(pos);
                counter++;
            }

            return array;
        }

        public PlaceableObject CreatePlaceableObject(GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            obj.AddComponent<FollowCursor>();
            return obj.GetComponent<PlaceableObject>();
        }

        private bool CanBePlaced(PlaceableObject placeableObject)
        {
            BoundsInt area = new BoundsInt
            {
                position = gridLayout.WorldToCell(objectToPlace.GetStartPosition()),
                size = placeableObject.Size
            };
            TileBase[] baseArray = GetTilesBlock(area, mainTileMap);
            foreach (var b in baseArray)
            {
                if (b == buildingTile)
                {
                    return false;
                }
            }

            return true;
        }

        public void TakeArea(Vector3Int start, Vector3Int size)
        {
            mainTileMap.BoxFill(start, buildingTile,
                start.x,
                start.y,
                start.x + size.x - 1,
                start.y + size.y - 1);
        }
    }
}