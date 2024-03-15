function StudyAbroad() {
    $(".study-abroad").slick({
        dots: false,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 500,
        arrows: true,
        autoplay: true,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right"></i></button>',
    });
    $(".banner-page").slick({
        dots: false,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 500,
        arrows: false,
        autoplay: true,
        autoplaySpeed: 3000,
    });
    $(".scholarship").slick({
        dots: false,
        infinite: true,
        slidesToShow: 3,
        slidesToScroll: 1,
        speed: 800,
        arrows: true,
        autoplay: true,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1, arrows: false,
                }
            },
            {
                breakpoint: 600, 
                settings: {
                    slidesToShow: 1, 
                    slidesToScroll: 1, arrows: false,
                }
            }
        ]
    });


    $('.study-abroad-grid').slick({
        rows: 5,
        dots: true,
        arrows: false,
        infinite: true,
        speed: 800,
        slidesToShow: 2,
        slidesToScroll: 2,
        prevArrow: '<button class="previous-slie"><i class="fal fa-chevron-circle-left"></i></button>',
        nextArrow: '<button class="next-slie"><i class="fal fa-chevron-circle-right"></i></button>',
        responsive: [
            {
                breakpoint: 1044,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    infinite: true,
                }
            },
            {
                breakpoint: 900,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    rows: 3,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    rows: 3,
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1   
                }
            }
        ]
    });
}

$(document).ready(function () {
    $(".input-number").niceNumber({
        autoSize: true,
        autoSizeBuffer: 1,
        maxValue: 1000, 
    })
    $(".input-number").on('input', function () {
        var value = parseInt($(this).val().replace(/\D/g, '')); 
        if (value > 100) {
            $(this).val('1000'); 
        }
    });

    VirtualSelect.init({
        ele: '.v-select',
        search: true,
    });
    VirtualSelect.init({
        ele: '.v-select-multiple',
        search: true,
        multiple: true,
        showValueAsTags: true,
        maxValues: 4,
    });
    $('.datetimepicker').datetimepicker({
        format: 'd/m/Y H:i',
        vi: {
            months: [
                "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"
            ],
            dayOfWeekShort: [
                "CN", "T2", "T3", "T4", "T5", "T6", "T7"
            ],
            dayOfWeek: ["Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy"]
        },
        minDate: new Date()
    });

    $("textarea.ckeditor").ckeditor();
    CKEDITOR.timestamp = new Date();
})

function toggleSidebar() {
    $(".sidebar").toggleClass("collapsed");
}
$('.message-success').delay(2000).fadeOut('slow');

function ListJob() {

}
function UpdateStatus(id, status) {
    $.post("/Employer/UpdateStatusJob", { id: id, type: status }, function (data) {
        if (data) {
            new Notify({
                status: 'success',
                text: 'Cập nhật trạng thái thành công',
                effect: 'slide',
                speed: 600,
                showIcon: true,
                showCloseButton: true,
                autoclose: true,
                autotimeout: 2000,
                gap: 77,
                distance: 20,
                type: 3,
                position: 'right top'
            })
            setTimeout(function () {
                window.location.reload();
            }, 1500);
        }
        else {
            new Notify({
                status: 'error',
                text: 'Thực hiện không thành công vui lòng thử lại',
                effect: 'slide',
                speed: 600,
                showIcon: true,
                showCloseButton: true,
                autoclose: true,
                autotimeout: 3000,
                gap: 77,
                distance: 20,
                type: 3,
                position: 'right top'
            })
        }
    });
}
function UpdateStatusArticle(id, status) {
    $.post("/Employer/UpdateStatusArticle", { id: id, type: status }, function (data) {
        if (data) {
            new Notify({
                status: 'success',
                text: 'Cập nhật trạng thái thành công',
                effect: 'slide',
                speed: 600,
                showIcon: true,
                showCloseButton: true,
                autoclose: true,
                autotimeout: 2000,
                gap: 77,
                distance: 20,
                type: 3,
                position: 'right top'
            })
            setTimeout(function () {
                window.location.reload();
            }, 1500);
        }
        else {
            new Notify({
                status: 'error',
                text: 'Thực hiện không thành công vui lòng thử lại',
                effect: 'slide',
                speed: 600,
                showIcon: true,
                showCloseButton: true,
                autoclose: true,
                autotimeout: 3000,
                gap: 77,
                distance: 20,
                type: 3,
                position: 'right top'
            })
        }
    });
}

function deleteStudy(id) {
    if (confirm("Bạn có chắc chắn xóa tin này không?")) {
        $.post("/Employer/DeleteStudy", { id: id }, function (data) {
            if (data) {
                new Notify({
                    status: 'success',
                    text: 'Cập nhật trạng thái thành công',
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 77,
                    distance: 20,
                    type: 3,
                    position: 'right top'
                })
                $("tr[data-id='" + id + "']").fadeOut();
            }
            else {
                new Notify({
                    status: 'error',
                    text: 'Thực hiện không thành công vui lòng thử lại',
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 77,
                    distance: 20,
                    type: 3,
                    position: 'right top'
                })
            }
        });
    }
}

function deleteJob(id) {
    if (confirm("Bạn có chắc chắn xóa tin này không?")) {
        $.post("/Employer/DeleteJob", { id: id }, function (data) {
            if (data) {
                new Notify({
                    status: 'success',
                    text: 'Cập nhật trạng thái thành công',
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 77,
                    distance: 20,
                    type: 3,
                    position: 'right top'
                })
                $("tr[data-id='" + id + "']").fadeOut();
            }
            else {
                new Notify({
                    status: 'error',
                    text: 'Thực hiện không thành công vui lòng thử lại',
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 77,
                    distance: 20,
                    type: 3,
                    position: 'right top'
                })
            }
        });
    }
}
function deleteArticle(id) {
    if (confirm("Bạn có chắc chắn xóa tin này không?")) {
        $.post("/Employer/DeleteArticle", { articleId: id }, function (data) {
            if (data) {
                new Notify({
                    status: 'success',
                    text: 'Cập nhật trạng thái thành công',
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 77,
                    distance: 20,
                    type: 3,
                    position: 'right top'
                })
                $("tr[data-id='" + id + "']").fadeOut();
            }
            else {
                new Notify({
                    status: 'error',
                    text: 'Thực hiện không thành công vui lòng thử lại',
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 77,
                    distance: 20,
                    type: 3,
                    position: 'right top'
                })
            }
        });
    }
}
$(document).ajaxStart(function () {
    $(".loading").addClass("active")
});

$(document).ajaxStop(function () {
    $(".loading").removeClass("active")
});
