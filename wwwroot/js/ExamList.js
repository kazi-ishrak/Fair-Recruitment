$(function () {

    $('#ReportListTable').DataTable({
        "processing": true,
        "language": {
            "processing": '<i class="fa fa-spinner fa-spin fa-2x fa-fw text-primary"></i><span class="sr-only">Loading...</span>'
        },
        "serverSide": true,
        "order": [0, "desc"],
        //deferRender: true,               //For Client Side Fast Loading
        "responsive": true,

        "ajax": {
            "url": "/api/ExamsList",
            "type": "POST",  //Important
            //"dataSrc": "",               //For Client Side
        },

        "columns": [
            {
                "data": null,
                "name": "SL.",
                "render": function (data, type, row, meta) {
                    return `<button class="btn btn-sm" onclick="OpenExamResultPage(${row.exam_code})"> <span style="text-decoration: underline; color: blue;">${meta.row + 1}</span></button>`;
                },
            },
            { "data": "office_name", "name": "office_name" },
            { "data": "post_name", "name": "post_name" },
            { "data": "exam_type_name", "name": "exam_type_name" },
            { "data": "exam_date", "name": "exam_date" },
            {
                "data": null,
                "width": "10%",
                "render": function (data, type, row, meta) {
                    return '<a href="#" onclick=GeneratePDfReport(' + row.id + ') class="btn btn-outline-light btn-sm text-primary" > <i class="fa fa-solid fa-file-pdf-o text-danger" aria-hidden="true"></i></a>  <a href="#" onclick=GenerateExcelReport(' + row.id + ') class="btn btn-outline-light btn-sm text-primary" > <i class="fa fa-solid fa-file-excel-o text-success" aria-hidden="true"></i></a>';
                },
            },
        ],

        select: true,
        //stateSave: true,

        //scrollY: 300,
        //scroller: {
        //    loadingIndicator: true
        //},

        "columnDefs": [
            { "className": "dt-center", "targets": "_all" }
        ],

        //"columnDefs": [{
        //    "targets": -1,
        //    "data": null,
        //    "render": function (data, type, row, meta) {
        //        return '<a href="EditMold/'+ meta.row +'" class="btn btn-primary"><i class="fa fa-pen" aria-hidden="true"></i></a>';
        //    }
        //}]
    });
});

/*function ExamResult(rowid, office, post, type, date) {
    console.log(office);
    window.location.href = `/ExamResultPage?id=${rowid}`;
    $('#officeButton').text('Office: ' + office);
}*/

function OpenExamResultPage(id) {

    window.location.href = `/ExamResultPage?id=${id}`;
}

function GenerateExcelReport(id) {

    window.location.href = `/api/ExamResults/excelDownload?ExamId=${id}`;
}

function GeneratePDfReport(id) {
    window.location.href = `/api/ExamResults/pdfDownload?ExamId=${id}`;
}