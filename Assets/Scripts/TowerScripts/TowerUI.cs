//Dan Ivanovas
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TowerUI : MonoBehaviour
{

    //THIS WAS AWFUL: THE CODE BARELY WORKS
    //I really hate working with ui
    //It's pretty convenient since a lot of using buttons to call functions so not much setup

    public RectTransform wheel;
    public GameObject elementPanel;
    public GameObject upgradePanel;
    public Button upgradeButton;
    public TMP_Text upgradeText;
    public TMP_Text sellText;

    public float heightAboveTower = 1.5f;

    
    private Camera gameCamera;

    //functions references
    private BaseTowerScript selectedTower;
    private TowerSounds sounds;
    private CurrencyManager currencyManager;

    void Start()
    {
        gameCamera = Camera.main;
        sounds = FindFirstObjectByType<TowerSounds>();
        currencyManager = FindFirstObjectByType<CurrencyManager>();

        //Only visible parts of the buttons are clickable
        foreach (Button button in wheel.GetComponentsInChildren<Button>(true))
        {
            Image image = button.GetComponent<Image>();

            if (image != null && image.sprite != null)
            {
                image.alphaHitTestMinimumThreshold = 0.1f;
            }
        }

        //Add hover sounds to all element buttons
        foreach (Button button in elementPanel.GetComponentsInChildren<Button>(true))
        {
            TowerElementHover hover = button.GetComponent<TowerElementHover>();

            if (hover == null)
            {
                hover = button.gameObject.AddComponent<TowerElementHover>();
            }

            hover.sounds = sounds;
        }

        CloseWheel();
    }

    void Update()
    {
        //Check if the player clicked
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        //Ignore clicks over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            BaseTowerScript tower = hit.collider.GetComponentInParent<BaseTowerScript>();

            if (tower != null && tower.IsPlaced)
            {
                OpenWheel(tower);
                return;
            }
        }

        CloseWheel();
    }

    void LateUpdate()
    {
        if (selectedTower == null)
        {
            if (wheel.gameObject.activeSelf)
            {
                CloseWheel();
            }

            return;
        }

        //Get the screen position above the selected tower
        Vector3 position = gameCamera.WorldToScreenPoint(selectedTower.transform.position + Vector3.up * heightAboveTower);

        //Hide the wheel if the tower is behind the camera
        bool visible = position.z > 0f;

        if (wheel.gameObject.activeSelf != visible)
        {
            wheel.gameObject.SetActive(visible);
        }

        if (visible)
        {
            wheel.position = position;
        }
    }

    void OpenWheel(BaseTowerScript tower)
    {
        selectedTower = tower;
        wheel.gameObject.SetActive(true);

        if (sounds != null)
        {
            sounds.PlaySelection();
        }

        UpdateWheel();
    }

    void UpdateWheel()
    {
        bool choosingElement = selectedTower.tier == 0;

        elementPanel.SetActive(choosingElement);
        upgradePanel.SetActive(!choosingElement);

        //Update the selling price
        sellText.text = "Sell $" + selectedTower.GetSellValue();

        if (choosingElement)
        {
            return;
        }

        UpdateUpgradeColour();

        bool maxTier = selectedTower.tier >= 3;

        upgradeButton.interactable = !maxTier;

        if (maxTier)
        {
            upgradeText.text = selectedTower.currentElement + "\nMAX TIER";
        }
        else
        {
            upgradeText.text = selectedTower.currentElement + "\nTier " + (selectedTower.tier + 1) + "\n$" + selectedTower.GetNextUpgradeCost();
        }
    }

    public void SellTower()
    {
        if (selectedTower == null || currencyManager == null)
        {
            return;
        }

        //Calculate how much gold the tower is worth
        int sellValue = selectedTower.GetSellValue();

        //Return gold to the player
        currencyManager.AddCurrency(sellValue);

        //Remove the tower from the scene
        GameObject tower = selectedTower.gameObject;

        CloseWheel();

        if (sounds != null)
        {
            sounds.PlaySell();
        }

        Destroy(tower);
    }

    void UpdateUpgradeColour()
    {
        string elementName = selectedTower.currentElement + "Button";

        foreach (Button button in elementPanel.GetComponentsInChildren<Button>(true))
        {
            if (button.name == elementName)
            {
                upgradeButton.colors = button.colors;
                return;
            }
        }
    }

    public void ChooseElement(int element)
    {
        if (selectedTower != null && selectedTower.InfuseTower((TowerElement)element))
        {
            if (sounds != null)
            {
                sounds.PlayElementClick();
            }

            CloseWheel();
        }
    }

    public void UpgradeTower()
    {
        if (selectedTower != null && selectedTower.UpgradeTower())
        {
            if (sounds != null)
            {
                sounds.PlayUpgrade();
            }

            UpdateWheel();
        }
    }

    public void CloseWheel()
    {
        selectedTower = null;
        wheel.gameObject.SetActive(false);
    }
}
