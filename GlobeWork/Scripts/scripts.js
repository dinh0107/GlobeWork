AOS.init();

function HomeJs() {

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
        //fade: true,
        //cssEase: 'linear',
        slidesToScroll: 3,
        prevArrow: '<button class="previous-slie"><i class="fal fa-chevron-circle-left"></i></button>',
        nextArrow: '<button class="next-slie"><i class="fal fa-chevron-circle-right"></i></button>',
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 6,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    slidesToShow: 3,
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3
                }
            }
        ]
    });
    $('.list-contruy').slick({
        infinite: true,
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
                    slidesToShow: 6,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: false
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    slidesToShow: 3,
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3
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


}
function Lesson() {
    $('.stepItem_title').click(function () {
        var contentId = $(this).data('toggle');
        $('#' + contentId).slideToggle();
    });
}
$("#subcribe").on("submit", function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        $.post("/Home/SubcribeForm", $(this).serialize(), function (data) {
            if (data.status) {
                $.toast({
                    heading: 'Liên hệ thành công',
                    text: data.msg,
                    icon: 'success'
                });
                $("#subcribe").trigger("reset");
            } else {
                $.toast({
                    heading: 'Liên hệ không thành công',
                    text: data.msg,
                    icon: 'error'
                });
            }
        });
    }
});
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
$("#form-contact").on("submit", function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        $.post("/Home/Contact", $(this).serialize(), function (data) {
            if (data.status) {
                $.toast({
                    heading: 'Liên hệ thành công',
                    text: data.msg,
                    icon: 'success'
                });
                $("#form-contact").trigger("reset");
            } else {
                $.toast({
                    heading: 'Liên hệ không thành công',
                    text: data.msg,
                    icon: 'error'
                });
            }
        });
    }
});
$(".order-form").on("submit", function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        $.post("/Home/Order", $(this).serialize(), function (data) {
            if (data.status) {
                $.toast({
                    heading: 'Liên hệ thành công',
                    text: data.msg,
                    icon: 'success'
                });
                $(".order-form").trigger("reset");
            } else {
                $.toast({
                    heading: 'Liên hệ không thành công',
                    text: data.msg,
                    icon: 'error'
                });
            }
        });
    }
});

function Sort(action) {
    $(document).on("change", "[data-filter]", function () {
        let sort = "";
        var catIds = "";
        var topicIds = "";
        let url = $("input[name=currentUrl]").val();
        const title = $(".breadcrumb-item.active").text();

        $("[name=sort]:selected").map(function () {
            sort += $(this).val();
        });
        console.log(sort)
        $("[name=catId]:checked").map(function () {
            catIds += $(this).val() + '-';
        })
        catIds = catIds.slice(0, -1);
        $("[name=topicId]:checked").map(function () {
            topicIds += $(this).val() + '-';
        })
        topicIds = topicIds.slice(0, -1);

        url = url.split('/').at(-1);
        window.history.pushState(title, "", url);
        $.get(action, { sort: sort, catId: catIds, topicId: topicIds }, function (data) {
            $("#list-item-sort").empty();
            $("#list-item-sort").html(data);
        });
    });
}

$(".user").click(function () {
    $(".box-user").toggleClass("active")
})
function addCart(n) {
    $.post("/gio-hang/them-vao-gio-hang", { courseId: n }, function (data) {
        if (data.result === 2) {
            $.toast({
                text: "Bạn chưa đăng nhập",
                icon: "error",
                position: "bottom-center"
            });
            window.location = url;
        }
        else if (data.result === 1) {
            $.toast({
                text: "Thêm vào giỏ hàng thành công",
                icon: "success",
                position: "bottom-center"
            });
            $(".cart-header").attr("data-count", data.count);
        } else {
            $.toast(
                {
                    text: "Quá trình thực hiện không thành công",
                    icon: "error",
                    position: "bottom-center"
                });
        }
    });
}
function addToCart() {
    let n = $("[name=courseId]").val();
    let m = $("[name=currentUrl]").val();
    $.post("/gio-hang/them-vao-gio-hang", { courseId: n, currentUrl: m, quantity: 1 }, function (data) {
        if (data.result === 2) {
            $.toast({
                text: "Bạn chưa đăng nhập",
                icon: "error",
                position: "bottom-center"
            });
        }
        else if (data.result === 1) {
            $.toast({
                text: "Thêm vào giỏ hàng thành công",
                icon: "success",
                position: "bottom-center"
            });
            window.location.href = "/gio-hang/thanh-toan";
        } else {
            $.toast(
                {
                    text: "Quá trình thực hiện không thành công",
                    icon: "error",
                    position: "bottom-center"
                });
        }
    });
}
$(".remove-product").click(function () {
    if (confirm("Bạn có chắc chắn xóa sản phẩm này khỏi giỏ hàng?")) {
        const recordToDelete = $(this).attr("data-id");
        if (recordToDelete !== "") {
            $.post("/ShoppingCart/RemoveFromCart", { "id": recordToDelete }, function (data) {
                if (data.Status === 1) {
                    $("div[data-row='" + recordToDelete + "']").fadeOut();
                    window.location.reload();
                } else {
                    alert("Quá trình thực hiện không thành công");
                }
            });
        }
    }
});

$(".search").click(function () {
    $(".search-box").addClass("active")
})
$(".remove-box").click(function () {
    $(".search-box").removeClass("active")
})
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
window.addEventListener("scroll", function () {
    if ($(this).scrollTop() > 200) {
        $(".header-fixed").addClass('active');
        $(".btn-scroll").fadeIn(200);
    } else {
        $(".header-fixed").removeClass('active');
        $(".btn-scroll").fadeOut(200);
    }
});
$(".btn-scroll").click(function () {
    $("html, body").animate({ scrollTop: 0 }, "");
});