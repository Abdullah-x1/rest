// step-progress.js

document.addEventListener("DOMContentLoaded", function () {
    const steps = document.querySelectorAll(".step-progress-container .step");
    const container = document.querySelector(".step-progress-container");

    if (!container || steps.length === 0) return;

    const currentStep = parseInt(container.getAttribute("data-current-step"));

    steps.forEach((step, index) => {
        const stepNumber = index + 1;

        step.classList.remove("active", "completed");

        if (stepNumber < currentStep) {
            step.classList.add("completed");
        } else if (stepNumber === currentStep) {
            step.classList.add("active");
        }
    });
});
