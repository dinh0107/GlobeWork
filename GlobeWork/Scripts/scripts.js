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
