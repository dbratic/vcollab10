//const { data } = require("jquery");

let BaseUrl = window.location.href;

$(function () {
    $('#txtStartDate').datetimepicker({
        minView: 2,
        pickTime: false,
        format: "mm/dd/yyyy"
    });
    $('#txtEndDate').datetimepicker({
        minView: 2,
        pickTime: false,
        format: "mm/dd/yyyy"
    });

    $('#txtTimesheetStartDate').datetimepicker({
        minView: 2,
        pickTime: false,
        format: "mm/dd/yyyy"
    });

    $('#txtTimesheetEndDate').datetimepicker({
        minView: 2,
        pickTime: false,
        format: "mm/dd/yyyy"
    });

    $('#lstRecordDate').on('change', function () {
        if (this.value == "Custom Date" || this.value == "Since Inception") {
            $('#divDateStart').show();
            $('#divDateEnd').show();
        } else {
            $('#divDateStart').hide();
            $('#divDateEnd').hide();
        }
        SetDateRange(this.value);
    });

    $('#lstUser').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        buttonWidth: '100%',
        inheritClass: true,
        buttonTextAlignment: 'left',
        maxHeight: 200,
        nonSelectedText: 'All Users',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstClient').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 200,
        nonSelectedText: 'All Clients',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstClientAccount').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 200,
        nonSelectedText: 'All Accounts',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstClientAccount1').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 200,
        nonSelectedText: 'All Accounts',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });
    $('#lstAccountStatus').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 200,
        nonSelectedText: 'All Status',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstModule').multiselect({
        includeSelectAllOption: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 165,
        nonSelectedText: 'All Modules',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstModule').on('change', function () {
        if (this.value == "Request") {
            $("#lstRequestType").prop('disabled', false);
            $('#lstRequestType option[value=""]').text("All Request Types");
        } else {
            $("#lstRequestType").prop('disabled', true);
            $("#lstRequestType").val('');
            $('#lstRequestType option[value=""]').text("");
        }
    });

    $('#lstDepartment').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 165,
        nonSelectedText: 'All Departments',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstEmploymentType').multiselect({
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 105,
        nonSelectedText: 'All Employment Types',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstLocation').multiselect({
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 105,
        nonSelectedText: 'All Locations',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    //Timesheet Approval Report
    $('#lstTimeDepartment').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 165,
        nonSelectedText: 'All Departments',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstTimeEmployeeType').multiselect({
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 105,
        nonSelectedText: 'All Employment Types',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstTimeLocation').multiselect({
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 105,
        nonSelectedText: 'All Locations',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });
    
    $('#lstTimeApprover').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 105,
        nonSelectedText: 'All Approvers',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });

    $('#lstTimeStatus').multiselect({
        includeSelectAllOption: true,
        enableCaseInsensitiveFiltering: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 105,
        nonSelectedText: 'All Status',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });
    $('#lstTimesheetType').on('change', function () {

        $("#lstTimeStatus").multiselect("refresh");
        $("#lstTimeDepartment").multiselect("refresh");
        $("#lstTimeEmployeeType").multiselect("refresh");
        $("#lstTimeLocation").multiselect("refresh");
        $("#lstTimeApprover").multiselect("refresh");
        $("#lstTimeStatus").multiselect("refresh");
        $("#txtTimesheetStartDate").val("");
        $("#txtTimesheetEndDate").val("");

        if (this.value == "TCR") {             
            $("#lstTimeStatus").multiselect("disable");
        } else {            
            $("#lstTimeStatus").multiselect("enable");
        }
    });      

    //End Timesheet Approval Report

    $('#lstEmpDepartment').multiselect({
        enableFiltering: true,
        enableCaseInsensitiveFiltering: true,
        includeSelectAllOption: true,
        inheritClass: true,
        buttonWidth: '100%',
        buttonTextAlignment: 'left',
        maxHeight: 165,
        nonSelectedText: 'All Departments',
        templates: {
            button: '<button type="button" class="multiselect dropdown-toggle" data-bs-toggle="dropdown"><span class="multiselect-selected-text"></span></button>'
        }
    });
    

    $('#lstReportType').on('change', function () {
        if (this.value == "HoursByAccount" || this.value == "HoursByAccountDetails" || this.value == "Compliance" || this.value == "UtilizationByPerson" || this.value == "UtilizationByDepartment" || this.value == "Inventory") {
            $("#lstUser").val("");
            $("#lstUser").multiselect("refresh");
            $("#lstUser").multiselect("disable");
            $("#lstClient").val("");
            $("#lstClient").multiselect("refresh");
            $("#lstClient").multiselect("disable");
            $("#lstClientAccount").val("");
            $("#lstClientAccount").multiselect("refresh");
            $("#lstClientAccount").multiselect("disable");
            $("#lstModule").val("");
            $("#lstModule").multiselect("refresh");
            $("#lstModule").multiselect("disable");
            $("#lstBillable").prop('disabled', true);
            $('#lstBillable option[value=""]').text("All");
            $("#lstEmploymentType").val("Emp");
            $("#lstEmploymentType").multiselect("refresh");
            if (this.value == "UtilizationByPerson") {
                $("#lstEmploymentType").multiselect("disable");
                $("#lstDepartment").multiselect("enable");
                $("#lstLocation").multiselect("enable");
                $("#lstRecordDate").prop('disabled', false);
                $("#lstRecordDate").val("Last Month").trigger("change");
            } else if (this.value == "UtilizationByDepartment") {
                $("#lstEmploymentType").multiselect("disable");
                $("#lstDepartment").multiselect("enable");
                $("#lstLocation").val("");
                $("#lstLocation").multiselect("refresh");
                $("#lstLocation").multiselect("disable");
                $("#lstRecordDate").prop('disabled', false);
                $("#lstRecordDate").val("Last Month").trigger("change");
            } else if (this.value == "Inventory") {
                $("#lstEmploymentType").val("");
                $("#lstEmploymentType").multiselect("refresh");
                $("#lstEmploymentType").multiselect("disable");
                $("#lstDepartment").val("");
                $("#lstDepartment").multiselect("refresh");
                $("#lstDepartment").multiselect("disable");
                $("#lstLocation").val("");
                $("#lstLocation").multiselect("refresh");
                $("#lstLocation").multiselect("disable");
                $("#lstRecordDate").val("Today").trigger("change");
                $("#lstRecordDate").prop('disabled', true);
            } else if (this.value == "HoursByAccount") {
                $("#lstEmploymentType").val("");
                $("#lstEmploymentType").multiselect("refresh");
                $("#lstEmploymentType").multiselect("disable");
                $("#lstDepartment").val("");
                $("#lstDepartment").multiselect("refresh");
                $("#lstDepartment").multiselect("disable");
                $("#lstLocation").val("");
                $("#lstLocation").multiselect("refresh");
                $("#lstLocation").multiselect("disable");
                $("#lstRecordDate").val("Today").trigger("change");
                $("#lstRecordDate").prop('disabled', true);
            } else if (this.value == "HoursByAccountDetails") {
                $("#lstEmploymentType").val("");
                $("#lstEmploymentType").multiselect("refresh");
                $("#lstEmploymentType").multiselect("disable");
                $("#lstDepartment").val("");
                $("#lstDepartment").multiselect("refresh");
                $("#lstDepartment").multiselect("enable");
                $("#lstLocation").val("");
                $("#lstLocation").multiselect("refresh");
                $("#lstLocation").multiselect("disable");
                $("#lstRecordDate").val("Today").trigger("change");
                $("#lstRecordDate").prop('disabled', true);
            } else {
                $("#lstEmploymentType").multiselect("enable");
                $("#lstDepartment").multiselect("enable");
                $("#lstLocation").multiselect("enable");
                $("#lstRecordDate").prop('disabled', false);
                $("#lstRecordDate").val("Last Week").trigger("change");
            }
        } else {
            $("#lstUser").multiselect("enable");
            $("#lstClient").multiselect("enable");
            $("#lstClientAccount").multiselect("enable");
            $("#lstModule").multiselect("enable");
            $("#lstBillable").prop('disabled', false);
            $("#lstEmploymentType").val("");
            $("#lstEmploymentType").multiselect("refresh");
            $("#lstEmploymentType").multiselect("enable");
            $("#lstDepartment").multiselect("enable");
            $("#lstLocation").multiselect("enable");
            $("#lstRecordDate").prop('disabled', false);
            $("#lstRecordDate").val("Today").trigger("change");
        }
    });

    $("#btnGenerateReport").on("click", function (e) {
        e.preventDefault();
        var userId = $("#lstUser").val();
        var clientId = $("#lstClient").val();
        var accountId = $("#lstClientAccount").val();
        var module = $("#lstModule").val();
        var requestType = $("#lstRequestType").val();
        var isBillable = $("#lstBillable").val();

        var recordDate = $("lstRecordDate");
        var recordStartDate;
        var recordEndDate; 
        var recordStartDate = $("#txtStartDate").val();
        var recordEndDate = $("#txtEndDate").val();
        var department = $("#lstDepartment").val();
        var location = $("#lstLocation").val();
        var employmentType = $("#lstEmploymentType").val();
        var reportType = $("#lstReportType").val();

        if (new Date(recordStartDate) > new Date(recordEndDate)) {
            alert("Start date cannot be greater than end date.");
            return;
        }

        let objRpt = {
            UserId: userId.join(","),
            ClientId: clientId.join(","),
            AccountId: accountId.join(","),
            Module: module.join(","),
            RequestType: requestType,
            IsBillable: isBillable,
            RecordStartDate: recordStartDate,
            RecordEndDate: recordEndDate,
            Department: department.join(","),
            Location: location.join(","),
            EmploymentType: employmentType.join(","),
            ReportType: reportType
        };

        GenerateReportTable(objRpt);
        $("#dvReport").removeClass("d-none");
        $("#errReportName").addClass("d-none");
    });

    $("#btnSaveReport").on("click", function (e) {
        e.preventDefault();
        var reportName = $("#txtReportName").val().trim();

        if (reportName != "") {
            $("#spnSave").addClass("d-none");
            $("#spnSpinner").removeClass("d-none");

            var userId = $("#lstUser").val();
            var clientId = $("#lstClient").val();
            var accountId = $("#lstClientAccount").val();
            var module = $("#lstModule").val();
            var requestType = $("#lstRequestType").val();
            var isBillable = $("#lstBillable").val();
            var recordDate = $("#lstRecordDate").val();

            var recordStartDate = $("#txtStartDate").val();
            var recordEndDate = $("#txtEndDate").val();
            var department = $("#lstDepartment").val();
            var location = $("#lstLocation").val();
            var employmentType = $("#lstEmploymentType").val();
            var reportType = $("#lstReportType").val();
            var filterType = $("#lstFilterType").val();

            if (filterType == "Public") {
                reportName += " ";
            }

            let objRpt = {
                UserId: userId.join(","),
                ClientId: clientId.join(","),
                AccountId: accountId.join(","),
                Module: module.join(","),
                RequestType: requestType,
                IsBillable: isBillable,
                RecordDate: recordDate,
                RecordStartDate: recordStartDate,
                RecordEndDate: recordEndDate,
                ReportName: reportName,
                Department: department.join(","),
                Location: location.join(","),
                EmploymentType: employmentType.join(","),
                ReportType: reportType,
                FilterType: filterType
            };

            $.ajax({
                type: "POST",
                url: BaseUrl + "SaveFilter",
                data: objRpt,
                success: function (data) {
                    $("#dvFailed").addClass("d-none");
                    if (data == "-1") {
                        if (filterType == "Public") {
                            $("#lstSavedReports").append($('<option>', {
                                value: reportName,
                                text: reportName
                            }));
                        } else {
                            $("#lstSavedReports option:first").after($('<option>', {
                                value: reportName,
                                text: reportName
                            }));
                        }
                    }
                    $("#lstSavedReports option:selected").removeAttr("selected");
                    $('#lstSavedReports option[value="' + reportName + '"]').attr('selected', true);
                    $("#lstSavedReports").removeClass("d-none");
                    setTimeout(function () {
                        $("#spnSpinner").addClass("d-none");
                        $("#spnSave").removeClass("d-none");
                    }, 1500);
                },
                error: function (xhr, status, errorThrown) {
                    $("#dvFailed").removeClass("d-none");
                    $("#dvFailed").html("There are some errors. " + errorThrown);
                    $("#spnSpinner").addClass("d-none");
                    $("#spnSave").removeClass("d-none");
                }
            });
            $("#errReportName").addClass("d-none");
        } else {
            $("#errReportName").removeClass("d-none");
        }
    });

    $("#btnDeleteReport").on("click", function (e) {
        e.preventDefault();
        var reportName = $("#lstSavedReports").val();

        if (reportName != "") {
            if (confirm('Are you sure you want to delete this filter?')) {
                $.ajax({
                    type: "POST",
                    url: BaseUrl + "DeleteFilterByReportName",
                    data: 'reportName=' + encodeURIComponent(reportName),
                    success: function (data) {
                        $("#dvFailed").addClass("d-none");
                        $("#lstSavedReports option:selected").remove();
                        $("#lstSavedReports option:first").prop("selected", true);
                        $("#lstSavedReports").trigger("change");
                    },
                    error: function (xhr, status, errorThrown) {
                        $("#dvFailed").removeClass("d-none");
                        $("#dvFailed").html("There are some errors. " + errorThrown);
                    }
                });
            }
        }
    });

    $("#lstSavedReports").on("change", function (e) {
        var reportName = $("#lstSavedReports").val();
        if (reportName.slice(-1) == " ") {
            $("#lstFilterType").val("Public");
        } else {
            $("#lstFilterType").val("Private");
        }
        $("#txtReportName").val(reportName);
        $("#dvReport").addClass("d-none");
        $("#errReportName").addClass("d-none");

        if (reportName == "") {
            $("#lstReportType").val("DetailedM").trigger("change");
            $("#lstUser").val("");
            $("#lstUser").multiselect("refresh");
            $("#lstClient").val("");
            $("#lstClient").multiselect("refresh");
            $("#lstClientAccount").val("");
            $("#lstClientAccount").multiselect("refresh");
            $("#lstModule").val("").trigger("change");
            $("#lstModule").multiselect("refresh");
            $("#lstRequestType").val("");
            $("#lstBillable").val("");
            $("#lstDepartment").val("");
            $("#lstDepartment").multiselect("refresh");
            $("#lstLocation").val("");
            $("#lstLocation").multiselect("refresh");
            $("#lstEmploymentType").val("");
            $("#lstRecordDate").val("Today").trigger("change");

            $("#spnPlaceholder").removeClass("d-none");
            $("#spnDelete").addClass("d-none");
        } else {
            $("#spnPlaceholder").addClass("d-none");
            $("#spnDelete").removeClass("d-none");
            $.ajax({
                type: "POST",
                url: BaseUrl + "GetFiltersByReportName",
                data: 'reportName=' + encodeURIComponent(reportName),
                success: function (data) {
                    $("#dvFailed").addClass("d-none");
                    var res = JSON.parse(data);

                    if (res.ReportType == "Summary")
                        $("#lstReportType").val("Summary").trigger("change");
                    else if (res.ReportType == "HoursByDay")
                        $("#lstReportType").val("HoursByDay").trigger("change");
                    else if (res.ReportType == "HoursByPerson")
                        $("#lstReportType").val("HoursByPerson").trigger("change");
                    else if (res.ReportType == "Compliance")
                        $("#lstReportType").val("Compliance").trigger("change");
                    else if (res.ReportType == "UtilizationByPerson")
                        $("#lstReportType").val("UtilizationByPerson").trigger("change");
                    else if (res.ReportType == "UtilizationByDepartment")
                        $("#lstReportType").val("UtilizationByDepartment").trigger("change");
                    else if (res.ReportType == "Inventory")
                        $("#lstReportType").val("Inventory").trigger("change");
                    else
                        $("#lstReportType").val("DetailedM").trigger("change");

                    $("#lstRecordDate").val(res.RecordDate).trigger("change");

                    if (res.RecordDate == "Custom Date") {
                        var dtStart = new Date(res.RecordStartDate);
                        var dtEnd = new Date(res.RecordEndDate);
                        var strStart = [pad(dtStart.getMonth() + 1), pad(dtStart.getDate()), dtStart.getFullYear()].join('/');
                        var strEnd = [pad(dtEnd.getMonth() + 1), pad(dtEnd.getDate()), dtEnd.getFullYear()].join('/');
                        $("#txtStartDate").val(strStart);
                        $("#txtStartDate").prop('disabled', false);
                        $("#txtEndDate").val(strEnd);
                    }
                    if (res.RecordDate == "Since Inception") {
                        var dtStart = new Date("08/01/2025");
                        var dtEnd = new Date();
                        var strStart = [pad(dtStart.getMonth() + 1), pad(dtStart.getDate()), dtStart.getFullYear()].join('/');
                        var strEnd = [pad(dtEnd.getMonth() + 1), pad(dtEnd.getDate()), dtEnd.getFullYear()].join('/');
                        $("#txtStartDate").val(strStart);
                        $("#txtStartDate").prop('disabled', true);
                        $("#txtEndDate").val(strEnd);
                    }


                    if (res.UserId) {
                        $("#lstUser").val(res.UserId.split(','));
                    } else {
                        $("#lstUser").val("");
                    }
                    $("#lstUser").multiselect("refresh");

                    if (res.ClientId) {
                        $("#lstClient").val(res.ClientId.split(','));
                    } else {
                        $("#lstClient").val("");
                    }
                    $("#lstClient").multiselect("refresh");

                    if (res.AccountId) {
                        $("#lstClientAccount").val(res.AccountId.split(','));
                    } else {
                        $("#lstClientAccount").val("");
                    }
                    $("#lstClientAccount").multiselect("refresh");

                    if (res.Module) {
                        $("#lstModule").val(res.Module.split(',')).trigger("change");
                    } else {
                        $("#lstModule").val("").trigger("change");
                    }
                    $("#lstModule").multiselect("refresh");

                    if (res.RequestType == "Service Request")
                        $("#lstRequestType").val("Service Request");
                    else if (res.RequestType == "Incident")
                        $("#lstRequestType").val("Incident");
                    else
                        $("#lstRequestType").val("");

                    if (res.IsBillable == "true")
                        $("#lstBillable").val("true");
                    else if (res.IsBillable == "false")
                        $("#lstBillable").val("false");
                    else
                        $("#lstBillable").val("");

                    if (res.Department) {
                        $("#lstDepartment").val(res.Department.split(','));
                    } else {
                        $("#lstDepartment").val("");
                    }
                    $("#lstDepartment").multiselect("refresh");

                    if (res.Location) {
                        $("#lstLocation").val(res.Location.split(','));
                    } else {
                        $("#lstLocation").val("");
                    }
                    $("#lstLocation").multiselect("refresh");

                    if (res.EmploymentType) {
                        $("#lstEmploymentType").val(res.EmploymentType.split(','));
                    } else {
                        $("#lstEmploymentType").val("");
                    }
                    $("#lstEmploymentType").multiselect("refresh");
                },
                error: function (xhr, status, errorThrown) {
                    $("#dvFailed").removeClass("d-none");
                    $("#dvFailed").html("There are some errors. " + errorThrown);
                }
            });
        }
    });

    $("#btnGenerateBudgetReport").on("click", function (e) {
        e.preventDefault();         
        var accountId = $("#lstClientAccount1").val();
        var statusId = $("#lstAccountStatus").val();        

        let objBudgetRpt = {             
            AccountId: accountId.join(","),
            StatusId: statusId.join(",")
        };

        GenerateBudgetReport(objBudgetRpt);
        $("#dvBudgetReport").removeClass("d-none");
        $("#dvBudgetFailed").addClass("d-none");
    });

    //Timesheet Approval Report
    $("#btnGenerateTimesheetReport").on("click", function (e) {
        e.preventDefault();
        var department = $("#lstTimeDepartment").val();
        var employmentType = $("#lstTimeEmployeeType").val();
        var location = $("#lstTimeLocation").val();
        var approver = $("#lstTimeApprover").val();
        var recordStartDate = $("#txtTimesheetStartDate").val();
        var recordEndDate = $("#txtTimesheetEndDate").val();
        var status = $("#lstTimeStatus").val();
        var rptType = $("#lstTimesheetType").val();

        if (new Date(recordStartDate) > new Date(recordEndDate)) {
            alert("Start date cannot be greater than end date.");
            return;
        }
        
        let objTimesheetRpt = {
            Department: "'" + department.join("','") + "'",
            EmployeeType: "'" + employmentType.join("','") + "'",
            Location: "'" + location.join("','") + "'",
            TimesheetStartDate: recordStartDate,
            TimesheetEndDate: recordEndDate,
            Approver: approver.join(","),
            Status: status.join(",")
        };

        if (rptType == "TAR") {
            GenerateTimesheetReport(objTimesheetRpt);
            $("#dvTimesheetReport").removeClass("d-none");
            $("#dvTimesheetFailed").addClass("d-none");
            $("#dvTimesheetCountReport").addClass("d-none");
        } else {
            GenerateTimesheetCountReport(objTimesheetRpt);
            $("#dvTimesheetCountReport").removeClass("d-none");
            $("#dvTimesheetFailed").addClass("d-none");
            $("#dvTimesheetReport").addClass("d-none");
        }
    });
    
    $("#btnShowEmployeeList").on("click", function (e) {
        e.preventDefault();
        var department = $("#lstEmpDepartment").val();

        let objEmpDepartment = {
            Department: "'" + department.join("','") + "'"
        };
        GetEmployeeList(objEmpDepartment);
        $("#empNameDepartment").removeClass("d-none");
    });

    SetDateRange("Today");
    
   
    $('#lstRequestType').prop('disabled', true);
    $('#lstRequestType option[value=""]').text("");
    $('#divDateStart').hide();
    $('#divDateEnd').hide();

    $("#btnDownload").on("click", function (e) {
        e.preventDefault();
        var userId = $("#lstUser").val();
        var clientId = $("#lstClient").val();
        var accountId = $("#lstClientAccount").val();
        var module = $("#lstModule").val();
        var requestType = $("#lstRequestType").val();
        var isBillable = $("#lstBillable").val();
        var recordStartDate = $("#txtStartDate").val();
        var recordEndDate = $("#txtEndDate").val();
        var department = $("#lstDepartment").val();
        var location = $("#lstLocation").val();
        var employmentType = $("#lstEmploymentType").val();

        let objRpt = {
            UserId: userId.join(","),
            ClientId: clientId.join(","),
            AccountId: accountId.join(","),
            Module: module.join(","),
            RequestType: requestType,
            IsBillable: isBillable,
            RecordStartDate: recordStartDate,
            RecordEndDate: recordEndDate,
            Department: department.join(","),
            Location: location.join(","),
            EmploymentType: employmentType.join(",")
        };

        Download(objRpt);
    })

    var isAppManager = $("#hdnAppManager").val();
    if (isAppManager && isAppManager.toString().toLowerCase() === 'true') {
        $('#lstClient').prop('disabled', true);
        try {
            if (typeof $('#lstClient').multiselect === 'function') {
                $('#lstClient').multiselect('disable');
            }
        } catch (e) {
            // ignore if multiselect isn't initialized or fails
        }
        $('#lstClient option[value=""]').text("");
    }

});

