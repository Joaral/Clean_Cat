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
            tilemap.CompressBounds();
            tilemapBounds = tilemap.cellBounds;
            Debug.Log("No se ha asignado un tilemap. Bounds asignados por defecto(0,0)");
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
        //este if obliga al player a moverse la casilla al completo antes de poder volver a leer el input
        //if (isMoving) return;   

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

        gridPosition = newPosition;

        Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
        targetPosition = new Vector3(cellCenter.x, transform.position.y, cellCenter.z);

        isMoving = true;
    }
    bool IsPositionValid(Vector2Int position)
    {
        if (tilemap == null) return true;

        if (position.x < tilemapBounds.xMin || position.x >= tilemapBounds.xMax)
        {
            Debug.Log(tilemapBounds.xMin + " " + tilemapBounds.xMax);
            return false;
        }
        if (position.y < tilemapBounds.yMin || position.y >= tilemapBounds.yMax)
        {
            Debug.Log(tilemapBounds.yMin + " " + tilemapBounds.yMax);
            return false;
        }

        Vector3Int cellPosition = new Vector3Int(position.x, position.y, 0);
        TileBase tile = tilemap.GetTile(cellPosition);

        return true;
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
    }
}

