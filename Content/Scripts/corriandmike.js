var activeMenuItem;
    
$(function() {
    $("#top-bar").show("slide", 1000);
    setTimeout(function () { $("#menu-container").slideDown(700); }, 500);
    setTimeout(function () {
        $.post(homeUrl, {}, function (data) {
            $("#content-container").html(data);
            $("#content-container").show("slide", 700);            
        }); 
        $("#home-link").addClass("active-link");
        activeMenuItem = $("#home-link");
    }, 1500);
    $("#show-slideshow").click(function () {
        if ($("#menu-container").is(":visible"))
            MinimizeAllWindows();
        else {
            MaximizeAllWindows();
        }
    });
    $("input[type=button]").button();
    $("select").addClass("ui-corner-all");
    $("#show-slideshow").qtip({
        content: { text: 'Click here to hide all menus and view the slideshow.' },
        style: { width: 160, classes: 'qtip-slideshow' },
        position: {
            my: 'top-right',
            at: 'bottom-center'
        },
        effect: function () { $(this).animate({ width: 0, opacity: 0 }, { duration: 500 }); }
    });
    setTimeout(function () {
        $("#show-slideshow").qtip('show');
        setTimeout(function () { $("#show-slideshow").qtip('hide').qtip('disable'); }, 6000);
    }, 2000);
});

$(function () {
    $("#rsvp-countdown-link").click(function() { $("#rsvp-link").click(); });
    $(".nav-link").click(function () {
        var linkClicked = $(this);
        var linkId = linkClicked.attr('linkId');
        ClickMenuItem(linkClicked);
        var dataSrc;
        switch (linkId) {
            case homeLink:
                dataSrc = homeUrl; break;
            case rsvpLink:
                dataSrc = rsvpUrl; break;
            case aboutLink:
                dataSrc = aboutUrl; break;
            case engagementPartyLink:
                dataSrc = engagementPartyUrl; break;
        }
        LoadPage(dataSrc);
    });
});

function LoadPage(dataSrc) {
    $("#content-container").hide('slide', { direction: 'left' }, 700);
    setTimeout(function () {
        $.post(dataSrc, {}, function (data) {
            $("#content-container").html(data);
            $("#content-container").show('slide', 700);
        });
    }, 700);
}

function ClickMenuItem(linkClicked) {
    activeMenuItem = linkClicked;
    $("#menu-list>li a").removeClass("active-link");
    linkClicked.addClass("active-link");
}

function MinimizeAllWindows() {
    $("#content-container").hide('slide', { direction: 'left' }, 700);
    setTimeout(function () {
        activeMenuItem.removeClass("active-link");
        $("#menu-container").slideUp(700);
        setTimeout(function () { $("#top-bar").addClass("ui-corner-bl"); }, 710);
        $("#show-slideshow").html("<span class=\"ui-icon ui-icon-circlesmall-plus\"></span>&nbsp;<span style=\"text-decoration: underline;\">Show menu</span>");
    }, 710);
}
function MaximizeAllWindows() {
    $("#top-bar").removeClass("ui-corner-bl");
    $("#menu-container").slideDown(700);
    setTimeout(function () {
        activeMenuItem.addClass("active-link");
        $("#content-container").show('slide', 700);
        $("#show-slideshow").html("<span class=\"ui-icon ui-icon-circlesmall-minus\"></span>&nbsp;<span style=\"text-decoration: underline;\">Hide menu</span>");
    }, 710);
}

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    return pattern.test(emailAddress);
};

$.fn.defaultButton = function (btn) {
    $(this).keypress(function (e) {
        if (e.keyCode && e.keyCode == 13) {
            btn.click();
        }
    });
};

$(function () {
    $.supersized({
        // Functionality
        slideshow: 1,			// Slideshow on/off
        autoplay: 1,			// Slideshow starts playing automatically
        start_slide: 1,			// Start slide (0 is random)
        stop_loop: 0,			// Pauses slideshow on last slide
        random: 1,			// Randomize slide order (Ignores start slide)
        slide_interval: 7000,		// Length between transitions
        transition: 1, 			// 0-None, 1-Fade, 2-Slide Top, 3-Slide Right, 4-Slide Bottom, 5-Slide Left, 6-Carousel Right, 7-Carousel Left
        transition_speed: 2000,		// Speed of transition
        new_window: 1,			// Image links open in new window/tab
        pause_hover: 0,			// Pause slideshow on hover
        keyboard_nav: 1,			// Keyboard navigation on/off
        performance: 1,			// 0-Normal, 1-Hybrid speed/quality, 2-Optimizes image quality, 3-Optimizes transition speed // (Only works for Firefox/IE, not Webkit)
        image_protect: 1,			// Disables image dragging and right click with Javascript

        // Size & Position						   
        min_width: 0,			// Min width allowed (in pixels)
        min_height: 0,			// Min height allowed (in pixels)
        vertical_center: 0,			// Vertically center background
        horizontal_center: 1,			// Horizontally center background
        fit_always: 0,			// Image will never exceed browser width or height (Ignores min. dimensions)
        fit_portrait: 1,			// Portrait images will not exceed browser height
        fit_landscape: 1,			// Landscape images will not exceed browser width

        // Components							
        slide_links: 'blank',	// Individual links for each slide (Options: false, 'num', 'name', 'blank')
        thumb_links: 1,			// Individual thumb links for each slide
        thumbnail_navigation: 0,			// Thumbnail navigation
        slides: [			// Slideshow Images
                { image: 'Content/Images/Slideshow/levine1142.jpg', title: 'EngagementPhoto1' },
                { image: 'Content/Images/Slideshow/levine1003.jpg', title: 'EngagementPhoto2' },
                { image: 'Content/Images/Slideshow/levine1009.jpg', title: 'EngagementPhoto3' },
                { image: 'Content/Images/Slideshow/levine1014.jpg', title: 'EngagementPhoto4' },
                { image: 'Content/Images/Slideshow/levine1044.jpg', title: 'EngagementPhoto5' },
                { image: 'Content/Images/Slideshow/levine1054.jpg', title: 'EngagementPhoto6' },
                { image: 'Content/Images/Slideshow/levine1096.jpg', title: 'EngagementPhoto7' },
                { image: 'Content/Images/Slideshow/levine1100.jpg', title: 'EngagementPhoto8' },
                { image: 'Content/Images/Slideshow/levine1118.jpg', title: 'EngagementPhoto9' },
                { image: 'Content/Images/Slideshow/levine1124.jpg', title: 'EngagementPhoto10' },
                { image: 'Content/Images/Slideshow/levine1161.jpg', title: 'EngagementPhoto11' }
            ],

        // Theme Options			   
        progress_bar: 1,			// Timer for each slide							
        mouse_scrub: 0

    });
});