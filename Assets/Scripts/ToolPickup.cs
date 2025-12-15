using UnityEngine;

public class ToolPickup : MonoBehaviour
{
    [Header("Tool Data")]
    public Tool toolData;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        if (spriteRenderer != null && toolData != null && toolData.icon != null)
        {
            spriteRenderer.sprite = toolData.icon;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToolInventory inventory = other.GetComponent<ToolInventory>();

            if (inventory != null)
            {
                //inventory.PickupTool(toolData);
                Destroy(gameObject);
            }
        }
    }
}
