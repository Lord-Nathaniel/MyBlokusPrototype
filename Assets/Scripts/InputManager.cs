using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class manages the keyboard and mouse input.
/// When the player interact with the game, it calls corresponding Actions.
/// </summary>
public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayerMask;

    private Vector3 lastPosition;

    public event Action OnLeftClicked, OnMiddleClicked, OnRightClicked, OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnLeftClicked?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();

        if (Input.GetMouseButtonDown(1))
            OnRightClicked?.Invoke();

        if (Input.GetMouseButtonDown(2))
            OnMiddleClicked?.Invoke();
    }

    /// <summary>
    /// Checks the UI EventSystem to know if the mouseCursor is over an UI element.
    /// </summary>
    /// <returns></returns>
    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    /// <summary>
    /// Gives the last mouse positions.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }
}
