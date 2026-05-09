using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPlacer : MonoBehaviour
{
    public Camera cam;

    [Header("Placement")]
    public LayerMask blockedLayers;

    private bool isPlacing = false;
    private bool canPlace = true;
    public float placementSpacing = 1.5f;

    private string currentItem = "";
    private GameObject previewObject;

    void Update()
    {
        if (isPlacing)
        {
            FollowMouse();
            CheckPlacementValidity();

            // confirm placement
            if (Input.GetMouseButtonDown(0))
            {
                ConfirmPlacement();
            }

            // cancel placement
            if (Input.GetMouseButtonDown(1) ||
                Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
            }
        }
        else
        {
            // start placing
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

        // don't place through UI
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

        // disable gameplay logic while previewing
        SetPreviewMode(previewObject, true);

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

        // turn real object back on
        SetPreviewMode(previewObject, false);

        // restore normal sprite color
        SpriteRenderer[] renderers =
            previewObject.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in renderers)
        {
            sr.color = Color.white;
        }

        // remove item from inventory
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

    canPlace = true;

    Vector2 pos = previewObject.transform.position;

    // blocked layers check
    Collider2D hit =
        Physics2D.OverlapCircle(
            pos,
            placementSpacing,
            blockedLayers
        );

    if (hit != null)
        canPlace = false;

    // too close to another placed object
    Collider2D[] nearby =
        Physics2D.OverlapCircleAll(
            pos,
            placementSpacing
        );

    foreach (Collider2D c in nearby)
    {
        if (c.gameObject == previewObject)
            continue;

        // only check placed objects
        if (c.CompareTag("Item"))
        {
            canPlace = false;
            break;
        }
        if (Physics2D.OverlapCircle(pos, 1f, LayerMask.GetMask("Player")))
    canPlace = false;
    }

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
            if (canPlace)
                sr.color = new Color(0f, 1f, 0f, 0.5f);
            else
                sr.color = new Color(1f, 0f, 0f, 0.5f);
        }
    }

    void SetPreviewMode(GameObject obj, bool previewMode)
{
    if (obj == null)
        return;

    // disable all scripts
    MonoBehaviour[] scripts =
        obj.GetComponentsInChildren<MonoBehaviour>(true);

    foreach (MonoBehaviour script in scripts)
    {
        // don't disable this placer script if somehow referenced
        if (script == this)
            continue;

        script.enabled = !previewMode;
    }

    // disable colliders
    Collider2D[] cols =
        obj.GetComponentsInChildren<Collider2D>(true);

    foreach (Collider2D col in cols)
        col.enabled = !previewMode;
}
}