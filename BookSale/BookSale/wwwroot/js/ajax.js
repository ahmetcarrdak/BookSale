$(".book_cart_insert").on("click", function () {
    var data = $(this).data("book");
    $.ajax({
        url: '/AddCartBook/',
        method: 'POST',
        data: {number: data},
        success: function (response) {
            if (response === "success") {
                Swal.fire({
                    title: "Success",
                    text: "The product has been added to the cart",
                    icon: "success"
                });
                $(".book_cart_insert")[0].reset();
            } else if (response === "update") {
                Swal.fire({
                    title: "Success",
                    text: "The product is already available, the number of products has been increased",
                    icon: "info"
                });
            } else if (response === "noSession") {
                Swal.fire({
                    title: "No Session",
                    text: "You must be logged in to add products to the cart",
                    icon: "info"
                });
            } else {
                alert(response);
            }
        },
        error: function (xhr, status, error) {
            alert(error);
        }
    });
});

$("#member-login-form").on("submit", function () {
    $.ajax({
        url: '/MemberLogin/',
        method: 'POST',
        data: $(this).serialize(),
        success: function (response) {
            if (response === "success") {
                Swal.fire({
                    title: "Success",
                    text: "Successfully entered!",
                    icon: "success"
                });
                $("#member-login-form")[0].reset();
                window.open('/MemberPanel/Member/Index', '_blank');
            } else if (response === "error") {
                Swal.fire({
                    title: "Ups",
                    text: "The record was not found!",
                    icon: "error"
                });
            } else {
                Swal.fire({
                    title: "Ups",
                    text: "Something went wrong",
                    icon: "info"
                });
            }
        },
        error: function (xhr, status, error) {
            alert(error);
        }
    });
});

$("#contact-form").on("submit", function () {
    $.ajax({
        url: '/ContactInsert/',
        method: 'POST',
        data: $(this).serialize(),
        success: function (response) {
            if (response === "success") {
                Swal.fire({
                    title: "Success",
                    text: "Your message has been received! Thank you",
                    icon: "success"
                });
                $("#contact-form")[0].reset();
            } else {
                Swal.fire({
                    title: "Ups",
                    text: response,
                    icon: "info"
                });
            }
        },
        error: function (xhr, status, error) {
            alert(error);
        }
    });
});

$("#member-register-form").on("submit", function () {

    var pass = $("#pass").val();
    var rpass = $("#rpass").val();

    if (pass === rpass) {
        $.ajax({
            url: '/MemberInsert/',
            method: 'POST',
            data: $(this).serialize(),
            success: function (response) {
                if (response === "success") {
                    Swal.fire({
                        title: "Success",
                        text: "Registration is successful! Thank you",
                        icon: "success"
                    });
                    $("#member-register-form")[0].reset();
                } else {
                    Swal.fire({
                        title: "Ups",
                        text: response,
                        icon: "info"
                    });
                }
            },
            error: function (xhr, status, error) {
                alert(error);
            }
        });
    } else {
        Swal.fire({
            title: "Info",
            text: "Passwords Do Not Match",
            icon: "info"
        });
    }
});

$("#sale_form").on("submit", function () {
    var notify = $(this).data("notify");
    $.ajax({
        url: '/SaleInsert/',
        method: 'POST',
        data:$(this).serialize(),
        success: function (response) {
            if (response === "Success") {
                Swal.fire({
                    title: "Success",
                    text: "Sale is successful! Thank you",
                    icon: "success"
                });
                window.location = "/Package/"
            } else {
                Swal.fire({
                    title: "Ups",
                    text: response,
                    icon: "info"
                });
            }
        },
        error: function (xhr, status, error) {
            alert(error);
        }
    });
});