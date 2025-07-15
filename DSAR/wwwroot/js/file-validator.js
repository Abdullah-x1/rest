function validateAndDisplayFiles(inputId, spanId, maxFiles = 5, maxSizeMB = 5) {
    const input = document.getElementById(inputId);
    const display = document.getElementById(spanId);

    if (!input || !display) return;

    const files = Array.from(input.files);

    // Check file count
    if (files.length > maxFiles) {
        Swal.fire("تنبيه", `الحد الأقصى للمرفقات هو ${maxFiles} ملفات`, "warning");
        input.value = '';
        display.textContent = '';
        return;
    }

    // Allowed extensions
    const allowedExtensions = [".doc", ".docx", ".xls", ".xlsx", ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"];

    for (let file of files) {
        const ext = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();

        if (!allowedExtensions.includes(ext)) {
            Swal.fire("صيغة غير مدعومة!", "يرجى رفع ملفات بصيغ .doc, .docx, .pdf, .xls, .xlsx أو صور.", "error");
            input.value = '';
            display.textContent = '';
            return;
        }

        if (file.size > maxSizeMB * 1024 * 1024) {
            Swal.fire("حجم الملف كبير", `الملف ${file.name} أكبر من ${maxSizeMB} ميغابايت.`, "error");
            input.value = '';
            display.textContent = '';
            return;
        }
    }

    // If valid, show file names
    display.textContent = files.map(file => file.name).join(', ');
}
