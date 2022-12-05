class ShoppingList {
    constructor() {
        this.server = "http://localhost:5000/api";
        this.ingredientBankElement = document.getElementById("ingredientBank");
        this.allButton = document.getElementById("allButton");
        this.adjunctButton = document.getElementById("adjunctButton");
        this.fermentableButton = document.getElementById("fermentableButton");
        this.hopButton = document.getElementById("hopButton");
        this.yeastButton = document.getElementById("yeastButton");

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
                        ingredientHTML += this.RenderIngredientElement(ingredientData[i], i+1);
                    }
                }
                else
                {
                    ingredientHTML += this.RenderIngredientElement(ingredientData[i], i+1);
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

    AddToCart(buttonId) {
        let index = buttonId.substr(2); //Button ids are saved as IB(Ingredient ID). Substringing it gives us the ingredient id
        fetch(`${this.server}/ingredients/${index}`)
        .then(response => response.json())
        .then(data => {
            console.log(data.name + " added to cart");
        });
    }

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

    RenderIngredientElement(ingredient, index) {
        return `
            <tr>
                <th scope="row">${ingredient.name}</th>
                <td>$30</td>
                <td>10lbs</td>
                <td><a>Water Inc.</a></td> <!-- Hover info for supplier contact info -->
                <td><button type="button" class="btn btn-warning ingredientButton" id="IB${index}">Add</button></td>
            </tr>
        `
    }
    
    //Render the table headers of the ingredient bank
    RenderIngredientBankTop() {
        return `
            <thead class="thead">
                <tr>
                    <th scope="col">Name</th>
                    <th scope="col">Cost/Unit</th>
                    <th scope="col">Weight/Unit</th>
                    <th scope="col">Supplier</th>
                    <th scope="col"></th>
                </tr>
            </thead>
        `
    }  
}

window.addEventListener("load", () => new ShoppingList);