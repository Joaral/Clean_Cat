using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum TileState
{
    Clean,
    Dirty,
    Wet
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Tilemap References")]
    public Tilemap dirtTilemap;
    public Tilemap wetTilemap;
    public Grid grid;

    [Header("Tiles")]
    public TileBase dirtTile;
    public TileBase wetTile;

    [Header("Level Stats")]
    public int totalDirtTiles = 0;
    public int cleanedTiles = 0;

    private Dictionary<Vector2Int, TileState> tileStates = new Dictionary<Vector2Int, TileState>();
    private UIManager uiManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        InitializeDirtTiles();
    }

    void InitializeDirtTiles()
    {
        if (dirtTilemap == null) return;

        BoundsInt bounds = dirtTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (dirtTilemap.HasTile(pos))
            {
                Vector2Int gridPos = new Vector2Int(pos.x, pos.y);
                tileStates[gridPos] = TileState.Dirty;
                totalDirtTiles++;
            }
        }

        Debug.Log($"Total de casillas sucias: {totalDirtTiles}");
        UpdateCleaningProgress();
    }

    public void CleanDirt(Vector2Int gridPosition)
    {
        Vector3Int cellPos = new Vector3Int(gridPosition.x, gridPosition.y, 0);

        if (tileStates.ContainsKey(gridPosition) && tileStates[gridPosition] == TileState.Dirty)
        {
            // Limpiar la suciedad
            dirtTilemap.SetTile(cellPos, null);
            tileStates[gridPosition] = TileState.Clean;
            cleanedTiles++;

            Debug.Log($"Casilla limpiada: {gridPosition}. Progreso: {cleanedTiles}/{totalDirtTiles}");
            UpdateCleaningProgress();
        }
    }

    public void MopTile(Vector2Int gridPosition)
    {
        Vector3Int cellPos = new Vector3Int(gridPosition.x, gridPosition.y, 0);

        // Solo se puede fregar si está limpio (no sucio)
        if (!tileStates.ContainsKey(gridPosition) || tileStates[gridPosition] != TileState.Dirty)
        {
            wetTilemap.SetTile(cellPos, wetTile);
            tileStates[gridPosition] = TileState.Wet;
            Debug.Log($"Casilla mojada: {gridPosition}");
        }
        else
        {
            Debug.Log("¡No puedes fregar sobre suciedad! Usa la escoba primero.");
        }
    }

    public bool IsTileWet(Vector3 worldPosition)
    {
        Vector3Int cellPos = grid.WorldToCell(worldPosition);
        Vector2Int gridPos = new Vector2Int(cellPos.x, cellPos.y);

        return tileStates.ContainsKey(gridPos) && tileStates[gridPos] == TileState.Wet;
    }

    public bool IsTileDirty(Vector2Int gridPosition)
    {
        return tileStates.ContainsKey(gridPosition) && tileStates[gridPosition] == TileState.Dirty;
    }

    void UpdateCleaningProgress()
    {
        if (uiManager != null)
        {
            float progress = totalDirtTiles > 0 ? (float)cleanedTiles / totalDirtTiles : 0f;
            uiManager.UpdateCleaningBar(progress);
        }
    }

    public float GetCleaningPercentage()
    {
        return totalDirtTiles > 0 ? (float)cleanedTiles / totalDirtTiles * 100f : 0f;
    }
}

