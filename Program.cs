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
                        "Add recipe","Edit recipe","List recipe","Exit","Remove category","Rename category"
        }));

    var userChoice = functionality.Count == 1 ? functionality[0] : null;

    // Adding a new recipe.
    if (userChoice == "Add recipe")
    {
        List<string> recipeIngredients = AnsiConsole.Ask<string>("1)Ingredients: [grey]seperate them by adding a - [/] ").Split("-").ToList();
        string recipeTitle = AnsiConsole.Ask<string>("2)Title: ");
        List<string> recipeInstructions = AnsiConsole.Ask<string>("3)Instructions: [grey]seperate them by adding a - [/] ").Split("-").ToList();
        List<string> recipeCategories = AnsiConsole.Ask<string>("4)Categories: [grey]seperate them by adding a - [/] ").Split("-").ToList();


        var recipe = new Recipe(recipeIngredients, recipeTitle, recipeInstructions, recipeCategories);
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = File.ReadAllText(sFilePath);
        var menu = JsonSerializer.Deserialize<List<Recipe>>(jsonString);
        if (menu != null)
        {
            menu.Add(recipe);
            File.WriteAllText(sFilePath, System.Text.Json.JsonSerializer.Serialize(menu, options));
        }

        AnsiConsole.Write("Successfully added the recipe!\n");
        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();
    }

    // Editing a recipe.
    else if (userChoice == "Edit recipe")
    {
        string jsonString = File.ReadAllText(sFilePath);
        var menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);

        var table = new Table().Border(TableBorder.Ascii2);
        table.Expand();
        table.AddColumn("[dodgerblue2]Title[/]");
        table.AddColumn(new TableColumn("[dodgerblue2]Ingredients[/]").LeftAligned());
        table.AddColumn(new TableColumn("[dodgerblue2]Instructions[/]").LeftAligned());
        table.AddColumn(new TableColumn("[dodgerblue2]Categories[/]").LeftAligned());

        for (int i = 0; i < menu.Count; i++)
        {
            table.AddRow(
                 String.Join("\n", menu[i].Title),
                 String.Join("\n", menu[i].Ingredients.Select(x => $"- {x}")),
                 String.Join("\n", menu[i].Instructions.Select((x, n) => $"- {x}")),
                 String.Join("\n", menu[i].Categories.Select((x) => $"- {x}")));
            table.AddEmptyRow();
        }
        AnsiConsole.Write(table);

        int index = AnsiConsole.Ask<int>("Enter the number of the recipe to edit:");
        Recipe recipeToEdit = menu[index - 1];
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

        var choiceEdit = attributeToEdit.Count == 1 ? attributeToEdit[0] : null;

        // Checking which element the user would like to edit. 
        if (choiceEdit == "categories")
        {
            List<string> newCategory = AnsiConsole.Ask<string>("Enter new categories: [green]seperate them by adding a dash - [/]?").Split("-").ToList();
            menu.Find(r => r.Id == recipeToEdit.Id).Categories = newCategory;

        }
        else if (choiceEdit == "instructions")
        {
            List<string> newInstructions = AnsiConsole.Ask<string>("Enter new instructions: [green]seperate them by adding a dash - .[/]?").Split("-").ToList();
            menu.Find(r => r.Id == recipeToEdit.Id).Instructions = newInstructions;
        }
        else if (choiceEdit == "ingredients")
        {
            List<string> newIngredients = AnsiConsole.Ask<string>("Enter new ingredients: [green]seperate them by adding a dash - .[/]?").Split("-").ToList();
            menu.Find(r => r.Id == recipeToEdit.Id).Ingredients = newIngredients;
        }
        else
        {
            string newTitle = AnsiConsole.Ask<string>("Enter new title:");
            menu.Find(r => r.Id == recipeToEdit.Id).Title = newTitle;
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
    // Lists all recipes with the same title.
    else if (userChoice == "List recipe")
    {
        string jsonString = File.ReadAllText(sFilePath);
        List<Recipe> menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);
        string listTitle = AnsiConsole.Ask<string>("Enter the title of the recipe to list:");
        List<Recipe> foundRecipe = menu.FindAll(r => r.Title == listTitle);

        // Recipe attributes displayed in a table.
        var table = new Table().Border(TableBorder.Ascii2);
        table.Expand();
        table.AddColumn("[dodgerblue2]Title[/]");
        table.AddColumn(new TableColumn("[dodgerblue2]Ingredients[/]").LeftAligned());
        table.AddColumn(new TableColumn("[dodgerblue2]Instructions[/]").LeftAligned());
        table.AddColumn(new TableColumn("[dodgerblue2]Categories[/]").LeftAligned());
        for (int i = 0; i < foundRecipe.Count; i++)
        {
            table.AddRow(
                 String.Join("\n", foundRecipe[i].Title),
                 String.Join("\n", foundRecipe[i].Ingredients.Select(x => $"- {x}")),
                 String.Join("\n", foundRecipe[i].Instructions.Select((x, n) => $"- {x}")),
                 String.Join("\n", foundRecipe[i].Categories.Select((x) => $"- {x}")));
            table.AddEmptyRow();
        }
        AnsiConsole.Write(table);
        AnsiConsole.Write("\n");
        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();
    }
    // Renames a category.
    else if (userChoice == "Rename category")
    {
        string jsonString = File.ReadAllText(sFilePath);
        var menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);
        string category = AnsiConsole.Ask<string>("Enter the name of the category to edit:");
        string newCategory = AnsiConsole.Ask<string>("Enter the category's new name:");
        List<Recipe> beforeRename = menu.FindAll(r => r.Categories.Contains(category));
        if (beforeRename.Any())
        {
            foreach (Recipe r in beforeRename)
            {
                r.Categories.Remove(category);
                r.Categories.Add(newCategory);
            }
            // Updating the json file.
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(sFilePath, System.Text.Json.JsonSerializer.Serialize(menu));
            AnsiConsole.Write("Successfully removed the category!\n");
        }
        else
        {
            AnsiConsole.Write("This category was not found\n");
        }
        bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
        if (!mainMenu)
        {
            break;
        }
        AnsiConsole.Clear();
    }
    // Removes all recipes associated with a specific category.
    else if (userChoice == "Remove category")
    {
        string jsonString = File.ReadAllText(sFilePath);
        var menu = System.Text.Json.JsonSerializer.Deserialize<List<Recipe>>(jsonString);
        string category = AnsiConsole.Ask<string>("Enter the name of the category to remove:");
        int removed = menu.RemoveAll(r => r.Categories.Contains(category));
        if (removed > 0)
        { 
            // Updating the json file.
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(sFilePath, System.Text.Json.JsonSerializer.Serialize(menu));
            AnsiConsole.Write("Successfully remooved the category!\n");
            bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
            if (!mainMenu)
            {
                break;
            }
            AnsiConsole.Clear();
        }
        else
        {
            AnsiConsole.Write("This category was not found\n");
            bool mainMenu = AnsiConsole.Confirm("Do you want to return to main menu?");
            if (!mainMenu)
            {
                break;
            }
            AnsiConsole.Clear();
        }
    }
    // Exiting.
    else
    {
        break;
    }
}
