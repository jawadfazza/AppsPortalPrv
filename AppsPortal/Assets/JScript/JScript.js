var Tables;
var BetweenFlag = false;
var CountriesCodes;
var DateFormat = moment().format(MomentDateFormat());
var DateTimeFormat = moment().format(MomentDateTimeFormat());
var hasDigit = new RegExp('.*[0-9].*');
var hasMix = new RegExp('([A-Z]+[a-z]+)|([a-z]+[A-Z]+)');
var ButtonClass = {
    Vote: 'btn btn-info',
    Delete: 'btn btn-danger',
    Restore: 'btn btn-warning',
    Print: 'btn btn-info',
    ProfilePhotoDelete: 'btn btn-danger',
    OrganizationLogoDelete: 'btn btn-danger',
    OfficePhotoDelete: 'btn btn-danger',
    Submit: 'btn btn-info',
    RetrieveBulkItems: 'btn btn-danger',
    ReminderCustodianBulkItems: 'btn btn-info',
    ReminderPendingConfirmationBulkItems: 'btn btn-info',
    ReminderForDelayInReturnItemsToStock: 'btn btn-info',
}
var IconClass = {
    Delete: 'fa fa-3x fa-trash-o ',
    ProfilePhotoDelete: 'fa fa-3x fa-trash-o ',
    Restore: 'fa fa-3x fa-history ',
    Print: 'fa fa-3x fa-print',
}
var NotificationType = {
    Success: "success",
    Error: "error",
    Warning: "warning",
    Info: "info",
    Default: null
}
var popOverSettings = {
    placement: 'auto top',
    trigger: 'focus',
    html: true,
    selector: '.input-validation-error',
    container: '#PopOverContainer',
}
var passwordPolicy = {
    "policy": {
        "length_min": 8,        // Minimum password length [>=0]
        "length_max": 20,       // Maximum password length [>=0]
        "required_upper": 1,    // Count of required UPPERCASE characters [>=0]
        "required_lower": 1,    // Count of required lowercase characters [>=0]
        "required_digits": 1,   // Count of required numeric characters [>=0]
        "required_special": 0   // Count of required special characters [>=0]
    }
};



String.prototype.endsWith = function (suffix) {
    //THIS Prototype added to make endsWith function compatible with IE
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

$('#MenuDataTable').on('processing.dt', function (e, settings, processing) {
    //$('#processingIndicator').css('display', processing ? 'block' : 'none');
}).dataTable();

$(document).on('focus', '.password-strength', function () {
    InitPasswordStrength();
});

$('body').popover(popOverSettings);

$(document).on('change', 'input', function () {
    if (this.value == '') {
        $(this).valid();
    }
});

$(document).on('keyup', 'input:text', function () {

    if ($(this).hasClass('input-validation-error')) {
        $(this).removeClass('input-validation-error')
    }
});

$(document).on('select2:select', function (event) {
    var select = $(event.target);
    var select2 = $('#select2-' + $(select).attr('id') + '-container');

    if ($(select).val() != '') {
        $(select).removeClass('input-validation-error');
        $(select2).removeClass('input-validation-error');
        $(select2).parents('.select2-selection').removeAttr('data-content');
    }
    else {
        if (select.attr('data-val')) {
            $(select2).addClass('input-validation-error');
            $(select2).parents('.select2-selection').attr('data-content', $(select).attr('data-error'));
        }
    }
});

$(document).on('change', 'select', function () {
    if ($(this).attr('data-val')) {
        if (this.value == '') {
            $(this).valid();
        }
        else {
            $(this).next().find('span').attr('data-content', '');
        }
    }
});

$(document).on('click', '.AuditUserMoreInfo', function () {
    $(this).parent().next('.UserMoreInfoPanel').toggle();
});

$(document).on('select2:closing', function (event) {
    var select = $(event.target);
    var select2 = $('#select2-' + $(select).attr('id') + '-container');
    if ($(select).val() != '') {
        $(select2).parents('span').removeAttr('data-content');
        $(select2).removeAttr('data-content');
    }
});

$(document).ready(function () {


    $('.matchHeight').matchHeight();

    //Unbind Scroll Event
    $(window).unbind('scroll');

    $(window).on('scroll', function () {
        if ($(window).scrollTop() > 400) {
            $('.scrollToTop').fadeIn();
        }
        else {
            $('.scrollToTop').fadeOut();
        }
    });

    //Rebuild all plugins 
    InitPlugins($('body'));

    //Very Important to Validate dates on different cultures
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || moment(value).format(DateTimeFormat);
    };

    //Disable submit on submit
    //$('form').submit(function () {
    //    if ($('.input-validation-error').length == 0) {
    //        $(this).find(':submit').prop('disabled', true);
    //    }
    //});

    $('h2').fitText(2, {
        minFontSize: '23px', maxFontSize: '30px'
    });

    $('li.search, li.lang').click(function () {
        $('li.notification').removeClass('active');
        $('#notificationPanel').hide();
        if ($('.overlay').hasClass('active')) {
            $('.overlay').removeClass('active');
        } else {
            $('.overlay').addClass('active');
        }
    })
    $('.icon--close').click(function () {
        if ($('.overlay').hasClass('active')) {
            $('.overlay').removeClass('active');
        } else {
            $('.overlay').addClass('active');
        }
    })
    $('li.notification').click(function () {
        $('li.search, li.lang').removeClass('active');
        $('.un--settings').removeClass('is--active');
        $('.un--search').removeClass('open');

        $('#notificationPanel').toggle();
        $(this).toggleClass('active');
        if ($('.overlay').hasClass('active')) {
            $('.overlay').removeClass('active');
        } else {
            $('.overlay').addClass('active');
        }


    });
    $('.overlay').click(function () {
        $('#notificationPanel').hide();
        $('li.notification').removeClass('active');
        $('li.active').removeClass('active');
        $('li.search, li.lang').removeClass('active');
        $('.un--settings').removeClass('is--active');
        $('.un--search').removeClass('open');


        if ($('button.c-hamburger').hasClass('is-active')) {
            $('button.c-hamburger').click();
        }
        $(this).removeClass('active');
    })

    // Jquery draggable
    $('.modal-dialog').draggable({
        handle: ".modal-header"
    });

    $('.select2-basic-ajax').select2({
        minimumInputLength: 3,
        ajax: {
            url: function () { return '/DropDownList/' + $(this).attr('data-action') },
            delay: 250,
            data: function (params) {
                var query = {
                    SearchKey: params.term
                }
                // Query parameters will be ?search=[term]&type=public
                return query;
            },
            processResults: function (data) {
                // Tranforms the top-level key of the response object from 'items' to 'results'

                return {
                    results: data.items
                };
            }
        }
    });


});

$(document).on('shown.bs.modal', function (e) {
    $('.date-picker-plugin').remove();
    $('html').addClass('modal-open');
});

$(document).on('hidden.bs.modal', function (e) {
    $('.date-picker-plugin').remove();
    InitDateTimePicker();
    $('#Loading-Container').show();
    $('#Partial-Container').html('');
    $('html').removeClass('modal-open');
    $('.popover').hide();
    $('.webui-popover').hide();
    $('.modal-dialog').removeClass('modal-lg');

});

$(document).on('ifChecked ifUnchecked', '.chkHead', function (event) {
    var DataTable = $(this).closest('table, .checkboxes-container');
    console.log(DataTable);
    if (event.type == 'ifChecked') {
        $('.chkNode', DataTable).iCheck('check');
        //$('.odd, .even', DataTable).addClass('SelectedRowStyle');
    } else {
        $('.chkNode', DataTable).iCheck('uncheck');
        //$('.odd, .even', DataTable).removeClass('SelectedRowStyle');
    }
});

$(document).on('ifChecked ifUnchecked', '.chkNode', function (event) {
    var DataTable = $(this).closest('table');
    var form = $('#' + DataTable.attr('id') + 'Form');
    var ID = $(this).val();
    var RV = $(this).parent().next().val();
    if (event.type == 'ifChecked') {
        //alert(DataTable.attr('id') + 'Form');
        if ($.fn.dataTable.isDataTable('#' + DataTable.attr('id'))) {
            if ($('#' + DataTable.attr('id')).DataTable().data().count() ==
                $('#' + DataTable.attr('id')).find('.chkNode:checked').length) {
                $('#' + DataTable.attr('id') + 'SelectAll').prop("checked", true);
                $('#' + DataTable.attr('id') + 'SelectAll').parent('div').addClass('checked');
                $('.chkHead', DataTable.closest('.checkboxes-container')).prop("checked", true);
                $('.chkHead', DataTable.closest('.checkboxes-container')).parent('div').addClass('checked');
            }
            $(this).closest('tr').addClass('SelectedRowStyle');
        }
        var t = '<div class="reindex-container">';
        t += '<input type="hidden" data-id="' + ID + '" class="chkPK" name= "' + $(this).attr('name') + '" value= "' + ID + '" />';
        t += '<input type="hidden" data-id="' + ID + '" class="chkRV" name= "' + $(this).parent().next().attr('name') + '" value= "' + RV + '" />';
        t += '</div>';
        $(form).append(t);
    }
    else {

        if ($.fn.dataTable.isDataTable('#' + DataTable.attr('id'))) {
            $('.chkHead', DataTable).prop("checked", false);
            $('.chkHead', DataTable).parent('div').removeClass('checked');

            $('.chkHead', DataTable.closest('.checkboxes-container')).prop("checked", false);
            $('.chkHead', DataTable.closest('.checkboxes-container')).parent('div').removeClass('checked');

            $(this).closest('tr').removeClass('SelectedRowStyle');

            var count = $('.chkPK', '#' + $(DataTable).attr('id') + 'Form').length;
            $('#' + $(DataTable).attr('id') + '_wrapper').find('.Count').text(count);
        }
        $(form).find('*[data-id="' + ID + '"]').parent().remove();

    }
    SelectedReocrdsInfo(DataTable.attr('id'));
});

$(document).on('ifChecked ifUnchecked', '.chkHead-accordion', function (event) {
    //
    //var Container = $(this).closest('form, .checkboxes-container');
    var Container = $(this).closest('.mScroll');
    if (event.type == 'ifChecked') {
        $('.chkNode-accordion', Container).parent('div').addClass('checked');
        $('.chkNode-accordion', Container).prop('checked', true);
    } else {
        $('.chkNode-accordion', Container).parent('div').removeClass('checked');
        $('.chkNode-accordion', Container).prop('checked', false);
    }
    $(Container).trigger('checkform.areYouSure');
    $('#SelectedMessage', Container).text($('.chkNode-accordion:checked').length + " of " + $('.chkNode-accordion').length + " Selected");
});

$(document).on('ifChecked ifUnchecked', '.chkNode-accordion', function (event) {
    var Container = $(this).closest('form, .checkboxes-container');
    var allChecked = $('.chkNode-accordion', Container).length == $('.chkNode-accordion:checked', Container).length;
    $('.chkHead-accordion', Container).prop('checked', allChecked);

    if (allChecked) {
        $('.chkHead-accordion', Container).parent('div').addClass('checked');
    }
    else {
        $('.chkHead-accordion', Container).parent('div').removeClass('checked');
    }
    $(Container).trigger('checkform.areYouSure');
    $('#SelectedMessage', Container).text($('.chkNode-accordion:checked').length + " of " + $('.chkNode-accordion').length + " Selected");
    $('.FindTextBox').val('').trigger('keyup');
});

$(document).on('click touchend', '.btnSetTime', function () {
    {
        $('body').click();
    }
});

$(document).on('click', '.Modal-Link', function (event) {
    //
    event.preventDefault();
    var RowIndex = -1;
    if ($(this).parents('table').length > 0) {
        var DataTableID = $(this).parents('table').attr('id');
        RowIndex = $('#' + DataTableID + '').DataTable().row($(this).parents('tr')).index();
    }

    $('#FormModal').modal('show');
    var largeModal = false;

    if ($(this).hasClass('lgModal')) {
        largeModal = true;
    }

    $('#Partial-Container').load($(this).attr('data-url'), function () {
        if (RowIndex != -1) {
            $('#FormModal').find('#RowIdx').val(RowIndex);
        }
        $('#Loading-Container').hide();
        InitPlugins($('#Partial-Container'));

        if ($('#FormModal').find('.ma5slider')) {
            if ($('.ma5slider').length > 0) {
                $('.ma5slider').ma5slider();
            }
            $('.horizontal-navs.outside-navs .navs-wrapper').height('700px');
            $('.slide-area').height('100%');
        }



        if (largeModal) {
            $('.modal-content').css('width', '150%');
            $('.modal-content').css('margin-left', '-175px');
        } else {
            $('.modal-content').css('width', 'inherit');

            $('.modal-content').css('margin-left', '0px');
        }

    });
});

$(document).on('click', '.Confirm', function (event) {

    event.preventDefault();

    var mode = $(this).attr('data-mode');
    var action = $(this).attr('data-submittype');
    PrepareModal(action, mode);
    EnableForm('#ConfirmModal');

    if (mode == 'Single') {
        $('#btnConfirm').attr('form', $(this).parents('form').attr('id'));
        $('#ConfirmModal').modal('show');
    }
    else if ($('#' + $(this).attr('data-datatable') + 'Form').children().length > 1) {
        $('#btnConfirm').attr('data-refreshdatatable', $(this).attr('data-datatable'));
        $('#btnConfirm').attr('form', $(this).attr('data-datatable') + 'Form');
        $('#ConfirmModal').modal('show');
    }
    else {
        Notify(NotificationType.Info, 'No Record Selected');
    }
});

$(document).on('click', '.datebox', function (e) {
    $(this).prev().click();
});

$(document).on('click', '.ConfirmOnModal', function (event) {
    $(this).parents('.modal-footer').find('.btn').hide();
    $(this).parents('.modal-footer').find('.CancelConfirmOnModal').show().prev().show().removeAttr('disabled');
});

$(document).on('click', '.CancelConfirmOnModal', function (event) {
    $(this).parents('.modal-footer').find('.btn').show();
    $(this).parents('.modal-footer').find('.CancelConfirmOnModal').hide().prev().hide().prop('disabled', true);
});

