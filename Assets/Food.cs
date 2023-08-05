using UnityEngine;

public class Food : MonoBehaviour
{
    private GridCreator gridCreatorObject = null;
    private GameObject grid; // The script that creates the grid

    void Start()
    {
        grid = GameObject.Find("Grid");
        gridCreatorObject = grid.GetComponent<GridCreator>();

        // Calculate the size of each cell based on the size of the cellPrefab
        float cellSize = gridCreatorObject.cellSize;

        // Calculate the offset to center the grid at the origin
        Vector2 offset = new Vector2((gridCreatorObject.columns / 2f) * cellSize, (gridCreatorObject.rows / 2f) * cellSize);

        // Choose a random position within the grid
        int row = Random.Range(1, gridCreatorObject.rows - 1);
        int column = Random.Range(1, gridCreatorObject.columns - 1);

        // Calculate the position of the object within the grid
        Vector2 position = new Vector2(column * cellSize, row * cellSize) - offset;

        transform.position = position;
    }

}
