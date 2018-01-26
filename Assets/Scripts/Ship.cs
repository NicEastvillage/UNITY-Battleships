using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class Ship : NetworkBehaviour {

    private Coord _pos;
    public Coord pos
    {
        get { return _pos; }
    }

    private int _facing;
    public int facing
    {
        get { return _facing; }
        private set
        {
            _facing = (int)Mathf.Repeat(value, 8);
        }
    }

    public float facingAngle { get { return facing * 45f; } }

    public int typeIndex { get; protected set; }
    public ShipType type { get { return GameController.instance.GetShipType(typeIndex); } }
    public int ownerIndex { get; private set; }
    private Attack[] attacks;

    public int health { get; protected set; }
    public float healthPercentage { get { return (float)health / type.health; } }
    public bool isDead { get { return health <= 0; } }

    public AnimationCurve ramCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float ramDuration = 3f;

    MeshRenderer model;
    bool isSailing = false;
    Vector3 sailVelocity = new Vector3();
    float sailVelocityAngle = 0;
    Coroutine sailingRoutine = null;

    // Use this for initialization
    void Start () {
        _pos = new Coord((int)transform.position.x, (int)transform.position.z);
        TurnInstant(AngleToFace(transform.eulerAngles.y));

        if (type == null)
        {
            // Something might be wrong
            // Ask server about initial state
            CmdUpdateState();
        }
    }

    public void Setup (int typeId, int ownerIndex, Color playerColor)
    {
        this.typeIndex = typeId;
        this.ownerIndex = ownerIndex;

        GameObject modelgo = Instantiate(type.model, transform.Find("Model").transform);
        model = GetComponentInChildren<MeshRenderer>();
        model.material.SetColor("_TeamColor", playerColor);

        health = type.health;

        attacks = new Attack[8];
        for (int i = 0; i < 8; i++)
        {
            AttackType atkType = type.attacks.Get(i);
            if (atkType != null)
            {
                attacks[i] = new Attack(atkType, i, this);
            }
        }
    }

    public void MoveForwards()
    {
        Coord newpos = CoordInFront();
        MapController.instance.GetTile(newpos).MoveShipHere(this);
    }

    public void MoveInstant(Coord pos, int facing)
    {
        MoveInstant(pos);
        TurnInstant(facing);
    }

    public void MoveInstant(Coord pos)
    {
        MapController.instance.GetTile(pos).MoveShipHere(this);

        _pos = pos;
        transform.position = pos.ToVector3Z();
    }

    public void TurnInstant(int facing)
    {
        this.facing = facing;
        transform.eulerAngles = new Vector3(0, facingAngle, 0);
    }

    public void MoveBySailing(Coord pos, int facing)
    {
        MapController.instance.GetTile(pos).MoveShipHere(this);

        _pos = pos;
        this.facing = facing;
        
        if (isSailing) StopCoroutine(sailingRoutine);
        sailingRoutine = StartCoroutine(MoveBySailingCoroutine(_pos, _facing, 1.5f));
    }

    IEnumerator MoveBySailingCoroutine(Coord to, int toAngle, float duration)
    {
        isSailing = true;
        float timePassed = 0;

        while (timePassed < duration * 3)
        {
            float ang = Mathf.SmoothDampAngle(transform.eulerAngles.y, toAngle * 45, ref sailVelocityAngle, duration, 50f, Time.deltaTime);
            transform.eulerAngles = new Vector3(0, ang, 0);
            transform.position = Vector3.SmoothDamp(transform.position, to.ToVector3Z(), ref sailVelocity, duration, 50f, Time.deltaTime);
            

            timePassed += Time.deltaTime;

            yield return null;
        }

        transform.position = to.ToVector3Z();
        transform.eulerAngles = new Vector3(0, toAngle * 45, 0);

        sailVelocity = new Vector3();
        sailVelocityAngle = 0;

        isSailing = false;
    }

    public Coord CoordInFront()
    {
        return pos + Coord.neighbours[facing];
    }

    public void TurnBy(int d)
    {
        TurnInstant(facing + d);
    }

    public static int AngleToFace(float a)
    {
        return (int)Mathf.Repeat(Mathf.Round(a / 45f), 8);
    }

    public void OnEndOfTurn()
    {
        for (int i = 0; i < 8; i++)
        {
            if (attacks[i] != null)
            {
                attacks[i].ReduceCooldown();
            }
        }
    }

    public Attack GetAttack(int index)
    {
        return attacks[index];
    }

    public bool IsDiagonal()
    {
        return facing % 2 == 1;
    }

    public void Ram()
    {
        Coord tpos = CoordInFront();
        
        // validate
        if (MapController.instance.IsWithinMap(tpos))
        {
            Ship target = MapController.instance.GetTile(tpos).ship;

            if (target == null)
            {
                Debug.LogError("Ram failed! No ship in front.");
                return;
            }
            else
            {
                int facingDiff = target.facing - facing;

                // animation
                if (isSailing) StopCoroutine(sailingRoutine);
                sailingRoutine = StartCoroutine(RamCoroutine(_pos, tpos));

                // dmg
                int dmg = type.attacks.ram.GetDamage(this, target);
                target.TakeDamage(dmg);
            }
        }

        GameController.instance.ResolveDeaths();
    }

    IEnumerator RamCoroutine(Coord startpos, Coord endpos)
    {
        float timePassed = 0;

        while (timePassed < ramDuration)
        {
            timePassed += Time.deltaTime;

            transform.position = Vector3.Lerp(startpos.ToVector3Z(), endpos.ToVector3Z(), ramCurve.Evaluate(timePassed / ramDuration));

            yield return null;
        }

        transform.position = startpos.ToVector3Z();
    }

    public event Action<int> OnTakeDamage; // arg: amount

    public void TakeDamage(int amount)
    {
        health -= amount;

        StartCoroutine(TakingDamageCoroutine());

        if (OnTakeDamage != null) OnTakeDamage(amount);

        if (health <= 0)
        {
            Debug.Log(this + " is dead!");
        }
    }

    IEnumerator TakingDamageCoroutine()
    {
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();

        yield return new WaitForSeconds(0.1f);

        mr.material.SetColor("_Color", Color.red);

        yield return new WaitForSeconds(0.05f);

        mr.material.SetColor("_Color", Color.white);

        yield return new WaitForSeconds(0.04f);

        mr.material.SetColor("_Color", Color.red);

        yield return new WaitForSeconds(0.03f);

        mr.material.SetColor("_Color", Color.white);

        yield return new WaitForSeconds(0.04f);

        mr.material.SetColor("_Color", Color.red);

        yield return new WaitForSeconds(0.03f);

        mr.material.SetColor("_Color", Color.white);
    }

    public void Sink()
    {
        MapController.instance.GetTile(pos).RemoveShip();

        GetComponentInChildren<UIShipInterface>().gameObject.SetActive(false);

        StartCoroutine(SinkCoroutine());
    }

    private IEnumerator SinkCoroutine()
    {
        float sink = 0;

        while (sink < 3f) 
        {
            sink += Time.deltaTime;

            transform.position += new Vector3(0, -Time.deltaTime * 0.5f, 0);

            yield return null;
        }

        Destroy(gameObject);
    }

    [Command]
    private void CmdUpdateState()
    {
        RpcUpdateState(typeIndex, ownerIndex, pos.x, pos.y, facing);
    }

    [ClientRpc]
    private void RpcUpdateState(int typeId, int owner, int x, int y, int f)
    {
        Setup(typeId, owner, GameController.instance.playerColors[owner]);
        MoveInstant(new Coord(x, y), f);
    }
}
