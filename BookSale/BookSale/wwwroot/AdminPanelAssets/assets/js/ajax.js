$("#customerStatusButton").on("click", function(){
    var id = $(this).data("id");
    
    $.ajax({
        url: "/AdminPanel/Admin/UpdateStatus",
        type: "POST",
        data: { id: id },
        success: function(response){
           alert(response);
        },
        error: function(error){
            console.error("Error fetching customer status:", error);
        }
    })
});

$("#deleteCustomer").on("click", function(){
   var id = $(this).data("id");
   
   $.ajax({
       url: "/AdminPanel/Admin/DeleteCustomer",
       type: "POST",
       data: { id: id },
       success: function(response){
           alert(response);
       },
       error: function(error){
           console.error("Error deleting customer:", error);
       }
   })
});

$("#deleteBook").on("click", function(){
    var id = $(this).data("id");
    
    $.ajax({
        url: "/AdminPanel/Admin/DeleteBook",
        type: "POST",
        data: { id: id },
        success: function(response){
            alert(response);
        },
        error: function(error){
            console.error("Error deleting book:", error);
        }
    })
})