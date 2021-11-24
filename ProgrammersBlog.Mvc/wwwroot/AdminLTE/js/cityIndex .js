$(document).ready(function () {

    /* DataTables start here. */

    const dataTable = $('#categoriesTable').DataTable({
        dom:
            "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-5'i><'col-sm-7'p>>",
        "order": [[6, "desc"]],
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
                        url: '/Admin/City/GetAllCities/',
                        contentType: "application/json",
                        beforeSend: function () {
                            $('#citiesTable').hide();
                            $('.spinner-border').show();
                        },
                        success: function (data) {
                            const cityListDto = jQuery.parseJSON(data);
                            dataTable.clear();
                            console.log(cityListDto);
                            if (cityListDto.ResultStatus === 0) {
                                $.each(cityListDto.Cities.$values,
                                    function (index, city) {
                                        const newTableRow = dataTable.row.add([
                                            city.Id,
                                            city.Name,
                                            city.Content,
                                            city.IsActive ? "Evet" : "Hayır",
                                            city.IsDeleted ? "Evet" : "Hayır",
                                            city.Note,
                                            convertToShortDate(city.CreatedDate),
                                            city.CreatedByName,
                                            convertToShortDate(city.ModifiedDate),
                                            city.ModifiedByName,
                                            `
                                <button class="btn btn-primary btn-sm btn-update" data-id="${city.Id}"><span class="fas fa-edit"></span></button>
                                <button class="btn btn-danger btn-sm btn-delete" data-id="${city.Id}"><span class="fas fa-minus-circle"></span></button>
                                            `
                                        ]).node();
                                        const jqueryTableRow = $(newTableRow);
                                        jqueryTableRow.attr('name', `${city.Id}`);
                                    });
                                dataTable.draw();
                                $('.spinner-border').hide();
                                $('#citiesTable').fadeIn(1400);
                            } else {
                                toastr.error(`${cityListDto.Message}`, 'İşlem Başarısız!');
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            $('.spinner-border').hide();
                            $('#categoriesTable').fadeIn(1000);
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

    /* Ajax GET / Getting the _CategoryAddPartial as Modal Form starts from here. */

    $(function () {
        const url = '/Admin/City/Add/';
        const placeHolderDiv = $('#modalPlaceHolder');
        $('#btnAdd').click(function () {
            $.get(url).done(function (data) {
                placeHolderDiv.html(data);
                placeHolderDiv.find(".modal").modal('show');
            });
        });

        /* Ajax GET / Getting the _CategoryAddPartial as Modal Form ends here. */

        /* Ajax POST / Posting the FormData as CategoryAddDto starts from here. */

        placeHolderDiv.on('click',
            '#btnSave',
            function (event) {
                event.preventDefault();
                const form = $('#form-city-add');
                const actionUrl = form.attr('action');
                const dataToSend = form.serialize();
                $.post(actionUrl, dataToSend).done(function (data) {
                    console.log(data);
                    const cityAddAjaxModel = jQuery.parseJSON(data);
                    console.log(cityAddAjaxModel);
                    const newFormBody = $('.modal-body', cityAddAjaxModel.CityAddPartial);
                    placeHolderDiv.find('.modal-body').replaceWith(newFormBody);
                    const isValid = newFormBody.find('[name="IsValid"]').val() === 'True';
                    if (isValid) {
                        placeHolderDiv.find('.modal').modal('hide');
                        const newTableRow = dataTable.row.add([
                            cityAddAjaxModel.CityDto.City.Id,
                            cityAddAjaxModel.CityDto.City.Name,
                            cityAddAjaxModel.CityDto.City.Content,
                            cityAddAjaxModel.CityDto.City.IsActive ? "Evet" : "Hayır",
                            cityAddAjaxModel.CityDto.City.IsDeleted ? "Evet" : "Hayır",
                            cityAddAjaxModel.CityDto.City.Note,
                            convertToShortDate(cityAddAjaxModel.CityDto.City.CreatedDate),
                            cityAddAjaxModel.CityDto.City.CreatedByName,
                            convertToShortDate(cityAddAjaxModel.CityDto.City.ModifiedDate),
                            cityAddAjaxModel.CityDto.City.ModifiedByName,
                                    `
                                <button class="btn btn-primary btn-sm btn-update" data-id="${cityAddAjaxModel.CityDto.City.Id}"><span class="fas fa-edit"></span></button>
                                <button class="btn btn-danger btn-sm btn-delete" data-id="${cityAddAjaxModel.CityDto.City.Id}"><span class="fas fa-minus-circle"></span></button>
                                            `
                                ]).node();
                                const jqueryTableRow = $(newTableRow);
                        jqueryTableRow.attr('name', `${cityAddAjaxModel.CityDto.City.Id}`);
                        dataTable.draw();
                        toastr.success(`${cityAddAjaxModel.CityDto.Message}`, 'Başarılı İşlem!');
                    } else {
                        let summaryText = "";
                        $('#validation-summary > ul > li').each(function () {
                            let text = $(this).text();
                            summaryText += `*${text}\n`;
                        });
                        toastr.warning(summaryText);
                    }
                });
            });
    });

    /* Ajax POST / Posting the FormData as CategoryAddDto ends here. */

    /* Ajax POST / Deleting a Category starts from here */

    $(document).on('click',
        '.btn-delete',
        function (event) {
            event.preventDefault();
            const id = $(this).attr('data-id');
            const tableRow = $(`[name="${id}"]`);
            const cityName = tableRow.find('td:eq(1)').text();
            Swal.fire({
                title: 'Silmek istediğinize emin misiniz?',
                text: `${cityName} adlı şehir silinicektir!`,
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
                        data: { cityId: id },
                        url: '/Admin/City/Delete/',
                        success: function (data) {
                            const cityDto = jQuery.parseJSON(data);
                            if (cityDto.ResultStatus === 0) {
                                Swal.fire(
                                    'Silindi!',
                                    `${cityDto.City.Name} adlı şehir başarıyla silinmiştir.`,
                                    'success'
                                );
                                dataTable.row(tableRow).remove().draw();
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Başarısız İşlem!',
                                    text: `${cityDto.Message}`,
                                });
                            }
                        },
                        error: function (err) {
                            console.log(err);
                            toastr.error(`${err.responseText}`, "Hata!")
                        }
                    });
                }
            });
        });

/* Ajax GET / Getting the _CityUpdatePartial as Modal Form starts from here. */

    $(function() {
        const url = '/Admin/City/Update/';
        const placeHolderDiv = $('#modalPlaceHolder');
        $(document).on('click',
            '.btn-update',
            function(event) {
                event.preventDefault();
                const id = $(this).attr('data-id');
                $.get(url, { cityId: id }).done(function(data) {
                    placeHolderDiv.html(data);
                    placeHolderDiv.find('.modal').modal('show');
                }).fail(function() {
                    toastr.error("Bir hata oluştu.");
                });
            });

    /* Ajax POST / Updating a City starts from here */

    placeHolderDiv.on('click',
        '#btnUpdate',
        function(event) {
            event.preventDefault();

            const form = $('#form-city-update');
            const actionUrl = form.attr('action');
            const dataToSend = form.serialize();
            $.post(actionUrl, dataToSend).done(function(data) {
                const cityUpdateAjaxModel = jQuery.parseJSON(data);
                console.log(cityUpdateAjaxModel);
                const newFormBody = $('.modal-body', cityUpdateAjaxModel.CityUpdatePartial);
                placeHolderDiv.find('.modal-body').replaceWith(newFormBody);
                const isValid = newFormBody.find('[name="IsValid"]').val() === 'True';
                if (isValid) {
                    const id = cityUpdateAjaxModel.CityDto.City.Id;
                    const tableRow = $(`[name="${id}"]`);
                    placeHolderDiv.find('.modal').modal('hide');
                    dataTable.row(tableRow).data([
                        cityUpdateAjaxModel.CityDto.City.Id,
                        cityUpdateAjaxModel.CityDto.City.Name,
                        cityUpdateAjaxModel.CityDto.City.Content,
                        cityUpdateAjaxModel.CityDto.City.IsActive ? "Evet" : "Hayır",
                        cityUpdateAjaxModel.CityDto.City.IsDeleted ? "Evet" : "Hayır",
                        cityUpdateAjaxModel.CityDto.City.Note,
                        convertToShortDate(cityUpdateAjaxModel.CityDto.City.CreatedDate),
                        cityUpdateAjaxModel.CityDto.City.CreatedByName,
                        convertToShortDate(cityUpdateAjaxModel.CityDto.City.ModifiedDate),
                        cityUpdateAjaxModel.CityDto.City.ModifiedByName,
                        `
                                <button class="btn btn-primary btn-sm btn-update" data-id="${cityUpdateAjaxModel
                            .CityDto.City.Id}"><span class="fas fa-edit"></span></button>
                                <button class="btn btn-danger btn-sm btn-delete" data-id="${cityUpdateAjaxModel
                            .CityDto.City.Id}"><span class="fas fa-minus-circle"></span></button>
                                            `
                    ]);
                    tableRow.attr("name", `${id}`);
                    dataTable.row(tableRow).invalidate();
                    toastr.success(`${cityUpdateAjaxModel.CityDto.Message}`, "Başarılı İşlem!");
                } else {
                    let summaryText = "";
                    $('#validation-summary > ul > li').each(function () {
                        let text = $(this).text();
                        summaryText = `*${text}\n`;
                    });
                    toastr.warning(summaryText);
                }
            }).fail(function(response) {
                console.log(response);
            });
        });

    });
});