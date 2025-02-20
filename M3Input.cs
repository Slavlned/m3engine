using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
    UP,
    DOWN,
    RIGHT,
    LEFT
}

public class M3Input : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private int layerMask;
    [SerializeField] private M3Board Board;
    [SerializeField] private string inputLayer = "Input";
    [SerializeField] private float delay = 1f;
    [SerializeField] private Camera inputCamera;
    private Direction direction;
    [SerializeField]
    private M3Object Selected;
    private Vector2 Point;
    private bool onDelay;
    private M3Object lastSelected;
    [SerializeField] private M3InputType inputType = M3InputType.DEFAULT;
    // ивент инпута
    [SerializeField] public Action<M3Pos> onBoosterInput;
    // задержка инпута
    private WaitForSeconds delayWaitForSeconds;
    
    private void Start()
    {
        layerMask = LayerMask.GetMask(inputLayer);
        delayWaitForSeconds = new WaitForSeconds(delay);
    }

    IEnumerator Delay()
    {
        yield return delayWaitForSeconds;
        onDelay = false;
        Selected = null;
        Point = new Vector2(0, 0);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData data)
    {
        // Debug.Log(AvailableForRaycast());
        if (inputType == M3InputType.BOOSTER) return;
        if (!AvailableForRaycast()) { lastSelected = null; return; }

        onDelay = true;
        StartCoroutine(Delay());

        RaycastHit2D hit;
        hit = Physics2D.Raycast(inputCamera.ScreenToWorldPoint(Input.mousePosition), transform.TransformDirection(Vector2.up), Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.GetComponent<M3Object>() == Selected)
            {
                return;
            }
        }

        if (Selected == null)
        {
            return;
        }

        Vector2 direction = Point - (Vector2)inputCamera.ScreenToWorldPoint(Input.mousePosition);


        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                M3Tile tile = Board.TileAt(
                    Selected.GetTile().Pos().X - 1,
                    Selected.GetTile().Pos().Y
                );                
                M3Object o = null;
                if (tile != null)
                {
                    o = tile.GetObject();
                }
                else
                {
                    return;
                }

                if (o != null)
                {
                    Board.SwapInput(
                        new M3Swap(o, Selected)
                    );
                }
                lastSelected = null;
            }
            else
            {
                M3Tile tile = Board.TileAt(
                    Selected.GetTile().Pos().X + 1,
                    Selected.GetTile().Pos().Y
                );                
                M3Object o = null;
                if (tile != null)
                {
                    o = tile.GetObject();
                }
                else
                {
                    return;
                }

                if (o != null)
                {
                    Board.SwapInput(
                        new M3Swap(o, Selected)
                    );
                }
                lastSelected = null;
            }
        }
        else
        {
            if (direction.y > 0)
            {
                M3Tile tile = Board.TileAt(
                    Selected.GetTile().Pos().X,
                    Selected.GetTile().Pos().Y - 1
                );
                M3Object o = null;
                if (tile != null)
                {
                    o = tile.GetObject();
                }
                else
                {
                    return;
                }

                if (o != null)
                {
                    Board.SwapInput(
                        new M3Swap(o, Selected)
                    );
                }
                lastSelected = null;
            }
            else
            {
                M3Tile tile = Board.TileAt(
                    Selected.GetTile().Pos().X,
                    Selected.GetTile().Pos().Y + 1
                );
                M3Object o = null;
                if (tile != null)
                {
                    o = tile.GetObject();
                }
                else
                {
                    return;
                }

                if (o != null)
                {
                    Board.SwapInput(
                        new M3Swap(o, Selected)
                    );
                }
                lastSelected = null;
            }
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData data)
    {
        if (!AvailableForRaycast()) { return; }

        RaycastHit2D hit;
        hit = Physics2D.Raycast(inputCamera.ScreenToWorldPoint(Input.mousePosition), new Vector2(1, 1), Mathf.Infinity, layerMask);

        if (hit.collider == null) { return; }

        if (hit.collider.gameObject.GetComponent<M3Object>() != null)
        {
            if (hit.collider.gameObject.GetComponent<M3Object>().GetState() == M3State.IDLE)
            {
                Selected = hit.collider.gameObject.GetComponent<M3Object>();
                if (inputType == M3InputType.BOOSTER)
                {
                    if (Selected.GetTile() != null)
                    {
                        onBoosterInput?.Invoke(Selected.GetTile().Pos());
                    }
                    return;
                }

                if (lastSelected != Selected)
                {
                    lastSelected = hit.collider.gameObject.GetComponent<M3Object>();
                }
                else
                {
                    lastSelected = null;
                }

                Point = inputCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData data)
    {

    }

    public bool AvailableForRaycast()
    {
        if (Board.GetLock()) { return false; }
        if (!Board.Idle()) { return false; }
        if (onDelay) { return false; }
        return true;
    }

    public void SetType(M3InputType _type)
    {
        // устанавливаем тип инпута
        inputType = _type;
    }
}