$(document).on('click', '.concurrencybox', function (event) {
    var current = $(this).attr('data-current');
    var db = $(this).attr('data-db');
    var showing = $(this).attr('data-showing');
    var control = $(this).attr('data-for');
    var form = $(this).attr('data-form');

    var $control = $('#' + control, '#' + form);
    if (showing == 'user') {
        if ($control.hasClass("select2-basic-multiple")) {
            $control.val(db.split(',')).trigger('change').removeClass('input-concurrency-error');
        } else {
            $control.val(db).trigger('change').removeClass('input-concurrency-error');
        }
        if ($control.is('select')) {
            $control.next().find('.select2-selection__rendered').removeClass('input-concurrency-error');
        } else {
            $control.removeClass('input-concurrency-error');
        }
        $(this).attr('data-showing', 'db').removeClass('fa-database').addClass('fa-thumbs-o-up');
    }
    else {
        if ($control.hasClass("select2-basic-multiple")) {
            $control.val(current.split(',')).trigger('change')
        } else {
            $control.val(current).trigger('change');
        }
        if ($control.is('select')) {
            $control.next().find('.select2-selection__rendered').addClass('input-concurrency-error');
        } else {
            $control.addClass('input-concurrency-error');
        }
        $(this).attr('data-showing', 'user').removeClass('fa-thumbs-o-up').addClass('fa-database');
    }
    $(form).find(':submit').prop('disabled', false);
});

$(document).on('select2:select', '.FilterOperation', function () {

    var op = $(this).val();
    //Between and Not Between Handling
    if (op == 'bt' || op == 'nb') {
        if (BetweenFlag) {
            RemoveBetween($(this));
        }
        BetweenFlag = true;
        AddBetween($(this));
    }
    else {
        if (BetweenFlag) {
            RemoveBetween($(this));
        }
    }
    //Empty and Not Empty Handling
    if (op == "em" || op == "nm") {
        AddEmpty(this);
    }
    else {
        RemoveEmpty(this);
    }
    $(this).parents().eq(1).next().find('.FilterValue').focus();

});

$(document).on('change', 'input', function (e) {
    if (e.which != 13) {
        $(this).popover('hide');
        $(this).parents('form').find('.ErrorSummary').hide();
    }
    else {
        $(this).popover('show');
    }
});

$(document).on('keyup', '.filter-section', function (event) {
    if (event.keyCode == 13) {
        $('.btnFind', this).click();
    }
    else if (event.keyCode == 27) {
        $('.btnClose', this).click();
    }
});

$(document).on('keypress', '.Numeric', function (event) {
    event = (event) ? event : window.event;
    var charCode = (event.which) ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
});

$(document).on('show.bs.popover', function () {
    var width = $(':focus').width() + parseInt($(':focus').css('padding-left')) + parseInt($(':focus').css('padding-right'));
    $('#PopOverContainer').css('width', width);
});

$(document).on('keyup', '.FillLocationSearch', function () {
    $('#txtCountryDescription').val($(this).val());
    $('#txtCountryDescription').focus();
    $(this).focus();
});

$(document).on('click', '.primary--nav > ul > li', function (e) {
    if ($(window).width() > 960) {

        if ($(this).find('.sub--menu').is(':visible')) {
            if (!$(e.target).is('.primary--nav > ul > li > ul > li > a')) {
                $(this).find('.sub--menu').slideUp();
            }
            else {
                //alert('I should tell you that Im loading...');
                $(e.target).addClass('loadingMenu');
            }
        }
        else {
            $('.sub--menu').hide();
            $(this).find('.sub--menu').slideDown('fast');
        }
    }
});

$(document).on('click', 'body', function (e) {
    var target = $(e.target);
    if (!target.is('.primary--nav > ul > li > a') && !target.is('.primary--nav > ul > li > ul > li > a')) {
        $('.sub--menu:visible').hide();
    }
});

$(document).on('click', '.ReadOnlyForm .select2', function (e) {
    $('.select2-dropdown').hide();
});

$('#accordion').on('show.bs.collapse', function (e) {
    if (!$(this).hasClass('overrideEnabled')) {

    }
    //alert('before oppening....'); 
});

$('#accordion').on('shown.bs.collapse', function (e) {
    if (!$(this).hasClass('overrideEnabled')) {

    }
    //alert('after oppening....');
});

$('#accordion').on('hide.bs.collapse', function () {
    if (!$(this).hasClass('overrideEnabled')) {
        $('html, body').animate({
            scrollTop: 0
        }, 0);
    }

});

$("#accordion").on('hidden.bs.collapse', function () {
    if (!$(this).hasClass('overrideEnabled')) {
        $('.panel-body', '#accordion').html('Loading...');
    }

});

function MomentDateFormat() {
    return 'LL'
};

function MomentDateTimeFormat() {
    return 'LLL'
};

function SelectedReocrdsInfo(TableID) {
    if (TableID == 'PartnersContributionsDataTable') {
        var Selected = $('.chkPK', '#' + TableID + 'Form').length;
        //alert($('#' + TableID + ' thead').is(':visible'));

        if (Selected > 0 && $('#' + TableID + ' thead').is(':visible')) {
            if ($('#' + TableID).parent().find('.selected-message').length < 1) {
                //$('#' + TableID + ' thead tr th:visible').not(':first').hide().addClass('hideFix');
                var message = "Record(s) selected from all page(s)";
                if ($("#" + TableID + "_length").length == 0) {
                    message = "Record(s) selected";
                }
                if ($('#' + TableID + ' thead tr th').length > 1) {
                    $('.dataTables_length').append('<th colspan="25" class="selected-message"><span class="Count">' + Selected + '</span><span style="font-weight:100;"> ' + message + ' </span><a class="ClearAll btn-link" onclick="ClearSelection(\'' + TableID + '\')">Clear All</a></th>');
                } else {
                    $('#' + TableID + ' b').hide();
                    $('.dataTables_length').append('<span class="selected-message"><span class="Count">' + Selected + '</span><span style="font-weight:100;"> ' + message + ' </span><a class="ClearAll btn-link" onclick="ClearSelection(\'' + TableID + '\')">Clear All</a></span>');
                }

            }
            else {
                $('#' + TableID + '_wrapper').find('.Count').text($('.chkPK', '#' + TableID + 'Form').length);
            }
        }
        else {
            $('#' + TableID + ' b').show();
            $('#' + TableID).parent().find('.selected-message').remove();
            $('#' + TableID + ' thead tr th.hideFix').removeClass('hideFix').show();
        }
    }
    else {
        var Selected = $('.chkPK', '#' + TableID + 'Form').length;
        //alert($('#' + TableID + ' thead').is(':visible'));

        if (Selected > 0 && $('#' + TableID + ' thead').is(':visible')) {
            if ($('#' + TableID).find('.selected-message').length < 1) {
                $('#' + TableID + ' thead tr th:visible').not(':first').hide().addClass('hideFix');
                var message = "Record(s) selected from all page(s)";
                if ($("#" + TableID + "_length").length == 0) {
                    message = "Record(s) selected";
                }
                if ($('#' + TableID + ' thead tr th').length > 1) {
                    $('#' + TableID + ' thead tr:first-child').append('<th colspan="25" class="selected-message"><span class="Count">' + Selected + '</span><span style="font-weight:100;"> ' + message + ' </span><a class="ClearAll btn-link" onclick="ClearSelection(\'' + TableID + '\')">Clear All</a></th>');
                } else {
                    $('#' + TableID + ' b').hide();
                    $('#' + TableID + ' thead tr th').append('<span class="selected-message"><span class="Count">' + Selected + '</span><span style="font-weight:100;"> ' + message + ' </span><a class="ClearAll btn-link" onclick="ClearSelection(\'' + TableID + '\')">Clear All</a></span>');
                }

            }
            else {
                $('#' + TableID + '_wrapper').find('.Count').text($('.chkPK', '#' + TableID + 'Form').length);
            }
        }
        else {
            $('#' + TableID + ' b').show();
            $('#' + TableID).find('.selected-message').remove();
            $('#' + TableID + ' thead tr th.hideFix').removeClass('hideFix').show();
        }
    }

}

function ClearSelection(DataTableID) {
    //this.preventDefault();
    $('input.chkNode', '#' + DataTableID).iCheck('uncheck');
    $('#' + DataTableID + 'Form').find('.chkPK, .chkRV').remove();
    $('#' + DataTableID).find('th.selected-message').remove();
    $('#' + DataTableID + ' thead tr th.hideFix').removeClass('hideFix').show();
}

function PrepareModal(action, mode) {
    var jsonObj = ConfirmModal[action];
    $.each(jsonObj, function (key, value) {

        //Jquery endsWith not compatible with IE
        if (key.endsWith(mode) && mode != null) {
            key = key.replace(mode, '');
        }

        //THIS CODE TO MAKE endsWith works in internet explorer
        //if (key.match(mode + "$") == mode) {
        //    key = key.replace(mode, '');
        //}

        var control = $("#ConfirmModal").find('#' + key);
        if ($(control).is(':submit')) {
            $(control).val(value);


            $(control).removeAttr('data-submittype');
            $(control).attr('data-submittype', action);

            $(control).removeClass().addClass(ButtonClass[action]);
        }
        else {
            $(control).text(value);
        }
    });
    $("#ModalIcon").removeClass().addClass(IconClass[action]);
}

function PrepareSignalRModal(title, message, callback) {
    $('#ModalTitle', '#SignalRModal').html(title);
    $('#ModalMessage', '#SignalRModal').html(message);
    $('#btnConfirmSignalR').attr('onclick', callback);
    $('#SignalRModal').modal('show');
}

function InitPasswordStrength() {
    $('.password-strength').on('keyup focus', function () {
        if (hasDigit.test($(this).val())) {
            $('.digit').addClass('passed-condition');
        }
        else {
            $('.digit').removeClass('passed-condition');
        }

        if (hasMix.test($(this).val())) {
            $('.mixed').addClass('passed-condition');
        }
        else {
            $('.mixed').removeClass('passed-condition');
        }
        if ($(this).val().length >= 8) {
            $('.minlen').addClass('passed-condition');
        }
        else {
            $('.minlen').removeClass('passed-condition');
        }
    });

    $('.password-strength').popover({
        placement: 'auto bottom',
        trigger: 'focus',
        html: true,
        content: '<div class="password-strength-container text-left">' +
            '<span class="pwd-title">Your password must have:</span>' +
            '<div class="pwd-errors-container">' +
            '<span class="pwd-condition minlen">8 or more characters</span>' +
            '<span class="pwd-condition mixed">Upper & lowercase letters</span>' +
            '<span class="pwd-condition digit">At least one number</span>' +
            '</div>' +
            '<span class="pwd-title">Strength: </b>' +
            '<div id="score_percent" style="display:inline-block;"></div>' +
            '<div id="password_strength"><div class="bar"></div></div>' +
            '<div class="pwd-message">Avoid using a password that you use with other websites or that might be easy for someone else to guess.</div>' +
            '</div>'
    }).addClass('my-super-popover');

    // Initialize plugin
    $(".password-strength").passtest(passwordPolicy);

    // Bind passtest event on pssword field
    $(".password-strength").bind("passtest", function () {

        var status = $(this).passtest().status;

        // Set password strength bar
        jQuery("#password_strength").find(".bar").stop().animate({ "width": status.score + "%" }, 200, "linear");
        jQuery("#password_strength .bar").css('background-color', StrengthColor(status.score));

        // Set password score percent
        jQuery("#score_percent").text(StrengthText(status.score));
    });
    $('.password-strength').removeAttr('data-content');
}

function InitPlugins(container) {

    $('.Toggle').bootstrapToggle();
    //Initialize Select2 Basic (no search)
    InitSelect2(container);

    //Initialize Select2 for country code
    InitSelect2CountryCode(container);

    //Initialize iCheck
    InitiCheck(container);

    //Initialize DatePicker
    InitDateTimePicker(container);

    //Prepare validator to be displayed on popover
    InitValidators(container);

    //Make Helpbox popover ready
    InitHelpbox(container);

    //Prevent Submit if the user didn't change anything.
    PreventSubmitWhenNoChange(container);

    //Password Strength
    InitPasswordStrength();

    //Initialize MalihuScroll
    InitMScroll();

    $('form').areYouSure();

    //This to avoid the flickering of plugins on input controls. 
    $('.forms-fadeIn').fadeIn().removeClass('forms-fadeIn');


    if ($(container).find('.Error').text() != "") {
        $(container).find('.Error').parents().show();
    }

    $("#TreeLevelsSlider").bootstrapSlider({
        ticks: [1, 2, 3, 4, 5, 6, 7],
        formatter: function (value) {
            return 'Level: ' + value;
        },
        step: 1,
        value: 1
    });

    $('#TreeLevelsSlider').slider().on('change', function (event) {
        var New = event.value.newValue;
        var Old = event.value.oldValue;
        if (New > Old) //Increase
        {
            for (var i = Old; i < New; i++) {
                $('li[aria-level=' + i + ']').each(function () {
                    $('#PermissionsTree').jstree("open_node", this);
                });
            }
        }
        else {
            // Decrease 
            //New < Old
            //2 < 5
            for (var i = Old; i >= New; i--) {
                $('li[aria-level=' + i + ']').each(function () {
                    $('#PermissionsTree').jstree("close_node", this);
                });
            }
        }
        if (New == 1) {
            $('#btnExpandCollapseTree').removeClass('expandedTree');
        }
    });

}

function InitHelpbox(container) {
    $('.helpbox', container).webuiPopover({
        placement: isRTL() ? 'top-right' : 'top-left',
        closeable: true,
        width: 'auto',
        animation: 'pop',
        title: $(this).attr('title'), //'Tipe',
        direction: isRTL() ? 'rtl' : 'ltr',
    });
}

function InitValidators(container) {
    $('input.form-control', container).attr('data-placement', 'auto top');
    $('input.form-control', container).attr('data-container', '#PopOverContainer');
    //Parse form after return with error by ajax call
    $.validator.unobtrusive.parse($('form'));
}

function InitSelect2(container) {

    $('.select2-basic-single', container).select2({
        placeholder: {
            id: '-1' // the value of the option
        },
        minimumResultsForSearch: "Infinity",
    });
    $(".select2-basic-multiple", container).select2({
        placeholder: {
            id: "-1"
        },
        //multiple: true, //DONT ENABLE THIS OPETION PLEASE Ba**sa of empty option will appear.
        minimumResultsForSearch: "Infinity",
        tokenSeparators: [','],
    });
    $('.select2-basic-search', container).select2({
        placeholder: {
            id: "-1"
        },
    });

    $('#LocationLong', '#PartnersContributionsForm').select2('destroy');
    $('#LocationLong', '#PartnersContributionsForm').select2({
        width: 'resolve',
        dropdownAutoWidth: true,
        placeholder: {
            id: "-1"
        }
    });
    $('#ActivityLong', '#PartnersContributionsForm').select2('destroy');
    $('#ActivityLong', '#PartnersContributionsForm').select2({
        width: 'resolve',
        dropdownAutoWidth: true,
        placeholder: {
            id: "-1"
        }
    });

    $('#FacilityGUID', '#PartnersContributionsForm').select2('destroy');
    $('#FacilityGUID', '#PartnersContributionsForm').select2({
        width: 'resolve',
        dropdownAutoWidth: true,
        placeholder: {
            id: "-1"
        }
    });

    $('#BeneficiariesTypeGUID', '#PartnersContributionsForm').select2('destroy');
    $('#BeneficiariesTypeGUID', '#PartnersContributionsForm').select2({
        width: 'resolve',
        dropdownAutoWidth: true,
        placeholder: {
            id: "-1"
        }
    });



}

