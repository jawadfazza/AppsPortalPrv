var resxPikadayTime = 'الوقت';

var IsPublic = {
    "91CB45ED-ADD8-49EF-BE69-FFC5AD2244B6": "عام",
    "91CB45ED-ADD8-49EF-BE69-FFC5AD2244B7": "ترخيص"
}
var IsActive = {
    "true": "نشط",
    "false": "متوقف"
}
var resxDataTableLanguage =
    {
        "sEmptyTable": "لم يتم العثور على اي قيد",
        "sInfo": "إظهار _START_ إلى _END_ من أصل _TOTAL_ قيد",
        "sInfoEmpty": "يعرض 0 إلى 0 من أصل 0 سجل",
        "sInfoFiltered": "(منتقاة من مجموع _MAX_ قيد)",
        "sInfoPostFix": "",
        "sInfoThousands": ",",
        "sLengthMenu": "_MENU_<span class='rpp'>قيود في الصفحة</span>",
        "sLoadingRecords": "",
        "sProcessing": "",
        "sSearch": "ابحث:",
        "sZeroRecords": "لا يوجد قيود في الجدول",
        "oPaginate": {
            "sFirst": "الأول",
            "sLast": "الأخير",
            "sNext": "التالي",
            "sPrevious": "السابق"
        },
        "oAria": {
            "sSortAscending": "اضغط للترتيب تصاعدياً",
            "sSortDescending": "اضغط للترتيب تنازلياً"
        }
    }


//Restore Message Resources
var ConfirmModal = {
    Restore: {
        ModalTitle: 'Confirm Restore',
        ModalMessageSingle: 'Are you sure you want to restore this record?',
        ModalMessageAll: 'Are you sure you want to restore selected record(s)?',
        btnConfirm: 'Restore',
    },
    Delete: {
        ModalTitle: 'Confirm Delete',
        ModalMessageSingle: 'Are you sure you want to delete this record?',
        ModalMessageAll: 'Are you sure you want to delete selected record(s)?',
        btnConfirm: 'Delete',
    }
}

var ConcurrencyMessage = {
    Title: 'Concurrency Occured',
    Message: 'The record you attempted to edit was modified by another user after you got the original values. The edit operation was cancelled. Click on the database icon to switch between your input and the current server values. If you still want to edit this record, click the Save Changes button again.Otherwise click Close button.',
    SameChangesMessage: 'The record you attempted to edit was modified by another user by exact same values, the edit operation was cancelled.',
    RecordDeletedMessage: 'The record you attempted to edit has been deleted by another user.'
}

