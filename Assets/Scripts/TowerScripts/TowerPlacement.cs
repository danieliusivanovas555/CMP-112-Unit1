//Dan Ivanovas
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    //I was so dumb I could've used a spherecast to check collision placement and just increase the value 
    //To our needs, Trying to get the 
    public GameObject towerPrefab;
    public GameObject previewPrefab;

    public LayerMask placementLayer;
    public LayerMask raycastLayers;
    public LayerMask blockedLayers;
    public float maxSlopeAngle = 20f;

    public RectTransform shopPanel;

    public Material validMaterial;
    public Material invalidMaterial;

    private Camera gameCamera;
    private CurrencyManager currencyManager;
    private BaseTowerScript towerStats;
    private TowerSounds sounds;

    private GameObject preview;
    private Renderer[] previewRenderers;

    private Vector3 placementPosition;
    private bool isDragging;
    private bool canPlace;
    private bool previousValid;
    private bool materialSet;

    void Start()
    {
        //Find the camera and managers
        gameCamera = Camera.main;
        currencyManager = FindFirstObjectByType<CurrencyManager>();
        towerStats = towerPrefab.GetComponent<BaseTowerScript>();
        sounds = FindFirstObjectByType<TowerSounds>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Prevent tower placement when the game is paused
        if (Time.timeScale == 0f)
        {
            return;
        }

        //Create the tower preview
        preview = Instantiate(previewPrefab);
        previewRenderers = preview.GetComponentsInChildren<Renderer>();

        //Disable preview colliders so they don't interfere with raycasts
        foreach (Collider collider in preview.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }

        isDragging = true;
        materialSet = false;

        UpdatePreview(eventData);

        //Play the selection sound
        if (sounds != null)
        {
            sounds.PlaySelection();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Update the preview while dragging
        if (isDragging)
        {
            UpdatePreview(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            return;
        }

        UpdatePreview(eventData);

        //Check if the tower can be placed and purchased
        if (canPlace && Time.timeScale > 0f && currencyManager.SpendCurrency(towerStats.towerCost))
        {
            GameObject tower = Instantiate(towerPrefab, placementPosition, towerPrefab.transform.rotation);

            tower.GetComponent<BaseTowerScript>().PlaceTower();

            //Play the placement sound
            if (sounds != null)
            {
                sounds.PlayPlacement();
            }
        }

        //Remove the preview after dragging
        Destroy(preview);
        preview = null;

        isDragging = false;
        canPlace = false;
    }

    void UpdatePreview(PointerEventData eventData)
    {
        canPlace = false;

        //Hide the preview when hovering over the shop
        if (shopPanel != null && RectTransformUtility.RectangleContainsScreenPoint(shopPanel, eventData.position, eventData.pressEventCamera))
        {
            preview.SetActive(false);
            return;
        }

        //Check where the player is pointing
        Ray ray = gameCamera.ScreenPointToRay(eventData.position);

        if (!Physics.Raycast(ray, out RaycastHit hit, 500f, raycastLayers, QueryTriggerInteraction.Ignore))
        {
            preview.SetActive(false);
            return;
        }

        placementPosition = hit.point;
        preview.transform.position = placementPosition;
        preview.SetActive(true);

        //Check if the surface is suitable for placement
        bool validSurface = (placementLayer.value & (1 << hit.collider.gameObject.layer)) != 0;
        bool validSlope = Vector3.Angle(hit.normal, Vector3.up) <= maxSlopeAngle;

        //Check if the position is blocked or too expensive
        bool blocked = IsBlocked();
        bool affordable = currencyManager.CurrentCurrency >= towerStats.towerCost;

        canPlace = validSurface && validSlope && !blocked && affordable;

        UpdateMaterial();
    }

    bool IsBlocked()
    {
        //Check for obstacles within the tower's placement radius
        return Physics.CheckSphere(placementPosition + Vector3.up * 0.1f, towerStats.placementRadius, blockedLayers, QueryTriggerInteraction.Ignore);
    }

    void UpdateMaterial()
    {
        //Only change materials when placement validity changes
        if (materialSet && canPlace == previousValid)
        {
            return;
        }

        materialSet = true;
        previousValid = canPlace;

        Material material;

        if (canPlace)
        {
            material = validMaterial;
        }
        else
        {
            material = invalidMaterial;
        }

        //Apply the material to every part of the preview
        foreach (Renderer renderer in previewRenderers)
        {
            Material[] materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }

            renderer.sharedMaterials = materials;
        }
    }

    void OnDisable()
    {
        //Remove the preview if placement is interrupted
        if (preview != null)
        {
            Destroy(preview);
        }

        preview = null;
        isDragging = false;
        canPlace = false;
    }
}
