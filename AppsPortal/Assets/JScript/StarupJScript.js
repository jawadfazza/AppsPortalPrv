function ReadOnlyForm(RestoreButton) {
    var form = $('#' + RestoreButton).parents('form');
    $('#' + $(form).attr('id') + ' :input:not(input[type=hidden])').each(function () {
        $(this).attr('readonly', 'readonly');
    });
    $(form).addClass('ReadOnlyForm');
}