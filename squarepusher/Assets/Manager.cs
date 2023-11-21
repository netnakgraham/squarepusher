using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject redBlockPrefab;

    public GameObject blueBlockPrefab;

    public GameObject blackBlockPrefab;

    public GameObject pusherBlockPrefab;

    private bool isPlayer1Turn = true;

    private GameObject highlightedTile;

    private Color highlightColor;

    void Start()
    {
        highlightColor = isPlayer1Turn ? Color.red : Color.blue;

        PlaceBlackBlockRandomly();
    }

    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast to determine the mouse position
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Pusher"))
                {
                     StartCoroutine(PushBlocksCoroutine(hit.collider));
                }
                else
                {
                    // Check if the mouse is over a grid tile
                    if (hit.collider.CompareTag("Selectable"))
                    {
                        GridTile gridTile =
                            hit.collider.GetComponent<GridTile>();

                        if (gridTile != null && !gridTile.IsOccupied())
                        {
                            PlaceBlock (gridTile);
                        }
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        UpdateTileHighlight();
    }

    void UpdateTileHighlight()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            GridTile gridTile = hit.collider.GetComponent<GridTile>();

            if (gridTile != null)
            {
                gridTile.HighlightTile (highlightColor);
            }
        }
    }

IEnumerator PushBlocksCoroutine(Collider pusherCollider)
    {
        // Determine the direction based on the pusher's orientation
        Vector3 pushDirection = pusherCollider.transform.forward;

        // Add the space of 1 unit in the push direction
        Vector3 offset = pushDirection * 1.0f;

        RaycastHit[] hits = new RaycastHit[10]; // Adjust the size based on your needs
        int hitCount = Physics.RaycastNonAlloc(pusherCollider.transform.position + offset, pushDirection, hits, Mathf.Infinity);

        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i].collider.CompareTag("Selectable"))
            {
                GridTile gridTile = hits[i].collider.GetComponent<GridTile>();

                // Check if the tile is not occupied
                if (gridTile != null && !gridTile.IsOccupied())
                {
                    // Find the row tiles
                    GridTile[] rowTiles = FindRowTiles(gridTile.transform.position, pushDirection);

                    // Move blocks in the row
                    MoveBlocksInRow(rowTiles);
                    break; // Stop after finding the first unoccupied tile
                }
            }
        }

        yield return null; // Wait for one frame to allow the grid to settle
    }

    GridTile[] FindRowTiles(Vector3 startTilePosition, Vector3 rowDirection)
    {
        int maxTiles = 10; // You may need to adjust this based on your grid size
        GridTile[] rowTiles = new GridTile[maxTiles];

        for (int i = 0; i < maxTiles; i++)
        {
            Vector3 tilePosition = startTilePosition + i * rowDirection;
            RaycastHit hit;

            if (Physics.Raycast(tilePosition, Vector3.down, out hit) && hit.collider.CompareTag("Selectable"))
            {
                rowTiles[i] = hit.collider.GetComponent<GridTile>();
            }
            else
            {
                break; // Stop if no more tiles are found in the row
            }
        }

        return rowTiles;
    }

    void MoveBlocksInRow(GridTile[] rowTiles)
    {
        // Check if the last tile in the row is not occupied
        if (rowTiles[rowTiles.Length - 1] != null && !rowTiles[rowTiles.Length - 1].IsOccupied())
        {
            // Move blocks in the row
            for (int i = rowTiles.Length - 2; i >= 0; i--)
            {
                if (rowTiles[i] != null && rowTiles[i + 1] != null)
                {
                    MoveBlock(rowTiles[i], rowTiles[i + 1]);
                }
            }
        }
    }

    void MoveBlock(GridTile sourceTile, GridTile targetTile)
    {
        if (
            sourceTile != null &&
            targetTile != null &&
            sourceTile.IsOccupied() &&
            !targetTile.IsOccupied()
        )
        {
            Instantiate(blackBlockPrefab,
            targetTile.transform.position,
            Quaternion.identity);
            targetTile.SetOccupied(true);

            // Remove the block from the source tile
            //sourceTile.ResetTileColor();
            sourceTile.SetOccupied(false);
        }
    }

    void PlaceBlackBlockRandomly()
    {
        GridTile[] gridTiles = FindObjectsOfType<GridTile>();
        float tileSize = 1.0f; // Assuming each tile has a size of 1 unit

        if (gridTiles.Length > 0)
        {
            int randomIndex = Random.Range(0, gridTiles.Length);
            GridTile randomTile = gridTiles[randomIndex];

            Vector3 roundedPosition =
                new Vector3(Mathf
                        .Round(randomTile.transform.position.x / tileSize) *
                    tileSize,
                    0.5f,
                    Mathf.Round(randomTile.transform.position.z / tileSize) *
                    tileSize);

            Instantiate(blackBlockPrefab, roundedPosition, Quaternion.identity);

            randomTile.SetOccupied(true);
        }
    }

    void PlaceBlock(GridTile gridTile)
    {
        float tileSize = 1.0f; // Assuming each tile has a size of 1 unit

        Vector3 roundedPosition =
            new Vector3(Mathf.Round(gridTile.transform.position.x / tileSize) *
                tileSize,
                0.5f,
                Mathf.Round(gridTile.transform.position.z / tileSize) *
                tileSize);

        if (isPlayer1Turn)
        {
            Instantiate(redBlockPrefab, roundedPosition, Quaternion.identity);
        }
        else
        {
            Instantiate(blueBlockPrefab, roundedPosition, Quaternion.identity);
        }

        isPlayer1Turn = !isPlayer1Turn;
        highlightColor = isPlayer1Turn ? Color.red : Color.blue;
    }

    void OnMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast to determine the mouse position
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the mouse is over a grid tile
            if (hit.collider.CompareTag("Selectable"))
            {
                // Highlight the tile with the corresponding color
                highlightedTile = hit.collider.gameObject;

                if (isPlayer1Turn)
                {
                    highlightedTile.GetComponent<Renderer>().material.color =
                        Color.red;
                }
                else
                {
                    highlightedTile.GetComponent<Renderer>().material.color =
                        Color.blue;
                }
            }
        }
    }

    void OnMouseExit()
    {
        // Reset the color when the mouse leaves a grid tile
        if (highlightedTile != null)
        {
            highlightedTile.GetComponent<Renderer>().material.color =
                Color.white;
            highlightedTile = null;
        }
    }
}
