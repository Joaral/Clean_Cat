using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [Header("Grid Reference")]
    public Grid grid;

    [Header("Tilemap Reference (opcional)")]
    public Tilemap tilemap;

    void Start()
    {
        if (grid == null)
        {
            Debug.LogError("¡Falta asignar el Grid en el Inspector!");
            return;
        }

        LookAtGridCenter();
    }

    void LookAtGridCenter()
    {
        Vector3 gridCenter;

        if (tilemap != null)
        {
            BoundsInt bounds = tilemap.cellBounds;
            Vector3 min = grid.CellToWorld(bounds.min);
            Vector3 max = grid.CellToWorld(bounds.max);
            gridCenter = (min + max) / 2f;
        }
        else
        {
            gridCenter = grid.transform.position;
        }

        Vector3 lookTarget = new Vector3(gridCenter.x, 0f, gridCenter.z);
        transform.LookAt(lookTarget);
    }

    // Gracias a esto, el editor actualizará la vista de la cámara al cambiar el Grid o Tilemap
    void OnValidate()
    {
        if (grid != null)
        {
            LookAtGridCenter();
        }
    }
}
