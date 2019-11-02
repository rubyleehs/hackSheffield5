using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapGen mapGen;
    public Map map;

    public Vector2Int resolution;
    public int maxSplitVariance;
    public int minSubAreaSize;

    public GameObject testCell;
    public GameObject testPath;

    private void Awake()
    {
        map = mapGen.GenerateNewMap(resolution,maxSplitVariance,minSubAreaSize);

        for (int y = 0; y < map.resolution.y; y++)
        {
            for (int x = 0; x < map.resolution.x; x++)
            {
                if (map.cellTypeMap[x, y] == 0) Instantiate(testCell, new Vector2(x, y), Quaternion.identity);
                if (map.cellTypeMap[x, y] == 2)
                {
                    Instantiate(testPath, new Vector2(x, y), Quaternion.identity);
                }
            }
        }
    }

    private void Update()
    {
        map.mainArea.ShowHeirachy();
    }
}


