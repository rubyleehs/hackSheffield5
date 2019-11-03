using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public static int NODE_SIZE = 32;
    public Node parent;
    public Vector2Int position;
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

    public Node(Vector2Int pos, bool passable)
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
    Node[,] grid;
    int gridRows
    {
        get
        {
            return grid.GetLength(1);
        }
    }
    int gridCols
    {
        get
        {
            return grid.GetLength(0);
        }
    }

    public PathFinderAStar(int[,] map, HashSet<int> passableIds)
    {
        grid = new Node[map.GetLength(0), map.GetLength(1)];
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                grid[x, y] = new Node(new Vector2Int(x, y), passableIds.Contains(map[x, y]));
            }
        }
    }

    public List<Vector2> Locate(Vector2Int starting, Vector2Int ending)
    {
        Node start = new Node(new Vector2Int(starting.x, starting.y), true);
        Node end = new Node(new Vector2Int(ending.x, ending.y), true);

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
                        for (int i = 0; i < openList.Count; i++)
                        {
                            if (n.cost > openList[i].cost) continue;
                            openList.Insert(i, n);
                        }
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

        if (row - 1 >= 0) { tempList.Add(grid[col, row + 1]); }
        if (row - 1 >= 0) { tempList.Add(grid[col, row - 1]); }
        if (col - 1 >= 0) { tempList.Add(grid[col - 1, row]); }
        if (col + 1 < gridCols) { tempList.Add(grid[col + 1, row]); }

        return tempList;
    }
}



