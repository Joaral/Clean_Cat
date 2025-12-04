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

    [Header("Grid Settings")]
    public float cellSize = 1f;

    [Header("Tilemaps")]
    public Tilemap dirtTilemap;
    public Tilemap wetTilemap;

    [Header("Prefabs")]
    public GameObject cleanPrefab;
    public GameObject wetPrefab;

    // Diccionario que guarda el estado de cada celda
    private Dictionary<Vector2Int, TileState> tileStates = new Dictionary<Vector2Int, TileState>();

    private UIManager uiManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        uiManager = FindFirstObjectByType<UIManager>();
    }

    // ===============================
    //          LIMPIAR SUCIEDAD
    // ===============================
    public void CleanDirt(Vector2Int gridPosition)
    {
        Vector3Int cellPos = new Vector3Int(gridPosition.x, gridPosition.y, 0);

        // Si no existe, asumimos que está sucio
        if (!tileStates.ContainsKey(gridPosition))
            tileStates[gridPosition] = TileState.Dirty;

        if (tileStates[gridPosition] == TileState.Dirty)
        {
            // Quitar tile del tilemap
            dirtTilemap.SetTile(cellPos, null);

            // Destruir cualquier prefab que exista en esa posición
            DestroyTileObjectAt(dirtTilemap, cellPos);

            tileStates[gridPosition] = TileState.Clean;

            // Instanciar prefab limpio si existe
            if (cleanPrefab != null)
                Instantiate(cleanPrefab, dirtTilemap.GetCellCenterWorld(cellPos), Quaternion.identity);

            Debug.Log($"Casilla limpiada: {gridPosition}");
        }
    }

    // ===============================
    //              MOJAR TILE
    // ===============================
    public void MopTile(Vector2Int gridPosition)
    {
        Vector3Int cellPos = new Vector3Int(gridPosition.x, gridPosition.y, 0);

        // No se puede mojar si sigue sucio
        if (tileStates.ContainsKey(gridPosition) && tileStates[gridPosition] == TileState.Dirty)
        {
            Debug.Log("¡No puedes fregar sobre suciedad!");
            return;
        }

        // Destruir cualquier prefab anterior
        DestroyTileObjectAt(wetTilemap, cellPos);

        // Quitar tile húmedo del tilemap
        wetTilemap.SetTile(cellPos, null);

        tileStates[gridPosition] = TileState.Wet;

        // Instanciar prefab mojado si existe
        if (wetPrefab != null)
            Instantiate(wetPrefab, wetTilemap.GetCellCenterWorld(cellPos), Quaternion.identity);

        Debug.Log($"Casilla mojada: {gridPosition}");
    }

    // ===============================
    //     CONSULTAS DE ESTADO
    // ===============================
    public bool IsTileWet(Vector2Int gridPosition)
    {
        return tileStates.ContainsKey(gridPosition) && tileStates[gridPosition] == TileState.Wet;
    }

    public bool IsTileDirty(Vector2Int gridPosition)
    {
        // Si no existe en el diccionario, asumimos que está sucia
        return !tileStates.ContainsKey(gridPosition) || tileStates[gridPosition] == TileState.Dirty;
    }

    public bool IsTileWet(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.y / cellSize)
        );
        return IsTileWet(gridPos);
    }

    public bool IsTileDirty(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(worldPos.x / cellSize),
            Mathf.RoundToInt(worldPos.y / cellSize)
        );
        return IsTileDirty(gridPos);
    }

    // ===============================
    //     DESTRUCCIÓN DE PREFABS TILE
    // ===============================
    void DestroyTileObjectAt(Tilemap tilemap, Vector3Int cellPos)
    {
        Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
        float radius = 0.1f; // margen pequeño

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, radius);

        foreach (var h in hits)
        {
            if (h.transform.parent == tilemap.transform)
            {
                Destroy(h.gameObject);
            }
        }
    }
}

