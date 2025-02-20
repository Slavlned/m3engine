// Задний тайлмап для красоты

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class M3BackgroundTilemap : MonoBehaviour
{
    // тайлмап
    [SerializeField]
    private Tilemap tilemap;
    
    // тайл
    [SerializeField]
    private TileBase ruleTile;
    
    // установка
    public void PrepareTilemap(Vector3 startPosition, List<M3Pos> positions)
    {
        // Устанавливаем сортировку только для этой сцены
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0, -1, 0);
        // установка позиции
        tilemap.gameObject.transform.position = startPosition-new Vector3(0,0.055f,0f);
        // тайлы
        tilemap.ClearAllTiles();
        foreach (M3Pos pos in positions)
        {
            tilemap.SetTile(new Vector3Int(pos.X, pos.Y), ruleTile);
        }
    }
    
    // при дестрое, выключаем спец-сортировку
    private void OnDestroy()
    {
        GraphicsSettings.transparencySortMode = TransparencySortMode.Default;
    }
}