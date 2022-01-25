using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TavernMenuEntryPanel : MonoBehaviour
{
    public Image imageIcon;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textGold;

    public GameObject textRecipe;
    public GameObject recipeContent;
    public GameObject resourcePrefab;
    public List<ResourcePanel> resourcePanels = new List<ResourcePanel>();

    private TavernRecipeData tavernRecipeData = null;
    private Guild guild;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadMenuItem(TavernMenuItem tavernMenuItem)
    {
        guild = GameLib.Guild();
        tavernRecipeData = guild.GetTavernRecipeData(tavernMenuItem.id);

        imageIcon.sprite = guild.GetResourceImage(tavernMenuItem.type == TavernItemType.Food ? ResourceType.Food : ResourceType.Drink);
        textName.text = tavernMenuItem.name;
        textGold.text = tavernMenuItem.gold.ToString();

        if (tavernRecipeData.requiredItems.Count == 0)
        {
            textRecipe.SetActive(false);
            recipeContent.SetActive(false);
        }
        else
        {
            textRecipe.SetActive(true);
            recipeContent.SetActive(true);
            foreach (var entry in tavernRecipeData.requiredItems)
                AddIngredient(entry);
        }
    }

    private void AddIngredient(RecipeEntry recipeEntry)
    {
        GameObject obj = Instantiate(resourcePrefab, recipeContent.transform, false);
        ResourcePanel resourcePanel = obj.GetComponent<ResourcePanel>();
        resourcePanels.Add(resourcePanel);

        resourcePanel.resourceImage.sprite = guild.GetResourceImage(recipeEntry.item.resourceType);
        resourcePanel.resourceText.text = recipeEntry.item.Name + " x" + recipeEntry.count.ToString();
    }
}
