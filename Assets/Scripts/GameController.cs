using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using System;

public class GameController : NetworkBehaviour {

    public int shipsPerPlayer = 2;
    public GameObject shipPrefab;
    public ShipType[] shipTypes;
    public Color[] playerColors;
    public Button nextTurnButton;
    public Text nextTurnButtonText;

    private Queue<Ship> turnQueue;
    public Queue<Ship> TurnQueueCopy { get { return new Queue<Ship>(turnQueue); } }

    public Ship currentShip { get { return turnQueue.Peek(); } }
    public int currentPlayer { get { return currentShip.ownerIndex; } }

    private TurnMove expectedMove;
    private bool currentPlayerHasMoved = false;
    public bool HasCurrentPlayerMoved() { return currentPlayerHasMoved; }

    public event Action OnTurnBegin;
    public event Action OnShipMove;
    public event Action OnTurnEnd;
    
    private static GameController _instance;
    public static GameController instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameController>();
            return _instance;
        }
    }
    
    void Awake()
    {
        _instance = this;
    }

    void Start ()
    {
        turnQueue = new Queue<Ship>();

        if (isServer)
            StartGame();
    }

    private void StartGame ()
    {
        List<Ship> allShips = new List<Ship>();

        // Test game setup! 
        for (int p = 0; p < 2; p++)
        {
            Ship[] ships = new Ship[shipsPerPlayer];

            for (int s = 0; s < shipsPerPlayer; s++)
            {
                ships[s] = Instantiate(shipPrefab).GetComponent<Ship>();
                ships[s].Setup(GetRandomShipType(), p, playerColors[p]);

                // NetworkServer.SpawnWithClientAuthority(ships[s])

                allShips.Add(ships[s]);
            }

            MapController.instance.PlaceShipsAtStartPositions(p, ships);
        }

        // random order
        allShips = allShips.OrderBy((s) => { return UnityEngine.Random.value; }).ToList();

        foreach (Ship s in allShips)
        {
            turnQueue.Enqueue(s);
        }


        // start first turn
        NextTurn(true);
    }

    [ClientRpc]
    private void RpcSetupShip(GameObject shipgo, int type, int owner)
    {
        Ship ship = shipgo.GetComponent<Ship>();

        ship.Setup(shipTypes[type], owner, playerColors[owner]);
    }

    private ShipType GetRandomShipType()
    {
        return shipTypes[UnityEngine.Random.Range(0, shipTypes.Length)];
    }

    public void NextTurn(bool isFirstTurn = false)
    {
        // end turn
        if (!isFirstTurn)
        {
            currentShip.OnEndOfTurn();

            if (OnTurnEnd != null) OnTurnEnd();

            AdvanceQueue();
        }

        // begin turn
        if (OnTurnBegin != null) OnTurnBegin();

        ExpectMove();
    }

    private void AdvanceQueue ()
    {
        Ship ship = turnQueue.Dequeue();
        turnQueue.Enqueue(ship);
    }

    private void ExpectMove ()
    {
        expectedMove = new TurnMove(currentShip);
        expectedMove.OnMoveComplete += OnMoveComplete;
        expectedMove.Show();

        nextTurnButton.interactable = false;
        nextTurnButtonText.text = "Move your ship";
    }

    void OnMoveComplete()
    {
        expectedMove.OnMoveComplete -= OnMoveComplete;
        currentPlayerHasMoved = true;

        nextTurnButton.interactable = true;
        nextTurnButtonText.text = "Next";
    }

    public void ResolveDeaths()
    {
        List<Ship> deadShips = new List<Ship>();
        foreach (Ship ship in turnQueue)
        {
            if (ship.isDead)
            {
                deadShips.Add(ship);
            }
        }

        RemoveShipsFromQueue(deadShips.ToArray());
        
        foreach (Ship ship in deadShips)
        {
            ship.Sink();
        }
    }

    public void RemoveShipsFromQueue(params Ship[] ships)
    {
        turnQueue = new Queue<Ship>(turnQueue.Except(ships));
    }
}
