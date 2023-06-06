/* Program name: Maze-generation
Project file name: GameController.cs
Author: Nigel Maynard
Date: 3/5/23
Language: C#
Platform: Unity/ VS Code
Purpose: Assessment
Description: This contains the logic for creating the player, monster and saying when you win
*/
using System;
using UnityEngine;

[RequireComponent(typeof(MazeConstructor))]           

public class GameController : MonoBehaviour
{
    private MazeConstructor constructor;
    [SerializeField] private int rows;
    [SerializeField] private int cols;
    public GameObject playerPrefab, monsterPrefab;

    private AIController aIController;

    void Awake()
    {
        constructor = GetComponent<MazeConstructor>();
        aIController = GetComponent<AIController>(); 
    }
    
    void Start()
    {
        constructor.GenerateNewMaze(rows, cols, OnTreasureTrigger);

        aIController.Graph = constructor.graph;
        aIController.Player = CreatePlayer();
        aIController.Monster = CreateMonster(); 
        aIController.HallWidth = constructor.hallWidth;
        aIController.StartAI();
    }


    // This creates the player
    private GameObject CreatePlayer()
    {
        Vector3 playerStartPosition = new Vector3(constructor.hallWidth, 1, constructor.hallWidth);  
        GameObject player = Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
        player.tag = "Generated";
        return player;
    }


    // This creates the monster
    private GameObject CreateMonster()
    {
        Vector3 monsterPosition = new Vector3(constructor.goalCol * constructor.hallWidth, 0f, constructor.goalRow * constructor.hallWidth);
        GameObject monster = Instantiate(monsterPrefab, monsterPosition, Quaternion.identity);
        monster.tag = "Generated";    
        return monster;
    }

    
    // This Triggers on you reaching the goal
    private void OnTreasureTrigger(GameObject trigger, GameObject other)
    { 
        Debug.Log("You Won!");
        aIController.StopAI();
    }
}