using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnMove {

    private class RammingMove
    {
        public TurnMove turnMove;
        public Coord pos;
        public Highlight highlight;

        public RammingMove (TurnMove turnMove, Coord pos, Highlight light)
        {
            this.turnMove = turnMove;
            this.pos = pos;
            highlight = light;
        }

        public void OnMouseClick()
        {
            turnMove.Execute(this);
        }
    }

    private class PossibleMove
    {
        public TurnMove turnMove;
        public Coord destination;
        public Highlight highlight;
        public int facing;

        public PossibleMove (TurnMove turnMove, Coord pos, Highlight light, int facing)
        {
            this.turnMove = turnMove;
            this.destination = pos;
            highlight = light;
            this.facing = facing;
        }

        public void OnMouseClick()
        {
            turnMove.Execute(this);
        }
    }

    public readonly Ship ship;
    
    public bool wasExecuted { get; protected set; }
    public System.Action OnMoveComplete;

    List<PossibleMove> possibleMoves;
    RammingMove ramMove;
    Highlight posHighlight;

    public TurnMove (Ship ship)
    {
        this.ship = ship;

        possibleMoves = new List<PossibleMove>();
    }

    public void Show()
    {
        Coord pos = ship.CoordInFront();

        if (MapController.instance.IsWithinMap(pos))
        {
            if (MapController.instance.GetTile(pos).ship != null)
            {
                // ram
                posHighlight = TileHighlighter.instance.CreateHighlight(pos, 5);

                ramMove = new RammingMove(this, pos, posHighlight);

                Tile tile = MapController.instance.GetTile(pos);
                tile.OnMouseClick += ramMove.OnMouseClick;
            }
            else
            {
                // move
                posHighlight = TileHighlighter.instance.CreateHighlight(pos, 2);

                for (int d = -1; d <= 1; d++)
                {
                    int facing = (int)Mathf.Repeat(ship.facing + d, 8);
                    Coord destination = pos + Coord.neighbours[facing];

                    if (MapController.instance.IsWithinMap(destination))
                    {
                        Highlight hl = TileHighlighter.instance.CreateHighlight(destination, 3, facing);

                        PossibleMove move = new PossibleMove(this, destination, hl, facing);

                        Tile destTile = MapController.instance.GetTile(destination);
                        destTile.OnMouseClick += move.OnMouseClick;

                        possibleMoves.Add(move);
                    }
                }
            }
        }
    }

    public void Hide()
    {
        if (ramMove != null)
        {
            MapController.instance.GetTile(ramMove.pos).OnMouseClick -= ramMove.OnMouseClick;
        }

        TileHighlighter.DestroyHighlight(posHighlight);

        foreach (PossibleMove move in possibleMoves)
        {
            MapController.instance.GetTile(move.destination).OnMouseClick -= move.OnMouseClick;

            TileHighlighter.DestroyHighlight(move.highlight);
        }
    }

    private void Execute(PossibleMove move)
    {
        ship.MoveBySailing(ship.CoordInFront(), move.facing);

        Hide();

        Finish();
    }

    private void Execute(RammingMove move)
    {
        ship.Ram();

        Hide();

        Finish();
    }

    private void Finish()
    {
        wasExecuted = true;

        possibleMoves.Clear();
        ramMove = null;

        if (OnMoveComplete != null) OnMoveComplete();
    }
}