function InitSelect2CountryCode(container) {
    $('.select2-basic-CountryCode', container).select2({
        //ajax: {
        //    url: "DropDownList/RemoteCountryCodes",
        //    dataType: 'json',
        //    processResults: function (data, params) {
        //        return {
        //            results: data,
        //        };
        //    },
        //    cache: true
        //},
        data: CountriesCodes,
        escapeMarkup: function (markup) { return markup; },
        templateResult: formatRepo,
        templateSelection: formatRepoSelection,
    });
}

function formatRepo(repo) {
    var markup = '';
    if (repo.id != '') {
        markup = "<img class='CountryFlag' src='http://10.240.224.199/Media/Flags/" + repo.src + ".png'/>" + "<div class='CountryCode'>" + repo.id + "</div>" + "<div class='CountryDescription'>(" + repo.text + ")</div>";
    }
    return markup;
}

function formatRepoSelection(repo) {
    if (repo.id != '') {
        return "<img class='SelectedCountryFlag' src='http://10.240.224.199/Media/Flags/" + repo.src + ".png'/>" + repo.id;
    }
    return '';
}

function InitRecordsPerPage() {
    $(".dataTables_length select").select2({
        width: 'resolve',
        minimumResultsForSearch: "Infinity"
    });
    $(".dataTables_length").addClass('showInline');

}

function InitiCheck(container) {
    $('input:checkbox:not(.iPhoneCheckBox), input:radio', container).iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
        increaseArea: '20%' // optional
    });
}

function InitDateTimePicker(container) {
    $('.date-picker', container).pikaday('destroy').pikaday({
        format: MomentDateFormat(),
        yearRange: [1930, 2025],
        isRTL: isRTL(),
        todayButton: true,
    });
    $('.datetime-picker', container).pikaday('destroy').pikaday({
        format: MomentDateTimeFormat(),
        yearRange: [1930, 2025],
        showTime: true,
        autoClose: false,
        isRTL: isRTL(),
        showTime: true,
        showMinutes: true,
        todayButton: true,
    });
}

function InitDataTable(Obj) {
    var Type = Obj['Type']
    var DataTableID = Obj['TableID'];
    var _responsive = true;
    if (DataTableID == 'PartnersContributionsDataTable') {
        _responsive = false;
    }
    var dom = '';
    if (Type == 'Index') {
        dom = 'litrp';
    }
    else {
        dom = 'tr';
        resxDataTableLanguage['sProcessing'] = '<i class="FieldDataTableLoading"></i>';
    }
    var table = $('#' + DataTableID).DataTable({
        dom: dom,
        order: [[Obj['OrderBy'], "asc"]],
        searching: true,
        stateSave: true,
        ordering: true,
        processing: true,
        language: resxDataTableLanguage,
        autoWidth: false,
        //fixedColumns: true,
        serverSide: true,
        deferRender: true,
        ajax: {
            method: 'POST',
            url: $('#' + DataTableID).attr('data-url'),
            data: {
                method: "datatables",
                filters: function () {

                    return ToEngDigits(BuildFilterString(DataTableID));
                }
            }
        },
        columns: Obj["Columns"],
        responsive: _responsive,
        fnPreDrawCallback: function () {
            SelectedReocrdsInfo(DataTableID);
            $('#' + DataTableID).find('th:first').addClass('dtProcessing').attr('disabled');
        },
        drawCallback: function () {
            $('#' + DataTableID + ' th:first').removeClass('sorting_asc');
            InitRecordsPerPage();
            InitiCheck($('#' + DataTableID));
            var api = $('#' + DataTableID).DataTable();
            var DataTableChkAll = $('#' + DataTableID + 'SelectAll')
            var numberOfChecked = $('#' + DataTableID).find('.chkNode:checked').length;
            var RowsCount = api.rows().count();
            if (RowsCount == numberOfChecked && RowsCount != 0) {
                $(DataTableChkAll).prop("checked", true).parent('div').addClass('checked');
            }
            else {
                $(DataTableChkAll).prop("checked", false).parent('div').removeClass('checked');
            }

            var Result = window.localStorage.getItem(DataTableID + 'Filter');
            if (Result != null) {
                if (Result.indexOf('[{"field":"Active","op":"eq","data":"false"}]') > 0) {
                    $('#' + DataTableID).addClass('deletedRecords');
                }
                else {
                    $('#' + DataTableID).removeClass('deletedRecords');
                }
            }
            $('#' + DataTableID).find('.dtProcessing').removeClass('dtProcessing');

        },
        rowCallback: function (row, data, index) {
            var PK = $(row).find('input:first-child').attr('value');
            if ($('#' + DataTableID + 'Form').find('*[data-id="' + PK + '"]').length > 0) {
                $(row).find('.chkNode').prop('checked', 'checked');
                $(row).addClass('SelectedRowStyle');
            }

        }
    });
    if (Type == 'Index') {
        if (DataTableID != 'PartnersContributionsDataTable') {
            new $.fn.dataTable.Buttons(table, {
                buttons: [
                    {
                        extend: 'copyHtml5',
                        text: '<i class="fa fa-files-o"></i>',
                        titleAttr: 'Copy',
                        className: 'btn btn-primary'
                    },
                    {
                        extend: 'excelHtml5',
                        text: '<i class="fa fa-file-excel-o"></i>',
                        titleAttr: 'Excel',
                        className: 'btn btn-primary'
                    },
                    {
                        extend: 'pdfHtml5',
                        text: '<i class="fa fa-file-pdf-o"></i>',
                        titleAttr: 'PDF',
                        className: 'btn btn-primary'
                    },
                    {
                        extend: 'print',
                        text: '<i class="fa fa-print"></i>',
                        title: '<h4>Report Title</h4>', //'<h4 style="color:red;">Report Title</h4>',
                        className: 'btn btn-primary',
                        customize: function (win) {
                            $(win.document.body).css('font-size', '10pt');
                            //.prepend(
                            //    '<img src="http://www.unhcr.org/assets/img/unhcr-logo.png" style="position:absolute; top:0; left:0;" />'
                            //);

                            $(win.document.body).find('table')
                                .addClass('compact')
                                .css('font-size', 'inherit');
                        }
                    }

                ]
            });
        }

        table.buttons(0, null).container().insertAfter($('.btnRefresh'));
        $('.dt-buttons').replaceWith(function () {
            return $('a', this);
        });


    }



}

function InitFilter() {

    $.each($('table.dataTable'), function () {
        var TableID = ($(this).attr('id'));
        var strFilter = window.localStorage.getItem(TableID + 'Filter');
        if (strFilter != null) {
            strFilter = strFilter.replace('"groupOp":"AND",', '');
            var obj = JSON.parse(strFilter);
            for (var i = 0; i < obj.rules.length; i++) {
                var parents = $('#filter_' + obj.rules[i].field).parents();
                if (i != obj.rules.length - 1) {
                    if (obj.rules[i].field == obj.rules[i + 1].field) {//Between detected
                        AddBetween($(parents).eq(2).prev().find('.FilterOperation'));
                        if (obj.rules[i].op == 'ge') {
                            $(parents).eq(2).prev().find('.FilterOperation').val('bt').trigger('change');
                        }
                        else {
                            $(parents).eq(2).prev().find('.FilterOperation').val('nb').trigger('change');
                        }
                        $('#filter_' + obj.rules[i].field + '.FilterValue:eq(0)').val(obj.rules[i].data).trigger('change');
                        $('#filter_' + obj.rules[i].field + '.FilterValue:eq(1)').val(obj.rules[i + 1].data).trigger('change');
                        i++;
                    }
                    else {
                        if (typeof $(parents).eq(2).prev().html() != 'undefined') {
                            $(parents).eq(2).prev().find('.FilterOperation').val(obj.rules[i].op).trigger('change');
                        } else {
                            $(parents).eq(1).prev().find('.FilterOperation').val(obj.rules[i].op).trigger('change');
                        }
                        if ($('#filter_' + obj.rules[i].field + '.FilterValue:eq(0)').hasClass("select2-basic-multiple")) {
                            $('#filter_' + obj.rules[i].field + '.FilterValue:eq(0)').val(obj.rules[i].data.split(',')).trigger('change');
                        } else {
                            $('#filter_' + obj.rules[i].field + '.FilterValue:eq(0)').val(obj.rules[i].data).trigger('change');
                        }

                    }
                }
                else {
                    if ($(($(parents).eq(2).children(0))[0]).attr('for') == "filter_" + obj.rules[i].field) {
                        $(parents).eq(1).prev().find('.FilterOperation').val(obj.rules[i].op);
                    } else {
                        $(parents).eq(2).prev().find('.FilterOperation').val(obj.rules[i].op);
                    }
                    $('#filter_' + obj.rules[i].field + '.FilterValue:eq(0)').val(obj.rules[i].data);

                }
            }
            SetFilterIndicator(TableID);
        }
    })
}

function InitMScroll() {
    $(".mScroll").mCustomScrollbar({
        axis: "y", // horizontal scrollbar
        scrollInertia: 0,
        mouseWheelPixels: 20
    });
}

function DataTableRefresh(TableID) {
    $('#' + TableID).DataTable().ajax.reload(function () {

        $('#FormModal').modal('hide');
        $('#ConfirmModal').modal('hide');

    }, false); //false stay on the current page, true go the first page. Ayas
}

function BuildFilterString(TableID) {
    var Body = '';
    var field = '';
    var op = '';
    var data = '';
    var Result = '';

    $('.FilterField', $('#' + TableID + 'Filter')).each(function () {

        //If Field Filter has two input for between
        field = $(this).find('.FilterValue').attr("data");
        op = $(this).find('.FilterOperation').val();
        data = $(this).find('.FilterValue').val();

        switch (op) {
            case 'bt':
                //Add first field to the filter string as greater than or equal.
                op = 'ge';
                field = $(this).find('.FilterValue:eq(0)').attr("data");
                data = $(this).find('.FilterValue:eq(0)').val();
                if (data != '' && data != 'null') {
                    Body += '{"field":"' + field + '","op":"' + op + '","data":"' + data + '"},';
                }
                //Add second field to the filter string as less than or equal.
                op = 'le';
                field = $(this).find('.FilterValue:eq(1)').attr("data");
                data = $(this).find('.FilterValue:eq(1)').val();
                if (data != '' && data != 'null') {
                    Body += '{"field":"' + field + '","op":"' + op + '","data":"' + data + '"},';
                }
                break;
            case "nb":
                //Add first field to the filter string as less than.
                op = 'lt';
                field = $(this).find('.FilterValue:eq(0)').attr("data");
                data = $(this).find('.FilterValue:eq(0)').val();
                //if (data != '' && data != 'null') {
                Body += '{"field":"' + field + '","op":"' + op + '","data":"' + data + '"},';
                //}
                //Add second field to the filter string as greater than or equal.
                op = 'gt';
                field = $(this).find('.FilterValue:eq(1)').attr("data");
                data = $(this).find('.FilterValue:eq(1)').val();
                //if (data != '' && data != 'null') {
                Body += '{"field":"' + field + '","op":"' + op + '","data":"' + data + '"},';
                //}
                break;

            case "em":
                Body += '{"field":"' + field + '","op":"eq","data":""},';
                break;
            case "nm":
                Body += '{"field":"' + field + '","op":"ne","data":""},';
                break;
            default:
                if (data != '') {
                    Body += '{"field":"' + field + '","op":"' + op + '","data":"' + data + '"},';
                }
        }
    });
    Result = '{"groupOp":"AND","rules":[' + Body.substring(0, Body.length - 1) + ']}';
    if (Result.indexOf('[{"field":"Active","op":"eq","data":"false"}]') > 0) {
        $('.btnDelete, .btnCreate', '#' + TableID + 'ActionsContainer').hide();
        $('.btnRestore', '#' + TableID + 'ActionsContainer').show();
        $('#' + TableID).addClass('deletedRecords');
    }
    else {
        $('.btnDelete, .btnCreate', '#' + TableID + 'ActionsContainer').show();
        $('.btnRestore', '#' + TableID + 'ActionsContainer').hide();
        $('#' + TableID).removeClass('deletedRecords');
    }
    return Result;
}

function ApplyFilter(TableID) {
    $('.chkPK, .chkRV', '#' + TableID + "Form").remove();
    //$('#' + TableID).DataTable().page(0).draw(false);//this will cause two request to the server.
    window.localStorage.setItem(TableID + 'Filter', BuildFilterString(TableID));
    $('div.alert', '#' + TableID + '_wrapper').remove();
    SetFilterIndicator(TableID);
    DataTableRefresh(TableID);
}

function ClearFilter(TableID) {
    $('.chkPK, .chkRV', '#' + TableID + "Form").remove();
    $('div.alert', '#' + TableID + '_wrapper').remove();
    $('#' + TableID).DataTable().page(0).draw(false);
    $('.FilterOperation, .FilterValue', $('#' + TableID + 'Filter')).each(function () {
        if ($(this).is('select')) {
            if ($(this).val() == 'bt' || $(this).val() == 'nb') {
                RemoveBetween($(this));
            }
            else {
                $(this).prop('selectedIndex', 0).change();
                if ($(this).hasClass('select2-basic-multiple')) {
                    $(this).val(null).trigger("change");
                }
            }
        }
        else {
            $(this).val('');
        }
    });
    $('.btnFilter').removeClass('btn-danger').addClass('btn-success');
    $('.btnFilter i').removeClass('fa-exclamation-triangle').addClass('fa-filter');
    window.localStorage.removeItem(TableID + 'Filter');
    ToggleFilter(TableID);
    DataTableRefresh(TableID);
}

function ToggleFilter(TableID) {
    $('#' + TableID + 'Filter').slideToggle();
    if (!$('#' + TableID).hasClass('field-datatable')) {
        $(window).scrollTop(0);
    }
}

