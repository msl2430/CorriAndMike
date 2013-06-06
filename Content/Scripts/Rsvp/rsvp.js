$(function () {
    $("input[type=button], input[type=submit]").button();
    $("#submit-rsvp").removeAttr('disabled');
    $("input[type=button]").button();
    $("select").addClass("ui-corner-all");
    $("#rsvp-login").click(function() {
        var invitationId = $("#InvitationId").val();
        $("#login-error-msg").text('');
        $.get(checkInvitationUrl, { InvitationId: invitationId },
            function() {
                LoadPage(getInvitationUrl + "?invitationId=" + invitationId);
            }).fail(function() {
                $("#login-error-msg").text("Invalid invitation number. Please try again.");
            });
    });
    $("#InvitationId").defaultButton($("#rsvp-login"));    
});

$("#submit-rsvp").on('click', function () {
    $(this).attr('disabled', 'disabled');
    if ($("#Invitation_Email").val() == '') {
        $("#rsvp-message").text("Please provide an email address.");
        $("#Invitation_Email").css('background-color', 'red');
    } else if (!isValidEmailAddress($("#Invitation_Email").val())) {
        $("#rsvp-message").text("Email address most be in format 'me@example.com'.");
    } else {
        var yesGuests = [];
        $(".rsvp-slider").each(function () {
            if ($(this).slider("option", "value") == 1) {
                yesGuests.push($(this).attr('guestId'));
            }
        });
        if ($("#additional-guest-name").length > 0) {
            console.log($("#slider-plus-one").slider("option", "value") !== 2);
            if ($("#additional-guest-name").val() !== '' && $("#slider-plus-one").slider("option", "value") == 1) {
                var fullName = $("#additional-guest-name").val().split(' ');
                $.ajax({
                    type: "POST",
                    url: addSingleGuestUrl,
                    data: { FirstName: fullName[0], LastName: fullName[1] == null ? "" : fullName[1], Invitations: [ravenInvitationId] },
                    success: function(data) {
                        yesGuests.push(data.Id);
                        SubmitRsvp(yesGuests);
                    },
                    error: function(xhr) {
                        switch (xhr.status) {
                        case 409:
                            alert('Guest exists!');
                            break;
                        }
                    }
                });
            } else if ($("#additional-guest-name").val() !== '') {
                $("#rsvp-message").text("Please provide a name for your guest.");
                $("#additional-guest-name").css('background-color', 'red');
            } else {
                SubmitRsvp(yesGuests);
            }
        } else {
            SubmitRsvp(yesGuests);
        }
    }
});

function SubmitRsvp(yesGuests) {
    $.post(submitInvitationUrl, { invitationId: invitationId, email: $("#Invitation_Email").val(), attendingGuests: yesGuests.join(",") }, function () {
        if (yesGuests.length > 0) {
            $("#rsvp-response-container").html("<label class=\"response-msg\">Thank you!<br/>We can't wait to see you there!</label>");
        } else {
            $("#rsvp-response-container").html("<label class=\"response-msg\">Thank you!<br/>We are sorry you can't make it and we will miss you.</label>");
        }
    }).fail(function() {
        $("#rsvp-response-container").html("<label class=\"response-msg\">We encountered an error submiting your RSVP. Please try again later.</label>");
    });
}

function SetExistingGuestsSlider() {
    $(".rsvp-slider").slider({
        min: 1,
        max: 2,
        value: 2,
        change: function () {  }
    });
    var guests = attendingGuests.split(',');
    $(".rsvp-slider").each(function() {
        if ($.inArray($(this).attr('guestId'), guests) >= 0) {
            $(this).slider("option", "value", 1);
        }
    });
}