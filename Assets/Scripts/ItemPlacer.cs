using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPlacer : MonoBehaviour
{
    public Camera cam;
[Header("Placement")]
public LayerMask blockedLayers;
    private bool isPlacing = false;
    private string currentItem = "";
    private GameObject previewObject;
private bool canPlace = true;

    void Update()
{
    if (isPlacing)
    {
        FollowMouse();
        CheckPlacementValidity();

        // click to place
        if (Input.GetMouseButtonDown(0))
        {
            ConfirmPlacement();
        }

        // cancel
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }
    else
    {
        // begin placement
        if (Input.GetMouseButtonDown(0))
        {
            StartPlacing();
        }
    }
}
    void StartPlacing()
    {
        if (PlayerInventory.Instance == null)
            return;

        if (string.IsNullOrEmpty(PlayerInventory.Instance.selectedItem))
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        currentItem = PlayerInventory.Instance.selectedItem;

        GameObject prefab =
            ItemDatabase.Instance.GetPrefab(currentItem);

        if (prefab == null)
        {
            Debug.Log("No prefab for " + currentItem);
            return;
        }

        previewObject = Instantiate(prefab);

        // ghost transparency
        SpriteRenderer[] renderers =
            previewObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }

        isPlacing = true;
    }

    void FollowMouse()
    {
        if (previewObject == null)
            return;

        Vector3 mouse =
            cam.ScreenToWorldPoint(Input.mousePosition);

        mouse.z = 0f;

        previewObject.transform.position = mouse;
    }

    void ConfirmPlacement()
{
    if (!canPlace)
    {
        Debug.Log("Invalid placement");
        return;
    }

    SpriteRenderer[] renderers =
        previewObject.GetComponentsInChildren<SpriteRenderer>();

    foreach (SpriteRenderer sr in renderers)
    {
        sr.color = Color.white;
    }

    PlayerInventory.Instance.RemoveSelectedHotbarItem();

    previewObject = null;
    currentItem = "";
    isPlacing = false;
}

    void CancelPlacement()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        currentItem = "";
        isPlacing = false;
    }
    void CheckPlacementValidity()
{
    if (previewObject == null)
        return;

    Collider2D col =
        previewObject.GetComponent<Collider2D>();

    if (col == null)
    {
        canPlace = true;
        return;
    }

    Collider2D hit =
        Physics2D.OverlapBox(
            col.bounds.center,
            col.bounds.size,
            0f,
            blockedLayers
        );

    canPlace = (hit == null);

    SetGhostColor();
}
void SetGhostColor()
{
    if (previewObject == null)
        return;

    SpriteRenderer[] renderers =
        previewObject.GetComponentsInChildren<SpriteRenderer>();

    foreach (SpriteRenderer sr in renderers)
    {
        Color c = sr.color;

        if (canPlace)
            sr.color = new Color(0f, 1f, 0f, 0.5f); // green
        else
            sr.color = new Color(1f, 0f, 0f, 0.5f); // red
    }
}
}