function SetFilterIndicator(TableID) {
    var pattren = '{"groupOp":"AND","rules":[{"field":"Active","op":"eq","data":"true"}]}';
    if (BuildFilterString(TableID) != pattren) {
        $('.btnFilter', '#' + TableID + 'ActionsContainer').addClass('btn-danger').removeClass('btn-success');
        $('.btnFilter i', '#' + TableID + 'ActionsContainer').addClass('fa-exclamation-triangle').removeClass('fa-filter');
    }
    else {
        $('.btnFilter', '#' + TableID + 'ActionsContainer').addClass('btn-success').removeClass('btn-danger');
        $('.btnFilter i', '#' + TableID + 'ActionsContainer').addClass('fa-filter').removeClass('fa-exclamation-triangle');
    }
}

function AddEmpty(element) {
    var div = $(element).parents().eq(1);
    div.next().find('.FilterValue').prop('disabled', true);
}

function RemoveEmpty(element) {
    var div = $(element).parents().eq(1);
    div.next().find('.FilterValue').prop('disabled', false);
}

function AddBetween(element) {
    var div = $(element).parents().eq(1);
    div.prev().addClass('col-lg-1 col-md-3 col-sm-3 col-xs-12');//Label
    div.addClass('col-lg-1 col-md-3 col-sm-3 col-xs-12');//Operation

    div.next().addClass('col-lg-2 col-md-3 col-sm-3 col-xs-6');//First Control
    div.next().clone().insertAfter($(element).parents().eq(1).next());
    div.next().next().addClass('col-lg-2 col-md-3 col-sm-3 col-xs-6');//Second Control
    if (div.next().find('.FilterValue').attr('datatype') == 'Date' || $(element).parents().eq(1).next().find('.FilterValue').attr('datatype') == 'DateTime') {
        InitDateTimePicker(div.parent());
    }
}

function RemoveBetween(element) {
    $(element).parents().eq(1).prev().removeClass('col-lg-1 col-md-1 col-sm-1 col-xs-12');
    $(element).parents().eq(1).removeClass('col-lg-1 col-md-1 col-sm-1 col-xs-12');
    $(element).parents().eq(1).next().next().remove();
}

//ValuesArray UpdateDropDownList([X,Y,Z],[{DropDownListID:'A',Action:'func'},{DropDownListID:'B',Action:'func'}]
function UpdateDropDownList(ValuesArray, TargetList) {
    $.each(TargetList, function (i, List) {
        var TargetDDL = $('#' + List["DropDownListID"]);
        $(TargetDDL).empty();

        if (List['Action'] != '') {
            $(TargetDDL).next().find('.select2-selection').addClass('LoadingDDL');
            $(TargetDDL).prop("disabled", true);
            var data = {};
            for (var i = 1; i <= ValuesArray.length; i++) {
                data["PK" + i] = ValuesArray[i - 1];
            }
            $.ajax({
                type: "Get",
                url: "/DropDownList/" + List['Action'],
                data: data,
                //datatype: "Json",
                success: function (data) {
                    if ($(TargetDDL).attr('data-noemptyselection') == null) {
                        (TargetDDL).append('<option></option');
                    }
                    $.each(data, function (index, obj) {
                        if (obj.Group != null) {
                            $(TargetDDL).append('<option value="' + obj.Value + '" data-Group="' + obj.Group.Name + '">' + obj.Text + '</option>');
                        } else {
                            $(TargetDDL).append('<option value="' + obj.Value + '">' + obj.Text + '</option>');
                        }
                    });
                    $(TargetDDL).next().find('.select2-selection').removeClass('LoadingDDL');
                    $(TargetDDL).prop("disabled", false);
                }
            });
        }
    });
}

function UpdateDropDownList(ValuesArray, TargetList, successFunction) {
    $.each(TargetList, function (i, List) {
        var TargetDDL = $('#' + List["DropDownListID"]);
        $(TargetDDL).empty();

        if (List['Action'] != '') {
            $(TargetDDL).next().find('.select2-selection').addClass('LoadingDDL');
            $(TargetDDL).prop("disabled", true);
            var data = {};
            for (var i = 1; i <= ValuesArray.length; i++) {
                data["PK" + i] = ValuesArray[i - 1];
            }
            $.ajax({
                type: "Get",
                url: "/DropDownList/" + List['Action'],
                data: data,
                //datatype: "Json",
                success: function (data) {
                    if ($(TargetDDL).attr('data-noemptyselection') == null) {
                        (TargetDDL).append('<option></option');
                    }
                    $.each(data, function (index, obj) {
                        if (obj.Group != null) {
                            $(TargetDDL).append('<option value="' + obj.Value + '" data-Group="' + obj.Group.Name + '">' + obj.Text + '</option>');
                        } else {
                            $(TargetDDL).append('<option value="' + obj.Value + '">' + obj.Text + '</option>');
                        }
                    });
                    $(TargetDDL).next().find('.select2-selection').removeClass('LoadingDDL');
                    $(TargetDDL).prop("disabled", false);
                    if (successFunction != null) {
                        successFunction();
                    }
                }
            });
        }
    });
}

//ValuesArray UpdateDropDownList([X,Y,Z],[{DropDownListID:'A',Action:'func'},{DropDownListID:'B',Action:'func'}]
function UpdateDropDownListFromMultiple(ValuesArray, TargetList) {
    $.each(TargetList, function (i, List) {
        var TargetDDL = $('#' + List["DropDownListID"]);

        if (List['Action'] != '') {
            $(TargetDDL).empty();
            $(TargetDDL).next().find('.select2-selection').addClass('LoadingDDL');
            $(TargetDDL).prop("disabled", true);
            var data = { PK1: ValuesArray.join('*') };
            $.ajax({
                type: "Get",
                url: "/DropDownList/" + List['Action'],
                data: data,
                datatype: "Json",
                success: function (data) {
                    if ($(TargetDDL).attr('data-noemptyselection') == null) {
                        (TargetDDL).append('<option></option');
                    }
                    $.each(data, function (index, obj) {
                        $(TargetDDL).append('<option value="' + obj.Value + '" data-Group="' + obj.Group.Name + '">' + obj.Text + '</option>');
                    });
                    $(TargetDDL).next().find('.select2-selection').removeClass('LoadingDDL');
                    $(TargetDDL).prop("disabled", false);
                }
            });
        }
    });
}

function createLoadingImage() {
    return '<img id="imgLoading" class="uiLoading" src="/Assets/Images/loading.gif">';
}

function Sequntialize(formName) {
    $('.reindex-container', '#' + formName).each(function (index, value) {
        $('input', this).each(function (i, v) {
            var modelName = $(this).attr('name').split('[')[0];
            if ($(this).attr('name').indexOf("[") >= 0) {
                var propertyName = $(this).attr('name').split('.')[1];
                $(this).attr('name', modelName + '[' + index + '].' + propertyName);
            }
            else {
                $(this).attr('name', 'models[' + index + '].' + modelName);
            }
        });
    });

    //$('.chkPK', '#' + formName).each(function (index, value) {
    //    var PK = $(this);
    //    var RV = $(this).next();
    //    $(PK).attr('name', $(PK).attr('name').split('.')[1]);
    //    $(RV).attr('name', $(RV).attr('name').split('.')[1]);
    //    $(PK).attr('name', 'models[' + index + '].' + $(PK).attr('name'));
    //    $(RV).attr('name', 'models[' + index + '].' + $(RV).attr('name'));
    //});
}

function PostbackSubmit(Submitter) {
    var formName = $(Submitter).parents('form').attr('id');
    $('#' + formName).valid();

    var error = $('.input-validation-error', '#' + formName).length;

    if (error > 0) {
        $('.select2-selection__rendered').removeAttr('style');
        $('.input-validation-error:first-child').parent().popover({
            trigger: 'focus',
            placement: 'auto top',
            container: '#PopOverContainer',
            content: ''
        });
        return;
    }
    else {
        $('#' + formName).submit();
    }
}

function SubmitForm(Submitter) {
    var formName = $(Submitter).parents('form').attr('id');
    if (formName == undefined) {
        formName = $(Submitter).attr('form');
    }
    //to prevent second submit when there is a validation error, No logical justification found for this case, Ayas, Amer and Jawad. 
    $('#' + formName).unbind('submit').submit(function (event) {
        event.preventDefault();
    });

    $('#' + formName).valid();
    var error = $('.input-validation-error', '#' + formName).length;

    if (error > 0) {
        $('.select2-selection__rendered').removeAttr('style');

        $('.input-validation-error:first-child').parent().popover({
            trigger: 'click',
            placement: 'auto top',
            container: '#PopOverContainer',
            content: ''
        });
        return;
    }

    var SubmitType = "";
    SubmitType = $(Submitter).attr('data-submittype');
    if (SubmitType == undefined) {
        SubmitType = '';
    }

    Sequntialize(formName);

    var form = $('#' + formName);
    //clear any concurrency highlighting style.
    $('.input-concurrency-error', '#' + formName).removeClass('input-concurrency-error');

    if ($(form).valid()) {

        var formData = form.serialize();

        $(form).unbind('submit').submit(function (event) {
            event.preventDefault();
            if (!FlashEmptyControls()) {
                return;
            }
            if ($('.modal-content').is(':visible')) {
                DisableForm('.modal-content')//if submit came from a popup modal
                $('.ModalCancel').after(createLoadingImage());//if submit came from a popup modal
            }
            else {
                DisableForm('#' + formName);
                $('.SLAME:last', '#' + formName).after(createLoadingImage());
            }
            var actionValue = "";

            if (form.attr('action').includes("DataTable")) { actionValue = form.attr('action').replace("DataTable", "DataTable" + SubmitType); }
            else { actionValue = form.attr('action') + SubmitType; }

            $.ajax({
                method: 'post',
                url: actionValue,
                data: formData,
                success: function (JsonResult, status, xhr) {
                    $(form).unbind('submit');
                    console.log(JsonResult);
                    if (JsonResult.Notify != null) {

                        ProcessPrimaryKey(JsonResult, form);
                        ProcessRowVersion(JsonResult, form);
                        ProcessPartialViews(JsonResult, form);
                        ProcessDataTable(JsonResult, form);
                        ProcessAffectedRecords(JsonResult, form);
                        ProcessDeleteClientSide(JsonResult, form);
                        EnableForm('#' + formName);
                        $(form).trigger('reinitialize.areYouSure');
                        ReadyFor(JsonResult, form);
                        ProcessUIButtons(JsonResult);
                        ProcessConcurrency(JsonResult, form);
                        ProcessCallBackFunctions(JsonResult, form);
                        ProcessNotify(JsonResult);
                    }
                    else //The result is partial view markups (when error)
                    {
                        form.replaceWith(JsonResult);
                        InitPlugins('#' + formName);
                    }
                },
                error: function (ex) {
                    Notify(NotificationType.Error, "There was an error submitting the record");
                    console.log("ex", ex);
                    console.log('Error: ' + ex.responseText);
                    EnableForm('#' + formName);
                }
            });

        });
        //This to avoid having critical action to be executed by Enter & having multiple Submit button on form-footer!
        if ($(Submitter).attr('type') == 'button') {
            $(form).submit();
        }
    }
    else {
        return false;
    }
}
//function SubmitForm(Submitter) {
//    var formName = $(Submitter).parents('form').attr('id');
//    if (formName != 'BillForm') {
//        if (formName == undefined) {
//            formName = $(Submitter).attr('form');
//        }
//        //to prevent second submit when there is a validation error, No logical justification found for this case, Ayas, Amer and Jawad. 
//        $('#' + formName).unbind('submit').submit(function (event) {
//            event.preventDefault();
//        });

//        $('#' + formName).valid();
//        
//        var error = $('.input-validation-error', '#' + formName).length;

//        if (error > 0) {
//            $('.select2-selection__rendered').removeAttr('style');

//            $('.input-validation-error:first-child').parent().popover({
//                trigger: 'click',
//                placement: 'auto top',
//                container: '#PopOverContainer',
//                content: ''
//            });
//            return;
//        }

//        var SubmitType = "";
//        SubmitType = $(Submitter).attr('data-submittype');
//        if (SubmitType == undefined) {
//            SubmitType = '';
//        }

//        Sequntialize(formName);

//        var form = $('#' + formName);
//        //clear any concurrency highlighting style.
//        $('.input-concurrency-error', '#' + formName).removeClass('input-concurrency-error');

//        if ($(form).valid()) {

//            var formData = form.serialize();
//            $(form).unbind('submit').submit(function (event) {
//                event.preventDefault();
//                if (!FlashEmptyControls()) {
//                    return;
//                }
//                if ($('.modal-content').is(':visible')) {
//                    DisableForm('.modal-content')//if submit came from a popup modal
//                    $('.ModalCancel').after(createLoadingImage());//if submit came from a popup modal
//                }
//                else {
//                    DisableForm('#' + formName);
//                    $('.SLAME:last', '#' + formName).after(createLoadingImage());
//                }
//                var actionValue = "";

//                if (form.attr('action').includes("DataTable")) { actionValue = form.attr('action').replace("DataTable", "DataTable" + SubmitType); }
//                else { actionValue = form.attr('action') + SubmitType; }

//                $.ajax({
//                    method: 'post',
//                    url: actionValue,
//                    data: formData,
//                    success: function (JsonResult, status, xhr) {
//                        $(form).unbind('submit');
//                        console.log(JsonResult);
//                        if (JsonResult.Notify != null) {
//                            ProcessPrimaryKey(JsonResult, form);
//                            ProcessRowVersion(JsonResult, form);
//                            ProcessPartialViews(JsonResult, form);
//                            ProcessDataTable(JsonResult, form);
//                            ProcessAffectedRecords(JsonResult, form);
//                            ProcessDeleteClientSide(JsonResult, form);
//                            EnableForm('#' + formName);
//                            $(form).trigger('reinitialize.areYouSure');
//                            ReadyFor(JsonResult, form);
//                            ProcessUIButtons(JsonResult);
//                            ProcessConcurrency(JsonResult, form);
//                            ProcessCallBackFunctions(JsonResult, form);
//                            ProcessNotify(JsonResult);
//                        }
//                        else //The result is partial view markups (when error)
//                        {
//                            form.replaceWith(JsonResult);
//                            InitPlugins('#' + formName);
//                        }
//                    },
//                    error: function (ex) {
//                        Notify(NotificationType.Error, "There was an error submitting the record");
//                        console.log("ex", ex);
//                        console.log('Error: ' + ex.responseText);
//                        EnableForm('#' + formName);
//                    }
//                });

