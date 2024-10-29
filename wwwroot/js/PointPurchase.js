// Define the rate for points to currency (ensure this matches the rate in your server-side logic)
const pointsToCurrencyRate = 20;

// JavaScript function to update the amount to pay dynamically
function updateAmountToPay() {
    // Get the number of points from the input field
    const points = parseInt(document.getElementById("pointsInput").value) || 0;

    // Calculate the amount to pay
    const amountToPay = points * pointsToCurrencyRate;

    // Update the amount to pay field
    document.getElementById("amountToPay").value = amountToPay.toFixed(2); // Limit to 2 decimal places
}

// Redirect to profile page after 5 seconds if the confirmation message is present
window.onload = function () {
    const confirmationMessage = document.getElementById("confirmationMessage");
    if (confirmationMessage) {
        setTimeout(function () {
            window.location.href = "/Customer/Profile"; // Direct URL to the Profile page
        }, 5000); // 5000 milliseconds = 5 seconds
    }
};
