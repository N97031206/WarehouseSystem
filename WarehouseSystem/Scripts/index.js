//$(document).ready(function () {

//    $('#userTable tfoot th').each(function () {
//        $(this).html('<input type="text" />');
//    });

//    var oTable = $('#userTable').DataTable({
//        "serverSide": true,
//        "ajax": {
//            "type": "POST",
//            "url": '/Home/DataHandler',
//            "contentType": 'application/json; charset=utf-8',
//            'data': function (data) { return data = JSON.stringify(data); }
//        },
//        "dom": 'frtiS',
//        "scrollY": 500,
//        "scrollX": true,
//        "scrollCollapse": true,
//        "scroller": {
//            loadingIndicator: false
//        },
//        "processing": true,
//        "paging": true,
//        "deferRender": true,
//        "columns": [
//       { "data": "userId" },
//       { "data": "email" },
//       { "data": "name" },
//       { "data": "createTime" },
//       { "data": "lastUpdateTime" },

//        ],
//        "order": [0, "asc"]

//    });

//});

$(document).ready(function () {
    $('#userTable').DataTable({
        "paging": true,
        "info": true,
        "searching": true,
        "columnDefs": [
            { "targets": 0, "width": "20%", "orderable": true },
            { "targets": 1, "width": "15%", "orderable": true },
            { "targets": 2, "width": "15%", "orderable": true },
            { "targets": 3, "width": "20%", "orderable": false },
            { "targets": 4, "width": "20%", "orderable": false },
            { "targets": 5, "width": "10%", "orderable": false }
        ],
    });

    //$('#myModal .modal-footer button').on('click', function (e) {
    //    var $target = $(e.target);
    //    if ($target.index() == 1) {
    //        deleteProject();
    //    }
    //});
});


//function setCartTable(cartTypeId) {
//    $('#cartTable_' + cartTypeId).DataTable({
//        'paging': true,
//        'info': false,
//        'searching': true,
//        'columnDefs': [
//            { 'targets': 0, 'width': "30%", 'orderable': true },
//            { 'targets': 1, 'width': "30%", 'orderable': false },
//            { 'targets': 2, 'width': "15%", 'orderable': true },
//            { 'targets': 3, 'width': "15%", 'orderable': true },
//            { 'targets': 4, 'width': "10%", 'orderable': false }
//        ],
//    });
//}

$(document).ready(function () {
    $('#cartTable').DataTable({
        "paging": true,
        "info": true,
        "searching": false,
        "columnDefs": [
            { "targets": 0, "width": "30%", "orderable": true },
            { "targets": 1, "width": "30%", "orderable": false },
            { "targets": 2, "width": "15%", "orderable": true },
            { "targets": 3, "width": "15%", "orderable": true },
            { "targets": 4, "width": "10%", "orderable": false }
        ],
    });
});

$(document).ready(function () {
    $('#fieldTable').DataTable({
        "paging": true,
        "info": true,
        "searching": true,
        "columnDefs": [
            { "targets": 0, "width": "40%", "orderable": true },
            { "targets": 1, "width": "20%", "orderable": true },
            { "targets": 2, "width": "20%", "orderable": true },
            { "targets": 3, "width": "20%", "orderable": false }
        ],
    });
});