document.addEventListener("DOMContentLoaded", function () {
    const signUpSwitch = document.querySelector(".switch-signup");
    const signInSwitch = document.querySelector(".switch-signin");
    const signInPartial = document.querySelector(".partial-Signin-Switch");
    const signUpPartial = document.querySelector(".partial-Signup-Switch");

    // Set initial active tab (e.g., Sign In active by default)
    signInSwitch.classList.add("active");
    signInPartial.classList.add("active");

    signUpSwitch.addEventListener("click", function () {
        // Switch to sign-up view
        signUpSwitch.classList.add("active");
        signInSwitch.classList.remove("active");
        signUpPartial.classList.add("active");
        signInPartial.classList.remove("active");
    });

    signInSwitch.addEventListener("click", function () {
        // Switch to sign-in view
        signInSwitch.classList.add("active");
        signUpSwitch.classList.remove("active");
        signInPartial.classList.add("active");
        signUpPartial.classList.remove("active");
    });
});

$(document).ready(function () {
    const password = $("#SignUp_Password");
    const confirmPassword = $("#SignUp_ConfirmPassword");
    const confirmPasswordError = $("#confirmPasswordError");

    // Validate form immediately on input
    confirmPassword.on("input", function () {
        if (confirmPassword.val() !== password.val()) {
            confirmPasswordError.text("Passwords do not match.");
        } else {
            confirmPasswordError.text("");
        }
    });

    // Use jQuery validation plugin to validate form
    $("form").validate({
        onkeyup: function (element) {
            $(element).valid();
        }
    });

    // Prevent form submission if passwords don't match
    $("form").on("submit", function (e) {
        if (confirmPassword.val() !== password.val()) {
            e.preventDefault();
            confirmPasswordError.text("Passwords do not match.");
        }
    });
});

