function setupAjaxSave(formSelector, buttonSelector, postUrl, attachmentNameSelector = "#attachmentName") {
    $(buttonSelector).on('click', function (e) {
        e.preventDefault();

        const formData = new FormData($(formSelector)[0]);
        formData.append("action", "save");

        $.ajax({
            url: postUrl,
            method: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    Swal.fire("نجاح", response.message, "success");
                    if (response.attachmentName && $(attachmentNameSelector).length) {
                        $(attachmentNameSelector).text(response.attachmentName);
                    }
                } else {
                    Swal.fire("تنبيه", response.message, "info");
                }
            },
            error: function () {
                Swal.fire("خطأ", "حدث خطأ أثناء الحفظ", "error");
            }
        });
    });
}
