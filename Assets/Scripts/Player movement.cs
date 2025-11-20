using UnityEditor.Tilemaps;
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

    [SerializeField]private bool isMoving = false;
    [SerializeField]private Vector3 targetPosition;
    [SerializeField]private BoundsInt tilemapBounds;

    [Header("Movement")]
    [SerializeField]private PlayerInput inputAction;
    [SerializeField]Vector2 moveInput;

    private void Awake()
    {
        inputAction = new PlayerInput();
    }
    void OnEnable()
    {
        inputAction.Enable();
        inputAction.Player.Move.performed += OnmoveInput;
    }
    void OnDisable()
    {
        inputAction.Player.Move.performed -= OnmoveInput;
        inputAction.Disable();
    }
    void Start()
    {
        if (grid == null)
        {
            Debug.LogError("¡Falta asignar el Grid en el Inspector!");
            return;
        }
        if(tilemap != null)
        {
            tilemapBounds = tilemap.cellBounds;
        }

        UpdatePosition();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }
    void UpdatePosition()
    {
        Vector3 worldPosition = grid.CellToWorld(new Vector3Int(gridPosition.x, gridPosition.y, 0));
        transform.position = worldPosition;
    }

    void OnMoveInput(InputAction.CallbackContext context)
    {
        if (isMoving) return;

        moveInput = context.ReadValue<Vector2>();
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
        }
    }

    void Move(Vector2Int direction)
    {

    }
    bool IsPositionValid(Vector2Int position)
    {
        return true;
    }
    void UpdatePosition()
    {

    }
    void MoveToTarget()
    {

    }

    void OnDrawGizmos()
    {

    }
}

