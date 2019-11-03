using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public Vector2Int resolution;
    public int[,] cellTypeMap;
    public SubArea mainArea;


    public Vector2Int RequestEmptyTile()
    {
        Vector2Int output = new Vector2Int(Random.Range(1, cellTypeMap.GetLength(0) - 2), Random.Range(1, cellTypeMap.GetLength(1) - 2));
        while(cellTypeMap[output.x, output.y] == 0)
        {
            output = new Vector2Int(Random.Range(1, cellTypeMap.GetLength(0) - 2), Random.Range(1, cellTypeMap.GetLength(1) - 2));
        }

        return output;
    }
}
