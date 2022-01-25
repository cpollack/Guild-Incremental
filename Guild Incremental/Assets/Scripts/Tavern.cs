using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TavernMenuItem
{
    public string id;
    public string name;
    public string description;
    public TavernItemType type;
    public Rank rank;
    public int gold;
}

public enum TavernItemType
{
    Alcohol,
    Food,
}

public class Tavern : GuildHall
{
    private Dictionary<string, TavernRecipeData> allRecipes;

    [SerializeField]    
    public List<TavernMenuItem> menuItems = new List<TavernMenuItem>();
    [SerializeField]
    public List<string> recipes = new List<string>();

    public TavernPanel tavernPanel;

    new void Awake()
    {
        base.Awake();
        LoadResources();
    }

    private void LoadResources()
    {
        allRecipes = new Dictionary<string, TavernRecipeData>();

        UnityEngine.Object[] recipeResources = Resources.LoadAll("Tavern", typeof(TavernRecipeData));
        foreach (UnityEngine.Object resource in recipeResources)
        {
            TavernRecipeData tavernMenuData = (TavernRecipeData)resource;
            allRecipes.Add(tavernMenuData.id, tavernMenuData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ResetGame()
    {
        LoadResources();
        
        //Add default menu items
        tavernPanel.ClearMenu();
        AddMenuItem("WateryAle");
        AddMenuItem("HardBread");

        tavernPanel.ClearRecipes();
        AddRecipe("RatTailSoup");
    }

    private void AddMenuItem(string menuItemName)
    {
        TavernRecipeData menuItemData = GetRecipeData(menuItemName);
        if (menuItemData == null) return;
        TavernMenuItem menuitem = CreateMenuItem(menuItemData);
        if (menuitem.id.Length == 0) return;
        menuItems.Add(menuitem);
        tavernPanel.AddMenuItem(menuitem);
    }

    private void AddRecipe(string recipeName)
    {
        TavernRecipeData tavernRecipeData = GetRecipeData(recipeName);
        if (tavernRecipeData == null) return;
        recipes.Add(tavernRecipeData.id);
        tavernPanel.AddRecipe(tavernRecipeData);

        //TEMP TESTING, projects are only available if issued by the guild master
        QuestBoard questBoard = GameObject.Find("Quest Board").GetComponent<QuestBoard>();
        questBoard.GenerateProjects();
    }

    public TavernRecipeData GetRecipeData(string id)
    {
        foreach (KeyValuePair<string, TavernRecipeData> kvp in allRecipes)
        {
            if (kvp.Value.id == id) return kvp.Value;
        }
        return null;
    }

    private TavernMenuItem CreateMenuItem(TavernRecipeData data)
    {
        TavernMenuItem tavernMenuitem = new TavernMenuItem();
        tavernMenuitem.id = data.id;
        tavernMenuitem.name = data.name;
        tavernMenuitem.description = data.description;
        tavernMenuitem.type = data.type;
        tavernMenuitem.rank = data.rank;
        tavernMenuitem.gold = data.gold;
        return tavernMenuitem;
    }

    /// <param name="adventurer"></param>
    /// <returns> bool - Whether the adventurer successfully dined at the tavern</returns>
    public bool VisitTavern(Adventurer adventurer)
    {
        int spendGold = 0;
        string action = "";
        bool dining = false;

        List<TavernMenuItem> drinks = GetItems(TavernItemType.Alcohol, adventurer.rank);
        if (drinks.Count == 0)
        {
            TavernMenuItem drink = drinks[UnityEngine.Random.Range(0, drinks.Count - 1)];
            spendGold += drink.gold;
            action += " a " + drink.name;
            dining = true;
        }

        List<TavernMenuItem> foods = GetItems(TavernItemType.Food, adventurer.rank);
        if (foods.Count == 0)
        {
            TavernMenuItem food = foods[UnityEngine.Random.Range(0, foods.Count - 1)];
            spendGold += food.gold;
            action += (action.Length > 0 ? " and " : " ") + food.name;
            dining = true;
        }

        adventurer.SetActionText("Ordered" + action + " at the Tavern, spending " + spendGold.ToString() + " gold.");
        guild.Gold += spendGold;
        return dining;
    }

    private List<TavernMenuItem> GetItems(TavernItemType itemType, Rank rank)
    {
        List<TavernMenuItem> items = new List<TavernMenuItem>();

        while (rank > 0 && items.Count > 0)
        {
            foreach (var menuItem in menuItems)
            {
                if (itemType == menuItem.type && rank == menuItem.rank) items.Add(menuItem);
            }

            rank--;
        }
        return items;
    }
}
