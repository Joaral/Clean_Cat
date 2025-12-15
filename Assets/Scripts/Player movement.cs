using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class Playermovement : MonoBehaviour
{
    [Header("grid reference")]
    public Grid grid;
    public Tilemap tilemap;

    [Header("movement settings")]
    public float moveSpeed = 5f;

    [Header("Grid Position")]
    public Vector2Int gridPosition = new Vector2Int(0, 0);

    [SerializeField] private bool isMoving = false;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private BoundsInt tilemapBounds;

    [Header("Movement")]
    [SerializeField] private PlayerInput inputAction;
    [SerializeField] Vector2 moveInput;

    [Header("other")]
    public int searchRadius = 10;

    private void Awake()
    {
        inputAction = new PlayerInput();
    }
    void OnEnable()
    {
        inputAction.Enable();
        inputAction.Player.Move.performed += OnMoveInput;
        inputAction.Player.Move.canceled += ctx => moveInput = Vector2.zero;

    }
    void OnDisable()
    {
        inputAction.Player.Move.canceled -= ctx => moveInput = Vector2.zero;
        inputAction.Player.Move.performed -= OnMoveInput;
        inputAction.Disable();
    }
    void Start()
    {
        if (grid == null)
        {
            Debug.LogError("¡Falta asignar el Grid en el Inspector!");
            return;
        }
        if (tilemap == null)
        {
            Debug.Log("No se ha asignado un tilemap. Bounds asignados por defecto(0,0)");
            return;
        }

        // DEBUG COMPLETO
        Debug.Log("=== DIAGNÓSTICO INICIAL ===");
        Debug.Log($"Grid Cell Size: {grid.cellSize}");
        Debug.Log($"Player Transform Position: {transform.position}");
        Debug.Log($"Grid Position configurada: {gridPosition}");

        // Ver qué celda corresponde a la posición del player
        Vector3Int calculatedCell = grid.WorldToCell(transform.position);
        Debug.Log($"Celda calculada desde Transform: {calculatedCell}");

        // Ver si hay tile en esa celda
        bool hasTileAtCalculated = tilemap.GetTile(calculatedCell) != null;
        Debug.Log($"¿Hay tile en {calculatedCell}? {hasTileAtCalculated}");

        // Ver si hay tile en (0,0)
        Vector3Int zeroCell = new Vector3Int(0, 0, 0);
        bool hasTileAtZero = tilemap.GetTile(zeroCell) != null;
        Debug.Log($"¿Hay tile en (0,0)? {hasTileAtZero}");

        // Ver los bounds del tilemap
        Debug.Log($"Tilemap Bounds: {tilemap.cellBounds}");

        // Contar tiles totales
        int tileCount = 0;
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                tileCount++;
                // Mostrar las primeras 5 posiciones con tiles
                if (tileCount <= 5)
                {
                    Debug.Log($"  Tile encontrado en: {pos}");
                }
            }
        }
        Debug.Log($"Total de tiles en Tilemap_Floor: {tileCount}");
        Debug.Log("=========================");

        // Usar la celda calculada en lugar de gridPosition manual
        gridPosition = new Vector2Int(calculatedCell.x, calculatedCell.y);

        if (!IsPositionValid(gridPosition))
        {
            Debug.LogWarning($"La posición inicial {gridPosition} no es válida. Buscando posicion valida...");
            gridPosition = FindNearestValidPosition(gridPosition);
        }

        UpdatePosition();
    }

    void Update()
    {
        if (!isMoving && moveInput != Vector2.zero) 
        {
            TryMovefrominput();
        }

        MoveToTarget();

    }

    void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();   
    }
    void TryMovefrominput()
    {
        Vector2Int direction = Vector2Int.zero;

        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            direction.x = moveInput.x > 0 ? 1 : -1;
        }
        else if (moveInput.y != 0)
        {
            direction.y = moveInput.y > 0 ? 1 : -1;
        }

        if (direction != Vector2Int.zero)
        {
            Move(direction);
            Vector3 lookDirection = new Vector3(direction.x, 0, direction.y);
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }


    void Move(Vector2Int direction)
    {
        Vector2Int newPosition = gridPosition + direction;

        Debug.Log($"Intentando mover a: {newPosition}");

        if (!IsPositionValid(newPosition))
        {
            Debug.Log("Movimiento bloqueado: fuera de límites");
            return;
        }

        //if (GridManager.Instance != null && GridManager.Instance.IsTileWet(grid.GetCellCenterWorld(new Vector3Int(newPosition.x, newPosition.y, 0))))
        //{
        //    Debug.Log("¡No puedes pisar casillas mojadas!");
        //    return;
        //}

        gridPosition = newPosition;

        Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
        targetPosition = new Vector3(cellCenter.x, transform.position.y, cellCenter.z);

        isMoving = true;
    }
    bool IsPositionValid(Vector2Int position)
    {
        if (tilemap == null)
        {
            Debug.LogWarning("No hay tilemap asignado. Se permite el movimiento.");
            return true;
        }

        //if (position.x < tilemapBounds.xMin || position.x >= tilemapBounds.xMax)
        //{
        //    Debug.Log(tilemapBounds.xMin + " " + tilemapBounds.xMax);
        //    return false;
        //}
        //if (position.y < tilemapBounds.yMin || position.y >= tilemapBounds.yMax)
        //{
        //    Debug.Log(tilemapBounds.yMin + " " + tilemapBounds.yMax);
        //    return false;
        //}

        Vector3Int cellPosition = new Vector3Int(position.x, position.y, 0);
        TileBase tile = tilemap.GetTile(cellPosition);

        if ( tile == null)
        {
            Debug.Log($"Movimiento bloqueado: No hay suelo en {position}");
            return false;
        }
        //if (GridManager.Instance != null && GridManager.Instance.IsTileWet(grid.GetCellCenterWorld(cellPosition)))
        //{
        //    Debug.Log("Movimiento bloqueado: Casilla mojada");
        //    return false;
        //}
        return true;
    }

    Vector2Int FindNearestValidPosition(Vector2Int startPosition)
    {
        for ( int radius = 1; radius <= searchRadius; radius++)
        {

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector2Int checkPosition = startPosition + new Vector2Int(x, y);
                    if (IsPositionValid(checkPosition))
                    {
                        Debug.Log($"Posición válida encontrada en {checkPosition}");
                        return checkPosition;
                    }
                }
            }
        }
        Debug.LogError("No se encontró una posición válida cercana.");
        return startPosition;
    }

    void UpdatePosition()
    {
        Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
        transform.position = new Vector3(cellCenter.x, transform.position.y, cellCenter.z);
        targetPosition = transform.position;

    }
    void MoveToTarget()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = Color.yellow;
        Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
        Vector3 gizmoPos = new Vector3(cellCenter.x, 0.1f, cellCenter.z);
        Gizmos.DrawWireCube(gizmoPos, new Vector3(grid.cellSize.x, 0.1f, grid.cellSize.y));

        if (tilemap != null)
        {
            Gizmos.color = Color.green;
            BoundsInt bounds = tilemap.cellBounds;
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                {
                    Vector3 center = grid.GetCellCenterWorld(pos);
                    Gizmos.DrawWireCube(new Vector3(center.x, 0.05f, center.z),
                                       new Vector3(grid.cellSize.x * 0.9f, 0.05f, grid.cellSize.y * 0.9f));
                }
            }
        }
    }
}

