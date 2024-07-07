$("#signUp").on("submit", function () {
    //alert();
    $.ajax({
        url: "/AuthPanel/Auth/CustomerRegister/",
        method: 'POST',
        data: $(this).serialize(),
        success: function (response) {
            if (response === "success") {
                Swal.fire({
                    title: "Success",
                    text: "Registration successful! Thank you",
                    icon: "success"
                });
                $("#signUp")[0].reset();
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
    })
});

$("#signIn").on("submit", function(){
   $.ajax({
       url: "/AuthPanel/Auth/CustomerLogin/",
       method: 'POST',
       data: $(this).serialize(),
       success: function (response) {
           if (response === "success") {
               Swal.fire({
                   title: "Success",
                   text: "Login successful! Thank you",
                   icon: "success"
               });
               $("#signUp")[0].reset();
               window.location = "/CustomerPanel/Customer/Index";
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
   }) 
});