function GetEmployeeList(objEmpDepartment) {
    debugger;
    var fileName = formatDateTimeYYYYMMDDHHMMSS();
    var tableName = "#tblEmpDepartment";
    new DataTable(tableName, {
        destroy: true,
        paging: false,
        processing: true,
        serverSide: true,
        filter: false,
        info: false,
        ordering: false,
        scrollX: true,
        scrollCollapse: true,
        scrollY: '50vh',
        layout: {
            topStart: {
                buttons: [
                    {
                        text: 'Export to Excel',
                        className: 'dt_btn',
                        extend: 'excel',
                        filename: 'BudgetReport_' + fileName,
                        title: ''
                    },
                    {
                        text: 'Export to CSV',
                        className: 'dt_btn',
                        extend: 'csv',
                        filename: 'BudgetReport_' + fileName,
                        title: ''
                    }
                ]
            }
        },        
        ajax: {
            type: "POST",
            url: BaseUrl + "GetEmployeeList",
            data: function (d) {
                console.log(d);
                return Object.assign(d, objEmpDepartment);
            }
        },
        columns: [
            { data: "employeeName" },
            { data: "department" }            
        ]
    });

}

function GenerateBudgetReport(objBudgetRpt) {
    var fileName = formatDateTimeYYYYMMDDHHMMSS();
    var tableName = "#tblBudgetReport";

    new DataTable(tableName, {
        destroy: true,
        paging: false,
        processing: true,
        serverSide: true,
        filter: false,
        info: false,
        ordering: false,
        scrollX: true,
        scrollCollapse: true,
        scrollY: '50vh',
        layout: {
            topStart: {
                buttons: [
                    {
                        text: 'Export to Excel',
                        className: 'dt_btn',
                        extend: 'excel',
                        filename: 'BudgetReport_' + fileName,
                        title: ''
                    },
                    {
                        text: 'Export to CSV',
                        className: 'dt_btn',
                        extend: 'csv',
                        filename: 'BudgetReport_' + fileName,
                        title: ''
                    }
                ]
            }
        },
        columnDefs:
            [
                {
                    targets: 7,
                    render: $.fn.dataTable.render.number(',', '.', 2, '')
                },
                {
                    targets: 8,
                    render: $.fn.dataTable.render.number(',', '.', 2, '')
                },
                {
                    targets: 9,
                    render: $.fn.dataTable.render.number(',', '.', 2, '')
                },
                {
                    targets: 10,
                    render: $.fn.dataTable.render.number(',', '.', 2, '')
                },
                {
                    targets: 11,
                    render: $.fn.dataTable.render.number(',', '.', 2, '')
                }
            ],
        ajax: {
            type: "POST",
            url: BaseUrl + "GetBudgetReportData",
            data: function (d) { 
                console.log(d);
                return Object.assign(d, objBudgetRpt);
            }
        },
        columns: [
            { data: "accountId" },     
            { data: "accountName" },
            { data: "openDate" },
            //{ data: "closeDate" },
            { data: "status" },
            { data: "ticketNumber" },
            {data: "ticketSubject" }, 
            { data: "taskNumber" },
            { data: "taskName" },
            { data: "estimatedHours" },
            { data: "billableHours" },
            { data: "nonBillableHours" },
            { data: "actualHours" },
            { data: "hourDifference" }          
        ]
    });
}

