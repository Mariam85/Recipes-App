using System;
using System.IO;
using System.Collections.Generic;
using Spectre.Console;
using System.Text.Json;
using System.Linq;

// Getting the json file path. 
string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
string sFile = System.IO.Path.Combine(sCurrentDirectory, @"..\..\..\Text.json");
string sFilePath = Path.GetFullPath(sFile);

// Emptying the json file at the start of each run.
File.WriteAllText(sFilePath, "[]");

// Looping till the user chooses to exit.
while (true)
{
    // The title using FigletText.
    AnsiConsole.Write(
                new FigletText("Recipes app")
                    .Centered()
                    .Color(Color.Aqua));

    // A prompt to pick the functionality.
    List<string> functionality = AnsiConsole.Prompt(
    new MultiSelectionPrompt<string>()
    .PageSize(10)
    .Title("[purple]Please pick the functionality you want to do[/]")
    .InstructionsText("[grey]([blue][/] use up and down arrows to toggle, press space then enter [green][/]to accept)[/]")
    .AddChoices(
        new[]{
                        "Add","Edit","List","Exit"
        }));

    string userChoice = functionality.Count == 1 ? functionality[0] : null;

    // Adding a new recipe.
    if (userChoice == "Add")
    {
        List<string> recipeIngredients = AnsiConsole.Ask<string>("1)Ingredients: [grey]seperate them by adding a - [/] ").Split("-").ToList();
        string recipeTitle = AnsiConsole.Ask<string>("2)Title: ");
        List<string> recipeInstructions = AnsiConsole.Ask<string>("3)Instructions: [grey]seperate them by adding a - [/] ").Split("-").ToList();
        List<string> recipeCategories = AnsiConsole.Ask<string>("4)Categories: [grey]seperate them by adding a - [/] ").Split("-").ToList();

        var recipe = new Recipe(recipeIngredients, recipeTitle, recipeInstructions, recipeCategories);
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = File.ReadAllText(sFilePath);
        List<Recipe> menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);
        menu.Add(recipe);
        File.WriteAllText(sFilePath, System.Text.Json.JsonSerializer.Serialize(menu, options));

        AnsiConsole.Write("Successfully added the recipe!\n");
        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();
    }

    // Editing a recipe.
    else if (userChoice == "Edit")
    {
        string title = AnsiConsole.Ask<string>("Enter the title of the recipe to edit:");
        var attributeToEdit = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
            .PageSize(10)
            .Title("[purple]Please pick what you want to edit:[/]")
            .InstructionsText("[grey]( use up and down arrows to toggle, press space then press enter to accept)[/]")
            .AddChoices(
                new[]
                {
                                "categories", "instructions", "ingredients", "title"
                }));

        string choiceEdit = attributeToEdit.Count == 1 ? attributeToEdit[0] : null;
        string jsonString = File.ReadAllText(sFilePath);
        List<Recipe> menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);

        // Checking which element the user would like to edit. 
        if (choiceEdit == "categories")
        {
            List<string> newCategory = AnsiConsole.Ask<string>("Enter new categories: [green]seperate them by adding a dash - [/]?").Split("-").ToList();
            menu.Find(r => r.Title == title).Categories = newCategory;
        }
        else if (choiceEdit == "instructions")
        {
            List<string> newInstructions = AnsiConsole.Ask<string>("Enter new instructions: [green]seperate them by adding a dash - .[/]?").Split("-").ToList();
            menu.Find(r => r.Title == title).Instructions = newInstructions;
        }
        else if (choiceEdit == "ingredients")
        {
            List<string> newIngredients = AnsiConsole.Ask<string>("Enter new ingredients: [green]seperate them by adding a dash - .[/]?").Split("-").ToList();
            menu.Find(r => r.Title == title).Ingredients = newIngredients;
        }
        else
        {
            string newTitle = AnsiConsole.Ask<string>("Enter new title:");
            menu.Find(r => r.Title == title).Title = newTitle;
        }

        // Updating the json file.
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(sFilePath, System.Text.Json.JsonSerializer.Serialize(menu));
        AnsiConsole.Write("Successfully edited the recipe!\n");
        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();

    }
    // Listing recipes.
    else if (userChoice == "List")
    {

    }
    // Exiting.
    else
    {
        break;
    }
}
