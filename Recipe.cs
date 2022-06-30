using System;
using System.Collections.Generic;
using System.Text;

public class Recipe
{
    public List<string> Ingredients { get; set; }
    public string Title { get; set; }
    public List<string> Instructions { get; set; }
    public List<string> Categories { get; set; }

    public Recipe(List<string> ingredients, string title, List<string> instructions, List<string> categories)
    {
        this.Ingredients = ingredients;
        this.Title = title;
        this.Instructions = instructions;
        this.Categories = categories;
    }
}
