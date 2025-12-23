using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float width = 64;
    [SerializeField] private float height = 64;

    [SerializeField] private Sprite baseImage;
    [SerializeField] private Sprite highlightImage;

    private Image image;

    //RectTransform rt;

    //Vector2 positionOnTheGrid = new Vector2();
    //Vector2Int gridPosition = new Vector2Int();

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = baseImage;
    }

    public Vector2Int GetGridPosition(Vector2 mousePosition)
    {
        throw new System.NotImplementedException("GetGridPosition");
        //positionOnTheGrid.x = mousePosition.x - rt.position.x;
        //positionOnTheGrid.y = mousePosition.y - rt.position.y;

        //gridPosition.x = (int)(positionOnTheGrid.x / width);
        //gridPosition.y = (int)(positionOnTheGrid.y / height);

        //return gridPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = highlightImage;


        //Vector2 position = GetGridPosition(Input.mousePosition);
        //String x = null;
        //String y = null;

        //Debug.Log($"Cell {x},{y} entered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = baseImage;
    }
}
