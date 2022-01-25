using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TavernRecipePanel : MonoBehaviour
{
    public Image imageIcon;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textRank;
    public TextMeshProUGUI textDescription;

    public Button buttonResearch;
    public TextMeshProUGUI textButton;

    public Slider slider;
    public TextMeshProUGUI textSlider;

    public GameObject EffectContent;
    public GameObject RecipeContent;
    public GameObject resourcePrefab;

    public Quest quest = null;
    public TavernRecipeData recipeData = null;
    private Guild guild;

    // Start is called before the first frame update
    void Start()
    {
        if (guild == null) 
            guild = GameObject.Find("Guild").GetComponent<Guild>();

        SetResearchState(quest != null);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIngredients();
    }

    public void SetRecipe(TavernRecipeData data, Guild guild)
    {
        this.guild = guild;
        quest = null;
        if (data == null)
            return;
        recipeData = data;
        imageIcon.sprite = guild.GetResourceImage(recipeData.type == TavernItemType.Food ? ResourceType.Food : ResourceType.Drink);
        textName.text = recipeData.name;
        textRank.text = " [Rank " + recipeData.rank.ToString() + "]";
        textDescription.text = recipeData.description;

        AddEffect(recipeData.gold);

        foreach (var recipeEntry in recipeData.requiredItems)
            AddIngredient(recipeEntry);
    }

    private void AddEffect(int gold)
    {
        GameObject obj = Instantiate(resourcePrefab, EffectContent.transform, false);
        ResourcePanel resourcePanel = obj.GetComponent<ResourcePanel>();

        resourcePanel.resourceImage.sprite = guild.GetResourceImage(ResourceType.Gold);
        resourcePanel.resourceText.text = gold.ToString() + " Gold per Sale";
    }

    private void AddIngredient(RecipeEntry recipeEntry)
    {
        GameObject obj = Instantiate(resourcePrefab, RecipeContent.transform, false);
        ResourcePanel resourcePanel = obj.GetComponent<ResourcePanel>();

        resourcePanel.resourceImage.sprite = guild.GetResourceImage(recipeEntry.item.resourceType);
        resourcePanel.resourceText.text = recipeEntry.item.Name + " x" + recipeEntry.count.ToString();
    }

    private void UpdateIngredients()
    {
        if (quest == null)
            return;

        int itr = 0;
        foreach (var recipeEntry in recipeData.requiredItems)
        {
            ResourcePanel resourcePanel = RecipeContent.transform.GetChild(itr).GetComponent<ResourcePanel>();                         
            resourcePanel.resourceText.text = recipeEntry.item.Name + " x" + recipeEntry.count.ToString() + "(" + quest.objectives[itr].current.ToString() + ")";
        }
    }

    public void SetLinkedQuest(Quest quest)
    {
        this.quest = quest;
        SetResearchState(true);
        UpdateIngredients();
    }

    private void SetResearchState(bool isResearching)
    {
        if (isResearching)
        {
            textButton.text = "Cancel";
            slider.gameObject.SetActive(true);
        }
        else
        {
            textButton.text = "Research";
            slider.gameObject.SetActive(false);
        }
    }
}
