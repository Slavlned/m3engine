using UnityEngine;

public class M3EditorTile : MonoBehaviour
{
    // клеточка редактора
    public M3EditorObj obj;
    public GameObject upLayerObj;
    public GameObject gravityLayerObj;
    public int X;
    public int Y;
    public bool IsGenerator;
    public bool IsEmpty;
    public bool IsDeliverer;
    public M3GravityDir gravityDir;
    public M3EditorCover bottomCover;
}