//            });
//            //This to avoid having critical action to be executed by Enter & having multiple Submit button on form-footer!
//            if ($(Submitter).attr('type') == 'button') {
//                $(form).submit();
//            }
//        }
//        else {
//            return false;
//        }
//    }
//    else {
//        $('#BillForm').submit(function (e) {
//            e.preventDefault();
//            var formData = new FormData(this);
//            $.ajax({
//                url: '/TBS/BillManagement/BillCreate',
//                type: 'POST',
//                data: formData,
//                cache: false,
//                contentType: false,
//                processData: false,
//                success: function (JsonResult) {
//                    ProcessNotify(JsonResult);
//                },
//                error: function (jqXHR, textStatus, errorThrown) {
//                    //do your own thing
//                    alert("fail");
//                }
//            });
//        });
//    }
//}

function ProcessUIButtons(JsonResult) {
    if (JsonResult.UIButtons != null) {
        $.each(JsonResult.UIButtons, function (index, value) {
            $('#' + JsonResult.UIButtons[index].Container).find('.ActionControl').remove();
        });
        $.each(JsonResult.UIButtons, function (index, value) {
            $('#' + JsonResult.UIButtons[index].Container).prepend(JsonResult.UIButtons[index].Button);
        });
    }
}

function ReadyFor(JsonResult, form) {
    if (JsonResult.NextPageMode != null) {
        // $('.form-footer', form).find('.ActionControl').remove(); look above 7 lines
        if (JsonResult.NextPageMode == 'Restore') {
            ReadOnlyForm($(form).attr('id'));
            $(form).find(':submit').prop('disabled', true);
        }
    }
}



function ProcessConcurrency(JsonResult, form) {
    var formName = $(form).attr('id');
    var changedFields = 0;
    //Remove any previous message
    $(form).find('.alert-warning').remove();
    if (JsonResult.Concurrency) {
        if (JsonResult.dbModel.Active) {
            $.each(JsonResult.dbModel, function (control, value) {
                var formControl = form.find('#' + control);
                if (value == null) value = "";

                console.log('db = ' + value);
                console.log('cl = ' + $(formControl).val());

                if (value != $(formControl).val() && formControl.attr('type') != 'hidden' && !formControl.is(":hidden") && !formControl.is('select')) {
                    formControl.next('.concurrencybox').remove();
                    formControl.after('<span  data-form="' + formName + '" data-current="' + $(formControl).val() + '" data-db="' + value + '" data-showing="user" data-for="' + formControl.attr('id') + '" class="fa fa-database concurrencybox"></span>');
                    formControl.addClass('input-concurrency-error');
                    changedFields++;
                }
                else if (value.toString() != $(formControl).val() && $(formControl).is('select')) {
                    formControl.next().next('.concurrencybox').remove();
                    formControl.next().after('<span  data-form="' + formName + '" data-current="' + $(formControl).val() + '" data-db="' + value + '" data-showing="user" data-for="' + formControl.attr('id') + '" class="fa fa-database concurrencybox"></span>');
                    formControl.next().find('.select2-selection__rendered').addClass('input-concurrency-error');
                    changedFields++
                }
            });
            if (changedFields > 0) {
                $((form).find('input[type=hidden]:first-child')).after(ShowConcurrencyMessage());
            }
            else {
                $((form).find('input[type=hidden]:first-child')).after(ShowConcurrencySameChanges());
            }
        }
        else {
            //record deleted case
            $((form).find('input[type=hidden]:first-child')).after(ShowConcurrencyDeleted());
        }
    }
    //clear any old concurrency injection
    else {
        $(form).find('.alert-warning').remove();
        $(form).find('.concurrencybox').remove();
    }
}

function ProcessNotify(JsonResult) {
    Notify(JsonResult.Notify.Type, JsonResult.Notify.Message);
    if (JsonResult.Notify.Type == NotificationType.Success) {
        $('.modal').modal('hide');
    }
}

function ProcessPrimaryKey(JsonResult, form) {

    if (JsonResult.PrimaryKey != null) {
        $(form).find('#' + JsonResult.PrimaryKey.ControlID).val(JsonResult.PrimaryKey.Value);
    }
    if (JsonResult.PrimaryKeys != null) {
        $.each(JsonResult.PrimaryKeys, function (index, value) {
            $(form).find('#' + JsonResult.PrimaryKeys[index].ControlID).val(JsonResult.PrimaryKeys[index].Value);
        });
    }
}


//function ProcessRedirectTo(JsonResult) {
//    if (JsonResult.RedirectTo != null) {
//        window.localStorage.setItem('MessageAfterPostback', JsonResult.Message);
//        window.location.replace(JsonResult.RedirectTo);
//    }
//}

function ProcessRowVersion(JsonResult, form) {
    if (JsonResult.RowVersions != null) {
        $.each(JsonResult.RowVersions, function (index, value) {

            $(form).find('#' + JsonResult.RowVersions[index].ControlID).val(JsonResult.RowVersions[index].Value);
        });
    }
}

function ProcessAffectedRecords(JsonResult, form) {
    if (JsonResult.AffectedRecordsGuids != null) {
        $.each(JsonResult.AffectedRecordsGuids, function (index, value) {
            $('#' + JsonResult.DataTable + 'Form').find('*[data-id="' + value + '"]').parent().remove();
        });
    }
}

function ProcessDeleteClientSide(JsonResult, form) {
    if (JsonResult.DeleteClientSide != null) {
        $.each(JsonResult.DeleteClientSide, function (index, value) {
            $(form).find('*[data-id="' + value + '"]').remove();
        });
    }
}

function ProcessUpdateClientSide(JsonResult, form, SubmitType) {
    if (JsonResult.dbModel != null) {
        if (JsonResult.dbModel.Active) {
            $.each(JsonResult.dbModel, function (control, value) {
                var formControl = form.find('#' + control);
                if (value == null) value = "";

                console.log('db = ' + value);
                console.log('cl = ' + $(formControl).val());


                if (value != $(formControl).val() && formControl.attr('type') != 'hidden' && !formControl.is(":hidden") && !formControl.is('select')) {
                    formControl.next('.concurrencybox').remove();
                    formControl.after('<span  data-form="' + formName + '" data-current="' + $(formControl).val() + '" data-db="' + value + '" data-showing="user" data-for="' + formControl.attr('id') + '" class="fa fa-database concurrencybox"></span>');
                    formControl.addClass('input-concurrency-error');
                    changedFields++;
                }
                else if (value.toString() != $(formControl).val() && $(formControl).is('select')) {
                    formControl.next().next('.concurrencybox').remove();
                    formControl.next().after('<span  data-form="' + formName + '" data-current="' + $(formControl).val() + '" data-db="' + value + '" data-showing="user" data-for="' + formControl.attr('id') + '" class="fa fa-database concurrencybox"></span>');
                    formControl.next().find('.select2-selection__rendered').addClass('input-concurrency-error');
                    changedFields++
                }
            });
            if (changedFields > 0) {
                $((form).find('input[type=hidden]:first-child')).after(ShowConcurrencyMessage());
            }
            else {
                $((form).find('input[type=hidden]:first-child')).after(ShowConcurrencySameChanges());
            }
        }
        else {
            //record deleted case
            $((form).find('input[type=hidden]:first-child')).after(ShowConcurrencyDeleted());
        }
    }
}

function ProcessDataTable(JsonResult, form) {
    if (JsonResult.DataTable != null)//This is a submit on model and needs to refresh a DataTable
    {
        DataTableRefresh(JsonResult.DataTable);
    }
}

function ProcessPartialViews(JsonResult, form) {
    if (JsonResult.PartialViews != null) {
        $.each(JsonResult.PartialViews, function (index, value) {
            if (JsonResult.PartialViews[index].Operation == 'Unload' || JsonResult.PartialViews[index].Operation == 'Delete') {
                $('#' + JsonResult.PartialViews[index].Container).empty();
            }
            else {
                //var Active = "";
                //if (JsonResult.PartialViews[index].Active != null) {
                //    //This to overcome the create new record where there is no factors, so we need to load the FormControls Partial View
                //    Active = '&Active=' + JsonResult.PartialViews[index].Active;
                //}
                $('#' + JsonResult.PartialViews[index].Container).load(JsonResult.PartialViews[index].Url + '?PK=' + JsonResult.PartialViews[index].PK); //DO NOT CHANGE (?PK=JsonResult.PartialViews[index].PK) TO (/JsonResult.PartialViews[index].PK) ---> DO IF YOU DARE :)
                $(form).find('.PK').val(JsonResult.PartialViews[index].PK);

            }
        });
        window.history.pushState("", "", window.location.pathname.replace("/Create", "/Update/" + JsonResult.PartialViews[0].PK));
    }
}

function ProcessCallBackFunctions(JsonResult, form) {
    debugger;
    //alert(JsonResult.CallbackFunction);
    if (JsonResult.CallbackFunction != null) {
        eval(JsonResult.CallbackFunction);
    } else {
        if ($('#FormModal').length > 0) {
            $('#FormModal').modal('hide');
        }
        if ($('#ConfirmModal').length > 0) {
            $('#ConfirmModal').modal('hide');
        }
        //

    }
}

function EnableForm(Container) {
    $(Container + ' :input').each(function () {
        if ($(this).prop('disabled')) {

        }
        if (!$(this).hasClass('MustReadOnly')) {
            $(this).removeAttr('disabled');
            $(this).removeAttr('readonly');
        }
    });
    $(Container).removeClass('ReadOnlyForm');
    if ($('#imgLoading').length > 0) {
        $('#imgLoading').remove();
    }
}

function DisableForm(form) {
    $(form + ' :input:not(input[type=hidden])').each(function () {
        $(this).attr('disabled', 'disabled');
    });
}

function EditSection(Handler, LoadURL, LoadContainer) {
    if ($(Handler).hasClass('Edit-Link')) {
        $(Handler).addClass('Edit-Link-Loading');
    }
    else { //Could be div
        $(Handler).parent().find('a.Edit-Link').addClass('Edit-Link-Loading');
    }
    $('#' + LoadContainer).load(LoadURL, function () {
        InitPlugins($('#' + LoadContainer));
    });
}

function CancelEditSection(Link, URL, LoadContainer) {
    if (Link != null) {
        $(Link).addClass('Edit-Link-Loading');
    }
    $('#' + LoadContainer).load(URL);
}

function LoadUrl(LoadContainer, _LoadURL, callBackFunc) {
    if (LoadContainer != '') {
        $('#' + LoadContainer).load(_LoadURL, function () {
            InitPlugins($('#' + LoadContainer));
            if (callBackFunc != null) { callBackFunc(); }
        });
    } else {
        window.location.replace(_LoadURL);
    }
}

//function EditProfileSection(Link, URL, PanelContainer) {
//    $(Link).addClass('Edit-Link-Loading');
//    $('#' + PanelContainer).load(URL);
//}

//function CancelEditProfileSection(Link, URL, PanelContainer) {
//    $(Link).addClass('Edit-Link-Loading');
//    $('#' + PanelContainer).load(URL);
//}

function createCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

function TogglePageWidth() {
    if (window.localStorage.getItem('fullScreen') == 'true') //Means current view is fullscreen
    {
        $(document.querySelector('link[href*="FullScreen.css"]')).remove();
        window.localStorage.setItem('fullScreen', false);
    }
    else {
        window.localStorage.setItem('fullScreen', true);
    }

    location.reload();
}

function PreventSubmitWhenNoChange(container) {
    $('form').on('dirty.areYouSure', function () {
        // Enable save button only as the form is dirty.
        $(this).find(':submit:visible:not(.IgnoreAYS)').removeAttr('disabled');
    });
    $('form').on('clean.areYouSure', function () {
        // Form is clean so nothing to save - disable the save button.
        if (!$('.concurrencybox', container).length > 0) {
            $(this).find(':submit:visible:not(.IgnoreAYS)').attr('disabled', 'disabled');
        }
    });
}

function ToEngDigits(str) {
    return str.replace(/[١٢٣٤٥٦٧٨٩٠]/g, function (d) {
        return d.charCodeAt(0) - 1632;
    });
}

function Notify(type, text) {
    if (text == null) {
        return false;
    }

    var bubble1 = "";
    var bubble2 = "";
    if (type == NotificationType.Success) {
        bubble1 = "red";
        bubble2 = "yellow";
    }
    else if (type == NotificationType.Error) {
        bubble1 = "green";
        bubble2 = "yellow";
    }
    else if (type == NotificationType.Warning) {
        bubble1 = "white";
        bubble2 = "blue";
    }
    else if (type == NotificationType.Info) {
        bubble1 = "pink";
        bubble2 = "brown";
    }
    else if (type == NotificationType.Default) {
        bubble1 = "pink";
        bubble2 = "brown";
    }
    var xPosOpen;
    //var xPosClose;
    if (isRTL()) {
        xPosOpen = { '-500': 0, delay: 0, duration: 500, easing: 'elastic.out' }
        xPosClose = { 0: '-500', delay: 10, duration: 500, easing: 'cubic.out' }
    } else {
        xPosOpen = { '500': 0, delay: 0, duration: 500, easing: 'elastic.out' }
        xPosClose = { 0: 500, delay: 10, duration: 500, easing: 'cubic.out' }
    }

    new Noty({
        type: type,
        theme: 'metroui',
        layout: 'topRight',
        timeout: text.length * 100,
        progressBar: true,
        closeWith: ['click', 'button'],
        text: text,
        animation: {
            open: function (promise) {
                var n = this;
                var Timeline = new mojs.Timeline();
                var body = new mojs.Html({
                    el: n.barDom,
                    //x: { '-500' : 0, delay: 0, duration: 500, easing: 'elastic.out' },
                    x: xPosOpen,
                    isForce3d: true,
                    onComplete: function () {
                        promise(function (resolve) {
                            resolve();
                        })
                    }
                });

                var parent = new mojs.Shape({
                    parent: n.barDom,
                    width: 200,
                    height: n.barDom.getBoundingClientRect().height,
                    radius: 0,
                    //x: { [150]: -150 },
                    duration: 1.2 * 500,
                    isShowStart: true
                });

                n.barDom.style['overflow'] = 'visible';
                parent.el.style['overflow'] = 'hidden';

                var burst = new mojs.Burst({
                    parent: parent.el,
                    count: 10,
                    top: n.barDom.getBoundingClientRect().height + 75,
                    degree: 90,
                    radius: 75,
                    //angle: { [-90]: 40 },
                    children: {
                        fill: bubble1,
                        delay: 'stagger(500, -50)',
                        radius: 'rand(8, 25)',
                        direction: -1,
                        isSwirl: true
                    }
                });

                var fadeBurst = new mojs.Burst({
                    parent: parent.el,
                    count: 2,
                    degree: 0,
                    angle: 75,
                    radius: { 0: 100 },
                    top: '90%',
                    children: {
                        fill: bubble2,
                        pathScale: [.65, 1],
                        radius: 'rand(12, 15)',
                        direction: [-1, 1],
                        delay: .8 * 500,
                        isSwirl: true
                    }
                });

                Timeline.add(body, burst, fadeBurst, parent);
                Timeline.play();
            },
            close: function (promise) {
                var n = this;
                new mojs.Html({
                    el: n.barDom,
                    x: xPosClose,
                    isForce3d: true,
                    onComplete: function () {
                        promise(function (resolve) {
                            resolve();
                        })
                    }
                }).play();
            }
        }
    }).show();
}

