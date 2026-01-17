using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayerMask;

    private Vector3 lastPosition;

    public event Action OnLeftClicked, OnMiddleClicked, OnMiddleScrollUp, OnMiddleScrollDown, OnRightClicked, OnExit;

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

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

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
