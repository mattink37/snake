using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public int rows = 16; // The number of rows in the grid
    public int columns = 16; // The number of columns in the grid
    public float cellSize = 0;
    public GameObject cellPrefab; // The prefab to use for each cell in the grid
    public GameObject foodPrefab;

    void Start()
    {
        Instantiate(foodPrefab);
        // Calculate the size of each cell based on the size of the cellPrefab
        cellSize = cellPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        // Calculate the offset to center the grid at the origin
        Vector2 offset = new Vector2((columns / 2f) * cellSize, (rows / 2f) * cellSize);

        // Create the top and bottom borders of the grid
        for (int column = 0; column < columns; column++)
        {
            // Calculate the position of the top border cell
            Vector2 topPosition = new Vector2(column * cellSize, 0) - offset;

            // Instantiate the top border cell at the calculated position
            Instantiate(cellPrefab, topPosition, Quaternion.identity, transform);

            // Calculate the position of the bottom border cell
            Vector2 bottomPosition = new Vector2(column * cellSize, (rows - 1) * cellSize) - offset;

            // Instantiate the bottom border cell at the calculated position
            Instantiate(cellPrefab, bottomPosition, Quaternion.identity, transform);
        }

        // Create the left and right borders of the grid
        for (int row = 1; row < rows - 1; row++)
        {
            // Calculate the position of the left border cell
            Vector2 leftPosition = new Vector2(0, row * cellSize) - offset;

            // Instantiate the left border cell at the calculated position
            Instantiate(cellPrefab, leftPosition, Quaternion.identity, transform);

            // Calculate the position of the right border cell
            Vector2 rightPosition = new Vector2((columns - 1) * cellSize, row * cellSize) - offset;

            // Instantiate the right border cell at the calculated position
            Instantiate(cellPrefab, rightPosition, Quaternion.identity, transform);
        }
    }
}
