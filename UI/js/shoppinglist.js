class ShoppingList {
    constructor() {
        this.server = "http://localhost:5000/api";
        this.ingredientBankElement = document.getElementById("ingredientBank");
        this.allButton = document.getElementById("allButton");
        this.adjunctButton = document.getElementById("adjunctButton");
        this.fermentableButton = document.getElementById("fermentableButton");
        this.hopButton = document.getElementById("hopButton");
        this.yeastButton = document.getElementById("yeastButton");

        this.genericSupplierCartDiv = document.getElementById("genericSupplierCartDiv");
        this.genericSupplierCart = document.getElementById("genericSupplierCart");

        //The Brewer wants to be able to sort ingredients for purchase by the supplier, but ingredient-supplier data is not available at this moment.
        //That's why there's kind of an ability for multiple carts to exist at once.
        this.carts = {
            ingredients: [

            ]
        }
        
        /*
        this.carts = {
            supplierID: {
                Ingredients: {
                    IngredientId:
                    IngredientName:
                    QuantityOrdering:
                    QuantityOnHand:

                    **Weight:
                    **Cost:
                    **Not in the database yet.
                }
                SupplierName:
            }
        }
        */

        this.populateIngredientBank(5); //Show all ingredients   
        this.ActivateSortButtons(); //Activate the sort buttons in the ingredient bank
    }

    //Get ingredients from table "ingredient". Ingredient type can be 1-4 to sort by ingredient type, or 5 for all.
    populateIngredientBank(ingredientType) {
        fetch(`${this.server}/ingredients`)
        .then(response => response.json())
        .then(ingredientData => {            
            let ingredientHTML = "";
            for (let i = 0; i < ingredientData.length; i++) {
                if (ingredientType != 5)
                {
                    if (ingredientData[i].ingredientTypeId == ingredientType)
                    {
                        ingredientHTML += this.RenderIngredientElement(ingredientData[i]);
                    }
                }
                else
                {
                    ingredientHTML += this.RenderIngredientElement(ingredientData[i]);
                }
            }

            this.ingredientBankElement.innerHTML = this.RenderIngredientBankTop() + ingredientHTML;

            //Initialize the buttons in the bank
            var ingredientAddButtons = document.getElementsByClassName("ingredientButton");
            for (let i = 0; i < ingredientAddButtons.length; i++) {
                ingredientAddButtons[i].onclick = this.AddToCart.bind(this, ingredientAddButtons[i].id);                
            }            
        })
    } 

    //Adds an item to the cart. In the future this will add to the cart of the proper supplier.
    AddToCart(buttonId) {
        let index = buttonId.substr(2); //Button ids are saved as IB(Ingredient ID). Substringing it gives us the ingredient id
        fetch(`${this.server}/ingredients/${index}`)
        .then(response => response.json())
        .then(data => {
            var check = this.carts.ingredients.findIndex(i => i.IngredientId == index);
            if (check > -1) {
                //List already has that ingredients
                this.carts.ingredients[check].Ordering += 1;
                this.RenderOldCartElement(check + 1);
            }
            else
            {
                if (this.carts.ingredients.length == 0)
                {
                    this.genericSupplierCartDiv.classList.remove("d-none");
                }

                var newIng = {
                    IngredientId: index,
                    IngredientName: data.name,
                    Ordering: 1,
                    OnHandQuantity: data.onHandQuantity,
                }

                this.carts.ingredients.push(newIng);
            
                //Append to the cart
                this.genericSupplierCart.innerHTML += this.RenderNewCartElement(newIng);
            }
            console.log(this.carts);  
        });
    }

    //Render a new row on the cart
    //In the future this will also show the weight and the cost of the item
    RenderNewCartElement(ingredient) {
        return `
            <tr>
                <th scope="row">${ingredient.IngredientName}</th>
                <td id="CI${ingredient.IngredientId}">${ingredient.Ordering}</td>
                <td>${ingredient.OnHandQuantity}</td>
            </tr>
        `
    }

    //Re-render an old row of the cart and update the quantity
    RenderOldCartElement(index) {
        console.log(index);
        document.getElementById("CI" + this.carts.ingredients[index-1].IngredientId).innerHTML = this.carts.ingredients[index - 1].Ordering;
    }

    //This function would render the cart head per supplier.
    //I'd also have to rework how the cart element creation exists a little bit to account for multiple tables.
    RenderCartHead(supplier) {

    }

    //This will render the total weight and cost of the current cart. It will be called every time something new is added to it.
    //Will need cost and weight data for each item to do this.
    //This may also indicate how close to a pallet completion the user is. That was one of the requested features.
    RenderCartInfo(supplier) {

    }

    //This just activates the sort buttons.
    ActivateSortButtons() {
        this.adjunctButton.onclick = () => {
            this.populateIngredientBank(1);
            this.allButton.classList.remove("disabled");
            this.adjunctButton.classList.add("disabled");
            this.fermentableButton.classList.remove("disabled");
            this.hopButton.classList.remove("disabled");
            this.yeastButton.classList.remove("disabled");
        }

        this.fermentableButton.onclick = () => {
            this.populateIngredientBank(2);
            this.allButton.classList.remove("disabled");
            this.adjunctButton.classList.remove("disabled");
            this.fermentableButton.classList.add("disabled");
            this.hopButton.classList.remove("disabled");
            this.yeastButton.classList.remove("disabled");
        }

        this.hopButton.onclick = () => {
            this.populateIngredientBank(3);
            this.allButton.classList.remove("disabled");
            this.adjunctButton.classList.remove("disabled");
            this.fermentableButton.classList.remove("disabled");
            this.hopButton.classList.add("disabled");
            this.yeastButton.classList.remove("disabled");
        }

        this.yeastButton.onclick = () => {
            this.populateIngredientBank(4);
            this.allButton.classList.remove("disabled");
            this.adjunctButton.classList.remove("disabled");
            this.fermentableButton.classList.remove("disabled");
            this.hopButton.classList.remove("disabled");
            this.yeastButton.classList.add("disabled");
        }

        this.allButton.onclick = () => {
            this.populateIngredientBank(5);
            this.allButton.classList.add("disabled");
            this.adjunctButton.classList.remove("disabled");
            this.fermentableButton.classList.remove("disabled");
            this.hopButton.classList.remove("disabled");
            this.yeastButton.classList.remove("disabled");
        }
    }

    //The cost, weight, and supplier are not provided in the database. If they were, I would add them here like the sample UI.
    RenderIngredientElement(ingredient) {
        return `
            <tr>
                <th scope="row">${ingredient.name}</th>
                <td><button type="button" class="btn btn-warning ingredientButton" id="IB${ingredient.ingredientId}">Add</button></td>
            </tr>
        `
    }
    
    //Render the table headers of the ingredient bank
    //In the future this will show which supplier, the weight, and the cost of the item.
    RenderIngredientBankTop() {
        return `
            <thead class="thead">
                <tr>
                    <th scope="col">Name</th>
                    <th scope="col"></th>
                </tr>
            </thead>
        `
    }

    //This would post the order to the database and add everything to the incoming list. 
    //It will also probably take the user to a new screen so they can input the incoming delivery date.
    PostOrder() {

    }

    //This would show the alerts at the top of the page to indicate when items are beyond their reorder point, or will be when taking into account upcoming orders.
    //It will prompt the user to add the items directly to their cart so they don't have to search the ingredient bank for it.
    RenderAlerts() {

    }

    //More ways to sort the ingredient list. By quantity and by name. Also just a way to search directly for the name. 
    //The ingredient list should also probably have a scroll bar of it's own instead of the main page scroll bar.

    //A quantity selector for when adding to cart. That way the user doesn't have to click multiple times.

    //Page should verify the user is the Brewer.

    //When displaying weight it should have the option to pick between imperial and metric units.
}

window.addEventListener("load", () => new ShoppingList);