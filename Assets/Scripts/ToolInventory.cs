using UnityEngine;
using UnityEngine.InputSystem;

public class ToolInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public Tool currentTool;

    [Header("Input")]
    [SerializeField] private PlayerInput inputAction;
    [SerializeField] private Playermovement playerMovement;

    private void Awake()
    {
        inputAction = new PlayerInput();
        playerMovement = GetComponent<Playermovement>();
    }

    private void OnEnable()
    {
        inputAction.Enable();
        inputAction.Player.UseTool.performed += OnUseTool;
    }
    private void OnDisable()
    {
        inputAction.Player.UseTool.performed -= OnUseTool;
        inputAction.Disable();
    }

    void OnUseTool(InputAction.CallbackContext context)
    {
        if (currentTool == null)
        {
            Debug.Log("No tienes ninguna herramienta equipada.");
            return;
        }

        UseTool();
    }

    void UseTool()
    {
        Vector2Int playerPos = playerMovement.gridPosition;

        switch (currentTool.toolType)
        {
            case ToolType.Broom:
                UseBroom(playerPos);
                break;

        }

    }
    void UseBroom(Vector2Int position)
    {
        Debug.Log("Usando la escoba para limpiar.");
        if (GridManager.Instance == null) return;

        GridManager.Instance.CleanDirt(position);
    }

    public void EquipTool(Tool newTool)
    {
        currentTool = newTool;
        Debug.Log($"Herramienta equipada: {newTool.toolName}");
    }

}
