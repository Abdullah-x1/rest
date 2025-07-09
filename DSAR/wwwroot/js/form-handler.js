function setupAjaxSave(formSelector, saveButtonSelector, saveUrl, attachmentNameSelector = "#file-name") {
    $(document).ready(function () {
        $(saveButtonSelector).click(function (e) {
            e.preventDefault();

            const formData = new FormData($(formSelector)[0]);
            formData.append('action', 'save');

            $.ajax({
                url: saveUrl,
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    if (response.success) {
                        Swal.fire({
                            icon: 'success',
                            title: response.message,
                            confirmButtonText: 'حسنًا',
                            confirmButtonColor: '#3085d6',
                            customClass: { popup: 'swal2-rtl' }
                        });

                        if (response.attachmentName && $(attachmentNameSelector).length) {
                            $(attachmentNameSelector).text(response.attachmentName);
                        }
                    } else {
                        Swal.fire({
                            icon: 'info',
                            title: response.message,
                            confirmButtonText: 'حسنًا',
                            confirmButtonColor: '#3085d6',
                            customClass: { popup: 'swal2-rtl' }
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'حدث خطأ أثناء الحفظ',
                        confirmButtonText: 'حسنًا',
                        confirmButtonColor: '#d33',
                        customClass: { popup: 'swal2-rtl' }
                    });
                }
            });
        });
    });
}
