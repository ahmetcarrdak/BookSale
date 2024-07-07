const altKonular = {
    "Romanlar": ["","Aşk", "Macera", "Bilim Kurgu", "Fantezi", "Tarihî", "Polisiye", "Gerilim", "Dram"],
    "Edebiyat": ["","Klasikler", "Modern Edebiyat", "Dünya Edebiyatı", "Türk Edebiyatı", "Şiir"],
    "Bilim": ["","Popüler Bilim", "Fizik", "Kimya", "Biyoloji", "Matematik", "Astronomi", "Teknoloji"],
    "Tarih": ["","Antik Tarih", "Orta Çağ Tarihi", "Osmanlı Tarihi", "Cumhuriyet Tarihi", "Dünya Tarihi", "Savaş Tarihi"],
    "Kişisel Gelişim": ["","Motivasyon", "Liderlik", "Psikoloji", "Zaman Yönetimi", "İlişki Yönetimi", "Sağlık ve Zindelik"],
    "Sanat": ["","Resim", "Müzik", "Tiyatro", "Sinema", "Fotoğrafçılık"],
    "Çocuk Kitapları": ["","Masallar", "Eğitici Kitaplar", "Çizgi Romanlar", "Fabl"],
    "Din": ["","İslam", "Hristiyanlık", "Yahudilik", "Doğu Dinleri", "Dinî Hikayeler"],
    "Hobi ve Eğlence": ["","Yemek Tarifleri", "Seyahat", "Bahçecilik", "El Sanatları", "Spor"],
    "Akademik": ["","Eğitim", "Hukuk", "Tıp", "Mühendislik", "Sosyal Bilimler"]
};

$('#anaKonu').change(function() {
    const secilenAnaKonu = $(this).val();
    const $altKonu = $('#altKonu');

    // Alt konuları temizle
    $altKonu.empty();

    // Yeni alt konuları ekle
    if (altKonular[secilenAnaKonu]) {
        altKonular[secilenAnaKonu].forEach(function(konu) {
            $altKonu.append(new Option(konu, konu));
        });
    } else {
        $altKonu.append(new Option('Seçiniz', ''));
    }
});