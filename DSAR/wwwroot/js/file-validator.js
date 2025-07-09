
function updateFileName(inputId, spanId) {
    const input = document.getElementById(inputId);
    const display = document.getElementById(spanId);

    if (!input || !display) return;

    if (input.files.length > 5) {
        Swal.fire("تنبيه", "الحد الأقصى للمرفقات هو 5 ملفات", "warning");
        input.value = '';
        display.textContent = '';
        return;
    }

    const allowedExtensions = [".doc", ".docx", ".xls", ".xlsx", ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"];

    const invalidFiles = Array.from(input.files).filter(file => {
        const ext = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
        return !allowedExtensions.includes(ext);
    });

    if (invalidFiles.length > 0) {
        Swal.fire("صيغة غير مدعومة!", "يرجى رفع ملفات بصيغ .doc, .docx, .pdf, .xls, .xlsx أو صور.", "error");
        input.value = '';
        display.textContent = '';
    } else {
        const fileNames = Array.from(input.files).map(file => file.name).join(', ');
        display.textContent = fileNames;
    }
}
