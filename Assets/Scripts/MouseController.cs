using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{

    public LayerMask mask;

    Camera cam;
    Coord lastCoord = new Coord(-1, -1);
    Coord isAboutToClickCoord;

    public static MouseController instance;

    void Awake()
    {
        instance = this;

        cam = Camera.main;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, mask))
        {
            Coord pos = new Coord(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));

            if (!lastCoord.IsEqualTo(pos))
            {
                if (MapController.instance.IsWithinMap(lastCoord))
                {
                    Tile leaveTile = MapController.instance.GetTile(lastCoord);
                    if (leaveTile.OnMouseLeave != null) leaveTile.OnMouseLeave();
                }
                if (MapController.instance.IsWithinMap(pos))
                {
                    Tile enterTile = MapController.instance.GetTile(pos);
                    if (enterTile.OnMouseEnter != null) enterTile.OnMouseEnter();

                    if (enterTile.ship != null)
                    {
                        UIShip.instance.SetShip(enterTile.ship);
                        UIShip.instance.Show();
                    } else
                    {
                        UIShip.instance.Hide();
                    }
                }
            }

            // clicks
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (MapController.instance.IsWithinMap(pos))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        isAboutToClickCoord = pos;
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        if (isAboutToClickCoord.IsEqualTo(pos))
                        {
                            Tile tile = MapController.instance.GetTile(pos);
                            if (tile.OnMouseClick != null) tile.OnMouseClick();
                        }
                    }
                }
            }

            lastCoord = pos;
        }
    }
}
