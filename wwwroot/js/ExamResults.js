//function ExamResultTable(id) {

//    $('#ReportListTable').DataTable({
//        "processing": true,
//        "language": {
//            "processing": '<i class="fa fa-spinner fa-spin fa-2x fa-fw text-primary"></i><span class="sr-only">Loading...</span>'
//        },
//        "serverSide": true,
//        "order": [0, "desc"],
//        //deferRender: true,               //For Client Side Fast Loading
//        "responsive": true,

//        "ajax": {
//            "url": "/api/ExamResults/resultTable",
//            "type": "POST",  //Important
//            //"dataSrc": "",               //For Client Side
//            "data": function (d) {

//                d.ExamId = id;
//                return d;
//            },
//            "contentType": "application/x-www-form-urlencoded"
//        },

//        "columns": [
//            {
//                "data": null,
//                "name": "SL.",
//                "render": function (data, type, row, meta) {

//                    return `${ meta.row + 1 }`;
//                },
//            },
//            { "data": "roll", "name": "roll" },
//            { "data": "name", "name": "name" },
//            { "data": "room_similarity", "name": "room_similarity" },
//            {
//                "data": "room_timestamp", "orderable": true,
//                "render": function (data, type, row, meta) {
//                    return formatDatetime(data);
//                },
//            },
//            {
//                "data": "room_decision", "orderable": true,
//                "render": function (data, type, row, meta) {
//                    if (data == true) {
//                        return '<button type="button" class="btn btn-light" style="pointer-events: none; opacity: 1;"><div class="fw-bold" style="color:green;"> Accept</div ></button > ';
//                    }
//                    else {
//                        return '<button type="button" class="btn btn-light" style="pointer-events: none; opacity: 1;"><div class="fw-bold" style="color:red;"> Reject</div ></button > ';
//                    }
//                },
//            },
//            {
//                "data": "enrolled_image",
//                "name": "enrolled_image",
//                "render": function (data, type, row) {
//                    if (data) {
//                        return `<img src="data:image/jpeg;base64,${data}" alt="Enrolled Image" style="width:50px; height:auto;" />`;
//                    } else {
//                        return 'No Image';
//                    }
//                }
//            },
//            {
//                "data": "room_captured_image",
//                "name": "room_captured_image",
//                "render": function (data, type, row) {
//                    if (data) {
//                        return `<img src="data:image/jpeg;base64,${data}" alt="Enrolled Image" style="width:50px; height:auto;" />`;
//                    } else {
//                        return 'No Image';
//                    }
//                }
//            },
//        ],
//        select: true,
//        //stateSave: true,
//        //scrollY: 300,
//        //scroller: {
//        //    loadingIndicator: true
//        //},

//        "columnDefs": [
//            { "className": "dt-center", "targets": "_all" }
//        ],

//        //"columnDefs": [{
//        //    "targets": -1,
//        //    "data": null,
//        //    "render": function (data, type, row, meta) {
//        //        return '<a href="EditMold/'+ meta.row +'" class="btn btn-primary"><i class="fa fa-pen" aria-hidden="true"></i></a>';
//        //    }
//        //}]
//    });
//}

