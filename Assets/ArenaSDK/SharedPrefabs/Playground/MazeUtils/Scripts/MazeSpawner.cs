using UnityEngine;
using System.Collections;

// <summary>
// Game object, that creates maze and instantiates it in scene
// </summary>
public class MazeSpawner : MonoBehaviour {
    public int Rows    = 5;
    public int Columns = 5;

    public bool ReinitlizeAtReset = true;

    public enum MazeGenerationAlgorithm {
        PureRecursive,
        RecursiveTree,
        RandomTree,
        OldestTree,
        RecursiveDivision,
    }

    public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;

    public int RandomSeed = -1;

    public GameObject Playground;
    public GameObject Wall   = null;
    public GameObject Pillar = null;


    private float CellWidth  = 5;
    private float CellHeight = 5;
    private Vector3 PlaygroundOffset;

    private BasicMazeGenerator mMazeGenerator = null;
    private bool AddGaps = false;

    private GameObject[,,] Walls;

    void
    Start()
    {
        if ((Playground.transform.localScale.x != 40f) || (Playground.transform.localScale.z != 40f)) {
            Debug.LogError(
                "Donot change the localScale.x and localScale.z of Playground in PlaygroundWithMaze, as it is matched with WallPrefab. Instead, change the localScale of PlaygroundWithMaze if needed.");
        }

        if (Wall.transform.localScale.x != 10f) {
            Debug.LogError(
                "Donot change the localScale.x of WallPrefab, as it is matched with Playground in PlaygroundWithMaze.");
        }

        CellWidth  = Playground.transform.lossyScale.x / Columns;
        CellHeight = Playground.transform.lossyScale.z / Rows;

        PlaygroundOffset = new Vector3(
            -Playground.transform.lossyScale.x / 2f + CellWidth / 2f,
            Playground.transform.position.y + 0.038f,
            -Playground.transform.lossyScale.z / 2f + CellHeight / 2f
        );

        if (Pillar != null) {
            for (int row = 0; row < Rows + 1; row++) {
                for (int column = 0; column < Columns + 1; column++) {
                    float x        = column * (CellWidth + (AddGaps?.2f : 0));
                    float z        = row * (CellHeight + (AddGaps?.2f : 0));
                    GameObject tmp = Instantiate(Pillar, new Vector3(x - CellWidth / 2, 0,
                        z - CellHeight / 2) + PlaygroundOffset,
                        Quaternion.identity) as GameObject;
                    tmp.transform.parent = transform;
                }
            }
        }

        Walls = new GameObject[Rows, Columns, 4];

        for (int row = 0; row < Rows; row++) {
            for (int column = 0; column < Columns; column++) {
                float x = column * (CellWidth + (AddGaps?.2f : 0));
                float z = row * (CellHeight + (AddGaps?.2f : 0));

                GameObject tmp;

                if (true) {
                    tmp = Instantiate(Wall, new Vector3(x + CellWidth / 2, 0,
                        z) + PlaygroundOffset,
                        Quaternion.Euler(0, 90, 0)) as GameObject; // right
                    tmp.transform.parent     = transform;
                    tmp.transform.localScale = new Vector3(
                        tmp.transform.localScale.x / Rows * gameObject.transform.localScale.z,
                        tmp.transform.localScale.y,
                        tmp.transform.localScale.z
                    );
                    Walls[row, column, 0] = tmp;
                }

                if (true) {
                    tmp = Instantiate(Wall, new Vector3(x, 0,
                        z + CellHeight / 2) + PlaygroundOffset,
                        Quaternion.Euler(0, 0, 0)) as GameObject; // front
                    tmp.transform.parent     = transform;
                    tmp.transform.localScale = new Vector3(
                        tmp.transform.localScale.x / Columns * gameObject.transform.localScale.x,
                        tmp.transform.localScale.y,
                        tmp.transform.localScale.z
                    );
                    Walls[row, column, 1] = tmp;
                }

                if (true) {
                    tmp = Instantiate(Wall, new Vector3(x - CellWidth / 2, 0,
                        z) + PlaygroundOffset,
                        Quaternion.Euler(0, 270, 0)) as GameObject; // left
                    tmp.transform.parent     = transform;
                    tmp.transform.localScale = new Vector3(
                        tmp.transform.localScale.x / Rows * gameObject.transform.localScale.z,
                        tmp.transform.localScale.y,
                        tmp.transform.localScale.z
                    );
                    Walls[row, column, 2] = tmp;
                }

                if (true) {
                    tmp = Instantiate(Wall, new Vector3(x, 0,
                        z - CellHeight / 2) + PlaygroundOffset,
                        Quaternion.Euler(0, 180, 0)) as GameObject; // back
                    tmp.transform.parent     = transform;
                    tmp.transform.localScale = new Vector3(
                        tmp.transform.localScale.x / Columns * gameObject.transform.localScale.x,
                        tmp.transform.localScale.y,
                        tmp.transform.localScale.z
                    );
                    Walls[row, column, 3] = tmp;
                }
            }
        }

        if (RandomSeed > 0) {
            Random.InitState(RandomSeed);
        }

        switch (Algorithm) {
            case MazeGenerationAlgorithm.PureRecursive:
                mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveTree:
                mMazeGenerator = new RecursiveTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RandomTree:
                mMazeGenerator = new RandomTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.OldestTree:
                mMazeGenerator = new OldestTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveDivision:
                mMazeGenerator = new DivisionMazeGenerator(Rows, Columns);
                break;
        }

        Reinitialize();
    } // Start

    public void
    Reinitialize()
    {
        mMazeGenerator.ClearMazeCell();
        mMazeGenerator.GenerateMaze();
        for (int row = 0; row < Rows; row++) {
            for (int column = 0; column < Columns; column++) {
                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
                Walls[row, column, 0].SetActive(cell.WallRight);
                Walls[row, column, 1].SetActive(cell.WallFront);
                Walls[row, column, 2].SetActive(cell.WallLeft);
                Walls[row, column, 3].SetActive(cell.WallBack);
            }
        }
    } // Reinitialize

    public void
    Reset()
    {
        if (ReinitlizeAtReset) {
            Reinitialize();
        }
    }
}
