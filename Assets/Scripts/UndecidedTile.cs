using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UndecidedTile : MonoBehaviour
{
    public DecidedTile Tile { get; private set; }

    public int x;
    public int y;
    public bool Collapsed { get; private set; }
    public bool? HasTop { get; private set; }
    public bool? HasLeft { get; private set; }
    public bool? HasBottom { get; private set; }
    public bool? HasRight { get; private set; }

    [SerializeField] private List<GameObject> possibleTiles = new();
    public List<GameObject> PossibleTiles => possibleTiles;


    public int Entropy { get; private set; }
    private int maxEntropy;
    private SpriteRenderer sprite;
    private TileGridController grid;

    private void Awake()
    {
        grid = FindFirstObjectByType<TileGridController>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        maxEntropy = 0;
        foreach (var tileObject in possibleTiles)
        {
            maxEntropy += tileObject.GetComponent<DecidedTile>().Commonality;
        }

        Entropy = maxEntropy;
        
        sprite.color = new Color(1f, 1f, 1f);
    }

    public void CollapseEntirely()
    {
        var range = 0;
        foreach (var tileObject in possibleTiles)
        {
            var tile = tileObject.GetComponent<DecidedTile>();
            range += tile.Commonality;
        }

        var randomIndex = Random.Range(0, range);
        range = 0;

        foreach (var tileObject in possibleTiles)
        {
            var tile = tileObject.GetComponent<DecidedTile>();
            range += tile.Commonality;

            if (randomIndex < range)
            {
                Tile = tile;
                Instantiate(tileObject, transform);
                Collapsed = true;
                break;
            }
        }

        if (!Collapsed)
        {
            throw new CollapsingException("No Tile to collapse to");
        }

        if (x > 0 && !grid.Tiles[x-1,y].Collapsed)
        {
            grid.Tiles[x-1,y].CollapseRight(Tile.HasLeft);
        }

        if (x < grid.GridSizeX-1 && !grid.Tiles[x+1, y].Collapsed)
        {
            grid.Tiles[x+1, y].CollapseLeft(Tile.HasRight);
        }

        if (y > 0 && !grid.Tiles[x, y-1].Collapsed)
        {
            grid.Tiles[x,y-1].CollapseTop(Tile.HasBottom);
        }

        if (y < grid.GridSizeY - 1 && !grid.Tiles[x, y+1].Collapsed)
        {
            grid.Tiles[x, y+1].CollapseBottom(Tile.HasTop);
        }
        Destroy(sprite);
    }

    public void CalculateEntropy()
    {
        if (Collapsed) return;

        var entropy = 0;
        foreach (var tileObject in possibleTiles)
        {
            entropy += tileObject.GetComponent<DecidedTile>().Commonality;
        }
        var entropyPercent = (float) entropy / maxEntropy;
        sprite.color = new Color(entropyPercent, entropyPercent, entropyPercent);
        Entropy = entropy;
    }

    public void CollapseTop(bool value)
    {
        if (HasTop.HasValue)
        {
            throw new CollapsingException("HasTop already collapsed");
        }
        HasTop = value;
        PruneTiles();
    }

    public void CollapseLeft(bool value)
    {
        if (HasLeft.HasValue) throw new CollapsingException("HasLeft already collapsed");
        HasLeft = value;
        PruneTiles();
    }

    public void CollapseBottom(bool value)
    {
        if (HasBottom.HasValue) throw new CollapsingException("HasBottom already collapsed");
        HasBottom = value;
        PruneTiles();
    }

    public void CollapseRight(bool value)
    {
        if (HasRight.HasValue) throw new CollapsingException("HasRight already collapsed");

        HasRight = value;
        PruneTiles();
    }

    private void PruneTiles()
    {
        possibleTiles.RemoveAll(objectT =>
        {
            var tile = objectT.GetComponent<DecidedTile>();

            if (HasBottom.HasValue && tile.HasBottom != HasBottom.Value) return true;
            if (HasRight.HasValue && tile.HasRight != HasRight.Value) return true;
            if (HasTop.HasValue && tile.HasTop != HasTop.Value) return true;
            if (HasLeft.HasValue && tile.HasLeft != HasLeft.Value) return true;

            return false;
        });
        
        CalculateEntropy();
    }
}