//Take a look for the three functions below. they are look the same => something wrong. Ayas
function ShowConcurrencyMessage() {
    var msg =
        '<div class="WarningSummary alert alert-warning alert-dismissable fade in">' +
        '<a class="close" data-dismiss="alert" aria-label="close">×</a>' +
        '<h4>' + ConcurrencyMessage.Title + '</h4>' +
        '<span class="warning">' + ConcurrencyMessage.Message + '</span>' +
        '</div>';
    return msg;
}

function ShowConcurrencySameChanges() {
    var msg =
        '<div class="WarningSummary alert alert-warning alert-dismissable fade in">' +
        '<a class="close" data-dismiss="alert" aria-label="close">×</a>' +
        '<h4>' + ConcurrencyMessage.Title + '</h4>' +
        '<span class="warning">' + ConcurrencyMessage.SameChangesMessage + '</span>' +
        '</div>';
    return msg;
}

function ShowConcurrencyDeleted() {
    var msg =
        '<div class="WarningSummary alert alert-warning alert-dismissable fade in">' +
        '<a class="close" data-dismiss="alert" aria-label="close">×</a>' +
        '<h4>' + ConcurrencyMessage.Title + '</h4>' +
        '<span class="warning">' + ConcurrencyMessage.RecordDeletedMessage + '</span>' +
        '</div>';
    return msg;
}

function isRTL() {
    if ($('body').hasClass('rtl')) {
        return true
    }
    else {
        return false
    }
}

ToBase64 = function (u8) {
    return btoa(String.fromCharCode.apply(null, u8));
}

function ConvertToBoolean(string) {
    switch (string.toLowerCase().trim()) {
        case "true": case "yes": case "1": return true;
        case "false": case "no": case "0": case null: return false;
        default: return string;
    }
}

function StrengthText(value) {
    if (value < 60) return '';
    else if (value == 60) return 'moderate';
    else if (value >= 90) return 'strong';
}

function StrengthColor(value) {
    if (value < 60) return 'red';
    else if (value == 60) return 'orange';
    else if (value >= 90) return 'green';
}

function RemoveProfilePhoto(url) {
    $('#imgProfileHeaderPhoto').attr('src', url + "?" + Math.random().toString());
    $('#imgProfilePhoto').attr('src', url + "?" + Math.random().toString());
}

function RemoveOrganizationLogo(url) {
    $('#imgLogo').attr('src', url + "?" + Math.random().toString());
}

function RemoveOfficePhoto(url) {
    $('#imgOfficePhoto').attr('src', url + "?" + Math.random().toString());
}

function BroadcastValue(value, targetClass) {
    $('.' + targetClass + '').val(value);
}

function AccordionNodeLoad(PKs) {
    $('.panel-heading', '#accordion').unbind('click').on('click', function (e) {
        var target = $(this).attr('href');
        var loadUrl = $(this).attr('data-url');
        if (loadUrl.indexOf("?") >= 0) {
            loadUrl += "&";
        } else {
            loadUrl += "?&";
        }
        $.each(PKs, function (key, value) {
            loadUrl += key + "=" + value + "&";
        });
        var isExpanded = $(this).find('.panel-title').attr('aria-expanded');
        if (isExpanded != 'true') {
            setTimeout(function () {
                $('.panel-body', target).load(loadUrl, function () {
                    InitPlugins(this);
                });
            }, 500);
        }
    });
}

$(document).on('show.bs.collapse', '.AuditHandler', function () {
    var Handler = $(this).find('.panel-heading');
    Handler.addClass('ExpandedAuditHistory');
    var target = Handler.attr('href');
    $('.panel-body', target).html('Loading...');
    var loadUrl = Handler.attr('data-url');
    var isExpanded = Handler.find('.panel-title').attr('aria-expanded');
    if (isExpanded != 'true') {
        setTimeout(function () {
            $('.panel-body', target).load(loadUrl, function () { });
        }, 500);
    }
});

$(document).on('hide.bs.collapse', '.AuditHandler', function () {
    var Handler = $(this).find('.panel-heading');

    Handler.removeClass('ExpandedAuditHistory');
    var target = Handler.attr('href');
    $('.panel-body', target).empty();
});

function FindDivInContainer(Value, Container, divClass) {
    var panel = $('#' + Container);
    var counter = 0;
    panel.find('div.' + divClass).each(function (index, div) {
        var found = false;
        var regExp = new RegExp(Value, 'i');
        if (regExp.test($(div).text())) {
            found = true;
        }
        if (found == true) $(div).show(); else $(div).hide();
    });
}

function SetWorkingDaysValue(chkBox) {
    var _url = $(chkBox).attr('data-url');
    var _OrganizationInstanceGUID = $('#OrganizationInstanceGUID').val();
    var _ValueGUID = $(chkBox).val();
    var _Checked = chkBox.checked;
    $.ajax({
        type: "POST",
        url: _url,
        contentType: "application/json; charset=utf-8",
        data: "{OrganizationInstanceGUID: '" + _OrganizationInstanceGUID + "','ValueGUID': '" + _ValueGUID + "','Checked': " + _Checked + " }",
        error: function (status) { /*alert("Javascript Error: #5844789")*/ },
    });
};

$(document).on('keydown', function (e) {
    if (e.ctrlKey && e.which === 83) { // Check for the Ctrl key being pressed, and if the key = [S] (83)
        //alert("CTLR S");
        e.preventDefault();
        return false;
    }
    else if (e.ctrlKey && e.which === 65) { // Check for the Ctrl key being pressed, and if the key = [A] (83)

        e.preventDefault();
        $(".chkHead").iCheck('check');
        return false;
    }
    //
});

function ClearForm(class_name) {
    jQuery("." + class_name).find(':input').each(function () {
        switch (this.type) {
            case 'password':
            case 'text':
            case 'textarea':
            case 'file':
            case 'select-one':
            case 'select-multiple':
            case 'date':
            case 'number':
            case 'tel':
            case 'email':
                jQuery(this).val('');
                jQuery(this).val('').trigger('change');
                break;
            case 'checkbox':
            case 'radio':
                this.checked = false;
                break;
        }
    });
}

function ShowOtherFields(DropDownList, WhenValues, OtherControlsClass) {

    var ddlValue = $(DropDownList).val();
    var OneOfValues = false;
    $.each(WhenValues, function (i, v) {
        if (v === ddlValue) {
            OneOfValues = true;
            return false;
        }
    });
    if (OneOfValues) {
        $('.' + OtherControlsClass).addClass('hidden').find('select').val('').trigger('change');
    }
    else {
        $('.' + OtherControlsClass).removeClass('hidden');
    }
}

function copyPageID(Text) {
    var $temp = $("<input>");
    $("body").append($temp);
    $temp.val(Text).select();
    document.execCommand("copy");
    $temp.remove();
}

$(document).on('click', '.copyPageID', function () {
    var span = $(this).prev();
    copyPageID('"' + $(this).attr('data-value') + '"');
    $(span).animate({ backgroundColor: "#87ca87" }, 500)
        .animate({ backgroundColor: "transparent" }, 500);

});
//<button onclick="copyToClipboard('#p1')">Copy TEXT 1</button>

function GetUserDetails(ID, Container) {
    $.ajax({
        url: '/Permissions/GetUserDetails/',
        data: { ID: ID },
        type: 'GET',
        success: function (Result) {
            $('.lblFullName', Container).text(Result.FullName);
            $('.lblEmailAddress', Container).text(Result.EmailAddress);
            $('.lblOrganization', Container).text(Result.Organization);
            $('.lblJobTitle', Container).text(Result.JobTitle);
            $('.lblOperation', Container).text(Result.Operation);
            $('.userProfilePhotoInPermissions', Container).attr('src', Result.ProfilePhoto);
        }
    });
}

function GetUserDetailsForProfileToProfile(ID, Container) {
    $.ajax({
        url: '/Permissions/GetUserDetailsForProfileToProfile/',
        data: { ID: ID },
        type: 'GET',
        success: function (Result) {
            $('.lblFullName', Container).text(Result.User.FullName);
            $('.lblEmailAddress', Container).text(Result.User.EmailAddress);
            $('.lblOrganization', Container).text(Result.User.Organization);
            $('.lblJobTitle', Container).text(Result.User.JobTitle);
            $('.lblOperation', Container).text(Result.User.Operation);
            $('.userProfilePhotoInPermissions', Container).attr('src', Result.User.ProfilePhoto);

            $("#SourceProfileGUID").empty();
            $("#SourceProfileGUID").append('<option></option');
            $.each(Result.OldProfiles, function (index, obj) {
                $("#SourceProfileGUID").append('<option value="' + obj.Value + '">' + obj.Text + '</option>');
            });

            $("#DestinationProfileGUID").empty();
            $.each(Result.CurrentProfile, function (index, obj) {
                $("#DestinationProfileGUID").append('<option value="' + obj.Value + '">' + obj.Text + '</option>');
            });

        }
    });
}

function TogglePermissionFilter(className) {
    //if ($('#PermissionContainer').is(':visible')) {
    ToggleFilter(className);
    $('.lblFullName').text('');
    $('.lblEmailAddress').text('Please select a user from above');
    $('.lblOrganization').text('');
    $('.lblJobTitle').text('');
    $('.lblOperation').text('');
    $('#userProfileImage').attr('src', '/Assets/Images/img.png');
    $("#btnShowFilter").toggle();
    //$("#btnCloseFilter").toggle();
    //}
}

function ClearTreeForm(className) {
    ClearForm(className);
    $('#userProfileImage').attr('src', '/Assets/Images/img.png');
    $('.userInfoInPermissions').text('');
    $('#PermissionContainer').hide();
}

function toggleMoreFilter(elem) {
    $('.toggleRow').toggle();
    if ($('.toggleRow').is(':visible')) {
        $(elem).text('Less');
    } else {
        $(elem).text('More');
    }
}

function FlashEmptyControls() {
    var error = false;
    var message = "Required fields are missing";
    $('.FlashOnEmptySelect2').each(function () {
        if ($(this).val() == null || $(this).val() == '') {
            for (var i = 0; i < 2; i++) {
                $("#select2-" + $(this).attr('id') + "-container")
                    .animate({ backgroundColor: "#ef8888" }, 250)
                    .animate({ backgroundColor: "transparent" }, 250);
            }
            error = true;
        }
    });
    if (error) {
        Notify(NotificationType.Error, message);
        return false;
    }
    return true;
}

function BuildTree() {
    if (!FlashEmptyControls()) {
        return;
    }
    $('#TreeLevelsSlider').bootstrapSlider('setValue', 1);
    $('.slider').hide();
    $("#PermissionContainer").show();
    $("#PermissionContainer *").prop('disabled', true);
    $("#btnCloseFilter").show();
    $('#PermissionsTree').jstree("destroy");
    $('#PermissionsTree').html('').append('<span class="loadingSpan" style="margin-left:4px;margin-top:2px;">Fetching Tree nodes... Please wait</span>');
    var FilterModel = {
        UserProfileGUID: $('#UserProfileGUID').val(),
        ApplicationGUID: $('#ApplicationGUID').val(),
        CategoryGUID: $('#CategoryGUID').val(),
        ActionGUID: $('#ActionGUID').val(),
        UNHCRBureauGUID: $('#UNHCRBureauGUID').val(),
        OperationGUIDs: $('#OperationGUIDs').val(),//.toString().replace(",", "*"),
        OrganizationGUIDs: $('#OrganizationGUIDs').val(),
        GovernorateGUIDs: $('#GovernorateGUIDs').val(),
        CountryGUIDs: $('#CountryGUIDs').val()
    };
    $.ajax({
        url: '/Permissions/GetRowMaterials/',
        type: 'GET',
        data: FilterModel,
        contentType: "application/json",
        traditional: true,
        success: function (RowMaterials) {
            console.log(RowMaterials);
            $('.loadingSpan').text('Building the tree...');
            setTimeout(function () {
                UserPermissions = RowMaterials[4];
                TotalActions = 0;
                ActionsCartesianProduct(RowMaterials, 0, null)
                $('#PermissionsTree').html(strHtml);
                $('#lblNodesTotal').text($('li.Action', '#PermissionsTree').length + ' Actions Found' + ' And You Have: ' + TotalActions + ' of Them');
                InitTree('PermissionsTree');
                strHtml = '';
                $("#PermissionContainer *").prop('disabled', false);
                $('.slider').css('margin-left', '15px').show();
            }, 10);
        }
    });
}

function InitTree(Element) {

    $('#' + Element).jstree({
        core: {
            expand_selected_onload: false,
            themes: { "icons": false },
            animation: 10,
            strings: { 'Loading ...': 'Building the tree' },
            dblclick_toggle: false,
        },
        search: {
            search_leaves_only: false,//treeSettings.SelectOnlyLeafes
            //search_callback: searchContainsAll,//search-function that checks for multiple search-terms (every term has to occur)
            show_only_matches: false,//filtering
            show_only_matches_children: false
        },
        plugins: ["checkbox", "Xsort", "search"],
    }).on('open_all.jstree', function () {
        $('#btnExpandCollapseTree').removeClass('ExpandSpining fa fa-spin fa-15x');
    });
}

function ExpandCollapseTree(Button, Tree) {

    if ($(Button).hasClass('expandedTree')) {
        $(Tree).jstree("close_all");
        $('#TreeLevelsSlider').bootstrapSlider('setValue', 1);
        $(Button).removeClass('expandedTree');
    }
    else {
        $(Button).addClass('ExpandSpining fa fa-spin fa-15x');
        setTimeout(function () {
            $(Tree).jstree("open_all");
            $('#TreeLevelsSlider').bootstrapSlider('setValue', 7);
            $(Button).addClass('expandedTree');
        }, 1);
    }
}

