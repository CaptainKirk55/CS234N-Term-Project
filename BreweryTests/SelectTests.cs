using CS243N_Term_Project.Models;

namespace BreweryTests
{
    [TestFixture]
    public class Tests
    {
        /*
        Stuff to pull to create the shopping list
        App_User -- Acknowledge that the user has permission (Brewer) to edit the shopping list
        Batch -- Scheduled start date is needed to know when to purchase ingredients. Also the recipe id and planned volume to know what ingredients to order
        Ingredient -- Contains the data for name, on hand quantity, and cost. 'Notes' would be nice to display on the shopping list page too.
        Ingredient_Inventory_Addition -- Has data for quantity of incoming inventory and when it'll arrive.
        Ingredient_Inventory_Subtraction -- Has data for missing inventory, due to spills or trades. "Reason" will note why it's missing and this data can be used to prompt the brewer to restock the unplanned shortage.
        Recipe -- Need the name of the recipe based on the recipe ID
        Recipe_Ingredient -- Need the quantity of ingredient per recipe unit. Also the ingredient ID to know what to order.
        Supplier -- Need this data to display on the page
            Supplier_Address -- Depending on how billing ends up getting done we may want this, and also Address and Address_Type to know the billing addresses of the suppliers.
        Unit_Type -- To get the wording nice when referencing an ingredient.

        Tables to modify with the shopping list
        Ingredient_Inventory_Addition -- After an order is placed add the order data to this table.
        */

        BitsContext dbContext;

        [SetUp]
        public void Setup()
        {
            dbContext = new BitsContext();
        }

        /*
         * No users in the database to test for at the moment.
         * When there is one, check for a brewer's name.
         */
        [Test]
        public void TestGetAppUsers()
        {
            AppUser? user;
            user = dbContext.AppUsers.Find(1);
            Assert.AreEqual(user, null);
        }

        /*
         * No batches in the database to test for at the moment.
         * When there is one, check for recipe_id, volume, and estimated_start_date.
         */
        [Test]
        public void TestGetBatch()
        {
            Batch? batch;
            batch = dbContext.Batches.Find(1);
            Assert.AreEqual(batch, null);
        }

        
        /*
         * Basic get ingredient test
         */
        [Test]
        public void TestGetIngredient()
        {
            Ingredient? i;
            i = dbContext.Ingredients.Find(1);
            Assert.AreEqual(i.Name, "Acid Malt");
        }

        /*
         * Print all ingredients out of stock
         */
        [Test]
        public void TestOutOfStock()
        {
            List<Ingredient> ingredients;
            ingredients = dbContext.Ingredients.Where(i => i.OnHandQuantity == 0).ToList();
            foreach(Ingredient ingredient in ingredients)
            {
                Console.WriteLine(ingredient.Name + " out of stock!");
            }

            //Out of stock value should be 1127. This won't be accurate when the database is actually functioning.
            Assert.AreEqual(ingredients.Count, 1127);
            Assert.AreEqual(ingredients[0].Name, "Acid Malt");
        }

        /*
         * Gets all ingredients that are incoming and their name.
         */
        [Test]
        public void TestIncomingIngredients()
        {
            var incomingList = dbContext.IngredientInventoryAdditions.Join
                (
                    dbContext.Ingredients,
                    incoming => incoming.IngredientId,
                    ingredient => ingredient.IngredientId,
                    (incoming, ingredient) => new { incoming.IngredientInventoryAdditionId, incoming.Quantity, ingredient.Name, incoming.IngredientId, incoming.EstimatedDeliveryDate }
                ).ToList();

            foreach(var i in incomingList)
            {
                Console.WriteLine("IIA ID: " + i.IngredientInventoryAdditionId + " has " + i.Quantity + " " + i.Name + " (Ingredient ID: " + i.IngredientId + ") incoming on " + i.EstimatedDeliveryDate);
            }

            Assert.AreEqual(incomingList.Count, 35);
            Assert.AreEqual("IIA ID: " + incomingList[0].IngredientInventoryAdditionId + " has " + incomingList[0].Quantity + " " + incomingList[0].Name + " (Ingredient ID: " + incomingList[0].IngredientId + ") incoming on " + incomingList[0].EstimatedDeliveryDate, "IIA ID: 1 has 8.164656 Pale Malt (2 Row) Bel (Ingredient ID: 67) incoming on 12/3/2020 10:39:17 AM");
        }

        /*
         * No ingredient inventory subtraction in the database to test for at the moment.
         * When there is one, perform a similar test to the incoming ingredients test.
         */
        [Test]
        public void TestIngredientInventorySubtraction()
        {
            IngredientInventorySubtraction? IIS;
            IIS = dbContext.IngredientInventorySubtractions.Find(1);
            Assert.AreEqual(IIS, null);
        }

