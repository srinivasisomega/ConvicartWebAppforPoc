document.addEventListener("DOMContentLoaded", function () {
    const monthSwitch = document.querySelector(".switch-Month");
    const yearSwitch = document.querySelector(".switch-Year");
    const monthPartial = document.querySelector(".partial-Month-Switch");
    const yearPartial = document.querySelector(".partial-Year-Switch");

    // Set initial active tab (Month active by default)
    monthSwitch.classList.add("active1");
    monthPartial.classList.add("active1");

    monthSwitch.addEventListener("click", function () {
        // Switch to month view
        monthSwitch.classList.add("active1");
        yearSwitch.classList.remove("active1");
        monthPartial.classList.add("active1");
        yearPartial.classList.remove("active1");
    });

    yearSwitch.addEventListener("click", function () {
        // Switch to year view
        yearSwitch.classList.add("active1");
        monthSwitch.classList.remove("active1");
        yearPartial.classList.add("active1");
        monthPartial.classList.remove("active1");
    });
});