using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
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
        public GameObject moneyTextObject;
        public TextMeshProUGUI moneyText;
        public int money = 100;

        [SerializeField] private Tilemap mainTileMap;
        [SerializeField] private TileBase buildingTile;

        public GameObject residentialPrefab;
        public GameObject commercialPrefab;
        public GameObject industrialPrefab;

        private PlaceableObject objectToPlace;
        private bool isPlacing;

        private void Awake()
        {
            instance = this;
            grid = gridLayout.gameObject.GetComponent<Grid>();
            moneyText = moneyTextObject.GetComponent<TextMeshProUGUI>();
            isPlacing = false;
        }

        private void Update()
        {
            moneyText.text = "Money: " + money;
            if (!isPlacing)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    objectToPlace = CreatePlaceableObject(residentialPrefab);
                }
                else if (Input.GetKeyDown(KeyCode.X))
                {
                    objectToPlace = CreatePlaceableObject(commercialPrefab);
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    objectToPlace = CreatePlaceableObject(industrialPrefab);
                }
            }

            if (!objectToPlace)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (CanBePlaced(objectToPlace))
                {
                    objectToPlace.Place();
                    Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                    TakeArea(start, objectToPlace.Size);
                    objectToPlace.AssignBuildingScript();
                }
                else
                {
                    Destroy(objectToPlace.gameObject);
                }

                objectToPlace = null;
                isPlacing = false;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                Destroy(objectToPlace.gameObject);
                objectToPlace = null;
                isPlacing = false;
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

            isPlacing = true;
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