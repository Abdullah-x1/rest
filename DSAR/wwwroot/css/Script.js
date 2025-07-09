
window.onload = function () {
    const popup = document.getElementById("termsPopup");
    const checkbox = document.getElementById("termsCheckbox");
    const confirmBtn = document.getElementById("acceptTermsBtn");

    if (popup) popup.style.display = "flex";

    checkbox?.addEventListener("change", function () {
        confirmBtn.disabled = !this.checked;
    });
}

function closePopup() {
    const popup = document.getElementById("termsPopup");
    popup.style.display = "none";
}

/*steps*/

document.querySelectorAll('textarea.auto-expand').forEach(textarea => {
    textarea.addEventListener('input', () => {
        textarea.style.height = 'auto';
        textarea.style.height = textarea.scrollHeight + 'px';
    });
});

/*step descrtption*/

