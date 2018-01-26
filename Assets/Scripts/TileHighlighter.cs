using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour {

    public enum SpriteType
    {
        Square, Dot, Arrow
    }

    [System.Serializable]
    private class Presets
    {
        public Color color = Color.white;
        public Color colorHovered = Color.white;
        public SpriteType spriteType = SpriteType.Square;
    }

    public GameObject prefab;

    [Header("Sprite Types")]
    [SerializeField]
    private Texture sprSquare;
    [SerializeField]
    private Texture sprDot;
    [SerializeField]
    private Texture sprArrow;

    [SerializeField]
    private Presets[] presets;

    public static TileHighlighter instance;

    void Awake()
    {
        instance = this;
    }

    public Highlight CreateHighlight(Coord position, Color color, Color colorHovered, SpriteType sprType, int facing)
    {
        Highlight high = Instantiate(prefab, transform).GetComponent<Highlight>();

        high.Setup(position, color, colorHovered, GetSpriteFromType(sprType), facing);

        return high;
    }

    public Highlight CreateHighlight(Coord position, Color color, Color colorHovered, SpriteType sprType)
    {
        return CreateHighlight(position, color, colorHovered, sprType, 0);
    }

    public Highlight CreateHighlight(Coord position, Color color, Color colorHovered)
    {
        return CreateHighlight(position, color, colorHovered, SpriteType.Square, 0);
    }

    public Highlight CreateHighlight(Coord position, Color color)
    {
        return CreateHighlight(position, color, color);
    }

    public Highlight CreateHighlight(Coord position, int preset, int facing)
    {
        return CreateHighlight(position, presets[preset].color, presets[preset].colorHovered, presets[preset].spriteType, facing);
    }

    public Highlight CreateHighlight(Coord position, int preset)
    {
        return CreateHighlight(position, preset, 0);
    }

    public Texture GetSpriteFromType(SpriteType type)
    {
        switch (type)
        {
            case SpriteType.Square: return sprSquare;
            case SpriteType.Arrow: return sprArrow;
            case SpriteType.Dot: return sprDot;
        }

        return null;
    }

    public static void DestroyHighlight(Highlight highlight)
    {
        highlight.Unregister();
        GameObject.Destroy(highlight.gameObject);
    }
}
