using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    //-1 walls, 0 uncheck, 1 room, 2 connect
    public Map GenerateNewMap(Vector2Int resolution, int maxSplitVariance, int minSubAreaSize)
    {
        Map outputMap = new Map();
        outputMap.resolution = resolution;
        outputMap.cellTypeMap = new int[resolution.x, resolution.y];
        outputMap.mainArea = new SubArea(Vector2Int.one, resolution - Vector2Int.one * 2);
        LayeredSplit(outputMap.mainArea, maxSplitVariance, minSubAreaSize, 5);
        ConvertSubAreasToRooms(ref outputMap);
        outputMap.mainArea.GiveUpAndForceBoringCorridors(ref outputMap);

        return outputMap;
    }

    public void LayeredSplit(SubArea subArea, int maxSplitVariance, int minSubAreaSize, int count)
    {
        if (count <= 0) return;
        subArea.SplitArea(maxSplitVariance, minSubAreaSize);
        if (subArea.childsSubArea == null) return;

        for (int i = 0; i < subArea.childsSubArea.Count; i++)
        {
            LayeredSplit(subArea.childsSubArea[i], maxSplitVariance, minSubAreaSize, count - 1);
        }
    }

    public void CreateMainConnections(ref Map map,SubArea subArea)
    {
        if (subArea.childsSubArea == null) return;
        for (int i = 0; i < subArea.childsSubArea.Count; i++)
        {
            ExtendConnections(ref map, subArea.childsSubArea[i]);
            CreateMainConnections(ref map, subArea.childsSubArea[i]);
        }

        //if(subArea.childsSubArea.Count ==2) Connect(ref map, subArea.childsSubArea[0], subArea.childsSubArea[1]);
    }

    public void ConvertSubAreasToRooms(ref Map map)
    {
        Queue<SubArea> q = new Queue<SubArea>();
        q.Enqueue(map.mainArea);
        SubArea subArea;
        while (q.Count > 0)
        {
            subArea = q.Dequeue();
            if (subArea.childsSubArea == null)
            {
                if(!subArea.isTooSmall) FillRectangle(ref map.cellTypeMap, 1, subArea.anchor, subArea.anchor + subArea.size - Vector2Int.one);
            }
            else
            {
                for (int i = 0; i < subArea.childsSubArea.Count; i++)
                {
                    q.Enqueue(subArea.childsSubArea[i]);
                }
            }
        }
    }

    public void FillRectangle(ref int[,] map, int value, Vector2Int start, Vector2Int end)
    {
        for (int dy = start.y; dy <= end.y; dy++)
        {
            for (int dx = start.x; dx <= end.x; dx++)
            {
                map[dx, dy] = value;
            }
        }
    }

    public bool ExtendConnections(ref Map map, SubArea subArea)
    {
        if (subArea.connection == null) return false;
        if (subArea.connection.Count == 0) Debug.Log("Meow");
        for (int i = 0; i < subArea.connection.Count; i++)
        {
            if (subArea.splitX)
            {
                int dx = 0;
                while (subArea.connection[i].x + dx < map.resolution.x - 1 && map.cellTypeMap[subArea.connection[i].x + dx, subArea.connection[i].y] == 0)
                {
                    map.cellTypeMap[subArea.connection[i].x + dx, subArea.connection[i].y] = 2;
                    dx++;
                }
                dx = -1;
                while (subArea.connection[i].x + dx > 0 && map.cellTypeMap[subArea.connection[i].x + dx, subArea.connection[i].y] == 0)
                {
                    map.cellTypeMap[subArea.connection[i].x + dx, subArea.connection[i].y] = 2;
                    dx--;
                }
            }
            else
            {
                int dy = 0;
                while (subArea.connection[i].y + dy < map.resolution.y - 1 && map.cellTypeMap[subArea.connection[i].x, subArea.connection[i].y + dy] == 0)
                {
                    map.cellTypeMap[subArea.connection[i].x, subArea.connection[i].y + dy] = 2;
                    dy++;
                }
                dy = -1;
                while (subArea.connection[i].y + dy > 0 && map.cellTypeMap[subArea.connection[i].x, subArea.connection[i].y + dy] == 0)
                {
                    map.cellTypeMap[subArea.connection[i].x, subArea.connection[i].y + dy] = 2;
                    dy--;
                }
            }
        }
        return true;
    }
    
    public bool Connect(ref Map map, SubArea a, SubArea b)
    {
        List<int> overlap = null;
        bool horizontalOverlap = true;
        int temp = 0;

        overlap = GetOverlap(a.anchor.y, a.anchor.y + a.size.y - 1, b.anchor.y, b.anchor.y + b.size.y - 1);
        if(overlap != null)
        {
            Debug.Log((a.anchor.x - 1) + " | " + (b.anchor.x + b.size.x));
            Debug.Log((b.anchor.x - 1) + " | " + ( a.anchor.x + a.size.x));
            if (a.anchor.x - 1 == b.anchor.x + b.size.x) temp = a.anchor.x - 1;
            else if (b.anchor.x - 1 == a.anchor.x + a.size.x) temp = b.anchor.x - 1;
            else overlap = null;
        }
        if (overlap == null)
        {
            overlap = GetOverlap(a.anchor.x, a.anchor.x + a.size.x - 1, b.anchor.x, b.anchor.x + b.size.x - 1);
            if (overlap != null)
            {
                horizontalOverlap = false;
                if (a.anchor.y - 1 == b.anchor.y + b.size.y) temp = a.anchor.y - 1;
                else if (b.anchor.y - 1 == a.anchor.y + a.size.y) temp = b.anchor.y - 1;
                else overlap = null;
            }

        }
        if (overlap == null) return false;
        else
        {
            int randPos = overlap[Random.Range(0, overlap.Count)];

            if (horizontalOverlap)
            {
                int dx = 0;
                while (temp + dx < map.resolution.x - 1 && map.cellTypeMap[temp + dx, randPos] == 0)
                {
                    map.cellTypeMap[temp + dx, randPos] = 2;
                    dx++;
                }
                dx = -1;
                while (temp + dx > 0 && map.cellTypeMap[temp + dx, randPos] == 0)
                {
                    map.cellTypeMap[temp + dx, randPos] = 2;
                    dx--;
                }
            }
            else
            {
                int dy = 0;
                while (temp + dy < map.resolution.y -1 && map.cellTypeMap[randPos, temp + dy] == 0)
                {
                    map.cellTypeMap[randPos, temp + dy] = 2;
                    dy++;
                }
                dy = -1;
                while (temp + dy > 0 && map.cellTypeMap[randPos, temp + dy] == 0)
                {
                    map.cellTypeMap[randPos, temp + dy] = 2;
                    dy--;
                }

            }

            return true;
        }
    }
    
    public List<int> GetOverlap(int start1, int end1, int start2, int end2)
    {
        if (start1 <= end2 && start2 <= end1)
        {
            List<int> overlap = new List<int>();
            for (int i = Mathf.Max(start1,start2); i <= Mathf.Min(end1,end2); i++)
            {
                overlap.Add(i);
            }
            return overlap;
        }
        else return null;
    }
}

