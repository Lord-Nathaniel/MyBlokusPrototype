using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int ID;
    public GridManager gridManager;
    public bool canRotate;
    public bool isInInventar;
    bool isDrag;
    Vector2 CurrentPos;
    Vector2 offset;
    Vector2 localMousePosition;

    RectTransform rect;
    Image sprite;
    HashSet<GameObject> cells = new HashSet<GameObject>();
    HashSet<GameObject> oldCells = new HashSet<GameObject>();
    HashSet<Vector2> tp = new HashSet<Vector2>();
    Transform CurrentParent;

    private void Start()
    {
        sprite = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        CurrentPos = rect.anchoredPosition;
        CurrentParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.parent = gridManager.transform;
        rect.anchorMin = new Vector2(.5f, .5f);
        rect.anchorMax = new Vector2(.5f, .5f);

        // Code utilisé pour libérer les cases de la grid, inutile ici
        //for (int i = 0; i < gridManager.cells.Count; i++)
        //{
        //    var cell = gridManager.cells[i];

        //    if (cell.FID == ID)
        //    {
        //        cell.setEmpty();
        //    }
        //}

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
           gridManager.GetComponent<RectTransform>(),
           eventData.position,
           eventData.pressEventCamera,
           out offset);


        offset -= rect.anchoredPosition;
        isDrag = true;
        sprite.raycastTarget = false;
        CurrentPos = rect.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //déplacement classique
        //transform.position = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridManager.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localMousePosition);

        rect.anchoredPosition = localMousePosition - offset;

        tp.Clear();
        cells.Clear();

        for (int i = 0; i < gridManager.cells.Count; i++)
        {
            var cell = gridManager.cells[i].GetComponent<CellItem>();

            if (cell.IsOverlaps(rect, ID))
            {
                Vector2 cellLocalPosition;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gridManager.GetComponent<RectTransform>(),
                    cell.rect.position,
                    eventData.pressEventCamera,
                    out cellLocalPosition);

                tp.Add(cellLocalPosition);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        sprite.raycastTarget = true;

        if (tp.Count < 1)
        {
            if (!isInInventar) transform.parent = CurrentParent;
            else
            {
                rect.anchoredPosition = CurrentPos;

                foreach (var c2 in oldCells)
                    c2.GetComponent<CellItem>().SetFilled(ID);
            }

            //foreach (var c2 in cells)
            //    c2.GetComponent<CellItem>().UpdateCell();

            return;
        }

        rect.anchoredPosition = tp.First();

        for (int i = 0; i < gridManager.cells.Count; i++)
        {
            var cell = gridManager.cells[i];
            if (cell.GetComponent<CellItem>().IsOverlaps(rect, ID))
                cells.Add(cell);
        }

        foreach (var cell in cells)
        {
            if (cell.GetComponent<CellItem>().FID != -1
                && cell.GetComponent<CellItem>().FID != ID
                || cell.GetComponent<CellItem>().fill)
            {
                if (!isInInventar) transform.parent = CurrentParent;
                else
                {
                    rect.anchoredPosition = CurrentPos;

                    //foreach (var c2 in oldCells)
                    //    c2.GetComponent<CellItem>().setFilled(ID);
                }

                //foreach (var c2 in cells)
                //    c2.UpdateCell();

                return;
            }
        }

        //foreach (var c2 in cells)
        //    c2.GetComponent<CellItem>().setFilled(ID);

        //gridManager.AddItem(this);
        CurrentPos = tp.First();

        oldCells.Clear();
        foreach (var c in cells)
            oldCells.Add(c);
    }

    void UpdateSlots()
    {
        //foreach (var c2 in cells)
        //    c2.UpdateCell();

        tp.Clear();
        cells.Clear();

        foreach (var cell in gridManager.cells)
        {
            if (cell.GetComponent<CellItem>().IsOverlaps(rect, ID))
            {
                cells.Add(cell);

                Vector2 cellLocalPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gridManager.GetComponent<RectTransform>(),
                    cell.GetComponent<CellItem>().rect.position,
                    null,
                    out cellLocalPosition);

                tp.Add(cellLocalPosition);
            }
        }
    }

    private void Update()
    {
        if (isDrag)
        {

            if (canRotate && Input.mouseScrollDelta.y >= 1 || canRotate && Input.mouseScrollDelta.y <= -1)
            {
                if (rect.pivot.x == 0)
                {
                    rect.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    rect.pivot = new Vector2(1, 1);
                    //offset += new Vector2(rect.sizeDelta.x / 2, rect.sizeDelta.y / 2);
                }
                else if (rect.pivot.x == 1)
                {
                    rect.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    rect.pivot = new Vector2(0, 1);
                    //offset -= new Vector2(rect.sizeDelta.x / 2, rect.sizeDelta.y / 2);
                }

                rect.anchoredPosition = localMousePosition - offset;
                UpdateSlots();
            }
        }
    }
}

