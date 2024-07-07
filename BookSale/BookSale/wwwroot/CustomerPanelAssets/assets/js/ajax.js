$(".BookEdit").on("click", function () {
    // Edit butonunu gizle
    $(this).hide();

    var id = $(this).data("id");

    // BookUpdateClose butonunu göster
    $(".BookUpdateClose[data-id='" + id + "']").show();

    // BookUpdate butonunu göster
    $(".BookUpdate[data-id='" + id + "']").show();
    $("input." + id).show();
});

$(".BookUpdateClose").on("click", function () {
    // BookUpdateClose butonunu gizle
    $(this).hide();

    var id = $(this).data("id");

    // BookUpdate butonunu gizle
    $(".BookUpdate[data-id='" + id + "']").hide();

    // Edit butonunu göster
    $(".BookEdit[data-id='" + id + "']").show();
    $("input." + id).hide();
});

$(".BookUpdate").on("click", function () {
    var id = $(this).data("id");
    var title = $("input." + id + "[name='title']").val();
    var subject = $("input." + id + "[name='subject']").val();
    var type = $("input." + id + "[name='type']").val();
    var price = $("input." + id + "[name='price']").val();
    var stock = $("input." + id + "[name='stock']").val();
    var author = $("input." + id + "[name='author']").val();

    $.ajax({
        url: "/CustomerPanel/Customer/UpdateBook",
        method: "POST",
        data: {bookId: id, title: title, subject: subject, type: type, price: price, stock: stock, author: author},
        success: function (response) {
            alert(response);
        },
        error: function (error) {
            alert(error);
        }
    })
});

$(".BookDelete").on("click", function () {
    var id = $(this).data("id");

    $.ajax({
        url: "/CustomerPanel/Customer/DeleteBook",
        method: "POST",
        data: {bookId: id},
        success: function (response) {
            alert(response);
        },
        error: function (error) {
            alert(error);
        }
    });

});

$("#insert_book").on("click", function () {
    $("#table").toggle();
    $("#insert_form").toggle();
});


/** **/
const altKonular = {
    "Romanlar": ["", "Aşk", "Macera", "Bilim Kurgu", "Fantezi", "Tarihî", "Polisiye", "Gerilim", "Dram"],
    "Edebiyat": ["", "Klasikler", "Modern Edebiyat", "Dünya Edebiyatı", "Türk Edebiyatı", "Şiir"],
    "Bilim": ["", "Popüler Bilim", "Fizik", "Kimya", "Biyoloji", "Matematik", "Astronomi", "Teknoloji"],
    "Tarih": ["", "Antik Tarih", "Orta Çağ Tarihi", "Osmanlı Tarihi", "Cumhuriyet Tarihi", "Dünya Tarihi", "Savaş Tarihi"],
    "Kişisel Gelişim": ["", "Motivasyon", "Liderlik", "Psikoloji", "Zaman Yönetimi", "İlişki Yönetimi", "Sağlık ve Zindelik"],
    "Sanat": ["", "Resim", "Müzik", "Tiyatro", "Sinema", "Fotoğrafçılık"],
    "Çocuk Kitapları": ["", "Masallar", "Eğitici Kitaplar", "Çizgi Romanlar", "Fabl"],
    "Din": ["", "İslam", "Hristiyanlık", "Yahudilik", "Doğu Dinleri", "Dinî Hikayeler"],
    "Hobi ve Eğlence": ["", "Yemek Tarifleri", "Seyahat", "Bahçecilik", "El Sanatları", "Spor"],
    "Akademik": ["", "Eğitim", "Hukuk", "Tıp", "Mühendislik", "Sosyal Bilimler"]
};

$('#anaKonu').change(function () {
    const secilenAnaKonu = $(this).val();
    const $altKonu = $('#altKonu');

    // Alt konuları temizle
    $altKonu.empty();

    // Yeni alt konuları ekle
    if (altKonular[secilenAnaKonu]) {
        altKonular[secilenAnaKonu].forEach(function (konu) {
            $altKonu.append(new Option(konu, konu));
        });
    } else {
        $altKonu.append(new Option('Seçiniz', ''));
    }
});

$('input[type=file]').change(function () {
    var input = $(this)[0];
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#book_img').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]); // Resmi base64 formatında oku
    }
});

$('#book_insert_form').submit(function (event) {
    event.preventDefault(); 
    
    var formData = new FormData(this);

    $.ajax({
        url: '/CustomerPanel/Customer/AddBook/', 
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            alert('Başarıyla gönderildi: ', response);
        },
        error: function (xhr, status, error) {
            alert('Hata oluştu: ', error);
        }
    });
});

$(".saleSystem").on("click", function(){
   var id = $(this).data("id"); 
   
   $.ajax({
       url: "/CustomerPanel/Customer/UpdateSaleStatus/",
       method: "POST",
       data: {bookId: id},
       success: function (response) {
           alert(response);
       },
       error: function (error) {
           alert(error);
       }
   });
});

$("#profileUpdate").on("submit", function (event) {
   $.ajax({
       url: "/CustomerPanel/Customer/UpdateProfile/",
       method: "POST",
       data: $(this).serialize(),
       success: function (response) {
           alert(response);
       },
       error: function (error) {
           alert(error);
       }
   }) 
});