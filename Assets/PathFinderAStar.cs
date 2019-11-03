using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public static int NODE_SIZE = 32;
    public Node parent;
    public Vector2 position;
    public float distance;
    public float cost;
    public float F
    {
        get
        {
            if (distance != -1 && cost != -1)
                return distance + cost;
            else
                return -1;
        }
    }
    public bool isPassable;

    public Node(Vector2 pos, bool passable)
    {
        parent = null;
        position = pos;
        distance = -1;
        cost = 1;
        isPassable = passable;
    }
}

public class PathFinderAStar : MonoBehaviour
{
    int[,] map;

    
    List<List<Node>> grid;
    int gridRows
    {
        get
        {
            return grid[0].Count;
        }
    }
    int gridCols
    {
        get
        {
            return grid.Count;
        }
    }
    

    public PathFinderAStar(List<List<Node>> gridNodes)
    {
        grid = gridNodes;
    }

    public List<Vector2> Locate(Vector2 starting, Vector2 ending)
    {
        Node start = new Node(new Vector2(starting.x, starting.y), true);
        Node end = new Node(new Vector2(ending.x, ending.y), true);

        Stack<Node> pathStack = new Stack<Node>();
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        List<Node> adjacentNodes;
        Node currentNode = start;

        openList.Add(start);

        while(openList.Count != 0 && !closedList.Exists(x => x.position == end.position))
        {
            currentNode = openList[0];
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            adjacentNodes = GetAdjacentNodes(currentNode);

            foreach(Node n in adjacentNodes)
            {
                if (!closedList.Contains(n) && n.isPassable)
                {
                    if (!openList.Contains(n))
                    {
                        n.parent = currentNode;
                        n.distance = Math.Abs(n.position.x - end.position.x) + Math.Abs(n.position.y - end.position.y);
                        n.cost = n.parent.cost + 1;
                        openList.Add(n);
                        openList = openList.OrderBy(Node => Node.F).ToList<Node>();
                    }
                }
            }
        }

        if(!closedList.Exists(x => x.position == end.position))
        {
            return null;
        }

        Node temp = closedList[closedList.IndexOf(currentNode)];
        while(temp.parent != start && temp != null)
        {
            pathStack.Push(temp);
            temp = temp.parent;
        }

        List<Vector2> path = new List<Vector2>();

        foreach (Node n in pathStack)
        {
            path.Add(n.position);
        }
        
        return path;
    }

    private List<Node> GetAdjacentNodes(Node n)
    {
        List<Node> tempList = new List<Node>();
        int row = (int)n.position.y;
        int col = (int)n.position.x;
        
        if(row - 1 >= 0) { tempList.Add(grid[col][row + 1]); }
        if(row - 1 >= 0) { tempList.Add(grid[col][row - 1]); }
        if(col - 1 >= 0) { tempList.Add(grid[col - 1][row]); }
        if(col + 1 < gridCols) { tempList.Add(grid[col + 1][row]); }

        return tempList;
    }

}



