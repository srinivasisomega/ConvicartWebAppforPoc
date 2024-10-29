function updateCookTimeMin() {
    const minValue = document.getElementById("cookTimeMin").value;
    document.getElementById("minCookTimeValue").textContent = minValue;
}

function updateCookTimeMax() {
    const maxValue = document.getElementById("cookTimeMax").value;
    document.getElementById("maxCookTimeValue").textContent = maxValue;
}

function updateMinPoints() {
    const minPointsValue = document.getElementById("minPoints").value;
    document.getElementById("minPointsValue").textContent = minPointsValue;
}

function updateMaxPoints() {
    const maxPointsValue = document.getElementById("maxPoints").value;
    document.getElementById("maxPointsValue").textContent = maxPointsValue;
}

// Event listeners for sliders to update displayed values
document.getElementById("cookTimeMin").addEventListener("input", updateCookTimeMin);
document.getElementById("cookTimeMax").addEventListener("input", updateCookTimeMax);
document.getElementById("minPoints").addEventListener("input", updateMinPoints);
document.getElementById("maxPoints").addEventListener("input", updateMaxPoints);

document.addEventListener("DOMContentLoaded", function () {
    const addToCartButtons = document.querySelectorAll(".Original-add-tocart .add-to-cart");
    const originalAddToCartDivs = document.querySelectorAll(".Original-add-tocart");
    const plusminusDivs = document.querySelectorAll(".Plusminus");

    // Show plusminus div and hide original add to cart div on add-to-cart click
    addToCartButtons.forEach(function (button, index) {
        button.addEventListener("click", function (event) {
            event.preventDefault();
            originalAddToCartDivs[index].style.display = "none";
            plusminusDivs[index].style.display = "flex";
        });
    });
});

document.addEventListener('DOMContentLoaded', function () {
    // Handle Add to Cart button clicks for each card
    document.querySelectorAll('.add-to-cart-plus').forEach(function (button) {
        button.addEventListener('click', function () {
            const productId = this.dataset.productId;
            addToCart(productId, 1);
        });
    });

    // Handle Remove from Cart button clicks for each card
    document.querySelectorAll('.remove-from-cart').forEach(function (button) {
        button.addEventListener('click', function () {
            const productId = this.dataset.productId;
            removeFromCart(productId, 1);
        });
    });
});

// Function to add item to cart
function addToCart(productId, quantity) {
    fetch('/Cart/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: new URLSearchParams({
            productId: productId,
            quantity: quantity
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const quantityDisplay = document.getElementById(`quantity-display-${productId}`);
                quantityDisplay.textContent = parseInt(quantityDisplay.textContent) + quantity;
            } else {
                window.location.href = data.redirectUrl;
            }
        });
}

// Function to remove item from cart
function removeFromCart(productId, quantity) {
    fetch('/Cart/RemoveFromCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: new URLSearchParams({
            productId: productId,
            quantity: quantity
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const quantityDisplay = document.getElementById(`quantity-display-${productId}`);
                const currentQuantity = parseInt(quantityDisplay.textContent);
                if (currentQuantity - quantity > 0) {
                    quantityDisplay.textContent = currentQuantity - quantity;
                } else {
                    // If quantity is 0 or less, hide or disable the plus/minus UI
                    quantityDisplay.textContent = 0;
                    document.querySelector(`.Plusminus[data-product-id="${productId}"]`).style.display = "none";
                    document.querySelector(`.Original-add-tocart[data-product-id="${productId}"]`).style.display = "block";
                }
            } else {
                window.location.href = data.redirectUrl;
            }
        });
}
document.addEventListener('DOMContentLoaded', function () {
    // Handle Add to Cart button clicks for original add to cart
    document.querySelectorAll('.add-to-cart-original').forEach(function (button) {
        button.addEventListener('click', function () {
            const productId = this.dataset.productId;
            addToCart(productId, 1);
            // Show the Plusminus div and hide the original add to cart div after adding
            const originalAddToCartDiv = this.closest('.Original-add-tocart');
            const plusminusDiv = originalAddToCartDiv.nextElementSibling;
            originalAddToCartDiv.style.display = "none";
            plusminusDiv.style.display = "flex";
        });
    });
});

// Function to show the recipe link on hover
function showRecipeLink(productId) {
    document.getElementById(`recipe-link-${productId}`).style.display = 'block';
}

// Function to hide the recipe link when not hovering
function hideRecipeLink(productId) {
    document.getElementById(`recipe-link-${productId}`).style.display = 'none';
}