public class SubArea
{
    public SubArea parentSubArea;
    public List<SubArea> childsSubArea;
    public List<Vector2Int> connection;
    public Vector2Int anchor;
    public Vector2Int size;
    public bool isTooSmall = false;
    public bool splitX;
    public int variance;

    public SubArea(Vector2Int anchor, Vector2Int size, SubArea parentSubArea = null)
    {
        this.anchor = anchor;
        this.size = size;
    }

    public void SplitArea(int maxSplitVariance, int minSubAreaSize)
    {
        if (size.x < 2 * minSubAreaSize || size.y < 2 * minSubAreaSize)
        {
            if (size.x < minSubAreaSize || size.y < minSubAreaSize) isTooSmall = true;
            return;
        }
        childsSubArea = new List<SubArea>();
        connection = new List<Vector2Int>();

        splitX = Random.Range(-Mathf.Pow(size.x, 4), Mathf.Pow(size.y, 4)) < 0;
        variance = Random.Range(-maxSplitVariance, maxSplitVariance);

        if (splitX)
        {
            if (size.x * 0.5f + variance - 1 > minSubAreaSize) childsSubArea.Add(new SubArea(anchor, new Vector2Int(Mathf.FloorToInt(size.x * 0.5f) + variance, size.y), this));
            if (size.x * 0.5f - variance - 1 > minSubAreaSize) childsSubArea.Add(new SubArea(anchor + Vector2Int.right * (Mathf.FloorToInt(size.x * 0.5f) + variance + 1), new Vector2Int(Mathf.FloorToInt(size.x * 0.5f) - variance - 1, size.y), this));
            if (size.x * 0.5f + variance - 1 > minSubAreaSize && size.x * 0.5f - variance - 1 > minSubAreaSize) connection.Add(new Vector2Int(anchor.x + Mathf.FloorToInt(size.x * 0.5f) + variance, anchor.y + Random.Range(0, size.y)));
        }
        else
        {
            if (size.y * 0.5f + variance - 1 > minSubAreaSize) childsSubArea.Add(new SubArea(anchor, new Vector2Int(size.x, Mathf.FloorToInt(size.y * 0.5f) + variance), this));
            if (size.y * 0.5f - variance - 1 > minSubAreaSize) childsSubArea.Add(new SubArea(anchor + Vector2Int.up * (Mathf.FloorToInt(size.y * 0.5f) + variance + 1), new Vector2Int(size.x, Mathf.FloorToInt(size.y * 0.5f) - variance - 1), this));
            if (size.y * 0.5f + variance - 1 > minSubAreaSize && size.y * 0.5f - variance - 1 > minSubAreaSize) connection.Add(new Vector2Int(anchor.x + Random.Range(0, size.x), anchor.y + (Mathf.FloorToInt(size.y * 0.5f) + variance)));
        }

        if (childsSubArea.Count == 0) childsSubArea = null;
    }

    public void ShowHeirachy()
    {
        if (childsSubArea == null) return;
        for (int i = 0; i < childsSubArea.Count; i++)
        {
            Debug.DrawLine(anchor + new Vector2(size.x * 0.5f, size.y * 0.5f), childsSubArea[i].anchor + new Vector2(childsSubArea[i].size.x * 0.5f, childsSubArea[i].size.y * 0.5f), Color.red);
            childsSubArea[i].ShowHeirachy();
        }
    }

    public void GiveUpAndForceBoringCorridors(ref Map map)
    {
        if (childsSubArea == null) return;
        if (childsSubArea.Count >= 2)
        {
            for (int x = childsSubArea[0].anchor.x + childsSubArea[0].size.x / 2; x <= childsSubArea[1].anchor.x + childsSubArea[1].size.x / 2; x++)
            {
                for (int y = childsSubArea[0].anchor.y + childsSubArea[0].size.y / 2; y <= childsSubArea[1].anchor.y + childsSubArea[1].size.y / 2; y++)
                {
                    map.cellTypeMap[x, y] = 2;
                }
            }
        }
        for (int i = 0; i < childsSubArea.Count; i++)
        {
            childsSubArea[i].GiveUpAndForceBoringCorridors(ref map);//
        }
    }
}
