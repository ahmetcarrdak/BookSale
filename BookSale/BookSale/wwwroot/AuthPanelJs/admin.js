$("#adminLogin").on("submit", function () {
   $.ajax({
       url: "/AuthPanel/Auth/AdminLogin/",
       method: 'POST',
       data: $(this).serialize(),
       success: function (response) {
           if (response === "success") {
               Swal.fire({
                   title: "Success",
                   text: "Login successful! Thank you",
                   icon: "success"
               });
               $("#adminLogin")[0].reset();
               window.location = "/AdminPanel/Admin/Index";
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