        [Test]
        public void TestGetRecipe()
        {
            List<Recipe> r;
            r = dbContext.Recipes.ToList();
            
            foreach(Recipe recipe in r)
            {
                Console.WriteLine(recipe.RecipeId + " " + recipe.Name);
            }

            Assert.AreEqual(r.Count, 4);
            Assert.AreEqual(r[0].RecipeId + " " + r[0].Name, "1 Fuzzy Tales Juicy IPA");
        }

        /*
         * Get the recipe ingredients and their names and the recipe names.
         */
        [Test]
        public void TestRecipeIngredient()
        {
            //Ideally this would get a list of ingredients per recipe
            //select recipe_id, group_concat(ingredient.name) from recipe_ingredient join ingredient on recipe_ingredient.ingredient_id =  ingredient.ingredient_id group by recipe_id; 
            //but group_concat does not exist so I'll have to come up with something else.

            var r = dbContext.RecipeIngredients
                .Join
                (
                    dbContext.Ingredients,
                    recipeIngredient => recipeIngredient.IngredientId,
                    ingredient => ingredient.IngredientId,
                    (recipeIngredient, ingredient) => new { recipeIngredient.RecipeId, ingredient.Name, recipeQuantity = recipeIngredient.Quantity, ingredientQuantity = ingredient.OnHandQuantity}
                )
                .Join
                (
                    dbContext.Recipes,
                    recipeIngredient => recipeIngredient.RecipeId,
                    recipe => recipe.RecipeId,
                    (recipeIngredient, recipe) => new { recipeIngredient.RecipeId, recipe.Name, ingredientName = recipeIngredient.Name, recipeIngredient.recipeQuantity, recipeIngredient.ingredientQuantity}
                )
                .ToList();

            foreach(var i in r)
            {
                Console.WriteLine(i.Name + " (ID: " + i.RecipeId + " Quantity: " + i.recipeQuantity + ")" + " needs " + i.ingredientName + " and the kitchen has " + i.ingredientQuantity + " of that.");
            }

            Assert.AreEqual(r.Count, 57);
            Assert.AreEqual(r[0].Name + " (ID: " + r[0].RecipeId + " Quantity: " + r[0].recipeQuantity + ")" + " needs " + r[0].ingredientName + " and the kitchen has " + r[0].ingredientQuantity + " of that.", "Cascade Orange Pale Ale (ID: 3 Quantity: 0.0850485) needs Cascade and the kitchen has 0 of that.");
        }

        /*
         * Get supplier data
         */
        [Test]
        public void TestSupplier()
        {
            List<Supplier> suppliers;
            suppliers = dbContext.Suppliers.ToList();

            foreach (Supplier supplier in suppliers)
            {
                Console.WriteLine("ID: " + supplier.SupplierId + " is " + supplier.Name + " and can have orders placed at " + supplier.Website);
            }

            Assert.AreEqual(suppliers.Count, 6);
            Assert.AreEqual("ID: " + suppliers[0].SupplierId + " is " + suppliers[0].Name + " and can have orders placed at " + suppliers[0].Website, "ID: 1 is BSG Craft Brewing and can have orders placed at https://bsgcraftbrewing.com/");
        }

        /*
         * Get suppliers where there is a contact
         */
        [Test]
        public void TestSuppliersWithContact()
        {
            List<Supplier> suppliers;
            suppliers = dbContext.Suppliers.Where(s => s.ContactFirstName != null).ToList();

            foreach(Supplier supplier in suppliers)
            {
                Console.WriteLine(supplier.ContactFirstName + " " + supplier.ContactLastName + " is with " + supplier.Name + " and can be reached at " + supplier.ContactPhone);
            }

            Assert.AreEqual(suppliers.Count, 3);
            Assert.AreEqual(suppliers[0].ContactFirstName + " " + suppliers[0].ContactLastName + " is with " + suppliers[0].Name + " and can be reached at " + suppliers[0].ContactPhone, "Zach Grossfeld is with Country Malt Group and can be reached at 3606996765");
        }

        [Test]
        public void TestUnitType()
        {
            List<UnitType> unitTypes;
            unitTypes = dbContext.UnitTypes.ToList();

            foreach (UnitType ut in unitTypes)
            {
                Console.WriteLine(ut.UnitTypeId + " " + ut.Name);
            }

            Assert.AreEqual(unitTypes.Count, 3);
            Assert.AreEqual(unitTypes[0].UnitTypeId + " " + unitTypes[0].Name, "1 each");
        }

    }
}