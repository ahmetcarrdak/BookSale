$(".Delete-Cart-Book").on("click", function (e) {
    var button = $(this);
    var x1 = $(this).data("x1");
    var x2 = $(this).data("x2");
    var x3 = $(this).data("x3");
    var x4 = $(this).data("x4");
    var x5 = $(this).data("x5");

    $.ajax({
        url: "/MemberPanel/Member/removeFromCart",
        method: "POST",
        data: {
            x1: x1,
            x2: x2,
            x3: x3,
            x4: x4,
            x5: x5
        },
        success: function (response) {
            //alert(response);
            if (response === 'success') {
                button.closest('tr').remove();
                refreshCartTotal();
            }
        },
        error: function (error) {
            alert(error)
        }
    });
});


$(".book_cart_insert").on("click", function (e) {
    var button = $(this);
    var x1 = $(this).data("x1");
    var x2 = $(this).data("x2");
    var x3 = $(this).data("x3");
    var x4 = $(this).data("x4");
    var x5 = $(this).data("x5");

    $.ajax({
        url: '/MemberPanel/Member/removeFromPassCart/',
        method: 'POST',
        data: {
            x1: x1,
            x2: x2,
            x3: x3,
            x4: x4,
            x5: x5
        },
        success: function (response) {
            if (response === "success") {
                button.closest('tr').remove();
                refreshPassCartTotal();
            }
        },
        error: function (xhr, status, error) {
            alert(error);
        }
    });
});

$("#profileForm").on("submit", function (e) {
    $.ajax({
        url: '/MemberPanel/Member/ProfileUpdate/',
        method: 'POST',
        data: $(this).serialize(),
        success: function (response) {
            if (response === "success") {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Update Is Success",
                    showConfirmButton: false,
                    timer: 1500
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

$("#MemberPanelCartSale").on("click", function (e) {
    $.ajax({
        url: '/MemberPanel/Member/CompletePurchase/',
        method: 'POST',
        data: $(this).serialize(),
        success: function (response) {
            if (response === "success") {
                Swal.fire({
                    position: "top-end",
                    icon: "success",
                    title: "Sale Is Success",
                    showConfirmButton: false,
                    timer: 1500
                });
                refreshCartTotal();
                $('table tbody tr').remove();
            } else {
                alert(response);
            }
        },
        error: function (xhr, status, error) {
            alert(error);
        }
    });
});


// Refresh Total Price
function refreshCartTotal() {
    $.ajax({
        url: '/MemberPanel/Member/CartTotal',
        type: 'GET',
        success: function (total) {
            $(".category").html("Total Price: $" + total);
        },
        error: function () {
            alert("Toplam fiyat güncellenirken bir hata oluştu.");
        }
    });
}

function refreshPassCartTotal() {
    $.ajax({
        url: '/MemberPanel/Member/PassCartTotal',
        type: 'GET',
        success: function (total) {
            $(".category").html("Total Price: $" + total);
        },
        error: function () {
            alert("Toplam fiyat güncellenirken bir hata oluştu.");
        }
    });
}


// Log Out Button Click Event
$('#logOut').on("click", function () {
    $.ajax({
        url: '/MemberPanel/Member/LogOut/',
        type: 'GET',
        progress: function () {
            $('a').disabled();
        },
        success: function () {
            window.location.reload();
        },
        error: function () {
            alert("Oturum kapatılırken bir hata oluştu.");
        }
    });
});