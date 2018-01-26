using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour {

    public GameObject linePrefab;
    public float yOffset = 0.003f;

    private List<LineRenderer> lines;

	// Use this for initialization
	void Start () {

        InitGrid();
	}
	
	private void InitGrid()
    {
        MapController mp = MapController.instance;
        lines = new List<LineRenderer>();

        for (int x = 0; x <= mp.width; x++)
        {
            LineRenderer line = ((GameObject)Instantiate(linePrefab, transform)).GetComponent<LineRenderer>();
            
            line.positionCount = 2;
            line.SetPositions(new Vector3[2]
            {
                new Vector3(x - 0.5f, yOffset, -0.5f),
                new Vector3(x - 0.5f, yOffset, mp.height - 0.5f)
            });

            lines.Add(line);
        }

        for (int y = 0; y <= mp.height; y++)
        {
            LineRenderer line = ((GameObject)Instantiate(linePrefab, transform)).GetComponent<LineRenderer>();

            line.positionCount = 2;
            line.SetPositions(new Vector3[2]
            {
                new Vector3( -0.5f, yOffset, y - 0.5f),
                new Vector3( mp.width - 0.5f, yOffset, y - 0.5f)
            });

            lines.Add(line);
        }
    }
}
