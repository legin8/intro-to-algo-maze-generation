/* Program name: Maze-generation
Project file name: Node.cs
Author: Nigel Maynard
Date: 3/5/23
Language: C#
Platform: Unity/ VS Code
Purpose: Assessment
Description: This contains the logic for moving from node to node
*/
public class Node
{
    public int x, y, gCost, hCost, fCost;      
    public bool isWalkable;
    public Node cameFromNode;

    public Node(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        hCost = 0;
        this.isWalkable = isWalkable;
    }
    
    public void CalculateFCost(){
        fCost = gCost + hCost;
    } 
}