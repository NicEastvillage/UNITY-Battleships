using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    [SerializeField]
    private int _width = 16;
    public int width { get { return _width; } }
    [SerializeField]
    private int _height = 16;
    public int height { get { return _height; } }
    [SerializeField]
    private int _startDistance = 16;
    public int startDistance { get { return _startDistance; } }

    private Tile[,] grid;
    private List<Ship> allShips;

    public static MapController instance;

    void Awake ()
    {
        instance = this;

        InitMap();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void InitMap ()
    {
        grid = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Tile(new Coord(x, y));
            }
        }

        allShips = new List<Ship>();
    }

    public Tile GetTile(Coord pos)
    {
        if (IsWithinMap(pos))
        {
            return grid[pos.x, pos.y];
        }

        return null;
    }

    public bool IsWithinMap(Coord pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public void PlaceShipsAtStartPositions(int ownerId, Ship[] ships)
    {
        if (ownerId == 0 || ownerId == 1)
        {
            int distFromBorder = (height - startDistance) / 2;
            int y = ownerId == 0 ? distFromBorder : height - 1 - distFromBorder;
            int facing = ownerId == 0 ? 0 : 4;

            for (int i = 0; i < ships.Length; i++)
            {
                Ship ship = ships[i];
                int x = (width) / 2 - ships.Length + 2 * i + 1;

                MapController.instance.GetTile(new Coord(x, y)).MoveShipHere(ship, true);
                ship.TurnInstant(facing);

                MapController.instance.RegisterShip(ship);
            }
        }
        else
        {
            Debug.LogError("3+ multiplayer start positions not implemented!");
        }
    }

    public void RegisterShip(Ship ship)
    {
        allShips.Add(ship);
    }

    public List<Ship> GetAllShips()
    {
        return new List<Ship>(allShips);
    }

    public Rect GetGridAsRect()
    {
        return new Rect(new Vector2(-0.5f, -0.5f), new Vector2(width - 0.5f, height - 0.5f));
    }

    void OnValidate()
    {
        _startDistance = Mathf.Clamp(_startDistance, 0, height);
    }
}
