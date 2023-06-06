/* Program name: Maze-generation
Project file name: MazeConstructor.cs
Author: Nigel Maynard
Date: 3/5/23
Language: C#
Platform: Unity/ VS Code
Purpose: Assessment
Description: This contains the logic for creating a new maze, getting rid of a maze and placing a goal/ treasure
*/
using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;
    
    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;

    public int[,] data{ get; private set; }

    public float placementThreshold = 0.1f;   // chance of empty space
    private MazeMeshGenerator meshGenerator;

    public float hallWidth{ get; private set; }
    public int goalRow{ get; private set; }
    public int goalCol{ get; private set; }

    public Node[,] graph;

    void Awake()
    {
        meshGenerator = new MazeMeshGenerator();
        hallWidth = meshGenerator.width;
        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
    }


    // This makes the new maze for you.
    public void GenerateNewMaze(int sizeRows, int sizeCols, TriggerEventHandler treasureCallback)
    {
        DisposeOldMaze();
        
        if (sizeRows % 2 == 0 && sizeCols % 2 == 0) Debug.LogError("Odd numbers work better for dungeon size.");

        data = FromDimensions(sizeRows, sizeCols);

        goalRow = data.GetUpperBound(0) - 1;
        goalCol = data.GetUpperBound(1) - 1;

        graph = new Node[sizeRows,sizeCols];

        for (int i = 0; i < sizeRows; i++)
        {
            for (int j = 0; j < sizeCols; j++)
            {
                graph[i, j] = data[i,j] == 0 ? new Node(i, j, true) : new Node(i, j, false);
            }
        }
        
        DisplayMaze();
        PlaceGoal(treasureCallback); 
    }


    public int[,] FromDimensions(int sizeRows, int sizeCols)
    {
        int[,] maze = new int[sizeRows, sizeCols];

        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)        
            for (int j = 0; j <= cMax; j++)            
                if (i == 0 || j == 0 || i == rMax || j == cMax)
                {
                    maze[i, j] = 1;                                    
                } else if (i % 2 == 0 && j % 2 == 0 && Random.value > placementThreshold)                                    
                {
                    maze[i, j] = 1;

                    int a = Random.value < .5 ? 0 : (Random.value < .5 ? -1 : 1);
                    int b = a != 0 ? 0 : (Random.value < .5 ? -1 : 1);
                    maze[i+a, j+b] = 1;
                }  
        return maze;
    }


    // This is the debug map that is on the screen
    void OnGUI()
    {
        if (!showDebug) return;

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++) msg += maze[i, j] == 0 ? "...." : "==";
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }


    // This puts the maze on the screen for you to see it.
    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(data);
        
        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] {mazeMat1, mazeMat2};
    }


    // This gets rid of the maze once it isn't needed anymore.
    public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects) Destroy(go);
    }


    // This sets the Treasure for you to find.
    private void PlaceGoal(TriggerEventHandler treasureCallback)
    {            
        GameObject treasure = GameObject.CreatePrimitive(PrimitiveType.Cube);
        treasure.transform.position = new Vector3(goalCol * hallWidth, .5f, goalRow * hallWidth);
        treasure.name = "Treasure";
        treasure.tag = "Generated";

        treasure.GetComponent<BoxCollider>().isTrigger = true;
        treasure.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

        TriggerEventRouter tc = treasure.AddComponent<TriggerEventRouter>();
        tc.callback = treasureCallback;
    }
}