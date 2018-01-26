using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {

    public Coord pos;
    public Color color;
    public Color colorHovered;
    public Texture texture;
    public int facing;

    [SerializeField]
    private MeshRenderer meshRenderer;

    public void Setup (Coord pos, Color color, Color colorHovered, Texture texture, int facing)
    {
        this.pos = pos;
        this.color = color;
        this.colorHovered = colorHovered;
        this.texture = texture;
        this.facing = facing;

        transform.position = pos.ToVector3Z();
        transform.eulerAngles = new Vector3(0, facing * 45f, 0);

        meshRenderer.material.SetTexture("_MainTex", texture);
        meshRenderer.material.SetColor("_Color", color);

        if (MapController.instance.IsWithinMap(pos))
        {
            MapController.instance.GetTile(pos).OnMouseEnter += Coord_Highlight;
            MapController.instance.GetTile(pos).OnMouseLeave += Coord_Dehighlight;
        }
    }

    public void Unregister ()
    {
        if (MapController.instance.IsWithinMap(pos))
        {
            MapController.instance.GetTile(pos).OnMouseEnter -= Coord_Highlight;
            MapController.instance.GetTile(pos).OnMouseLeave -= Coord_Dehighlight;
        }
    }

    private void Coord_Highlight ()
    {
        meshRenderer.material.SetColor("_Color", colorHovered);
    }

    private void Coord_Dehighlight()
    {
        meshRenderer.material.SetColor("_Color", color);
    }
}
