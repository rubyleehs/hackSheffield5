using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapGen mapGen;
    public static Map map;
    public Transform wallParent;

    public Vector2Int resolution;
    public int maxSplitVariance;
    public int minSubAreaSize;

    public static PathFinderAStar pathFinder;

    public int numCoins;

    public GameObject testCell;
    public GameObject testPath;
    public GameObject testCoin;

    private void Awake()
    {
        map = mapGen.GenerateNewMap(resolution,maxSplitVariance,minSubAreaSize);
        pathFinder = new PathFinderAStar(map.cellTypeMap, new HashSet<int>() { 1, 2 });

        for (int y = 0; y < map.resolution.y; y++)
        {
            for (int x = 0; x < map.resolution.x; x++)
            {
                if (map.cellTypeMap[x, y] == 0) Instantiate(testCell, new Vector2(x , y), Quaternion.identity, wallParent);
                if (map.cellTypeMap[x, y] == 2)
                {
                    //Instantiate(testPath, new Vector2(x, y), Quaternion.identity);
                }
            }
        }

        for (int i = 0; i < numCoins; i++) {
            Instantiate(testCoin, getEmpty(), Quaternion.identity);
        }
    }

    public Vector2 getEmpty() {
        while (true) {
            int x = Random.Range(0, resolution.x - 1);
            int y = Random.Range(0, resolution.y - 1);

            if (map.cellTypeMap[x, y] == 1) {
                return new Vector2 (x, y);
            }
        }
    }

    private void Update()
    {
        map.mainArea.ShowHeirachy();
    }
}


