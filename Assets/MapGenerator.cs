using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using PathFinding.Scripts.UIManagers;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    MapGridDataManager m_GridData;
    
    [SerializeField]
    MapDataScriptableObject m_MapData;

    [SerializeField]
    float m_OffsetX;

    [SerializeField]
    float m_OffsetY;

    [SerializeField]
    float m_Magnification;

    // Start is called before the first frame update
    void Start()
    {
        GenerateTileMap();
    }

    public void SetMapData(MapDataScriptableObject newMapData)
    {
        m_MapData = newMapData;
    }

    void GenerateTileMap()
    {
        var width = m_GridData.mapWidth;
        var height = m_GridData.mapHeight;
        
        var pathTilemap = m_GridData.pathTileMap;
        var colliderTileMap = m_GridData.colliderObjectContainer;
        
        var pathTiles = new Tuple<List<Vector3Int>, List<TileBase>>(new List<Vector3Int>(), new List<TileBase>());
        var colliderTiles = new Tuple<List<Vector3Int>, List<GameObject>>(new List<Vector3Int>(), new List<GameObject>());
        
        for (int xCoord = 0; xCoord < width; xCoord++)
        {
            for (int yCoord = 0; yCoord < height; yCoord++)
            {
                var perlinNoise = GetPerlinNoise(xCoord, yCoord);

                foreach (var tileData in m_MapData.mapTiles)
                {
                    if (perlinNoise <= tileData.percentage)
                    {
                        if (tileData.prefab != null)
                        {
                            var obstacleObject = Instantiate(tileData.prefab, m_GridData.colliderObjectContainer.transform);
                            var currentPosInGrid = new Vector3Int(xCoord, yCoord, 0);
                            obstacleObject.transform.position = PathfindingManager.tilemapGrid.CellToWorld(currentPosInGrid);
                            
                            colliderTiles.Item1.Add(currentPosInGrid);
                            colliderTiles.Item2.Add(obstacleObject);
                            break;
                        }
                        else
                        {
                            pathTiles.Item1.Add(new Vector3Int(xCoord,yCoord,0));
                            pathTiles.Item2.Add(tileData.tile);
                            break;
                        }
                    }
                }
            }
        }
        
        colliderTileMap.SetObjects(colliderTiles.Item1.ToArray(), colliderTiles.Item2.ToArray());
        pathTilemap.SetTiles(pathTiles.Item1.ToArray(),pathTiles.Item2.ToArray());
    }

    float GetPerlinNoise(int x, int y)
    {
        var xNoise = (x - m_OffsetX) / m_Magnification;
        var yNoise = (y - m_OffsetY) / m_Magnification;
        
        
        var rawPerlin = Mathf.PerlinNoise(xNoise, yNoise);

        return Mathf.Clamp(rawPerlin, 0, 1);
    }
}
