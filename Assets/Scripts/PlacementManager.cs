using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    private GameObject objectToPlace;

    void Awake()
    {
        Instance = this;
    }

    public void StartPlacement(GameObject prefab)
    {
        objectToPlace = prefab;
    }

    void Update()
    {
        if (objectToPlace == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = GetMouseWorldPosition();
            Instantiate(objectToPlace, pos, Quaternion.identity);
            objectToPlace = null;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}