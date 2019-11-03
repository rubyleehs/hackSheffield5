using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
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

public class PathFinderAStar
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
        if (starting == ending) return new List<Vector2>() { ending };
        Node start = new Node(new Vector2Int(starting.x, starting.y), true);
        Node end = new Node(new Vector2Int(ending.x, ending.y), true);

        Stack<Node> pathStack = new Stack<Node>();
        List<Node> openList = new List<Node>();
        Dictionary<Vector2Int,Node> closedList = new Dictionary<Vector2Int, Node>();
        List<Node> adjacentNodes;
        bool endFound = false;
        Node currentNode = start;

        openList.Add(start);
        while(openList.Count != 0 && !endFound)
        {
            currentNode = openList[0];
            openList.RemoveAt(0);
            if (currentNode == null) continue;
            closedList.Add(currentNode.position, currentNode);
            if (currentNode.position == end.position)
            {
                endFound = true;
                break;
            }            
            adjacentNodes = GetAdjacentNodes(currentNode);
            foreach(Node n in adjacentNodes)
            {
                if (n.isPassable && !closedList.ContainsKey(n.position))
                {
                    if (!openList.Contains(n))
                    {
                        n.parent = currentNode;
                        n.distance = Vector2.Distance(n.position, end.position);
                        n.cost = n.parent.cost + 1;
                        if (openList.Count == 0) openList.Add(n);
                        else
                        {
                            for (int i = 0; i < openList.Count; i++)
                            {
                                if (i != openList.Count - 1 && n.cost + n.distance > openList[i].cost + openList[i].distance) continue;
                                openList.Insert(i, n);
                                break;
                            }
                        }
                    }
                }
            }
        }

        if(!endFound)
        {
            Debug.Log("EndNotFound!");
            return new List<Vector2>() { starting };
        }

        Node temp = closedList[currentNode.position];
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

        if (row - 1 >= 0) { tempList.Add(grid[col, row - 1]); }
        if (row + 1 < gridRows) { tempList.Add(grid[col, row + 1]); }
        if (col - 1 >= 0) { tempList.Add(grid[col - 1, row]); }
        if (col + 1 < gridCols) { tempList.Add(grid[col + 1, row]); }

        return tempList;
    }
}