function GenerateTimesheetReport(objTimesheetRpt) {
   // debugger;
    var fileName = formatDateTimeYYYYMMDDHHMMSS();
    var tableName = "#tblTimesheetReport";
    new DataTable(tableName, {
        destroy: true,
        paging: false,
        processing: true,
        serverSide: true,
        filter: false,
        info: false,
        ordering: false,
        scrollX: true,
        scrollCollapse: true,
        scrollY: '50vh',
        layout: {
            topStart: {
                buttons: [
                    {
                        text: 'Export to Excel',
                        className: 'dt_btn',
                        extend: 'excel',
                        filename: 'TimesheetReport_' + fileName,
                        title: ''
                    },
                    {
                        text: 'Export to CSV',
                        className: 'dt_btn',
                        extend: 'csv',
                        filename: 'TimesheetReport_' + fileName,
                        title: ''
                    }
                ]
            }
        },
        ajax: {
            type: "POST",
            url: BaseUrl + "GetTimesheetData",
            data: function (d) {
                return Object.assign(d, objTimesheetRpt);
            }
        },
        columns: [
            { data: "timeSheetId" },
            {data: "technicianId" },
            { data: "technicianName"},
            { data: "department"},           
            { data: "timesheetStartDate"},
            { data: "timesheetEndDate"},
            { data: "totalTimeSpent"},
            { data: "timeSheetStatus"},
            { data: "approvedBy"},
            { data: "approvedDate"},
            { data: "empLocation"},
            { data: "employmentType"}
        ]
    });
}

