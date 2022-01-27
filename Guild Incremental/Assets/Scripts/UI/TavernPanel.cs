using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernPanel : MonoBehaviour
{
    [Header("Menu")]
    public GameObject menuContent;
    public GameObject menuCategoryPanelPrefab;
    public List<TavernMenuCategoryPanel> menuCategoryPanels = new List<TavernMenuCategoryPanel>();

    [Header("Recipe Research")]
    public GameObject recipeContent;
    public GameObject recipePanelPrefab;
    public List<TavernRecipePanel> recipePanels = new List<TavernRecipePanel>();

    private Guild guild;
    private bool categoriesLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
        if (guild == null)
            guild = GameObject.Find("Guild").GetComponent<Guild>();

        if (!categoriesLoaded) 
            LoadMenuCategories();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMenuCategories()
    {
        if (guild == null)
            guild = GameObject.Find("Guild").GetComponent<Guild>();

        int rank = (int)guild.Rank;
        while (rank >= 0)
        {
            GameObject obj = Instantiate(menuCategoryPanelPrefab, menuContent.transform, false);
            TavernMenuCategoryPanel tavernMenuCategoryPanel = obj.GetComponent<TavernMenuCategoryPanel>();
            tavernMenuCategoryPanel.rank = (Rank)rank;
            menuCategoryPanels.Add(tavernMenuCategoryPanel);

            rank--;
        }
        categoriesLoaded = true;
    }

    public TavernMenuCategoryPanel GetMenuCategory(Rank rank)
    {
        foreach (var cat in menuCategoryPanels)
        {
            if (cat.rank == rank) return cat;
        }
        return null;
    }

    public void AddMenuItem(TavernMenuItem tavernMenuItem)
    {
        if (!categoriesLoaded) 
            LoadMenuCategories();

        TavernMenuCategoryPanel tavernMenuCategoryPanel = GetMenuCategory(tavernMenuItem.rank);
        if (tavernMenuCategoryPanel == null)
            return;

        tavernMenuCategoryPanel.AddMenuItem(tavernMenuItem);
    }

    public void ClearMenu()
    {
        foreach (var panel in menuCategoryPanels)
            Destroy(panel.gameObject);
    }

    public void AddRecipe(TavernRecipeData tavernRecipeData)
    {
        if (tavernRecipeData == null)
            return;

        GameObject obj = Instantiate(recipePanelPrefab, recipeContent.transform, false);
        TavernRecipePanel tavernRecipePanel = obj.GetComponent<TavernRecipePanel>();
        
        tavernRecipePanel.SetRecipe(tavernRecipeData, guild);
        //check for existing quest?

        recipePanels.Add(tavernRecipePanel);
    }

    public void ClearRecipes()
    {
        foreach (var panel in recipePanels)
            Destroy(panel.gameObject);
    }
}
