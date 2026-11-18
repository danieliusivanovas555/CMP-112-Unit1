//Dan Ivanovas
using UnityEngine;
using System.Collections;

public enum TowerElement
{
    None,
    Fire,
    Ice,
    Lightning,
    Earth
}

public class BaseTowerScript : MonoBehaviour
{

    public int towerCost = 100;
    public int damage = 1;
    public float attackRange = 5f;
    public float attackSpeed = 1f;

    public float placementRadius = 0.75f;

  
    public int infusionCost = 100;
    public int tier2Cost = 150;
    public int tier3Cost = 250;

    public TowerElement currentElement = TowerElement.None;
    public int tier = 0;


    [Range(0f, 1f)]
    public float sellPercentage = 0.7f;

    public Renderer towerRenderer;
    public int materialIndex = 3;

    public Material fireMaterial;
    public Material iceMaterial;
    public Material lightningMaterial;
    public Material earthMaterial;

    public float transitionDuration = 1.5f;

    public Color fireColour = new Color(1f, 0.3f, 0.1f);
    public Color iceColour = new Color(0.2f, 0.6f, 1f);
    public Color lightningColour = new Color(1f, 0.85f, 0.2f);
    public Color earthColour = new Color(0.3f, 0.8f, 0.3f);

    private CurrencyManager currencyManager;
    private ElementUpgrade elementUpgrade;
    private Material transitionMaterial;

    public bool IsPlaced { get; private set; }

    void Awake()
    {
        //Find the currency manager in the scene
        currencyManager = FindFirstObjectByType<CurrencyManager>();
    }

    public virtual void PlaceTower()
    {
        //Mark the tower as placed
        IsPlaced = true;
    }

    public bool InfuseTower(TowerElement element)
    {
        //Prevent infusion if the tower isn't placed or already upgraded
        if (!IsPlaced || tier != 0)
        {
            return false;
        }

        ElementUpgrade selectedElement;
        Material selectedMaterial;
        Color selectedColour;

        //Select the element and its material
        switch (element)
        {
            case TowerElement.Fire:
                selectedElement = new FireElement();
                selectedMaterial = fireMaterial;
                selectedColour = fireColour;
                break;

            case TowerElement.Ice:
                selectedElement = new IceElement();
                selectedMaterial = iceMaterial;
                selectedColour = iceColour;
                break;

            case TowerElement.Lightning:
                selectedElement = new LightningElement();
                selectedMaterial = lightningMaterial;
                selectedColour = lightningColour;
                break;

            case TowerElement.Earth:
                selectedElement = new EarthElement();
                selectedMaterial = earthMaterial;
                selectedColour = earthColour;
                break;

            default:
                return false;
        }

        //Check if the correct material is assigned
        if (towerRenderer == null || selectedMaterial == null ||
            materialIndex < 0 ||
            materialIndex >= towerRenderer.sharedMaterials.Length)
        {
            Debug.LogError("Tower material setup is incorrect.", this);
            return false;
        }

        //Check if the player can afford the infusion
        if (!currencyManager.SpendCurrency(infusionCost))
        {
            return false;
        }

        currentElement = element;
        elementUpgrade = selectedElement;
        tier = 1;

        //Apply the first element upgrade
        elementUpgrade.ApplyTier(this, tier);

        //Change the tower material over time
        StartCoroutine(TransitionMaterial(selectedMaterial, selectedColour));

        return true;
    }

    IEnumerator TransitionMaterial(Material targetMaterial, Color elementColour)
    {
        Material[] materials = towerRenderer.sharedMaterials;
        Material originalMaterial = materials[materialIndex];

        float halfDuration = transitionDuration / 2f;

        //Change immediately if the transition is disabled
        if (halfDuration <= 0f)
        {
            materials[materialIndex] = targetMaterial;
            towerRenderer.sharedMaterials = materials;
            yield break;
        }

        //Get the original and new material colours
        Color originalColour = originalMaterial.GetColor("_BaseColor");
        Color originalEmission = originalMaterial.GetColor("_EmissionColor");

        Color finalColour = targetMaterial.GetColor("_BaseColor");
        Color finalEmission = targetMaterial.GetColor("_EmissionColor");

        //Create a temporary material for the transition
        transitionMaterial = new Material(originalMaterial);
        transitionMaterial.EnableKeyword("_EMISSION");

        materials[materialIndex] = transitionMaterial;
        towerRenderer.sharedMaterials = materials;

        float elapsed = 0f;

        //Fade out the original material
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.SmoothStep(
                0f, 1f, Mathf.Clamp01(elapsed / halfDuration)
            );

            transitionMaterial.SetColor(
                "_BaseColor",
                Color.Lerp(originalColour, Color.black, progress)
            );

            transitionMaterial.SetColor(
                "_EmissionColor",
                Color.Lerp(originalEmission, Color.black, progress)
            );

            yield return null;
        }

        Material oldMaterial = transitionMaterial;

        //Switch to the new material while dark
        transitionMaterial = new Material(targetMaterial);
        transitionMaterial.EnableKeyword("_EMISSION");

        transitionMaterial.SetColor("_BaseColor", Color.black);
        transitionMaterial.SetColor("_EmissionColor", Color.black);

        materials[materialIndex] = transitionMaterial;
        towerRenderer.sharedMaterials = materials;

        Destroy(oldMaterial);

        elapsed = 0f;

        //Fade into the new element material
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / halfDuration));

            transitionMaterial.SetColor("_BaseColor",Color.Lerp(elementColour, finalColour, progress));

            transitionMaterial.SetColor("_EmissionColor",Color.Lerp(Color.black, finalEmission, progress));

            yield return null;
        }

        //Apply the final material
        materials[materialIndex] = targetMaterial;
        towerRenderer.sharedMaterials = materials;

        Destroy(transitionMaterial);
        transitionMaterial = null;
    }

    public bool UpgradeTower()
    {
        //Prevent upgrading an unplaced or maximum tier tower
        if (!IsPlaced || tier == 0 || tier >= 3)
        {
            return false;
        }

        //Check if the tower has an element upgrade
        if (elementUpgrade == null)
        {
            return false;
        }

        //Check if the player can afford the upgrade
        if (!currencyManager.SpendCurrency(GetNextUpgradeCost()))
        {
            return false;
        }

        //Increase the tower tier and apply its upgrade
        tier++;

        elementUpgrade.ApplyTier(this, tier);

        return true;
    }

    public int GetNextUpgradeCost()
    {
        //Return the price of the next tier
        if (tier == 1)
        {
            return tier2Cost;
        }

        if (tier == 2)
        {
            return tier3Cost;
        }

        return 0;
    }

    public int GetSellValue()
    {
        //Calculate the total amount spent on the tower
        int totalCost = towerCost;

        if (tier >= 1)
        {
            totalCost += infusionCost;
        }

        if (tier >= 2)
        {
            totalCost += tier2Cost;
        }

        if (tier >= 3)
        {
            totalCost += tier3Cost;
        }

        //Return a percentage of the total cost
        return Mathf.RoundToInt(totalCost * sellPercentage);
    }

    void OnDestroy()
    {
        //Remove the temporary material if the tower is destroyed
        if (transitionMaterial != null)
        {
            Destroy(transitionMaterial);
        }
    }
}
