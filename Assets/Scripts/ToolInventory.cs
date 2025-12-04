using UnityEngine;
using UnityEngine.InputSystem;

public class ToolInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int maxTools = 2;

    [Header("Current Tools")]
    public Tool[] tools = new Tool[2];
    public int currentToolIndex = 0;

    [Header("Input")]
    [SerializeField] private PlayerInput inputAction;

    private Playermovement playerMovement;
    private UIManager uiManager;

    private void Awake()
    {
        inputAction = new PlayerInput();
        playerMovement = GetComponent<Playermovement>();
    }

    private void OnEnable()
    {
        inputAction.Enable();
        inputAction.Player.UseTool.performed += OnUseTool;
        inputAction.Player.SwitchTool.performed += OnSwitchTool;
    }

    private void OnDisable()
    {
        inputAction.Player.UseTool.performed -= OnUseTool;
        inputAction.Player.SwitchTool.performed -= OnSwitchTool;
        inputAction.Disable();
    }

    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        UpdateUI();
    }

    void OnUseTool(InputAction.CallbackContext context)
    {
        if (tools[currentToolIndex] != null)
        {
            UseTool(playerMovement.gridPosition);
        }
    }

    void OnSwitchTool(InputAction.CallbackContext context)
    {
        if (tools[0] != null && tools[1] != null)
        {
            currentToolIndex = (currentToolIndex + 1) % maxTools;
            UpdateUI();
            Debug.Log($"Cambiado a herramienta: {tools[currentToolIndex].toolName}");
        }
    }

    public void PickupTool(Tool newTool)
    {
        // Si hay espacio libre, añadir la herramienta
        for (int i = 0; i < maxTools; i++)
        {
            if (tools[i] == null)
            {
                tools[i] = newTool;
                Debug.Log($"Herramienta recogida: {newTool.toolName} en slot {i}");
                UpdateUI();
                return;
            }
        }

        // Si no hay espacio, reemplazar la herramienta actual
        Debug.Log($"Reemplazando {tools[currentToolIndex].toolName} por {newTool.toolName}");
        tools[currentToolIndex] = newTool;
        UpdateUI();
    }

    public void UseTool(Vector2Int gridPosition)
    {
        Tool currentTool = tools[currentToolIndex];

        if (currentTool == null) return;

        if (currentTool.toolType == ToolType.Broom)
        {
            GridManager.Instance.CleanDirt(gridPosition);
        }
        else if (currentTool.toolType == ToolType.Mop)
        {
            GridManager.Instance.MopTile(gridPosition);
        }
    }

    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateToolInventory(tools, currentToolIndex);
        }
    }

    public Tool GetCurrentTool()
    {
        return tools[currentToolIndex];
    }
}
