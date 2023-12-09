using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TileGridController : MonoBehaviour
{
    private int gridSizeX;
    public int GridSizeX => gridSizeX;
    [SerializeField] private int gridSizeY = 10;
    public int GridSizeY => gridSizeY;
    [SerializeField] private GameObject baseTile;

    public UndecidedTile[,] Tiles;


    private bool done;
    
    private CameraController _cameraController;

    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = FindFirstObjectByType<SceneLoader>();
        _cameraController = FindFirstObjectByType<CameraController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _sceneLoader.StartLoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        WaveFunctionCollapse();
    }

    public void WaveFunctionCollapse()
    {
        var aspectRatio = (float) Screen.width / (float) Screen.height;
        gridSizeX = Mathf.CeilToInt(gridSizeY * aspectRatio);
        _cameraController.SetCamPositionAndSize();
        CleanUp();
        InitializeGrid();
        //FirstStep();
        StartCoroutine(WaveFunctionCollapseMainLoop());
    }
    
    private IEnumerator WaveFunctionCollapseMainLoop()
    {
        while (!done)
        {
        
            var coolTiles = GetTilesWithLowestEntropy();
        
            if (done) break;

            coolTiles[Random.Range(0, coolTiles.Count)].CollapseEntirely();
            yield return null;
        }
    }

    private void CleanUp()
    {
        StopAllCoroutines();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void InitializeGrid()
    {
        Tiles = new UndecidedTile[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Tiles[x, y] = Instantiate(baseTile, transform).GetComponent<UndecidedTile>();
                Tiles[x, y].transform.position = new Vector2(x, y);
                Tiles[x, y].x = x;
                Tiles[x, y].y = y;
            }
        }
    }

    private void FirstStep()
    {
        Tiles[Random.Range(0, gridSizeX), Random.Range(0, gridSizeY)].GetComponent<UndecidedTile>().CollapseEntirely();
    }

    private List<UndecidedTile> GetTilesWithLowestEntropy()
    {
        var lowestEntropy = int.MaxValue;
        var lowestTiles = new List<UndecidedTile>();
        
        foreach (var tile in Tiles)
        {
            var entropy = tile.Entropy;
            if (tile.Collapsed || entropy > lowestEntropy)
            {
                continue;
            }
            
            if (entropy == lowestEntropy)
            {
                lowestTiles.Add(tile);
                continue;
            }

            lowestEntropy = entropy;
            lowestTiles.Clear();
            lowestTiles.Add(tile);
        }

        if (lowestTiles.Count == 0)
        {
            done = true;
        }

        return lowestTiles;
    }
    
    
}
