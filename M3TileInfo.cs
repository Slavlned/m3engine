using System.Collections.Generic;

[System.Serializable]
public class M3Param
{
    public string Name; 
    public string Value;
}

[System.Serializable]
public class M3ObjectInfo
{
    public M3ObjectType type;
    public bool IsRandom;
    public List<M3Param> parameters;
    // TO:DO public M3ObjectSettings settings;
}

[System.Serializable]
public class M3TileInfo
{
    // ифно о тайле
    public M3Pos Pos;
    public M3ObjectInfo Info;
    // генератор ли
    public bool IsGenerator;
    // деливерер
    public bool IsDeliverer;
    // гравитация
    public M3GravityDir GravityDir;
    // не заполнять ли вовсе (аддитивный тайл)
    // для объектов у которых больше 1 клетки
    // public bool IsAdditional;
}