function ExamResultTable(id) {

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
            "url": "/api/ExamResults/resultTable",
            "type": "POST",  //Important
            //"dataSrc": "",               //For Client Side
            "data": function (d) {

                d.ExamId = id;
                return d;
            },
            "contentType": "application/x-www-form-urlencoded"
        },

        "columns": [
            {
                "data": null,
                "name": "SL.",
                "render": function (data, type, row, meta) {

                    return `${meta.row + 1}`;
                },
            },
            { "data": "position", "name": "position" },
            { "data": "roll", "name": "roll" },
            { "data": "name", "name": "name" },
            { "data": "marks", "name": "marks" },
            {
                "data": "enrolled_image",
                "name": "enrolled_image",
                "render": function (data, type, row) {
                    if (data) {
                        return `<img src="data:image/jpeg;base64,${data}" alt="Enrolled Image" style="width:50px; height:auto;" />`;
                    } else {
                        return 'No Image';
                    }
                }
            },
            {
                "data": "room_captured_image",
                "name": "room_captured_image",
                "render": function (data, type, row) {
                    if (data) {
                        return `<img src="data:image/jpeg;base64,${data}" alt="room_captured_image" style="width:50px; height:auto;" />`;
                    } else {
                        return 'No Image';
                    }
                }
            },
            { "data": "room_similarity", "name": "room_similarity" },
            {
                "data": "room_decision", "orderable": true,
                "render": function (data, type, row, meta) {
                    if (data == true) {
                        return '<button type="button" class="btn btn-light" style="pointer-events: none; opacity: 1;"><div class="fw-bold" style="color:green;"> Accept</div ></button > ';
                    }
                    else if (data == false) {
                        return '<button type="button" class="btn btn-light" style="pointer-events: none; opacity: 1;"><div class="fw-bold" style="color:red;"> Reject</div ></button > ';
                    }
                    else {
                        return '';
                    }
                },
            },
            {
                "data": "room_timestamp", "orderable": true,
                "render": function (data, type, row, meta) {
                    if (data == null) {
                        return '';
                    }
                    else {
                        return formatDatetime(data);
                    }
                },
            },
            {
                "data": "gate_captured_image",
                "name": "gate_captured_image",
                "render": function (data, type, row) {
                    if (data) {
                        return `<img src="data:image/jpeg;base64,${data}" alt="gate_captured_image" style="width:50px; height:auto;" />`;
                    } else {
                        return 'No Image';
                    }
                }
            },
            { "data": "gate_similarity", "name": "gate_similarity" },
            {
                "data": "gate_decision", "orderable": true,
                "render": function (data, type, row, meta) {
                    if (data == true) {
                        return '<button type="button" class="btn btn-light" style="pointer-events: none; opacity: 1;"><div class="fw-bold" style="color:green;"> Accept</div ></button > ';
                    }
                    else if (data == false) {
                        return '<button type="button" class="btn btn-light" style="pointer-events: none; opacity: 1;"><div class="fw-bold" style="color:red;"> Reject</div ></button > ';
                    }
                    else {
                        return '';
                    }
                },
            },
            {
                "data": "gate_timestamp", "orderable": true,
                "render": function (data, type, row, meta) {

                    if (data == null) {
                        return '';
                    }
                    else {
                        return formatDatetime(data);
                    }
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
}


$(function () {

    const urlParams = new URLSearchParams(window.location.search);
    const examId = urlParams.get('id');

    ExamResult(examId);
    ExamResultTable(examId);
    $('#downloadExcelReport').on('click', () => GenerateExcelReport(examId));
    $('#downloadPDFReport').on('click', () => GeneratePdfReport(examId));
});

function GenerateExcelReport(id) {
    window.location.href = `/api/ExamResults/excelDownload?ExamId=${id}`;
}
function GeneratePdfReport(id) {
    window.location.href = `/api/ExamResults/pdfDownload?ExamId=${id}`;
}
function ExamResult(id) {

    $.ajax({
        url: `/api/ExamResults/result?id=${id}`,
        method: 'GET',
        contentType: 'application/json',
        success: function (data) {
            $('#officeButton').text('Office: ' + data[0].office_name).css('font-weight', 'bold');
            $('#postButton').text('Post: ' + data[0].post_name).css('font-weight', 'bold');
            $('#dateButton').text('Date: ' + data[0].exam_date).css('font-weight', 'bold');
            $('#typeButton').text('Type: ' + data[0].exam_type_name).css('font-weight', 'bold');
        },

    });
}

function formatDatetime(data) {
    var dateObj = new Date(data);
    var day = ("0" + dateObj.getDate()).slice(-2);
    var monthNames = ["January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"];
    var month = monthNames[dateObj.getMonth()];
    var year = dateObj.getFullYear();
    var hours = dateObj.getHours();
    var ampm = hours >= 12 ? "PM" : "AM";
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    var minutes = ("0" + dateObj.getMinutes()).slice(-2);
    var seconds = ("0" + dateObj.getSeconds()).slice(-2);
    return year + " " + month + " " + day + ", " + hours + ":" + minutes + ":" + seconds + " " + ampm;
}

