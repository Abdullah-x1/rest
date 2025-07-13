function setupFileValidation(inputSelector, nameDisplaySelector) {
    const allowedExtensions = [
        ".doc", ".docx", ".xls", ".xlsx", ".pdf",
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    ];

    document.querySelector(inputSelector)?.addEventListener('change', function () {
        const input = this;
        const file = input.files[0];
        const fileName = file?.name || '';
        const extension = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();

        if (!allowedExtensions.includes(extension)) {
            input.value = '';
            document.querySelector(nameDisplaySelector).textContent = '';
            Swal.fire({
                title: "صيغة غير مدعومة!",
                text: "يرجى رفع ملف بصيغة .doc, .docx, .pdf, .xls, .xlsx أو صورة.",
                icon: "error",
                allowOutsideClick: true,
                allowEscapeKey: true,
                scrollbarPadding: false,
            });
        } else {
            document.querySelector(nameDisplaySelector).textContent = fileName;
        }
    });
}
