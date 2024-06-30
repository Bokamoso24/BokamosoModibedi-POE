using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BokamosoModibedi_POE
{
    public partial class MainWindow : Window
    {
        private List<Recipe> recipes = new List<Recipe>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            string name = PromptDialog("Enter recipe name:");
            if (string.IsNullOrWhiteSpace(name)) return;

            if (!int.TryParse(PromptDialog("Enter the number of ingredients:"), out int ingredientCount) || ingredientCount < 1)
            {
                MessageBox.Show("Invalid number of ingredients.");
                return;
            }

            Recipe recipe = new Recipe(name);

            for (int i = 0; i < ingredientCount; i++)
            {
                string ingredientName = PromptDialog($"Enter ingredient {i + 1} name:");
                if (string.IsNullOrWhiteSpace(ingredientName)) return;

                if (!double.TryParse(PromptDialog($"Enter the quantity of {ingredientName}:"), out double quantity) || quantity <= 0)
                {
                    MessageBox.Show("Invalid quantity.");
                    return;
                }

                if (!int.TryParse(PromptDialog($"Enter the calories of {ingredientName}:"), out int calories) || calories <= 0)
                {
                    MessageBox.Show("Invalid calories.");
                    return;
                }

                string foodGroup = PromptDialog($"Enter food group for {ingredientName}:");

                recipe.AddIngredient(ingredientName, quantity, "", calories, foodGroup);
            }

            if (!int.TryParse(PromptDialog("Enter the number of steps:"), out int stepCount) || stepCount < 1)
            {
                MessageBox.Show("Invalid number of steps.");
                return;
            }

            for (int i = 0; i < stepCount; i++)
            {
                string stepDescription = PromptDialog($"Enter step {i + 1} description:");
                recipe.AddStep(stepDescription);
            }

            recipes.Add(recipe);

            // Check total calories and show warning if exceeding 300
            if (recipe.TotalCalories > 300)
            {
                OutputTextBox.Text = "Recipe added successfully with warning: Total calories exceed 300.";
            }
            else
            {
                OutputTextBox.Text = "Recipe added successfully.";
            }
        }

        private void DisplayRecipes_Click(object sender, RoutedEventArgs e)
        {
            if (recipes.Count == 0)
            {
                OutputTextBox.Text = "No recipes available.";
                return;
            }

            // Sort the recipes alphabetically by name
            var sortedRecipes = recipes.OrderBy(r => r.Name).ToList();

            StringBuilder recipeList = new StringBuilder("Recipes:\n");
            foreach (var recipe in sortedRecipes)
            {
                recipeList.AppendLine($"Recipe: {recipe.Name}");

                // Check if total calories exceed 300 and append warning message
                if (recipe.TotalCalories > 300)
                {
                    recipeList.AppendLine($"*** Warning: Total calories exceed 300 ***");
                }

                recipeList.AppendLine("Ingredients:");
                foreach (var ingredient in recipe.Ingredients)
                {
                    recipeList.AppendLine($"- {ingredient.Name}: {ingredient.Quantity} {ingredient.Unit} ({ingredient.Calories} calories) ({ingredient.FoodGroup})");
                }

                recipeList.AppendLine("Steps:");
                for (int i = 0; i < recipe.Steps.Count; i++)
                {
                    recipeList.AppendLine($"{i + 1}. {recipe.Steps[i]}");
                }

                recipeList.AppendLine($"Total Calories: {recipe.TotalCalories}");
                recipeList.AppendLine();
            }

            OutputTextBox.Text = recipeList.ToString();
        }

        private void ViewRecipeDetails_Click(object sender, RoutedEventArgs e)
        {
            if (recipes.Count == 0)
            {
                OutputTextBox.Text = "No recipes available.";
                return;
            }

            DisplayRecipes_Click(sender, e);
            if (!int.TryParse(PromptDialog("Enter the number of the recipe to view details:"), out int choice) || choice < 1 || choice > recipes.Count)
            {
                MessageBox.Show("Invalid recipe number.");
                return;
            }

            recipes[choice - 1].DisplayRecipe(OutputTextBox);
        }

        private void ScaleRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (recipes.Count == 0)
            {
                OutputTextBox.Text = "No recipes available.";
                return;
            }

            DisplayRecipes_Click(sender, e);
            if (!int.TryParse(PromptDialog("Enter the number of the recipe to scale:"), out int choice) || choice < 1 || choice > recipes.Count)
            {
                MessageBox.Show("Invalid recipe number.");
                return;
            }

            if (!double.TryParse(PromptDialog("Enter scaling factor (0.5 for half, 2 for double, 3 for triple):"), out double factor) || factor <= 0)
            {
                MessageBox.Show("Invalid scaling factor.");
                return;
            }

            foreach (var ingredient in recipes[choice - 1].Ingredients)
            {
                ingredient.Quantity = ingredient.OriginalQuantity * factor;
            }

            OutputTextBox.Text = "Recipe scaled successfully.";
            recipes[choice - 1].DisplayRecipe(OutputTextBox);
        }

        private void ResetQuantities_Click(object sender, RoutedEventArgs e)
        {
            if (recipes.Count == 0)
            {
                OutputTextBox.Text = "No recipes available.";
                return;
            }

            DisplayRecipes_Click(sender, e);
            if (!int.TryParse(PromptDialog("Enter the number of the recipe to reset quantities:"), out int choice) || choice < 1 || choice > recipes.Count)
            {
                MessageBox.Show("Invalid recipe number.");
                return;
            }

            foreach (var ingredient in recipes[choice - 1].Ingredients)
            {
                ingredient.Quantity = ingredient.OriginalQuantity;
            }

            OutputTextBox.Text = "Quantities reset successfully.";
            recipes[choice - 1].DisplayRecipe(OutputTextBox);
        }

        private void ClearAllData_Click(object sender, RoutedEventArgs e)
        {
            if (recipes.Count == 0)
            {
                OutputTextBox.Text = "No recipes available.";
                return;
            }

            DisplayRecipes_Click(sender, e);
            if (!int.TryParse(PromptDialog("Enter the number of the recipe to clear:"), out int choice) || choice < 1 || choice > recipes.Count)
            {
                MessageBox.Show("Invalid recipe number.");
                return;
            }

            recipes.RemoveAt(choice - 1);
            OutputTextBox.Text = "Selected recipe cleared.";
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FilterByIngredient_Click(object sender, RoutedEventArgs e)
        {
            string ingredient = PromptDialog("Enter the ingredient name to filter by:");
            if (string.IsNullOrWhiteSpace(ingredient)) return;

            var filteredRecipes = recipes.Where(r => r.Ingredients.Any(i => i.Name.Equals(ingredient, StringComparison.OrdinalIgnoreCase))).ToList();

            if (filteredRecipes.Count == 0)
            {
                OutputTextBox.Text = "No recipes found with the specified ingredient.";
                return;
            }

            StringBuilder recipeList = new StringBuilder("Filtered Recipes by Ingredient:\n");
            foreach (var recipe in filteredRecipes)
            {
                recipeList.AppendLine($"Recipe: {recipe.Name}");

                // Check if total calories exceed 300 and append warning message
                if (recipe.TotalCalories > 300)
                {
                    recipeList.AppendLine($"*** Warning: Total calories exceed 300 ***");
                }

                recipeList.AppendLine("Ingredients:");
                foreach (var ingredientItem in recipe.Ingredients)
                {
                    recipeList.AppendLine($"- {ingredientItem.Name}: {ingredientItem.Quantity} {ingredientItem.Unit} ({ingredientItem.Calories} calories) ({ingredientItem.FoodGroup})");
                }

                recipeList.AppendLine("Steps:");
                for (int i = 0; i < recipe.Steps.Count; i++)
                {
                    recipeList.AppendLine($"{i + 1}. {recipe.Steps[i]}");
                }

                recipeList.AppendLine($"Total Calories: {recipe.TotalCalories}");
                recipeList.AppendLine();
            }

            OutputTextBox.Text = recipeList.ToString();
        }

        private void FilterByFoodGroup_Click(object sender, RoutedEventArgs e)
        {
            string foodGroup = PromptDialog("Enter the food group to filter by:");
            if (string.IsNullOrWhiteSpace(foodGroup)) return;

            var filteredRecipes = recipes.Where(r => r.Ingredients.Any(i => i.FoodGroup.Equals(foodGroup, StringComparison.OrdinalIgnoreCase))).ToList();

            if (filteredRecipes.Count == 0)
            {
                OutputTextBox.Text = "No recipes found with the specified food group.";
                return;
            }

            StringBuilder recipeList = new StringBuilder("Filtered Recipes by Food Group:\n");
            foreach (var recipe in filteredRecipes)
            {
                recipeList.AppendLine($"Recipe: {recipe.Name}");

                // Check if total calories exceed 300 and append warning message
                if (recipe.TotalCalories > 300)
                {
                    recipeList.AppendLine($"*** Warning: Total calories exceed 300 ***");
                }

                recipeList.AppendLine("Ingredients:");
                foreach (var ingredientItem in recipe.Ingredients)
                {
                    recipeList.AppendLine($"- {ingredientItem.Name}: {ingredientItem.Quantity} {ingredientItem.Unit} ({ingredientItem.Calories} calories) ({ingredientItem.FoodGroup})");
                }

                recipeList.AppendLine("Steps:");
                for (int i = 0; i < recipe.Steps.Count; i++)
                {
                    recipeList.AppendLine($"{i + 1}. {recipe.Steps[i]}");
                }

                recipeList.AppendLine($"Total Calories: {recipe.TotalCalories}");
                recipeList.AppendLine();
            }

            OutputTextBox.Text = recipeList.ToString();
        }

        private void FilterByMaxCalories_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(PromptDialog("Enter the maximum number of calories:"), out int maxCalories) || maxCalories < 1)
            {
                MessageBox.Show("Invalid number of calories.");
                return;
            }

            var filteredRecipes = recipes.Where(r => r.TotalCalories <= maxCalories).ToList();

            if (filteredRecipes.Count == 0)
            {
                OutputTextBox.Text = "No recipes found within the specified calorie limit.";
                return;
            }

            StringBuilder recipeList = new StringBuilder("Filtered Recipes by Max Calories:\n");
            foreach (var recipe in filteredRecipes)
            {
                recipeList.AppendLine($"Recipe: {recipe.Name}");

                // Check if total calories exceed 300 and append warning message
                if (recipe.TotalCalories > 300)
                {
                    recipeList.AppendLine($"*** Warning: Total calories exceed 300 ***");
                }

                recipeList.AppendLine("Ingredients:");
                foreach (var ingredientItem in recipe.Ingredients)
                {
                    recipeList.AppendLine($"- {ingredientItem.Name}: {ingredientItem.Quantity} {ingredientItem.Unit} ({ingredientItem.Calories} calories) ({ingredientItem.FoodGroup})");
                }

                recipeList.AppendLine("Steps:");
                for (int i = 0; i < recipe.Steps.Count; i++)
                {
                    recipeList.AppendLine($"{i + 1}. {recipe.Steps[i]}");
                }

                recipeList.AppendLine($"Total Calories: {recipe.TotalCalories}");
                recipeList.AppendLine();
            }

            OutputTextBox.Text = recipeList.ToString();
        }

        private string PromptDialog(string text)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(text, "Input Required", "");
        }
    }

    public class Recipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<string> Steps { get; set; }

        public Recipe(string name)
        {
            Name = name;
            Ingredients = new List<Ingredient>();
            Steps = new List<string>();
        }

        public void AddIngredient(string name, double quantity, string unit, int calories, string foodGroup)
        {
            Ingredients.Add(new Ingredient { Name = name, Quantity = quantity, OriginalQuantity = quantity, Calories = calories, FoodGroup = foodGroup });
        }

        public void AddStep(string step)
        {
            Steps.Add(step);
        }

        public void DisplayRecipe(TextBox outputTextBox)
        {
            StringBuilder recipeDetails = new StringBuilder();
            recipeDetails.AppendLine($"Recipe: {Name}");
            recipeDetails.AppendLine("Ingredients:");
            foreach (var ingredient in Ingredients)
            {
                recipeDetails.AppendLine($"- {ingredient.Name}: {ingredient.Quantity} {ingredient.Unit} ({ingredient.Calories} calories) ({ingredient.FoodGroup})");
            }

            recipeDetails.AppendLine("Steps:");
            for (int i = 0; i < Steps.Count; i++)
            {
                recipeDetails.AppendLine($"{i + 1}. {Steps[i]}");
            }

            recipeDetails.AppendLine($"Total Calories: {TotalCalories}");

            outputTextBox.Text = recipeDetails.ToString();

            // Show warning message if total calories exceed 300
            if (TotalCalories > 300)
            {
                outputTextBox.Text += "\n\n*** Warning: Total calories exceed 300 ***";
            }
        }

        public int TotalCalories
        {
            get { return Ingredients.Sum(i => i.Calories); }
        }
    }

    public class Ingredient
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
        public double OriginalQuantity { get; set; }
        public int Calories { get; set; }
        public string Unit { get; set; }
        public string FoodGroup { get; set; }
    }
}