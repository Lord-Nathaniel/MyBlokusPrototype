using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Cell Size")]
    [SerializeField] private float width = 64;
    [SerializeField] private float height = 64;

    [Header("Cell Sprites")]
    [SerializeField] private Sprite baseImage;
    [SerializeField] private Sprite highlightImage;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = baseImage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = highlightImage;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = baseImage;
    }
}