function SubmitPermissions() { //Submit
    $("#PermissionContainer *").prop('disabled', true);
    $('.SLAME').after(createLoadingImage());
    var start = new Date().getTime();
    var model = [];
    var changedNodes = [];
    var Nodes = $('#PermissionsTree').jstree(true).get_json('#', { flat: true });
    $.each(Nodes, function (i, v) {
        var t = {};
        if (v.li_attr.actionguid != null) {
            if (v['li_attr']['data-jstree'] != null && v['state']['selected'] == false) {
                t['actionguid'] = v.li_attr.actionguid;
                t['factorstoken'] = v.li_attr.factorstoken;
                t['status'] = 'False'
                model.push(t);
                changedNodes.push(v.id);
            }
            else if (v['li_attr']['data-jstree'] == null && v['state']['selected'] == true) {
                t['actionguid'] = v.li_attr.actionguid;
                t['factorstoken'] = v.li_attr.factorstoken;
                t['status'] = 'True'
                model.push(t);
                changedNodes.push(v.id);
            }
        }
    });
    if (model.length > 0) {
        $.ajax({
            url: '/Permissions/PermissionsSubmit/',
            data: { SubmittedPermissionsList: model, TargetedUserProfileGUID: $('#UserProfileGUID').val(), TargetedApplicationGUID: $('#ApplicationGUID').val() },
            type: 'POST',
            success: function (jSON) {
                //simulate reloading the tree
                $.each(changedNodes, function (i, v) {
                    if ($('#PermissionsTree').jstree(true).get_node(v).data.jstree != null) {
                        $('#PermissionsTree').jstree(true).get_node(v)['data']['data-jstree'] = null;
                        $('#PermissionsTree').jstree(true).get_node(v)['data']['jstree'] = null;
                        $('#PermissionsTree').jstree(true).get_node(v)['li_attr']['data-jstree'] = null;
                        $('#PermissionsTree').jstree(true).get_node(v)['state']['selected'] = false;
                    }
                    else {
                        $('#PermissionsTree').jstree(true).get_node(v)['data']['data-jstree'] = '{ "opened": false, "selected": true }';
                        $('#PermissionsTree').jstree(true).get_node(v)['data']['jstree'] = '{ "opened": false, "selected": true }';
                        $('#PermissionsTree').jstree(true).get_node(v)['li_attr']['data-jstree'] = '{ "opened": false, "selected": true }';
                        $('#PermissionsTree').jstree(true).get_node(v)['state']['selected'] = true;
                    }
                });
                var end = new Date().getTime();
                var time = end - start;
                console.log('Execution time took: ' + time + ' Miliseconds for ' + jSON.Result + ' records');
                Notify(NotificationType.Success, "Permissions Saved Successfully.");
                $("#PermissionContainer *").prop('disabled', false);
                $('#imgLoading').remove();
            }
        });
    }
    else {
        Notify(NotificationType.Info, 'It seems you did not change anything, there is no need to submit!');
        $("#PermissionContainer *").prop('disabled', false);
        $('#imgLoading').remove();
    }
}

$(document).on('keyup', '#search_field', function () {
    $("#PermissionsTree").jstree("search", $(this).val());
});

var TotalActions = 0;
var ParentsArray = [];
var PurposeArray = [];
var Mother = [];
var DNA = [];
var strHtml = "";
var CurrentAction = "";
var strChecked = " data-jstree='{\"opened\":true,\"selected\":true}' ";
var UserPermissions = [];

function ActionsCartesianProduct(RowMaterials, Index, PID) {
    var Current = Enumerable.From(RowMaterials[Index]).Where(function (x) { return x.ParentID == PID || PID == null }).Select(function (x) { return x }).ToArray();
    strHtml += '<ul>';
    $.each(Current, function (i, v) {
        Mother = [];
        CurrentAction = v["ID"];
        if (Index == 1) { // 1 means category level
            DNA = v["FactorsArray"];
        }
        if (Index == 2) { //2 means action level
            if (DNA.length == 0) { // mean this action has no factors and should has li_attr
                var HasAction = Enumerable.From(UserPermissions).Where(function (x) { return x.ActionGUID == CurrentAction }).Count() > 0;
                if (HasAction) {
                    strHtml += '<li' + strChecked + ' class="Action" actionguid = "' + CurrentAction + '">' + v["Text"];
                    TotalActions++;
                }
                else {
                    strHtml += '<li class="Action" actionguid = "' + CurrentAction + '">' + v["Text"];
                }
            }
            else {
                strHtml += '<li>' + v["Text"];
            }
            $.each(DNA, function (i, v) {
                Mother.push(Enumerable.From(RowMaterials[3]).Where(function (x) { return x.FactorTypeID == v["FactorTypeID"] }).Select(function (x) { return x }).ToArray());
            });
            ParentsArray = [];
            PurposeArray = [];
            FactorsCartesianProduct(Mother, 0);
            strHtml += '</li>';
        }
        else {
            strHtml += '<li>' + v["Text"];
        }
        ActionsCartesianProduct(RowMaterials, Index + 1, v["ID"]);
        strHtml += '</ul>';
    });
}

function FactorsCartesianProduct(Mother, Index) {
    if (Index < DNA.length) {
        var isLastLevel = (Index + 1 == DNA.length);
        var CSSClass = isLastLevel ? 'Action' : '';
        var PID = ConvertDependsOnArrayToParentID(DNA[Index]["DependsOn"]);
        //This code solve the Organization as first factor
        var Current = null;

        if (Index == 0) { // Ignore parent for the first factor (ParentID = something or Null is the same!)
            Current = Enumerable.From(Mother[Index]).Where(function (x) { return x }).Select(function (x) { return x }).ToArray();
        }
        else {
            Current = Enumerable.From(Mother[Index]).Where(function (x) { return x.ParentID == PID }).Select(function (x) { return x }).ToArray();
        }
        strHtml += '<ul>';
        $.each(Current, function (i, v) {
            ParentsArray[Index] = v["ID"];
            var csvFactorsToken = '';// isLastLevel ? ParentsArray.join(',') : ''; //take only purpose = true
            if (isLastLevel) {
                //Take only purpose = True
                csvFactorsToken = '';
                $.each(DNA, function (i, v) {
                    if (v['Purpose'] == true) {
                        PurposeArray.push(ParentsArray[i]);
                    }
                });
                csvFactorsToken = PurposeArray.join(',');
                PurposeArray = [];
            }

            var HasAction = Enumerable.From(UserPermissions).Where(function (x) { return x.ActionGUID == CurrentAction && x.FactorsToken == csvFactorsToken }).Count() > 0;
            if (isLastLevel) {
                if (HasAction && isLastLevel) {
                    strHtml += '<li' + strChecked + ' class="' + CSSClass + '" actionguid = "' + CurrentAction + '" FactorsToken = "' + csvFactorsToken + '">' + v["Text"];
                    TotalActions++;
                }
                else {
                    strHtml += '<li class="' + CSSClass + '" actionguid = "' + CurrentAction + '" FactorsToken = "' + csvFactorsToken + '">' + v["Text"];
                }
            }
            else { //it is not an action it is just group
                strHtml += '<li class="' + CSSClass + '">' + v["Text"];
            }

            FactorsCartesianProduct(Mother, Index + 1);
            strHtml += '</li>';
        });
        strHtml += '</ul>';
    }
}

function ConvertDependsOnArrayToParentID(DepandsOnCSV) {
    if (DepandsOnCSV != null) {
        var Result = '';
        var temp = [];

        var arrayVal = DepandsOnCSV.split(',');
        $.each(arrayVal, function (i, v) {
            temp.push(ParentsArray[v]);
        });
        Result = temp.join(',');
        return Result.toUpperCase();//because SQL Casting GUID to UpperCase string :(
    }
    return null;
}

function BuildMyPermissionsTree(AppID, Div) {
    $('#uiApplicationName').text($(Div).text());
    $('.SelectedApplication').removeClass('SelectedApplication');
    $('#TreeStuff').hide();
    $('#LoadingPermissions').show();
    $('#PermissionsTree').html('').append('<span class="loadingSpan" style="margin-left:4px;margin-top:2px;">Fetching Tree nodes... Please wait</span>');

    $(Div).addClass('SelectedApplication');
    $.ajax({
        url: '/AccountSettings/GetMyPermissionsTree/',
        type: 'GET',
        data: { ApplicationGUID: AppID },

        contentType: "application/json",
        traditional: true,
        success: function (RowMaterials) {
            $('.loadingSpan', '#PermissionsTree').text('Building the tree...');
            setTimeout(function () {
                UserPermissions = RowMaterials[4];
                TotalActions = 0;
                if ($('#PermissionsTree').jstree(true) != false) {
                    $('#PermissionsTree').jstree().destroy();
                }
                $('#jsTreeMyPermissions').empty().append('<div data-key="TreeKey" id="PermissionsTree"></div>');
                ReadonlyActionsCartesianProduct(RowMaterials, 0, null)
                $('#PermissionsTree').html(strHtml);
                $('#lblNodesTotal').text($('li.Action', '#PermissionsTree').length + ' Actions Found' + ' And You Have: ' + TotalActions + ' of Them');

                InitMyPermissionsTree('PermissionsTree');
                strHtml = '';
                $("#PermissionContainer *").prop('disabled', false);
                $('.slider').css('margin-left', '15px').show();
            }, 10);
            $('#TreeStuff').show();
            $('#LoadingPermissions').hide();

        }
    });
}

function InitMyPermissionsTree(Element) {

    $('#' + Element).jstree({
        core: {
            expand_selected_onload: false,
            themes: { "icons": false },
            animation: 10,
            strings: { 'Loading ...': 'Building the tree' },
            dblclick_toggle: false,
        },
        search: {
            search_leaves_only: true,//treeSettings.SelectOnlyLeafes
            //search_callback: searchContainsAll,//search-function that checks for multiple search-terms (every term has to occur)
            show_only_matches: true,//filtering
            show_only_matches_children: false
        },
        plugins: ["search"],
    }).on('open_all.jstree', function () {
        $('#btnExpandCollapseTree').removeClass('ExpandSpining fa fa-spin fa-15x');
    });
}

function ReadonlyActionsCartesianProduct(RowMaterials, Index, PID) {
    var Current = Enumerable.From(RowMaterials[Index]).Where(function (x) { return x.ParentID == PID || PID == null }).Select(function (x) { return x }).ToArray();
    strHtml += '<ul>';
    $.each(Current, function (i, v) {
        Mother = [];
        CurrentAction = v["ID"];
        if (Index == 1) { // 1 means category level
            DNA = v["FactorsArray"];
        }
        if (Index == 2) { //2 means action level
            if (DNA.length == 0) { // mean this action has no factors and should has li_attr
                var HasAction = Enumerable.From(UserPermissions).Where(function (x) { return x.ActionGUID == CurrentAction }).Count() > 0;
                if (HasAction) {
                    strHtml += '<li' + strChecked + ' class="Action ActionOn" actionguid = "' + CurrentAction + '">' + v["Text"];
                    TotalActions++;
                }
                else {
                    strHtml += '<li class="Action ActionOff" actionguid = "' + CurrentAction + '">' + v["Text"];
                }
            }
            else {
                strHtml += '<li>' + v["Text"];
            }
            $.each(DNA, function (i, v) {
                Mother.push(Enumerable.From(RowMaterials[3]).Where(function (x) { return x.FactorTypeID == v["FactorTypeID"] }).Select(function (x) { return x }).ToArray());
            });
            ParentsArray = [];
            PurposeArray = [];
            ReadonlyFactorsCartesianProduct(Mother, 0);
            strHtml += '</li>';
        }
        else {
            strHtml += '<li>' + v["Text"];
        }
        ReadonlyActionsCartesianProduct(RowMaterials, Index + 1, v["ID"]);
        strHtml += '</ul>';
    });
}

function ReadonlyFactorsCartesianProduct(Mother, Index) {
    if (Index < DNA.length) {
        var isLastLevel = (Index + 1 == DNA.length);
        var CSSClass = isLastLevel ? 'Action' : '';
        var PID = ConvertDependsOnArrayToParentID(DNA[Index]["DependsOn"]);
        //This code solve the Organization as first factor
        var Current = null;

        if (Index == 0) { // Ignore parent for the first factor (ParentID = something or Null is the same!)
            Current = Enumerable.From(Mother[Index]).Where(function (x) { return x }).Select(function (x) { return x }).ToArray();
        }
        else {
            Current = Enumerable.From(Mother[Index]).Where(function (x) { return x.ParentID == PID }).Select(function (x) { return x }).ToArray();
        }
        strHtml += '<ul>';
        $.each(Current, function (i, v) {
            ParentsArray[Index] = v["ID"];
            var csvFactorsToken = '';// isLastLevel ? ParentsArray.join(',') : ''; //take only purpose = true
            if (isLastLevel) {
                //Take only purpose = True
                csvFactorsToken = '';
                $.each(DNA, function (i, v) {
                    if (v['Purpose'] == true) {
                        PurposeArray.push(ParentsArray[i]);
                    }
                });
                csvFactorsToken = PurposeArray.join(',');
                PurposeArray = [];
            }

            var HasAction = Enumerable.From(UserPermissions).Where(function (x) { return x.ActionGUID == CurrentAction && x.FactorsToken == csvFactorsToken }).Count() > 0;
            if (isLastLevel) {

                if (HasAction && isLastLevel) {
                    strHtml += '<li' + strChecked + ' class="' + CSSClass + ' ActionOn" actionguid = "' + CurrentAction + '" FactorsToken = "' + csvFactorsToken + '">' + v["Text"];
                    TotalActions++;
                }
                else {
                    strHtml += '<li class="' + CSSClass + ' ActionOff" actionguid = "' + CurrentAction + '" FactorsToken = "' + csvFactorsToken + '">' + v["Text"];
                }
            }
            else { //it is not an action it is just group
                strHtml += '<li class="' + CSSClass + '">' + v["Text"];
            }

            FactorsCartesianProduct(Mother, Index + 1);
            strHtml += '</li>';
        });
        strHtml += '</ul>';
    }
}

$(document).on('click', '#jsTreeMyPermissions li.ActionOn a', function () {
    alert('I can tell you who gave you this action ' + $(this).parent('li').attr('actionguid'));
});

$(document).on('click', '#jsTreeMyPermissions li.ActionOff a', function () {
    alert('Do you want this action ' + $(this).parent('li').attr('actionguid') + ' ????, I can tell you who has it!');
});