function GenerateTimesheetCountReport(objTimesheetRpt) {
    var fileName = formatDateTimeYYYYMMDDHHMMSS();
    var tableName = "#tblTimesheetCountReport";
    new DataTable(tableName, {
        destroy: true,
        paging: false,
        processing: true,
        serverSide: true,
        filter: false,
        info: false,
        ordering: false,
        scrollX: true,
        scrollCollapse: true,
        scrollY: '50vh',
        layout: {
            topStart: {
                buttons: [
                    {
                        text: 'Export to Excel',
                        className: 'dt_btn',
                        extend: 'excel',
                        filename: 'TimesheetCountReport_' + fileName,
                        title: ''
                    },
                    {
                        text: 'Export to CSV',
                        className: 'dt_btn',
                        extend: 'csv',
                        filename: 'TimesheetCountReport_' + fileName,
                        title: ''
                    }
                ]
            }
        },
        ajax: {
            type: "POST",
            url: BaseUrl + "GetTimesheetCountData",
            data: function (d) {
                return Object.assign(d, objTimesheetRpt);
            }              
        },
        columns: [             
            { data: "employeeName" },
            { data: "createDate" },
            { data: "department" },
            { data: "employeeType" },
            { data: "empLocation"},
            { data: "approver" },
            { data: "timesheetStartDate" },
            { data: "timesheetEndDate"},
            { data: "lastTimesheetDate" },
            { data: "entryCount" }           
        ]
    });
}


