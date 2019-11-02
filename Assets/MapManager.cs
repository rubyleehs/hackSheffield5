using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Vector2Int mapSize;
    public int mapMainNodesCount;

    public List<MapNodes> mapNodes;


    [HideInInspector]
    public int[,] mapTiles;

    private void Awake()
    {
        mapTiles = new int[mapSize.x,mapSize.y];
        GenerateMap();
    }

    private void GenerateMap()
    {
        for (int i = 0; i < mapMainNodesCount; i++)
        {
            AddNewMainMapNode();
        }
        
    }

    private void AddNewMainMapNode()
    {
        if(mapNodes == null || mapNodes.Count == 0)
        {
            mapNodes = new List<MapNodes>();
            mapNodes.Add(new MapNodes(Vector2Int.zero));
        }
        else
        {
            int rand;
            Vector2Int pos;
            while (true)
            {
                rand = (int)Random.Range(0, 11);
                pos = mapNodes[mapNodes.Count - 1].center + ((rand < 6f) ? Vector2Int.up : Vector2Int.right) * ((rand % 2 == 0) ? 1 : -1);
                if (!MapNodes.mainPathHashSet.Contains(pos))
                {
                    mapNodes.Add(new MapNodes(pos));
                    break;
                }
            }    
        }
    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < mapNodes.Count - 1; i++)
        {
            Debug.DrawLine((Vector2)mapNodes[i].center, (Vector2)mapNodes[i + 1].center, Color.red);
        }
        
    }
}

[System.Serializable]
public class MapNodes
{
    public static HashSet<Vector2Int> mainPathHashSet;

    public MapNodes prev;
    public MapNodes next;

    public Vector2Int center;
    public Vector2 nodeRadius;
    public List<MapNodes> subNodes;

    public MapNodes(Vector2Int center)
    {
        this.center = center;
        if (mainPathHashSet == null) mainPathHashSet = new HashSet<Vector2Int>();
        MapNodes.mainPathHashSet.Add(center);
    }
}