function UpdateWorkingDaysURL() {
    $('#WorkdingDaysHeader').attr('data-url', '/Configurations/WorkingDaysConfigUpdate?DutyStationGUID=' + $("#DutyStationsGUID").val());
}

function LoadWorkingDays(DutyStationsGUID) {
    LoadUrl('WorkingDaysBody', '/Configurations/WorkingDaysConfigUpdate?PK=' + $("#OrganizationInstanceGUID").val() + '&DutyStationGUID=' + DutyStationsGUID);
}

$(document).on('ifChecked ifUnchecked', '.WorkingDayCheckBox', function (event) {
    if (event.type == 'ifChecked') {
        $(this).parents('div.DayRecord').find('select').select2('enable');
        var FirstValueFrom = $('select:enabled:first-child', '.DayRecord').val();
        var FirstValueTo = $('select:enabled', '.DayRecord').eq(1).val();
        $($(this).parents('div.DayRecord').find('select')[0]).val(FirstValueFrom).trigger('change');
        $($(this).parents('div.DayRecord').find('select')[1]).val(FirstValueTo).trigger('change');

    }
    else {
        $(this).parents('div.DayRecord').find('select').val('').trigger('change');
        $(this).parents('div.DayRecord').find('select').prop("disabled", true).select2();
    }
});


var AllowFetching = true;
var lastScrollTop = 0;

function GetGlobalAudit(FormID, isScroll) {
    if (isScroll == false) {
        $('#Result').html('');
        $('#RankID').val('0');
    }
    $('#LoadingAudit').show();

    if (AllowFetching) {
        AllowFetching = false;
        $.ajax({
            type: "POST",
            url: $('#' + FormID).attr('action'),
            data: $('#' + FormID).serialize(),
            datatype: "Json",
            success: function (data) {
                var strHTML = '';
                $.each(data, function (i, v) {
                    strHTML +=
                    '<div class="col-lg-12 AuditActionRecord">' +
                    '<div class="col-md-4"><b>' + i +'-'+ v["ActionDescription"] + '</b></div>' +
                    '<div class="col-md-8 text-right text-left-xs"><span>' + moment(v["ExecutionTime"]).utc().format('MMMM DD, YYYY hh:mm:ss A') + '</span></div>' +

                    '<div class="col-lg-12"><a onclick="AuditDataDetails(this,\'' + v["jsFunction"] + '\',\'' + v["ActionGUID"] + '\',\'' + moment(v["ExecutionTime"]).utc().format('YYYY-MM-DD HH:mm:ss.SSS') + '\');">Record Details</a></div>' +

                        '<div class="col-md-12"><a class="AuditUserMoreInfo">' + v["ExecutedBy"] + '</a></div>' +
                        '<div class="UserMoreInfoPanel" style="display: none;">' +
                        '<div class="col-md-12">' + v["JobTitleDescription"] + '</div>' +
                        '<div class="col-md-12">' + v["OrganizationInstanceDescription"] + '</div>' +
                        '</div>' +
                        '</div>';

                    //Modified Fields
                    if (v["UpdatedFields"].length > 0) {
                        //Orange headers
                        strHTML += '<div class="col-lg-12 col-nopadding AuditFieldsContainer">' +
                            '<div class="col-md-4 hidden-xs" > <span class="updateLabel">Field Name</span></div>' +
                            '<div class="col-md-4 hidden-xs"><span class="updateLabel">Before Change</span></div>' +
                            '<div class="col-md-4 hidden-xs"><span class="updateLabel">After Change</span></div>';

                        $.each(v["UpdatedFields"], function (i, v) {

                            strHTML += '<div class="col-lg-12 col-nopadding AuditFieldRecord">' +
                                '<div class="col-md-4">' + v["FieldName"] + '</div>' +
                                '<div class="col-md-4"><span class="OldValueMobile visible-xs">Before Change</span>' + v["BeforeChange"] + '</div>' +
                                '<div class="col-md-4"><span class="NewValueMobile visible-xs">After Change</span>' + v["AfterChange"] + '</div>' +
                                '</div>';
                        });

                        strHTML += '</div>';
                    }

                    //Separator
                    strHTML += '<div class="BSDIVHR col-lg-12"></div>';

                    $('#Result').append(strHTML);
                    strHTML = '';
                });
                if (data.length > 0) {
                    $('#RankID').val(($('#RankID').val() * 1) + 100);
                }
                //else {
                //    strHTML = '<div>No More Result Found, we should stop call the server</div>';
                //    $('#Result').append(strHTML);
                //}
                AllowFetching = true;
                $('#LoadingAudit').hide();
                $(window).unbind('scroll');
                $(window).scroll(function (event) {
                    var st = $(this).scrollTop();
                    if (st > lastScrollTop) {
                        // downscroll code
                        if ($(window).scrollTop() + $(window).height() > $(document).height() - ($('.footer').height() + 700)) {
                            GetGlobalAudit(FormID, true);
                        }
                    } else {
                        // upscroll code
                        //Think about remove from top and fetch them again back to scroll top! like facebook messenger.
                    }
                    lastScrollTop = st;
                });
            },
            error: function (status) {
                //alert("Error");
            }
        });
    }
}

function AuditDataDetails(Link, ActionResult, ActionGUID, ExecutionTime) {
    if ($(Link).next().hasClass('AuditRecordMessage')) {
        $(Link).next('.AuditRecordMessage').toggle();
    }
    else {
        var Message = '';
        $.ajax({
            type: "Get",
            url: "/Audit/" + ActionResult,
            data: { ActionGUID: ActionGUID, ExecutionTime: ExecutionTime },
            datatype: "Json",
            success: function (data) {
                Message += '<div class="AuditRecordMessage">';
                $.each(data, function (i, v) {
                    Message += '<span style="display:block;">- ' + data[i].Message + '</span>';
                });
                Message += '</div>';
                $(Link).after(Message);
            }
        });
    }
}

function searchDivs(inputVal, divClass) {
    window.localStorage.setItem('codeTablesFilter', inputVal);
    //A new function created so it can be called on document.ready to filiter the result.
    findInstanceResult(inputVal, divClass);

}

function findInstanceResult(inputVal, divClass) {
    $('div.' + divClass).each(function (index, div) {
        var found = false;
        var regExp = new RegExp(inputVal, 'i');
        if (regExp.test($(div).text())) {
            found = true;
            //return false;
        }
        if (found == true) $(div).show(); else $(div).hide();
    });
}


function LoginMoreDetails(Link) {
    var table = $('#AuditLoginsDataTable').DataTable();
    var tr = $(Link).parent().closest('tr');
    var row = table.row(tr);
    console.log(row.data());

    if (row.child.isShown()) {
        // This row is already open - close it
        row.child.hide();
        tr.removeClass('ExpandedLoginDetails');

    }
    else {
        // Open this row
        row.child(LoginMoreDetailsTemplate(row.data())).show();
        tr.addClass('ExpandedLoginDetails');
    }
}



/* Formatting function for row details - modify as you need */
function LoginMoreDetailsTemplate(d) {
    // d is the original data object for the row
    var strHTML = "";
    if (d.IsMobileDevice) { //true meanS mobile.
        strHTML =
            '<div class="row LoginMoreDetails">' +
            '<div class="col-lg-9" >' +
            '<div class="col-lg-1 col-xs-3 text-center">' +
            '<i class="fa fa-mobile fa-3x" style="color:#666;"></i>' +
            '</div>' +
            '<div class="col-lg-8 col-xs-9">' +
            '<div class="row">' +
            '<label class="col-lg-3">Device:</label>' +
            '<div class="col-lg-9">' + d.Environment + ' - ' + d.Browser + '</div>' +
            '</div>' +
            '<div class="row">' +
            '<label class="col-lg-3">Location:</label>' +
            '<div class="col-lg-9">' + d.City + ', ' + d.CountryName + '</div>' +
            '</div>' +
            '<div class="row">' +
            '<label class="col-lg-3">IP Address:</label>' +
            '<div class="col-lg-9">' + d.UserHostAddress + '</div>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';
    }
    else {
        strHTML =
            '<div class="row LoginMoreDetails">' +
            '<div class="col-lg-9" >' +
            '<div class="col-lg-1 col-xs-3 text-center">' +
            '<i class="fa fa-laptop fa-3x" style="color:#666;"></i>' +
            '</div>' +
            '<div class="col-lg-8 col-xs-9">' +
            '<div class="row">' +
            '<label class="col-lg-3">Environment:</label>' +
            '<div class="col-lg-9">' + d.Environment + ' - ' + d.Browser + '</div>' +
            '</div>' +
            '<div class="row">' +
            '<label class="col-lg-3">Location:</label>' +
            '<div class="col-lg-9">' + d.City + ', ' + d.CountryName + '</div>' +
            '</div>' +
            '<div class="row">' +
            '<label class="col-lg-3">IP Address:</label>' +
            '<div class="col-lg-9">' + d.UserHostAddress + '</div>' +
            '</div>' +
            '<div class="row">' +
            '<label class="col-lg-3">Computer Name:</label>' +
            '<div class="col-lg-9">' + d.ComputerName + '</div>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';
    }
    return strHTML;
}

function goTop() {
    $('html, body').animate({ scrollTop: '0px' }, 500);
}

function ProGresMoreDetails(Link) {
    var table = $('#IndividualsDataTable').DataTable();
    var tr = $(Link).parent().closest('tr');
    var row = table.row(tr);
    console.log(row.data());

    if (row.child.isShown()) {
        // This row is already open - close it
        row.child.hide();
        tr.removeClass('ExpandedLoginDetails');

    }
    else {
        // Open this row
        ProGresMoreDetailsTemplate(row.data(), row)

        tr.addClass('ExpandedLoginDetails');



    }
}

function ProGresMoreDetailsTemplate(d, row) {

    $.ajax({
        method: 'post',
        datatype: 'text',
        url: "/PRG/Progres/LoadingPhoto/" + d.IndividualGUID,
        success: function (JsonResult, status, xhr) {
            if (JsonResult.Photo != null) {
                var strHTML =
                    '<div class="row LoginMoreDetails">' +
                    '<div class="col-lg-9" >' +
                    '<div class="col-lg-2 col-xs-4 text-center">' +
                    '<img id="imgProfileHeaderPhoto"  width="150" src="data:image/png;base64,' + ToBase64(JsonResult.Photo) + '">' +
                    '</div>' +
                    '<div class="col-lg-8 col-xs-8">' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Family Name:</label>' +
                    '<div class="col-lg-9">' + d.FamilyName + '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Middle Name:</label>' +
                    '<div class="col-lg-9">' + d.MiddleName + '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Maiden Name:</label>' +
                    '<div class="col-lg-9">' + d.MaidenName + '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Mother Name:</label>' +
                    '<div class="col-lg-9">' + d.MotherName + '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Registration Date:</label>' +
                    '<div class="col-lg-9">' + moment(d.RegistrationDate).format('MMMM DD, YYYY') + '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Date of Birth:</label>' +
                    '<div class="col-lg-9">' + moment(d.DateofBirth).format('MMMM DD, YYYY') + '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Origin Country:</label>' +
                    '<div class="col-lg-9">' + d.OriginCountryCode + '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Nationality:</label>' +
                    '<div class="col-lg-9">' + d.NationalityCode + '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Gender:</label>' +
                    '<div class="col-lg-9">' + d.SexCode + '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<label class="col-lg-3">Individual Age:</label>' +
                    '<div class="col-lg-9">' + d.IndividualAge + '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                row.child(strHTML).show();
                // return strHTML;
            }
        },
        error: function (ex) {
            Notify(NotificationType.Error, "There was an error Loading Photo");
            console.log("ex", ex);
            console.log('Error: ' + ex.responseText);
        }
    });
    return "";
}


$('.NotExistPageLink').click(function (e) {
    e.preventDefault();
    Notify(NotificationType.Warning, "Sorry, page dose not exist yet.");
})

//$('.select2').select2({
//    placeholder: 'Press CTRL+A for selecr or unselect all options'
//});

//$('.select2[multiple]').siblings('.select2-container').append('<span class="select-all"></span>');

//$(document).on('click', '.select-all', function (e) {
//    selectAllSelect2($(this).siblings('.selection').find('.select2-search__field'));
//});

//$(document).on("keyup", ".select2-search__field", function (e) {
//    var eventObj = window.event ? event : e;
//    if (eventObj.keyCode === 65 && eventObj.ctrlKey)
//        selectAllSelect2($(this));

//});


//function selectAllSelect2(that) {

//    var selectAll = true;
//    var existUnselected = false;
//    var id = that.parents("span[class*='select2-container']").siblings('select[multiple]').attr('id');
//    var item = $("#" + id);

//    item.find("option").each(function (k, v) {
//        if (!$(v).prop('selected')) {
//            existUnselected = true;
//            return false;
//        }
//    });

//    selectAll = existUnselected ? selectAll : !selectAll;
//    item.find("option").prop('selected', selectAll).trigger('change');

//}


$('.select2[multiple]').siblings('.select2-container').append('<span class="select-all"></span>');
$(document).on('click', '.select-all', function (e) {
    selectAllSelect2($(this).siblings('.selection').find('.select2-search__field'));
});
$(document).on("keyup", ".select2-search__field", function (e) {
    var eventObj = window.event ? event : e;
    if (eventObj.keyCode === 65 && eventObj.ctrlKey)
        selectAllSelect2($(this));

});
function selectAllSelect2(that) {

    var selectAll = true;
    var existUnselected = false;
    var id = that.parents("span[class*='select2-container']").siblings('select[multiple]').attr('id');
    var item = $("#" + id);

    item.find("option").each(function (k, v) {
        if (!$(v).prop('selected')) {
            existUnselected = true;
            return false;
        }
    });

    selectAll = existUnselected ? selectAll : !selectAll;
    item.find("option").prop('selected', selectAll);//.trigger('change');
    item.find("option:first").trigger('change');
    if (item.find("option:first").val() == '') { item.find("option:first").remove(); }
    if (existUnselected) { item.next().addClass('select2scroll'); } else { item.next().removeClass('select2scroll'); }
}

$(function () {
    $('.store-app-access').click(function (e) {
        //e.preventDefault();
        var ApplicationGUID = $(this).attr('data-appId');
        $.ajax({
            type: "POST",
            url: "/Audit/ApplicationAccessAuditCreate",
            data: { ApplicationGUID: ApplicationGUID },
            success: function () { },
            error: function () { }
        });
    });
});





