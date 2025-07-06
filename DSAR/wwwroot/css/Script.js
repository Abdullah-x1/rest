
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

let index = document.querySelectorAll("#tableBody tr").length;
function addRow() {
    const tableBody = document.getElementById("tableBody");
    const buttonContainer = document.querySelector(".button-container");
    if (tableBody.rows.length < 20) { // Maximum 10 rows
        const row = document.createElement("tr");
        row.innerHTML = `
                    <td><input name="Descriptions[${index}].Description1" class="form-control" /></td>
                    <td><input name="Descriptions[${index}].Description2" class="form-control" /></td>
                `;
        tableBody.appendChild(row);
        index++;
    }
    // Disable add button if max rows reached
    if (tableBody.rows.length >= 20) {
        document.querySelector(".btn-success").disabled = true;
    }
}

function deleteLastRow() {
    const tableBody = document.getElementById("tableBody");
    if (tableBody.rows.length > 1) { // Keep at least 4 rows
        tableBody.deleteRow(tableBody.rows.length - 1);
        index--;
        // Re-enable add button if below max rows
        if (tableBody.rows.length < 20) {
            document.querySelector(".btn-success").disabled = false;
        }
    }
}