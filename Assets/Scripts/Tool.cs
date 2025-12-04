using UnityEngine;

public enum ToolType
{
    Broom,
    Mop,
    Vacuum,
    Duster,
    Spray
}

[CreateAssetMenu(fileName = "New Tool", menuName = "CleanCat/Tool")]
public class Tool : ScriptableObject
{
    public string toolName;
    public ToolType toolType;
    public Sprite icon;
    public GameObject prefab;
}
