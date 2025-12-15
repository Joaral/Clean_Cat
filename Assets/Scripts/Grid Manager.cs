using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Reference")]
    public Grid grid;

    [Header("Tilemaps")]
    public Tilemap dirtTilemap;

    [Header("Stats")]
    public int totalDirtTiles;
    public int cleanedTiles = 0;

    // Diccionario que guarda el estado de cada celda
    private HashSet<Vector2Int> dirtyPositions = new HashSet<Vector2Int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CountDirtTiles();
    }

    // ===============================
    //          LIMPIAR SUCIEDAD
    // ===============================
    public void CountDirtTiles()
    {
        if (dirtTilemap == null)
        {
            Debug.LogError("Dirt Tilemap is not assigned!");
            return;
        }

        BoundsInt bounds = dirtTilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (dirtTilemap.HasTile(pos))
            {
                Vector2Int gridPos = new Vector2Int(pos.x, pos.y);
                dirtyPositions.Add(gridPos);
                totalDirtTiles++;
            }
        }
        Debug.Log($"Total dirt tiles counted: {totalDirtTiles}");
    }
    public void CleanDirt(Vector2Int gridPosition)
    {
        if (!dirtyPositions.Contains(gridPosition))
        {
            Debug.Log($"No dirt to clean at {gridPosition}.");
            return;
        }

        Vector3Int cellPos = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        dirtTilemap.SetTile(cellPos, null);

        dirtyPositions.Remove(gridPosition);
        cleanedTiles++;
        Debug.Log($"¡Casilla limpiada! Progreso: {cleanedTiles}/{totalDirtTiles}");

        if (cleanedTiles >= totalDirtTiles)
        {
            Debug.Log("¡Todas las casillas están limpias! Nivel completado.");
        }
    }
    public bool IsDirty(Vector2Int gridPosition)
    {
        return dirtyPositions.Contains(gridPosition);
    }

    public float GetCleaningProgress()
    {
        if (totalDirtTiles == 0) return 0f;
        return (float)cleanedTiles / totalDirtTiles;
    }
}
