using System;
using Unity.VisualScripting;
using UnityEngine;

/**
 * Покрытие
 */
public class M3Cover : MonoBehaviour
{
    // тип
    [SerializeField]
    private M3ObjectType _type;

    // получение типа
    public M3ObjectType GetType() => _type;
    
    // ивенты
    public Action onCoverDamage;
    // доска
    [SerializeField]
    private M3Board _board;
    // тайл
    [SerializeField]
    private M3Tile _tile;
    
    /*
     * Геттеры и сеттеры
     */
    public M3Tile GetTile() => _tile;
    public void SetTile(M3Tile tile) => _tile = tile;
    public M3Board GetBoard() => _board;
    public void SetBoard(M3Board board) => _board = board;
    
    // дамаг
    public void Damage()
    {
        onCoverDamage?.Invoke();
    }
}