function GenerateReportTable(objRpt) {
    var fileName = formatDateTimeYYYYMMDDHHMMSS();
    var reportType = objRpt.ReportType;
    var tableName = "#tblReport";
    var columns = [];

    $("#tblReport_wrapper").removeClass("d-none");
    $("#tblReport_wrapper").addClass("d-none");
    $("#tblReportSummary_wrapper").removeClass("d-none");
    $("#tblReportSummary_wrapper").addClass("d-none");
    $("#tblReportHoursByDay_wrapper").removeClass("d-none");
    $("#tblReportHoursByDay_wrapper").addClass("d-none");
    $("#tblReportHoursByPerson_wrapper").removeClass("d-none");
    $("#tblReportHoursByPerson_wrapper").addClass("d-none");
    $("#tblHoursByAccount_wrapper").removeClass("d-none");
    $("#tblHoursByAccount_wrapper").addClass("d-none");
    $("#tblHoursByAccountDetails_wrapper").removeClass("d-none");
    $("#tblHoursByAccountDetails_wrapper").addClass("d-none");
    $("#tblReportCompliance_wrapper").removeClass("d-none");
    $("#tblReportCompliance_wrapper").addClass("d-none");
    $("#tblReportUtilizationByPerson_wrapper").removeClass("d-none");
    $("#tblReportUtilizationByPerson_wrapper").addClass("d-none");
    $("#tblReportUtilizationByDepartment_wrapper").removeClass("d-none");
    $("#tblReportUtilizationByDepartment_wrapper").addClass("d-none");
    $("#tblReportInventory_wrapper").removeClass("d-none");
    $("#tblReportInventory_wrapper").addClass("d-none");
    $("#tblReport").removeClass("d-none");
    $("#tblReport").addClass("d-none");
    $("#tblReportSummary").removeClass("d-none");
    $("#tblReportSummary").addClass("d-none");
    $("#tblReportHoursByDay").removeClass("d-none");
    $("#tblReportHoursByDay").addClass("d-none");
    $("#tblReportHoursByPerson").removeClass("d-none");
    $("#tblReportHoursByPerson").addClass("d-none");
    $("#tblHoursByAccount").removeClass("d-none");
    $("#tblHoursByAccount").addClass("d-none");
    $("#tblHoursByAccountDetails").removeClass("d-none");
    $("#tblHoursByAccountDetails").addClass("d-none");
    $("#tblReportCompliance").removeClass("d-none");
    $("#tblReportCompliance").addClass("d-none");
    $("#tblReportUtilizationByPerson").removeClass("d-none");
    $("#tblReportUtilizationByPerson").addClass("d-none");
    $("#tblReportUtilizationByDepartment").removeClass("d-none");
    $("#tblReportUtilizationByDepartment").addClass("d-none");
    $("#tblReportInventory").removeClass("d-none");
    $("#tblReportInventory").addClass("d-none");

    if (reportType == "HoursByDay") {
        columns.push({ data: "recordDate", width: 107 });
        columns.push({ data: "totalHours", width: 105 });

        tableName = "#tblReportHoursByDay";
        $("#tblReportHoursByDay").removeClass("d-none");
        $("#tblReportHoursByDay_wrapper").removeClass("d-none");

        new DataTable(tableName, {
            destroy: true,
            paging: false,
            processing: true,
            serverSide: true,
            filter: false,
            info: false,
            ordering: false,
            scrollX: true,
            scrollCollapse: true,
            scrollY: '50vh',
            layout: {
                topStart: {
                    buttons: [
                        {
                            text: 'Export to Excel',
                            className: 'dt_btn',
                            extend: 'excel',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        },
                        {
                            text: 'Export to CSV',
                            className: 'dt_btn',
                            extend: 'csv',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        }
                    ],
                }
            },
            ajax: {
                type: "POST",
                url: BaseUrl + "GetReportDataHoursByDay",
                data: objRpt
            },
            columns: columns,
            drawCallback: function (settings) {
                var api = this.api();
                var totalHrs = 0;
                api.rows().every(function (rowIdx, tableLoop, rowLoop) {
                    var data = this.data();
                    totalHrs += data.totalHours;

                });
                $("#totalHrs").html(" - Total Hours: " + Math.round(totalHrs * 100) / 100);
            }
        });
    } else if (reportType == "HoursByPerson") {
        columns.push({ data: "personName", width: 207 });
        columns.push({ data: "totalHours", width: 105 });

        tableName = "#tblReportHoursByPerson";
        $("#tblReportHoursByPerson").removeClass("d-none");
        $("#tblReportHoursByPerson_wrapper").removeClass("d-none");

        new DataTable(tableName, {
            destroy: true,
            paging: false,
            processing: true,
            serverSide: true,
            filter: false,
            info: false,
            ordering: false,
            scrollX: true,
            scrollCollapse: true,
            scrollY: '50vh',
            layout: {
                topStart: {
                    buttons: [
                        {
                            text: 'Export to Excel',
                            className: 'dt_btn',
                            extend: 'excel',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        },
                        {
                            text: 'Export to CSV',
                            className: 'dt_btn',
                            extend: 'csv',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        }
                    ],
                }
            },
            ajax: {
                type: "POST",
                url: BaseUrl + "GetReportDataHoursByPerson",
                data: objRpt
            },
            columns: columns,
            drawCallback: function (settings) {
                var api = this.api();
                var totalHrs = 0;
                api.rows().every(function (rowIdx, tableLoop, rowLoop) {
                    var data = this.data();
                    totalHrs += data.totalHours;

                });
                $("#totalHrs").html(" - Total Hours: " + Math.round(totalHrs * 100) / 100);
            }
        });
    } else if (reportType == "HoursByAccount") {        

        $.ajax({
            url: BaseUrl + "GetReportDataHourByAccount",
            type: 'POST',
            data: objRpt,
            success: function (response) {
                loadDynamicTable(response);
            },
            error: function (xhr, status, error) {
                alert('Error loading report: ' + error);
            }
        });
        function loadDynamicTable(response) {
            var tableName = "#tblHoursByAccount";

            $("#tblHoursByAccount").removeClass("d-none");
            $("#tblHoursByAccount_wrapper").removeClass("d-none");
            $("#totalHrs").html("");

            if ($.fn.DataTable.isDataTable(tableName)) {
                $(tableName).DataTable().destroy();
                $(tableName).empty();
            }

            var columns = response.headers.map(function (header, index) {
                var columnDef = {
                    title: header,
                    data: header
                };
                if (index <= 2) {
                    columnDef.defaultContent = "";  // Empty string for text columns
                } else {
                    columnDef.defaultContent = "0.00";  // Zero for numeric columns
                }

                if (index > 2) {
                    columnDef.render = function (data, type, row) {
                        if (type === 'display' || type === 'filter') {
                            return parseFloat(data || 0).toFixed(2);
                        }
                        return data;
                    };

                    columnDef.className = 'dt-right';
                }

                if (header === 'Grand Total') {
                    columnDef.className = 'dt-right fw-bold';
                }

                return columnDef;
            });

            new DataTable(tableName, {
                destroy: true,
                data: response.data,
                columns: columns,
                paging: false,
                processing: true,
                serverSide: false,
                filter: false,
                info: false,
                ordering: false,
                scrollX: true,
                scrollCollapse: true,
                scrollY: '50vh',
                rowCallback: function (row, data, index) {
                    if (index === response.data.length - 1) {
                        $(row).css('font-weight', 'bold');
                    }
                },
                layout: {
                    topStart: {
                        buttons: [
                            {
                                text: 'Export to Excel',
                                className: 'dt_btn',
                                extend: 'excel',
                                filename: 'HourByAccountReport_' + fileName,
                                title: ''
                            },
                            {
                                text: 'Export to CSV',
                                className: 'dt_btn',
                                extend: 'csv',
                                filename: 'HourByAccountReport_' + fileName,
                                title: ''
                            }
                        ]
                    }
                },
                drawCallback: function (settings) {
                    $(this.api().table().header()).find('th').css('font-size', '11px');
                }
            });
        }
    } else if (reportType == "HoursByAccountDetails") {

        $.ajax({
            url: BaseUrl + "GetReportDataHourByAccountDetails",
            type: 'POST',
            data: objRpt,
            success: function (response) {
                console.log(response);
                loadDynamicTable1(response);
            },
            error: function (xhr, status, error) {
                alert('Error loading report: ' + error);
            }
        });
        function loadDynamicTable1(response) {
            var tableName = "#tblHoursByAccountDetails";
            $("#tblHoursByAccountDetails").removeClass("d-none");
            $("#tblHoursByAccountDetails_wrapper").removeClass("d-none");
            $("#totalHrs").html("");

            if ($.fn.DataTable.isDataTable(tableName)) {
                $(tableName).DataTable().destroy();
                $(tableName).empty();
            }

            var columns = response.headers.map(function (header, index) {
                var columnDef = {
                    title: header,
                    data: header
                };

                if (index <= 3) {
                    columnDef.defaultContent = "";  // Empty string for text columns
                } else {
                    columnDef.defaultContent = "0.00";  // Zero for numeric columns
                }

                if (index > 3) {
                    columnDef.render = function (data, type, row) {
                        if (type === 'display' || type === 'filter') {
                            return parseFloat(data || 0).toFixed(2);
                        }
                        return data;
                    };

                    columnDef.className = 'dt-right';
                }

                if (header === 'Grand Total') {
                    columnDef.className = 'dt-right fw-bold';
                }
                return columnDef;
            });

            new DataTable(tableName, {
                destroy: true,
                data: response.data,
                columns: columns,
                paging: false,
                processing: true,
                serverSide: false,
                filter: false,
                info: false,
                ordering: false,
                scrollX: true,
                scrollCollapse: true,
                scrollY: '50vh',
                rowCallback: function (row, data, index) {
                    if (index === response.data.length - 1) {
                        $(row).css('font-weight', 'bold');
                    }
                },
                layout: {
                    topStart: {
                        buttons: [
                            {
                                text: 'Export to Excel',
                                className: 'dt_btn',
                                extend: 'excel',
                                filename: 'HourByAccountDetailsReport_' + fileName,
                                title: ''
                            },
                            {
                                text: 'Export to CSV',
                                className: 'dt_btn',
                                extend: 'csv',
                                filename: 'HourByAccountDetailsReport_' + fileName,
                                title: ''
                            }
                        ]
                    }
                },
                drawCallback: function (settings) {
                    $(this.api().table().header()).find('th').css('font-size', '11px');
                }
            });
        }


    } else if (reportType == "Compliance") {
        columns.push({ data: "userId" });
        columns.push({ data: "userName" });
        columns.push({ data: "employmentType" });
        columns.push({ data: "departmentName" });
        columns.push({ data: "location" });
        columns.push({ data: "totalHours", width: 100 });

        tableName = "#tblReportCompliance";
        $("#tblReportCompliance").removeClass("d-none");
        $("#tblReportCompliance_wrapper").removeClass("d-none");

        new DataTable(tableName, {
            destroy: true,
            paging: false,
            processing: true,
            serverSide: true,
            filter: false,
            info: false,
            ordering: false,
            scrollX: true,
            scrollCollapse: true,
            scrollY: '50vh',
            layout: {
                topStart: {
                    buttons: [
                        {
                            text: 'Export to Excel',
                            className: 'dt_btn',
                            extend: 'excel',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        },
                        {
                            text: 'Export to CSV',
                            className: 'dt_btn',
                            extend: 'csv',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        }
                    ],
                }
            },
            ajax: {
                type: "POST",
                url: BaseUrl + "GetReportDataCompliance",
                data: objRpt
            },
            columns: columns,
            drawCallback: function (settings) {
                var api = this.api();
                var totalHrs = 0;
                api.rows().every(function (rowIdx, tableLoop, rowLoop) {
                    var data = this.data();
                    totalHrs += data.totalHours;

                });
                $("#totalHrs").html(" - Total Hours: " + Math.round(totalHrs * 100) / 100);
            }
        });
    } else if (reportType == "UtilizationByPerson") {
        columns.push({ data: "userId" });
        columns.push({ data: "userName" });
        columns.push({ data: "userEmail" });
        columns.push({ data: "departmentName" });
        columns.push({ data: "location" });
        columns.push({ data: "startDate", width: 70 });
        columns.push({ data: "endDate", width: 70 });
        columns.push({ data: "businessDays", width: 80 });
        columns.push({ data: "billableHours", width: 70 });
        columns.push({ data: "nonBillableHours", width: 80 });
        columns.push({ data: "utilizationPercentage", width: 70 });

        tableName = "#tblReportUtilizationByPerson";
        $("#tblReportUtilizationByPerson").removeClass("d-none");
        $("#tblReportUtilizationByPerson_wrapper").removeClass("d-none");

        new DataTable(tableName, {
            destroy: true,
            paging: false,
            processing: true,
            serverSide: true,
            filter: false,
            info: false,
            ordering: false,
            scrollX: true,
            scrollCollapse: true,
            scrollY: '50vh',
            layout: {
                topStart: {
                    buttons: [
                        {
                            text: 'Export to Excel',
                            className: 'dt_btn',
                            extend: 'excel',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        },
                        {
                            text: 'Export to CSV',
                            className: 'dt_btn',
                            extend: 'csv',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        }
                    ],
                }
            },
            ajax: {
                type: "POST",
                url: BaseUrl + "GetReportDataUtilizationByPerson",
                data: objRpt
            },
            columns: columns,
            drawCallback: function (settings) {
                var api = this.api();
                var totalHrs = 0;
                var totalDays = 0;
                api.rows().every(function (rowIdx, tableLoop, rowLoop) {
                    var data = this.data();
                    totalHrs += data.billableHours;
                    if (data.userId == 2057) {
                        totalDays += data.businessDays / 2;
                    } else {
                        totalDays += data.businessDays;
                    }
                });
                if (totalDays == 0) {
                    $("#totalHrs").html("");
                } else {
                    $("#totalHrs").html(" - Total Utilization: " + Math.round((totalHrs / (totalDays * 8)) * 100 * 100) / 100 + "%");
                }
            }
        });
    } else if (reportType == "UtilizationByDepartment") {
        columns.push({ data: "departmentName" });
        columns.push({ data: "startDate" });
        columns.push({ data: "endDate" });
        columns.push({ data: "businessDays", width: 90 });
        columns.push({ data: "billableHours", width: 90 });
        columns.push({ data: "nonBillableHours", width: 90 });
        columns.push({ data: "utilizationPercentage", width: 90 });

        tableName = "#tblReportUtilizationByDepartment";
        $("#tblReportUtilizationByDepartment").removeClass("d-none");
        $("#tblReportUtilizationByDepartment_wrapper").removeClass("d-none");

        new DataTable(tableName, {
            destroy: true,
            paging: false,
            processing: true,
            serverSide: true,
            filter: false,
            info: false,
            ordering: false,
            scrollX: true,
            scrollCollapse: true,
            scrollY: '50vh',
            layout: {
                topStart: {
                    buttons: [
                        {
                            text: 'Export to Excel',
                            className: 'dt_btn',
                            extend: 'excel',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        },
                        {
                            text: 'Export to CSV',
                            className: 'dt_btn',
                            extend: 'csv',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        }
                    ],
                }
            },
            ajax: {
                type: "POST",
                url: BaseUrl + "GetReportDataUtilizationByDepartment",
                data: objRpt
            },
            columns: columns,
            drawCallback: function (settings) {
                var api = this.api();
                var totalHrs = 0;
                var totalDays = 0;
                api.rows().every(function (rowIdx, tableLoop, rowLoop) {
                    var data = this.data();
                    totalHrs += data.billableHours;
                    totalDays += data.businessDays;
                });
                //$("#totalHrs").html(" - Total Utilization: " + Math.round((totalHrs / (totalDays * 8)) * 100 * 100) / 100 + "%");
                $("#totalHrs").html("");
            }
        });
    } else if (reportType == "Inventory") {
        columns.push({ data: "clientId", width: 70 });
        columns.push({ data: "clientName" });
        columns.push({ data: "vCollabId", width: 80 });
        columns.push({ data: "accountId", width: 70 });
        columns.push({ data: "accountName" });
        columns.push({ data: "createdDate" });
        columns.push({ data: "accountActivity" });
        columns.push({ data: "lastWorkLog" });
        columns.push({ data: "projectId", width: 70 });
        columns.push({ data: "projectName" });        

        tableName = "#tblReportInventory";
        $("#tblReportInventory").removeClass("d-none");
        $("#tblReportInventory_wrapper").removeClass("d-none");

        new DataTable(tableName, {
            destroy: true,
            paging: false,
            processing: true,
            serverSide: true,
            filter: false,
            info: false,
            ordering: false,
            scrollX: true,
            scrollCollapse: true,
            scrollY: '50vh',
            layout: {
                topStart: {
                    buttons: [
                        {
                            text: 'Export to Excel',
                            className: 'dt_btn',
                            extend: 'excel',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        },
                        {
                            text: 'Export to CSV',
                            className: 'dt_btn',
                            extend: 'csv',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        }
                    ],
                }
            },
            ajax: {
                type: "POST",
                url: BaseUrl + "GetReportDataInventory"
            },
            columns: columns,
            drawCallback: function (settings) {
                $("#totalHrs").html("");
            }
        });
    } else {
        if (reportType == "Detailed" || reportType == "DetailedM") {
            columns.push({ data: "clientId" });
            columns.push({ data: "clientName", width: 150 });
            columns.push({ data: "accountId" });
            columns.push({ data: "accountName", width: 150 });
            columns.push({ data: "userId" });
            columns.push({ data: "userName", width: 120 });
            columns.push({ data: "userEmail", width: 180 });
            columns.push({ data: "employmentType" });
            columns.push({ data: "departmentName" });
            columns.push({ data: "location" });
            columns.push({ data: "workLogId" });
            columns.push({ data: "module" });
            columns.push({ data: "moduleId" });
            columns.push({ data: "moduleTitle", width: 150 });
            columns.push({ data: "requestType" });
            columns.push({ data: "taskId" });
            columns.push({ data: "taskTitle", width: 250 });
            columns.push({ data: "workLogDescription", width: 350 });
            columns.push({ data: "isBillable" });
            columns.push({ data: "recordStartDate", width: 70 });
            columns.push({ data: "recordEndDate", width: 70 });
            columns.push({ data: "workLogCreatedDate", width: 70 });
            columns.push({ data: "workLogCreatedTime", width: 70 });
            columns.push({ data: "timeSpentHrs" });
            columns.push({ data: "billingStatus" });
            columns.push({ data: "createdBy" });
            columns.push({ data: "vcOldProjectId", width: 50 });

            $("#tblReport").removeClass("d-none");
            $("#tblReport_wrapper").removeClass("d-none");
        } else {

            columns.push({ data: "clientName", width: 150 });
            columns.push({ data: "accountName", width: 150 });
            columns.push({ data: "userName", width: 120 });
            columns.push({ data: "departmentName" });
            columns.push({ data: "location" });
            columns.push({ data: "workLogId" });
            columns.push({ data: "taskId" });
            columns.push({ data: "taskTitle", width: 250 });
            columns.push({ data: "workLogDescription", width: 350 });
            columns.push({ data: "isBillable" });
            columns.push({ data: "recordEndDate", width: 70 });
            columns.push({ data: "timeSpentHrs" });

            tableName = "#tblReportSummary";
            $("#tblReportSummary").removeClass("d-none");
            $("#tblReportSummary_wrapper").removeClass("d-none");
        }

        new DataTable(tableName, {
            destroy: true,
            paging: false,
            processing: true,
            serverSide: true,
            filter: false,
            info: false,
            ordering: false,
            scrollX: true,
            scrollCollapse: true,
            scrollY: '50vh',
            layout: {
                topStart: {
                    buttons: [
                        {
                            text: 'Export to Excel',
                            className: 'dt_btn',
                            extend: 'excel',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        },
                        {
                            text: 'Export to CSV',
                            className: 'dt_btn',
                            extend: 'csv',
                            filename: 'SDPlusReport_' + fileName,
                            title: ''
                        }
                    ],
                }
            },
            ajax: {
                type: "POST",
                url: BaseUrl + "GetReportData",
                data: objRpt
            },
            columns: columns,
            drawCallback: function (settings) {
                var api = this.api();
                var totalHrs = 0;
                api.rows().every(function (rowIdx, tableLoop, rowLoop) {
                    var data = this.data();
                    totalHrs += data.timeSpentHrs;

                });
                $("#totalHrs").html(" - Total Hours: " + Math.round(totalHrs * 100) / 100);
            }
        });
    }
}

