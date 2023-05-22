var resxPikadayTime = 'Time';

var IsPublic = {
    "91CB45ED-ADD8-49EF-BE69-FFC5AD2244B6": "Public",
    "91CB45ED-ADD8-49EF-BE69-FFC5AD2244B7": "Authorized"
}
var IsActive = {
    "true": "Active",
    "false": "Stopped"
}
var resxDataTableLanguage =
    {
        "sEmptyTable": "No record available",
        "sInfo": "Showing _START_ to _END_ of _TOTAL_ entries",
        "sInfoEmpty": "Showing 0 to 0 of 0 entries",
        "sInfoFiltered": "(filtered from _MAX_ total entries)",
        "sInfoPostFix": "",
        "sInfoThousands": ",",
        "sLengthMenu": "_MENU_ <span class='rpp'>Records per page</span>",
        "sLoadingRecords": "",
        "sProcessing": "<i class='IndexDataTableLoading'></i>",
        "sSearch": "Search:",
        "sZeroRecords": "No matching records found",
        "oPaginate": {
            "sFirst": "First",
            "sLast": "Last",
            "sNext": "Next",
            "sPrevious": "Previous"
        },
        "oAria": {
            "sSortAscending": ": activate to sort column ascending",
            "sSortDescending": ": activate to sort column descending"
        }
    }


//Restore Message Resources
var ConfirmModal = {
    Delete: {
        ModalTitle: 'Confirm Delete',
        ModalMessageSingle: 'Are you sure you want to delete this record?',
        ModalMessageAll: 'Are you sure you want to delete selected record(s)?',
        btnConfirm: 'Confirm Delete',
    },
    Restore: {
        ModalTitle: 'Confirm Restore',
        ModalMessageSingle: 'Are you sure you want to restore this record?',
        ModalMessageAll: 'Are you sure you want to restore selected record(s)?',
        btnConfirm: 'Confirm Restore',
    },
    ProfilePhotoDelete: {
        ModalTitle: 'Confirm Delete',
        ModalMessageSingle: 'Are you sure you want to delete your profile photo?',
        btnConfirm: 'Confirm Delete',
    },
    OrganizationLogoDelete: {
        ModalTitle: 'Confirm Delete',
        ModalMessageSingle: 'Are you sure you want to delete this logo?',
        btnConfirm: 'Confirm Delete',
    },
    OfficePhotoDelete: {
        ModalTitle: 'Confirm Delete',
        ModalMessageSingle: 'Are you sure you want to delete this photo?',
        btnConfirm: 'Confirm Delete',
    }
}

var ConcurrencyMessage = {
    Title: 'Concurrency Occured',
    Message: 'The record you attempted to edit was modified by another user after you got the original values. The edit operation was cancelled. Click on the database icon to switch between your input and the current server values. If you still want to edit this record, click the Save Changes button again.Otherwise click Close button.',
    SameChangesMessage: 'The record you attempted to edit was modified by another user by exact same values, the edit operation was cancelled.',
    RecordDeletedMessage: 'The record you attempted to edit has been deleted by another user.'
}
