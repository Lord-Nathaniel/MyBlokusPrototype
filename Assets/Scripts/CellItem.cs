using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(RectTransform))]
public class CellItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Cell Size")]
    [SerializeField] private float width = 64;
    [SerializeField] private float height = 64;

    [Header("Cell Sprites")]
    [SerializeField] private Sprite baseImage;
    [SerializeField] private Sprite highlightImage;
    [SerializeField] private Sprite forbiddenImage;
    [SerializeField] private Sprite filledImage;

    private Image image;

    public RectTransform rect { get; private set; }
    public bool fill { get; private set; }
    public int FID { get; private set; }

    private void Start()
    {
        image = GetComponent<Image>();
        image.sprite = baseImage;
        rect = GetComponent<RectTransform>();
        FID = -1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = highlightImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = baseImage;
    }

    public void SetFilled(int FID)
    {
        this.FID = FID;
        fill = true;
    }

    public void SetEmpty()
    {
        FID = -1;
        fill = false;
    }

    public bool IsOverlaps(RectTransform rectB, int id)
    {
        Rect rectAWorld = GetWorldSpaceRect(rect);
        Rect rectBWorld = GetWorldSpaceRect(rectB);

        bool hover = rectAWorld.Overlaps(rectBWorld);

        if (hover && !fill)
            image.sprite = highlightImage;
        else if (hover && fill && id != FID)
            image.sprite = forbiddenImage;
        else if (!fill)
            image.sprite = baseImage;
        else if (fill)
            image.sprite = filledImage;

        return hover;
    }

    Rect GetWorldSpaceRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector2 min = new Vector2(corners.Min(c => c.x), corners.Min(c => c.y));
        Vector2 size = new Vector2(corners.Max(c => c.x) - min.x, corners.Max(c => c.y) - min.y);
        return new Rect(min, size);
    }
}