function formatDateTimeYYYYMMDDHHMMSS() {
    const now = new Date();

    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0'); // Month is 0-indexed
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');

    return `${year}${month}${day}${hours}${minutes}${seconds}`;
}

function Download(objRpt) {
    $.ajax({
        type: "POST",
        url: BaseUrl + "download",
        data: objRpt,
        success: function (data) {
            $("#dvFailed").addClass("d-none");
            window.location = BaseUrl + "download?id=" + data;
        },
        error: function (xhr, status, errorThrown) {
            $("#dvFailed").removeClass("d-none");
            $("#dvFailed").html("There are some error. " + errorThrown);
        }
    });
}

function pad(s) { return (s < 10) ? '0' + s : s; }

function SetDateRange(dtValue) {
    var today = new Date();
    var dtToday = [pad(today.getMonth() + 1), pad(today.getDate()), today.getFullYear()].join('/');
    var dtYesterday = [pad(today.getMonth() + 1), pad(today.getDate() - 1), today.getFullYear()].join('/');
    var firstday = new Date(today.setDate(today.getDate() - today.getDay()));
    var lastday = new Date(today.setDate(today.getDate() - today.getDay() + 6));
    var dtCurrentWeekStart = [pad(firstday.getMonth() + 1), pad(firstday.getDate()), firstday.getFullYear()].join('/');
    var dtCurrentWeekEnd = [pad(lastday.getMonth() + 1), pad(lastday.getDate()), lastday.getFullYear()].join('/');
    var lastWeekStartDate = new Date(today.setDate(today.getDate() - today.getDay() - 7));
    var lastWeekEndDate = new Date(today.setDate(today.getDate() + 6));
    var dtLastWeekStart = [pad(lastWeekStartDate.getMonth() + 1), pad(lastWeekStartDate.getDate()), lastWeekStartDate.getFullYear()].join('/');
    var dtLastWeekEnd = [pad(lastWeekEndDate.getMonth() + 1), pad(lastWeekEndDate.getDate()), lastWeekEndDate.getFullYear()].join('/');

    today = new Date();
    var firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
    var lastDayOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    var dtFirstDayOfMonth = [pad(firstDayOfMonth.getMonth() + 1), pad(firstDayOfMonth.getDate()), firstDayOfMonth.getFullYear()].join('/');
    var dtLastDayOfMonth = [pad(lastDayOfMonth.getMonth() + 1), pad(lastDayOfMonth.getDate()), lastDayOfMonth.getFullYear()].join('/');

    var lastDayOfLastMonth = new Date(today.getFullYear(), today.getMonth(), 0);
    var firstDayofLastMonth = new Date(lastDayOfLastMonth.getFullYear(), lastDayOfLastMonth.getMonth(), 1);
    var dtFirstDayLastMonth = [pad(firstDayofLastMonth.getMonth() + 1), pad(firstDayofLastMonth.getDate()), firstDayofLastMonth.getFullYear()].join('/');
    var dtLastDayLastMonth = [pad(lastDayOfLastMonth.getMonth() + 1), pad(lastDayOfLastMonth.getDate()), lastDayOfLastMonth.getFullYear()].join('/');

    if (dtValue == "Today") {
        $("#txtStartDate").val(dtToday);
        $("#txtEndDate").val(dtToday);
    } else if (dtValue == "Yesterday") {
        $("#txtStartDate").val(dtYesterday);
        $("#txtEndDate").val(dtYesterday);
    } else if (dtValue == "Current Week") {
        $("#txtStartDate").val(dtCurrentWeekStart);
        $("#txtEndDate").val(dtCurrentWeekEnd);
    } else if (dtValue == "Last Week") {
        $("#txtStartDate").val(dtLastWeekStart);
        $("#txtEndDate").val(dtLastWeekEnd);
    } else if (dtValue == "Current Month") {
        $("#txtStartDate").val(dtFirstDayOfMonth);
        $("#txtEndDate").val(dtLastDayOfMonth);
    } else if (dtValue == "Last Month") {
        $("#txtStartDate").val(dtFirstDayLastMonth);
        $("#txtEndDate").val(dtLastDayLastMonth);
    } else if (dtValue == "Custom Date") {
        $("#txtStartDate").val(dtToday);
        $("#txtStartDate").prop('disabled', false);
        $("#txtEndDate").val(dtToday);
    }
    else if (dtValue == "Since Inception") {
        $("#txtStartDate").val("08/01/2025");
        $("#txtStartDate").prop('disabled', true);
        $("#txtEndDate").val(dtToday);
    }


}

function formatNumber(num) {
    const options = {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    };
    const formatted = Number(num).toLocaleString('en', options);
    return formatted;
}