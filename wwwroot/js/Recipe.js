    document.addEventListener("DOMContentLoaded", function () {
        const productId = @Html.Raw(ViewBag.ProductId); // Get ProductId from ViewBag

    // Call this function when the document is ready or as needed
    loadRecipeSteps(productId); // Load steps based on the product ID

    // Add to Cart functionality
    document.querySelectorAll('.add-to-cart-original').forEach(function (button) {
        button.addEventListener('click', function () {
            addToCart(productId, 1); // Use productId from ViewBag
            const originalAddToCartDiv = this.closest('.Original-add-tocart');
            const plusminusDiv = originalAddToCartDiv.nextElementSibling;
            originalAddToCartDiv.style.display = "none";
            plusminusDiv.style.display = "flex";
        });
        });

    // Increment quantity
    document.querySelectorAll('.add-to-cart-plus').forEach(function (button) {
        button.addEventListener('click', function () {
            addToCart(productId, 1); // Use productId from ViewBag
        });
        });

    // Decrement quantity
    document.querySelectorAll('.remove-from-cart').forEach(function (button) {
        button.addEventListener('click', function () {
            removeFromCart(productId, 1); // Use productId from ViewBag
        });
        });
    });

    // Function to load recipe steps
    function loadRecipeSteps(productId) {
        $.get('@Url.Action("GetRecipeSteps", "Store")', { productId: productId }, function (data) {
            $('#recipe-steps-container').html(data); // Assuming there's a div with this ID in your main view
        });
    }

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
