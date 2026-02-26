using UnityEngine;
using UnityEngine.UI;

public class ResponsiveGrid : MonoBehaviour
{
    public GridLayoutGroup gridLayout;
    public RectTransform parentRect;

    void Start()
    {
        UpdateGrid();
    }

    void UpdateGrid()
    {
        if (gridLayout == null || parentRect == null) return;

        // Get the parent size
        var width = parentRect.rect.width;
        float height = parentRect.rect.height;

        // Divide the space equally between two cells
        float cellWidth = width / 2;
        float cellHeight = height;

        // Apply the new cell size
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
    }

    void Update()
    {
        UpdateGrid(); // Keep it updated when resizing
    }
}
