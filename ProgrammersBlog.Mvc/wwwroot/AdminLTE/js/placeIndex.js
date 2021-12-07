$(document).ready(function () {

    /* DataTables start here. */

   const dataTable = $('#placesTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        buttons: [
            {
                text: 'Ekle',
                attr: {
                    id: "btnAdd",
                },
                className: 'btn btn-success',
                action: function (e, dt, node, config) {
                }
            },
            {
                text: 'Yenile',
                className: 'btn btn-warning',
                action: function (e, dt, node, config) {
                    $.ajax({
                        type: 'GET',
                        url: '/Admin/Place/GetAllPlaces/',
                        contentType: "application/json",
                        beforeSend: function () {
                            $('#placesTable').hide();
                            $('.spinner-border').show();
                        },
                        success: function (data) {
                            const placeResult = jQuery.parseJSON(data);
                            dataTable.clear();
                            console.log(placeResult);
                            if (placeResult.Data.ResultStatus === 0) {
                                let categoriesArray = [];
                                $.each(placeResult.Data.Places.$values,
                                    function (index, place) {
                                        const newPlace = getJsonNetObject(place, placeResult.Data.Places.$values);
                                        let newCategory = getJsonNetObject(newPlace.Category, newPlace);
                                        if (newCategory !== null) {
                                            categoriesArray.push(newCategory);
                                        }
                                        if (newCategory === null) {
                                            newCategory = categoriesArray.find((category) => {
                                                return category.$id === newPlace.Category.$ref;
                                            });
                                        }
                                        console.log(newOlace);
                                        console.log(newCategory);
                                        const newTableRow = dataTable.row.add([
                                            newPlace.Id,
                                            newCategory.Name,
                                            newCity.Name,
                                            newPlace.Name,
                                            `<img src="/img/${newPlace.PlacePicture}" alt="${newPlace.Name}" class="my-image-table" />`,
                                            `${convertToShortDate(newPlace.Date)}`,
                                            `${newArticle.IsActive ? "Evet" : "Hayır"}`,
                                            `${newArticle.IsDeleted ? "Evet" : "Hayır"}`,
                                            `${convertToShortDate(newArticle.CreatedDate)}`,
                                            newPlace.CreatedByName,
                                            `${convertToShortDate(newPlace.ModifiedDate)}`,
                                            newPlace.ModifiedByName,
                                            `
                                <a class="btn btn-primary btn-sm btn-update" href="/Admin/Place/Update?articleId=${newPlace.Id}"><span class="fas fa-edit"></span></a>
                                <button class="btn btn-danger btn-sm btn-delete" data-id="${newPlace.Id}"><span class="fas fa-minus-circle"></span></button>
                                            `
                                        ]).node();
                                        const jqueryTableRow = $(newTableRow);
                                        jqueryTableRow.attr('name', `${newPlace.Id}`);
                                    });
                                dataTable.draw();
                                $('.spinner-border').hide();
                                $('#olacesTable').fadeIn(1400);
                            } else {
                                toastr.error(`${placeResult.Data.Message}`, 'İşlem Başarısız!');
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            $('.spinner-border').hide();
                            $('#placesTable').fadeIn(1000);
                            toastr.error(`${err.responseText}`, 'Hata!');
                        }
                    });
                }
            }
        ],
        language: {
            "sDecimal": ",",
            "sEmptyTable": "Tabloda herhangi bir veri mevcut değil",
            "sInfo": "_TOTAL_ kayıttan _START_ - _END_ arasındaki kayıtlar gösteriliyor",
            "sInfoEmpty": "Kayıt yok",
            "sInfoFiltered": "(_MAX_ kayıt içerisinden bulunan)",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLengthMenu": "Sayfada _MENU_ kayıt göster",
            "sLoadingRecords": "Yükleniyor...",
            "sProcessing": "İşleniyor...",
            "sSearch": "Ara:",
            "sZeroRecords": "Eşleşen kayıt bulunamadı",
            "oPaginate": {
                "sFirst": "İlk",
                "sLast": "Son",
                "sNext": "Sonraki",
                "sPrevious": "Önceki"
            },
            "oAria": {
                "sSortAscending": ": artan sütun sıralamasını aktifleştir",
                "sSortDescending": ": azalan sütun sıralamasını aktifleştir"
            },
            "select": {
                "rows": {
                    "_": "%d kayıt seçildi",
                    "0": "",
                    "1": "1 kayıt seçildi"
                }
            }
        }
    });

    /* DataTables end here */

    /* Ajax POST / Deleting a Place starts from here */

    $(document).on('click',
        '.btn-delete',
        function (event) {
            event.preventDefault();
            const id = $(this).attr('data-id');
            const tableRow = $(`[name="${id}"]`);
            const articleTitle = tableRow.find('td:eq(2)').text();
            Swal.fire({
                title: 'Silmek istediğinize emin misiniz?',
                text: `${articleTitle} başlıklı lokasyon silinicektir!`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Evet, silmek istiyorum.',
                cancelButtonText: 'Hayır, silmek istemiyorum.'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        data: { articleId: id },
                        url: '/Admin/Place/Delete/',
                        success: function (data) {
                            const placeResult = jQuery.parseJSON(data);
                            if (placeResult.ResultStatus === 0) {
                                Swal.fire(
                                    'Silindi!',
                                    `${placeResult.Message}`,
                                    'success'
                                );

                                dataTable.row(tableRow).remove().draw();
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Başarısız İşlem!',
                                    text: `${placeResult.Message}`,
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            toastr.error(`${err.responseText}`, "Hata!");
                        }
                    });
                }
            });
        });

});