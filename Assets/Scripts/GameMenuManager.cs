using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    [Header("Menu Settings")]
    [SerializeField] private GameObject menuZone;
    [SerializeField] private Button menuCloseButton;
    [SerializeField] private Button menuOptionsButton;
    [SerializeField] private Button menuMainMenuButton;

    private void Awake()
    {
        ServiceManager.Register(this);
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister<GameMenuManager>();
    }

    private void Hide(GameObject toHide)
    {
        toHide.SetActive(false);
    }

    private void Toggle(GameObject toToggle)
    {
        if (toToggle.activeInHierarchy)
            toToggle.SetActive(false);
        else
            toToggle.SetActive(true);
    }
}
