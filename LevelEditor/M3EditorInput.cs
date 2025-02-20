using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class M3EditorInput : MonoBehaviour, IEventSystemHandler, IPointerDownHandler
{
    // инпут для редактора

    [SerializeField] private int layerMask;
    [SerializeField] private M3Editor editor;
    [SerializeField] private string inputLayer = "Input";
    [SerializeField] private float delay = 1f;
    [SerializeField] private Camera inputCamera;

    private void Start()
    {
        layerMask = LayerMask.GetMask(inputLayer);
    }

    // при нажатии
    void IPointerDownHandler.OnPointerDown(PointerEventData data)
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(inputCamera.ScreenToWorldPoint(Input.mousePosition), new Vector2(1, 1), Mathf.Infinity, layerMask);

        if (hit.collider == null) { return; }
        if (hit.collider.gameObject.GetComponent<M3EditorTile>() != null)
        {
            M3EditorTile t = hit.collider.gameObject.GetComponent<M3EditorTile>();
            if (editor.GetTool() != null)
            {
                editor.GetTool().Use(t.X, t.Y, editor.GetM3Type());
            }
        }
    }
}