
AOS.init();

function HomeJs() {

    $('.owl-carousel').owlCarousel({
        loop: false,
        margin: 10,
        nav: true,
        dots: false,
        navText: ["<i class='fal fa-chevron-circle-left'></i>", "<i class='fal fa-chevron-circle-right'></i>"],
        items: 5

    }) 

    $(".list-banner").slick({
        dots: false,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: true,
        arrows: false,
        autoplaySpeed: 3000,
    });

    
    $('.list-hotjob').slick({
        rows: 5,
        dots: false,
        arrows: true,
        infinite: true,
        speed: 800,
        slidesToShow: 3,
        slidesToScroll: 3,
        prevArrow: '<button class="previous-slie"><i class="fal fa-chevron-circle-left"></i></button>',
        nextArrow: '<button class="next-slie"><i class="fal fa-chevron-circle-right"></i></button>',
        responsive: [
            {
                breakpoint: 1044,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    rows: 2,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    rows: 1,
                    arrows: false,
                    dots: true,
                }
            }
        ]
    });
    //$('.list-hotjob').slick({
    //    rows: 5,
    //    dots: false,
    //    arrows: true,
    //    infinite: true,
    //    speed: 800,
    //    slidesToShow: 3,
    //    //fade: true,
    //    //cssEase: 'linear',
    //    slidesToScroll: 3,
    //    prevArrow: '<button class="previous-slie"><i class="fal fa-chevron-circle-left"></i></button>',
    //    nextArrow: '<button class="next-slie"><i class="fal fa-chevron-circle-right"></i></button>',
    //    responsive: [
    //        {
    //            breakpoint: 1024,
    //            settings: {
    //                slidesToShow: 6,
    //                slidesToScroll: 3,
    //                infinite: true,
    //                dots: false
    //            }
    //        },
    //        {
    //            breakpoint: 600,
    //            settings: {
    //                slidesToShow: 3,
    //                slidesToScroll: 1,
    //                slidesToShow: 3,
    //            }
    //        },
    //        {
    //            breakpoint: 480,
    //            settings: {
    //                slidesToShow: 3,
    //                slidesToScroll: 3
    //            }
    //        }
    //    ]
    //});
    $('.list-contruy').slick({
        infinite: false,
        speed: 300,
        slidesToShow: 4,
        slidesToScroll: 1,
        prevArrow: '<button class="previous-fb"><i class="fal fa-chevron-circle-left"></i></button>',
        nextArrow: '<button class="next-fb"><i class="fal fa-chevron-circle-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    dots: true,
                    arrows: false,
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    dots: true,
                    arrows: false,
                }
            }
        ]
    });
    $('.list-career').slick({
        rows: 2,
        dots: false,
        arrows: true,
        infinite: true,
        speed: 800,
        slidesToShow: 4,
        //fade: true,
        //cssEase: 'linear',
        slidesToScroll: 3,
        prevArrow: '<button class="previous-career"><i class="fal fa-chevron-circle-left"></i></button>',
        nextArrow: '<button class="next-career"><i class="fal fa-chevron-circle-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    dots: true,
                    arrows: false,
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    dots: true,
                    arrows: false,
                }
            }
        ]
    });
    $('.list-support').slick({
        infinite: true,
        speed: 300,
        slidesToShow: 4,
        slidesToScroll: 1,
        prevArrow: '<button class="previous-support"><i class="fal fa-chevron-circle-left"></i></button>',
        nextArrow: '<button class="next-support"><i class="fal fa-chevron-circle-right"></i></button>',
        responsive: [
            {
                breakpoint: 1044,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
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
    $('[data-toggle="tooltip"]').tooltip()
    $(".list-bannerjob").slick({
        dots: false,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        speed: 1000,
        autoplay: true,
        arrows: false,
        autoplaySpeed: 3000,
    });
    $('.owl-carousel[data-selector="' + 1 + '"]').addClass('active');
    $(".filter-select").change(function () {
        var value = $(this).val();
        $('.section-filter').removeClass('active');
        $('.owl-carousel').removeClass('active');
        $('.owl-carousel[data-selector="' + value + '"]').addClass('active');
        $('.section-filter[data-selector="' + value + '"]').addClass('active');
    });
    $(".section-filter").change(function () {
        var dataId = $(".section-filter[data-selector='1']").val();
        var career = $(".section-filter[data-selector='4']").val();
        var wage = $(".section-filter[data-selector='2']").val();
        var exp = $(".section-filter[data-selector='3']").val();
        $.post("/Home/GetJob", { cityId: dataId, careerId: career, wage: wage, exp: exp }, function (data) {
            $("#filter").empty();
            $("#filter").html(data);
        })
    })

    $(".contruy-name").click(function () {
        $(".contruy-name").removeClass("active");
        $(this).addClass("active");
        var dataId = $(this).data("id");
        var career = $(this).data("career-id");
        var wage = $(this).data("wage");
        var exp = $(this).data("exp");
        $.post("/Home/GetJob", { cityId: dataId, careerId: career, wage: wage, exp: exp }, function (data) {
            $("#filter").empty();
            $("#filter").html(data);
        })
    });

    $(".setting-filter").click(function () {
        $(".group-filter").toggleClass("active")
    })

}

function Details() {
    $('.list-img').slick({
        infinite: true,
        speed: 300,
        slidesToShow: 3,
        slidesToScroll: 1,
        prevArrow: '<button class="previous-fb"><i class="fa-solid fa-chevron-left"></i></button>',
        nextArrow: '<button class="next-fb"><i class="fa-solid fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
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
    $('.policy').slick({
        infinite: true,
        speed: 800,
        slidesToShow: 3,
        slidesToScroll: 1,
        prevArrow: '<button class="previous-fb"><i class="fal fa-chevron-circle-left"></i></button>',
        nextArrow: '<button class="next-fb"><i class="fal fa-chevron-circle-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2
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
    $(".show-more").click(function () {
        $(".intro").toggleClass("active");
        var buttonText = $(".intro").hasClass("active") ? "Ẩn bớt" : "Xem thêm";
        $(".show-more .icon").text(buttonText);
        var iconClass = $(".intro").hasClass("active") ? "fa-chevron-up" : "fa-chevron-down";
        $(".show-more i").removeClass("fa-chevron-up fa-chevron-down").addClass(iconClass);
    });
    $('.stepItem_title').click(function () {
        var contentId = $(this).data('toggle');
        $('#' + contentId).slideToggle();
    });

    $(".apply-form").submit(function (e) {
        e.preventDefault(); 
        var buttonElement = $(".button1");
        var formData = new FormData($(this)[0]); 
        $.ajax({
            url: "/Home/Apply", 
            type: 'POST', 
            data: formData, 
            contentType: false, 
            processData: false, 
            success: function (response) {
                if (response.success) {
                    new Notify({
                        status: 'success',
                        text: response.message,
                        effect: 'slide',
                        speed: 600,
                        showIcon: true,
                        showCloseButton: true,
                        autoclose: true,
                        autotimeout: 3000,
                        gap: 10,
                        distance: 20,
                        type: 3,
                        position: 'right bottom'
                    });
                    $(".close").click();
                    buttonElement.attr("data-target", "");
                    buttonElement.text("Đã ứng tuyển");
                } else {
                    new Notify({
                        status: 'error',
                        text: response.message,
                        effect: 'slide',
                        speed: 600,
                        showIcon: true,
                        showCloseButton: true,
                        autoclose: true,
                        autotimeout: 3000,
                        gap: 10,
                        distance: 20,
                        type: 3,
                        position: 'right bottom'
                    });
                }
            },
            error: function (xhr, status, error) {
                alert("Đã xảy ra lỗi: " + error);
            }
        });
    });
    $(".advise-form").submit(function (e) {
        e.preventDefault();
        var formData = new FormData($(this)[0]);
        $.ajax({
            url: "/Home/AdviseForm",
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    new Notify({
                        status: 'success',
                        text: response.message,
                        effect: 'slide',
                        speed: 600,
                        showIcon: true,
                        showCloseButton: true,
                        autoclose: true,
                        autotimeout: 3000,
                        gap: 10,
                        distance: 20,
                        type: 3,
                        position: 'right bottom'
                    });
                    $(".close").click();
                } else {
                    new Notify({
                        status: 'error',
                        text: response.message,
                        effect: 'slide',
                        speed: 600,
                        showIcon: true,
                        showCloseButton: true,
                        autoclose: true,
                        autotimeout: 3000,
                        gap: 10,
                        distance: 20,
                        type: 3,
                        position: 'right bottom'
                    });
                }
            },
            error: function (xhr, status, error) {
                alert("Đã xảy ra lỗi: " + error);
            }
        });
    });
    $(".copy-value").click(function () {
        var dataValue = $("#url-request").data("value");
        var tempInput = $("<input>");
        $("body").append(tempInput);
        tempInput.val(dataValue).select();
        document.execCommand("copy");
        tempInput.remove();
        new Notify({
                status: 'success',
                text: "Sao chép thành công !!!",
                effect: 'slide',
                speed: 600,
                showIcon: true,
                showCloseButton: true,
                autoclose: true,
                autotimeout: 3000,
                gap: 10,
                distance: 20,
                type: 3,
                position: 'right bottom'
            });
    });
}

// Theo dõi , Lưu tin
function StudyDetai() {
    function handleFollowButtonClick(button, companyId, isFollow) {
        var actionUrl = isFollow ? "/Home/Follow" : "/Home/UnFollow";
        var buttonText = isFollow ? "Bỏ theo dõi công ty" : "Theo dõi công ty";
        var buttonClassToRemove = isFollow ? "follow" : "unfollow";
        var buttonClassToAdd = isFollow ? "unfollow" : "follow";

        $.post(actionUrl, { id: companyId }, function (data) {
            if (data.success) {
                button.text(buttonText);
                button.removeClass(buttonClassToRemove).addClass(buttonClassToAdd);
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        });
    }

    function handleLikeStudy(button, jobId, isLike) {
        var actionUrl = isLike ? "/Home/LikeStudy" : "/Home/UnStudy";
        var buttonText = isLike ? "Bỏ lưu" : "Lưu tin";
        var buttonClassToRemove = isLike ? "like" : "unlike";
        var buttonClassToAdd = isLike ? "unlike" : "like";
        $.post(actionUrl, { id: jobId }, function (data) {
            if (data.success) {
                button.text(buttonText);
                button.removeClass(buttonClassToRemove).addClass(buttonClassToAdd);
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        });
    }
    //Theo dõi công ty
    $(document).on("click", ".follow", function () {
        var button = $(this);
        var companyId = button.data("company-id");
        handleFollowButtonClick(button, companyId, true);
    });

    $(document).on("click", ".unfollow", function () {
        var button = $(this);
        var companyId = button.data("company-id");
        handleFollowButtonClick(button, companyId, false);
    });

    //Like Job
    $(document).on("click", ".likestudy", function () {
        var button = $(this);
        var jobId = button.data("study");
        handleLikeStudy(button, jobId, true);
    });
    $(document).on("click", ".unlikestudy", function () {
        var button = $(this);
        var jobId = button.data("study");
        handleLikeStudy(button, jobId, false);
    });
}
function ActionDetail() {
    function handleFollowButtonClick(button, companyId, isFollow) {
        var actionUrl = isFollow ? "/Home/Follow" : "/Home/UnFollow";
        var buttonText = isFollow ? "Bỏ theo dõi công ty" : "Theo dõi công ty";
        var buttonClassToRemove = isFollow ? "follow" : "unfollow";
        var buttonClassToAdd = isFollow ? "unfollow" : "follow";

        $.post(actionUrl, { id: companyId }, function (data) {
            if (data.success) {
                button.text(buttonText);
                button.removeClass(buttonClassToRemove).addClass(buttonClassToAdd);
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        });
    }

    function handleLikeJob(button, jobId, isLike) {
        var actionUrl = isLike ? "/Home/Likejob" : "/Home/UnLike";
        var buttonText = isLike ? "Bỏ lưu" : "Lưu tin";
        var buttonClassToRemove = isLike ? "like" : "unlike";
        var buttonClassToAdd = isLike ? "unlike" : "like";
        $.post(actionUrl, { id: jobId }, function (data) {
            if (data.success) {
                button.text(buttonText);
                button.removeClass(buttonClassToRemove).addClass(buttonClassToAdd);
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        });
    }
    //Theo dõi công ty
    $(document).on("click", ".follow", function () {
        var button = $(this);
        var companyId = button.data("company-id");
        handleFollowButtonClick(button, companyId, true);
    });

    $(document).on("click", ".unfollow", function () {
        var button = $(this);
        var companyId = button.data("company-id");
        handleFollowButtonClick(button, companyId, false);
    });

    //Like Job
    $(document).on("click", ".like", function () {
        var button = $(this);
        var jobId = button.data("job");
        handleLikeJob(button, jobId, true);
    });
    $(document).on("click", ".unlike", function () {
        var button = $(this);
        var jobId = button.data("job");
        handleLikeJob(button, jobId, false);
    });
}
//Hết

function CompanyJs() {
    $('.lits-com').slick({
        rows: 2,
        dots: true,
        arrows: false,
        infinite: true,
        speed: 800,
        slidesToShow: 3,
        slidesToScroll: 3,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 2,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    dots: true
                }
            }
        ]
    });
}
function articleDetail() {
    $(".relate").slick({
        dots: false,
        infinite: true,
        slidesToShow: 3,
        slidesToScroll: 1,
        speed: 1000,
        arrows: false,
        autoplay: true,
        autoplaySpeed: 3000,
        prevArrow: '<button class="chevron-prev"><i class="fal fa-chevron-left"></i></button>',
        nextArrow: '<button class="chevron-next"><i class="fal fa-chevron-right"></i></button>',
        responsive: [
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 3,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                }
            }
        ]
    });
}
function User() {
    $('.type-input').click(function () {
        var input = $(this).prev('input');

        if (input.attr('type') === 'password') {
            input.attr('type', 'text');
            $(this).find('i').removeClass('fa-eye').addClass('fa-eye-slash ');
        } else {
            input.attr('type', 'password');
            $(this).find('i').removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });
}


function ProfileJs() {
    $('.change-info').click(function (e) {
        e.preventDefault();
        var quantity = $('#number').val();
        var url = $(this).data('action') + '?quantity=' + encodeURIComponent(quantity);
        $.fancybox.open({
            src: url,
            type: 'ajax',
            touch: false,
            smallBtn: false
        });
    });

    var i = 1;
    $("#expUp").fileupload({
        add: function (e, data) {
            var uploadErrors = [];
            var acceptFileTypes = /^image\/(gif|jpe?g|png)$/i;
            if (data.originalFiles[0]["type"].length && !acceptFileTypes.test(data.originalFiles[0]["type"])) {
                uploadErrors.push("Chỉ chấp nhận định dạng jpeg, jpg, png, gif");
            }
            if (data.originalFiles[0]["size"] > 4000000) {
                uploadErrors.push("Dung lượng ảnh lớn hơn 4MB");
            }
            var totalImg = $("#expImg .col-lg-4").length;
            console.log(totalImg)
            if (totalImg >= 10) {
                uploadErrors.push("Chỉ đăng tối đa 10 ảnh");
            }
            if (uploadErrors.length > 0) {
                alert(uploadErrors.join("\n"));
                return false;
            } else {
                data.submit();
            }
            return true;
        },
        url: "/Uploader/Upload?folder=exp&r=" + Math.random(),
        dataType: "json",
        done: function (e, data) {
            console.log(data.result.files)
            $.each(data.result.files, function (index, file) {
                $('#expImg').append('<div class="col-lg-4 mb-3"><div class="card card-ex"><input type="hidden" name="Pictures" value="' + file.name + '" /><img src="/images/exp/' + file.name + '" /><div class="remove removexp" onclick="removeFile(' + i + ')"><i class="fa-solid fa-trash" ></i></div></div></div>');
            });
            i = i + 1;
            $("#progress").fadeOut(2000);
        },
        start: function () {
            $("#progress .progress-bar").css("width", "0");
            $("#progress").show();
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $("#progress .progress-bar").css("width", progress + "%");
        }
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled');
    $(document).on('click', '.removexp', function () {
        const imageCard = $(this).closest('.col-lg-4');
        const index = imageCard.index();
        imageCard.remove();
        const inputFiles = $('#expImg')[0].files;
        const newFiles = Array.from(inputFiles).filter((_, i) => i !== index);
        $('#expImg').prop('files', newFiles);
    });


     //Chứng chỉ

    var l = 1;
    $("#fileuploadCer").fileupload({
        add: function (e, data) {
            var uploadErrors = [];
            var acceptFileTypes = /^image\/(gif|jpe?g|png)$/i;
            if (data.originalFiles[0]["type"].length && !acceptFileTypes.test(data.originalFiles[0]["type"])) {
                uploadErrors.push("Chỉ chấp nhận định dạng jpeg, jpg, png, gif");
            }
            if (data.originalFiles[0]["size"] > 4000000) {
                uploadErrors.push("Dung lượng ảnh lớn hơn 4MB");
            }
            var totalImg = $("#cerImages .col-lg-4").length;
            console.log(totalImg)
            if (totalImg >= 10) {
                uploadErrors.push("Chỉ đăng tối đa 10 ảnh");
            }
            if (uploadErrors.length > 0) {
                alert(uploadErrors.join("\n"));
                return false;
            } else {
                data.submit();
            }
            return true;
        },
        url: "/Uploader/Upload?folder=cer&r=" + Math.random(),
        dataType: "json",
        done: function (e, data) {
            console.log(data.result.files)
            $.each(data.result.files, function (index, file) {
                $('#cerImages').append('<div class="col-lg-4 mb-3"><div class="card card-ex"><input type="hidden" name="Pictures" value="' + file.name + '" /><img src="/images/cer/' + file.name + '" /><div class="remove removcer" onclick="removeFile(' + i + ')"><i class="fa-solid fa-trash" ></i></div></div></div>');
            });
            l = l + 1;
            $("#progress").fadeOut(2000);
        },
        start: function () {
            $("#progress .progress-bar").css("width", "0");
            $("#progress").show();
        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $("#progress .progress-bar").css("width", progress + "%");
        }
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled');
    $(document).on('click', '.removcer', function () {
        const imageCard = $(this).closest('.col-lg-4');
        const index = imageCard.index();
        imageCard.remove();
        const inputFiles = $('#fileuploadCer')[0].files;
        const newFiles = Array.from(inputFiles).filter((_, l) => l !== index);
        $('#fileuploadCer').prop('files', newFiles);
    });

    // Dự án
    var a = 1;
    $("#projectImg").fileupload({
        add: function (e, data) {
            var uploadErrors = [];
            var acceptFileTypes = /^image\/(gif|jpe?g|png)$/i;
            if (data.originalFiles[0]["type"].length && !acceptFileTypes.test(data.originalFiles[0]["type"])) {
                uploadErrors.push("Chỉ chấp nhận định dạng jpeg, jpg, png, gif");
            }
            if (data.originalFiles[0]["size"] > 4000000) {
                uploadErrors.push("Dung lượng ảnh lớn hơn 4MB");
            }
            var totalImg = $("#project-img .col-lg-4").length;
            console.log(totalImg)
            if (totalImg >= 10) {
                uploadErrors.push("Chỉ đăng tối đa 10 ảnh");
            }
            if (uploadErrors.length > 0) {
                alert(uploadErrors.join("\n"));
                return false;
            } else {
                data.submit();
            }
            return true;
        },
        url: "/Uploader/Upload?folder=project&r=" + Math.random(),
        dataType: "json",
        done: function (e, data) {
            console.log(data.result.files)
            $.each(data.result.files, function (index, file) {
                $('#project-img').append('<div class="col-lg-4 mb-3"><div class="card card-ex"><input type="hidden" name="Pictures" value="' + file.name + '" /><img src="/images/project/' + file.name + '" /><div class="remove removProject" onclick="removeFile(' + i + ')"><i class="fa-solid fa-trash" ></i></div></div></div>');
            });
            a = a + 1;
           
        },
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled');
    $(document).on('click', '.removProject', function () {
        const imageCard = $(this).closest('.col-lg-4');
        const index = imageCard.index();
        imageCard.remove();
        const inputFiles = $('#projectImg')[0].files;
        const newFiles = Array.from(inputFiles).filter((_, a) => a !== index);
        $('#projectImg').prop('files', newFiles);
    });
    // Hoạt động
    var ac = 1;
    $("#activity-file").fileupload({
        add: function (e, data) {
            var uploadErrors = [];
            var acceptFileTypes = /^image\/(gif|jpe?g|png)$/i;
            if (data.originalFiles[0]["type"].length && !acceptFileTypes.test(data.originalFiles[0]["type"])) {
                uploadErrors.push("Chỉ chấp nhận định dạng jpeg, jpg, png, gif");
            }
            if (data.originalFiles[0]["size"] > 4000000) {
                uploadErrors.push("Dung lượng ảnh lớn hơn 4MB");
            }
            var totalImg = $("#project-img .col-lg-4").length;
            console.log(totalImg)
            if (totalImg >= 10) {
                uploadErrors.push("Chỉ đăng tối đa 10 ảnh");
            }
            if (uploadErrors.length > 0) {
                alert(uploadErrors.join("\n"));
                return false;
            } else {
                data.submit();
            }
            return true;
        },
        url: "/Uploader/Upload?folder=activity&r=" + Math.random(),
        dataType: "json",
        done: function (e, data) {
            console.log(data.result.files)
            $.each(data.result.files, function (index, file) {
                $('#activity-img').append('<div class="col-lg-4 mb-3"><div class="card card-ex"><input type="hidden" name="Pictures" value="' + file.name + '" /><img src="/images/activity/' + file.name + '" /><div class="remove removacti" onclick="removeFile(' + i + ')"><i class="fa-solid fa-trash" ></i></div></div></div>');
            });
            ac = ac + 1;

        },
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled');
    $(document).on('click', '.removacti', function () {
        const imageCard = $(this).closest('.col-lg-4');
        const index = imageCard.index();
        imageCard.remove();
        const inputFiles = $('#activity-file')[0].files;
        const newFiles = Array.from(inputFiles).filter((_, ac) => ac !== index);
        $('#activity-file').prop('files', newFiles);
    });
}
function ProfileAction() {
    $("#active-ed").change(function () {
        if ($(this).is(":checked")) {
            $(".end-ed").hide();
        } else {
            $(".end-ed").show();
        }
    });
    $("#active-ex").change(function () {
        if ($(this).is(":checked")) {
            $(".end-ex").hide();
        } else {
            $(".end-ex").show();
        }
    });
    $("#active-ce").change(function () {
        if ($(this).is(":checked")) {
            $(".end-ce").hide();
        } else {
            $(".end-ce").show();
        }
    });

    $("#active-ac").change(function () {
        if ($(this).is(":checked")) {
            $(".end-ac").hide();
        } else {
            $(".end-ac").show();
        }
    });

    $(".edu-form").submit(function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.post("/User/Education", formData, function (data) {
            if (data.success) {
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
                $(".close").click();
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        })
    })

    $(".edit-education").click(function () {
        var actionUrl = $(this).data("action-url");
        $.get(actionUrl, function (data) {
            $("#educationModal .modal-content").html(data);
            $("#educationModal").modal("show");
        });
    });
    $(".edit-activity").click(function () {
        var actionUrl = $(this).data("action-url");
        $.get(actionUrl, function (data) {
            $("#educationModal .modal-content").html(data);
            $("#educationModal").modal("show");
        });
    });

    $(".edit-exp").click(function () {
        var actionUrl = $(this).data("action-url");
        $.get(actionUrl, function (data) {
            $("#educationModal .modal-content").html(data);
            $("#educationModal").modal("show");
        });
    });

    $(".edit-cer").click(function () {
        var actionUrl = $(this).data("action-url");
        $.get(actionUrl, function (data) {
            $("#educationModal .modal-content").html(data);
            $("#educationModal").modal("show");
        });
    });


    $(".exp-form").submit(function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.post("/User/Experience", formData, function (data) {
            if (data.success) {
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
                $("#experience").click()
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        })
    })

    $(".skill-form").submit(function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.post("/User/Skill", formData, function (data) {
            if (data.success) {
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
                $(".close").click();
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        })
    })

    $(".edit-skill").click(function () {
        var actionUrl = $(this).data("action-url");
        $.get(actionUrl, function (data) {
            $("#educationModal .modal-content").html(data);
            $("#educationModal").modal("show");
        });
    });

    $(".certificate-form").submit(function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.post("/User/Certificate", formData, function (data) {
            if (data.success) {
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
                $(".close").click()
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        })
    })

    $(".project-form").submit(function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.post("/User/Project", formData, function (data) {
            if (data.success) {
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
                $(".close").click()
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        })
    })

    $(".activity-form").submit(function (e) {
        e.preventDefault();
        var formData = $(this).serialize();
        $.post("/User/Activity", formData, function (data) {
            if (data.success) {
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
                $(".close").click()
                setTimeout(function () {
                    location.reload();
                }, 1500);
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        })
    })


}
$(".back-top").click(function () {
    $("html, body").animate({ scrollTop: 0 }, 0);
});
$(".hamburger").click(function () {
    $(this).toggleClass("active");
    $(".menu-mb").toggleClass("active");
    $(".overlay").toggleClass("active");
});
$(".expand-bar").click(function () {
    $(this).toggleClass('open');
    $(this).siblings('.sub-nav-mb').slideToggle();
});
$(".close-course").click(function () {
    var icon = $(this).find("i");
    $(this).toggleClass("active")
    icon.toggleClass("fa-bars fa-times");
    $(".nav-right").toggleClass("active")
});
$(".btn-scroll").click(function () {
    $("html, body").animate({ scrollTop: 0 }, "");
});
function QickLike() {
    function handleLikeJob(button, jobId, isLike) {
        var actionUrl = isLike ? "/Home/Likejob" : "/Home/UnLike";
        var buttonClassToRemove = isLike ? "likejob" : "unlikejob";
        var removeactive = isLike ? "" : "active";
        var buttonClassToAdd = isLike ? "unlikejob" : "likejob";
        var active = isLike ? "active" : "";
        $.post(actionUrl, { id: jobId }, function (data) {
            if (data.success) {
                button.removeClass(buttonClassToRemove).addClass(buttonClassToAdd);
                button.removeClass(removeactive).addClass(active);
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        });
    }

    $(document).on("click", ".likejob", function () {
        var button = $(this);
        var jobId = button.data("id");
        handleLikeJob(button, jobId, true);
    });
    $(document).on("click", ".unlikejob", function () {
        var button = $(this);
        var jobId = button.data("id");
        handleLikeJob(button, jobId, false);
    });
}
function QickStudy() {
    function handleLikeStudy(button, studyId, isLike) {
        var actionUrl = isLike ? "/Home/LikeStudy" : "/Home/UnStudy";
        var buttonClassToRemove = isLike ? "likejob" : "unlikejob";
        var removeactive = isLike ? "" : "active";
        var buttonClassToAdd = isLike ? "unlikejob" : "likejob";
        var active = isLike ? "active" : "";
        $.post(actionUrl, { id: studyId }, function (data) {
            if (data.success) {
                button.removeClass(buttonClassToRemove).addClass(buttonClassToAdd);
                button.removeClass(removeactive).addClass(active);
                new Notify({
                    status: 'success',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            } else {
                new Notify({
                    status: 'error',
                    text: data.message,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        });
    }

    $(document).on("click", ".likejob", function () {
        var button = $(this);
        var studyId = button.data("id");
        handleLikeStudy(button, studyId, true);
    });
    $(document).on("click", ".unlikejob", function () {
        var button = $(this);
        var studyId = button.data("id");
        handleLikeStudy(button, studyId, false);
    });
}
$(".box-notification").click(function () {
    $(".notification").toggleClass("active")
})
function updateFileName(input) {
    var selectedFileName = document.getElementById('selectedFileName');
    if (input.files && input.files.length > 0) {
        selectedFileName.textContent = input.files[0].name;
    } else {
        selectedFileName.textContent = 'Tải lên CV từ máy tính của bạn';
    }
}
function Sort(action) {
    $(document).on("click", "[data-filter]", function () {
        let url = $("input[name=currentUrl]").val();
        let status = $("[name=status]").val();
        let type = $("[name=type]").val(); 
      
        $("[name=status]").on("change", function () {
            status += $(this).val();
        });
        $("[name=type]").on("change", function () {
            type += $(this).val();
        });
        url = url.split('/').at(-1);
        window.history.pushState("", "", url);
        $.get(action, { type: type, status: status, url }, function (data) {
            $("#sort").empty();
            $("#sort").html(data);
        });
    });
}
function ShowLinkInput() {
    $(".input-link").slideToggle();
}


$(".add-item").click(function () {
    $(".close-info").click()
})
$("#form-contact").on("submit", function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        $.post("/Home/Contact", $(this).serialize(), function (data) {
            if (data.status) {
                new Notify({
                    status: 'success',
                    text: data.msg,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
                $("#form-contact").trigger("reset");
            } else {
                new Notify({
                    status: 'error',
                    text: data.msg,
                    effect: 'slide',
                    speed: 600,
                    showIcon: true,
                    showCloseButton: true,
                    autoclose: true,
                    autotimeout: 3000,
                    gap: 10,
                    distance: 20,
                    type: 3,
                    position: 'right bottom'
                });
            }
        });
    }
});
$('.toggle-icon').click(function () {
    $(this).find('i').toggleClass("fa-bars fa-xmark");
    $(".box-menu-repon").slideToggle()
});
$('.icon-reponsive').click(function () {
    $(this).toggleClass("active");
    $(this).closest('li').find('.ul-reponsive').slideToggle();
});
$('.icon-study').click(function () {
    $(this).closest('li').next('.study-repon').slideToggle(); $(this).toggleClass("active");
});