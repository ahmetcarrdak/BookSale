# Online Kitap Satış Platformu (BookSale)

Bu proje, EldaSoft tarafından gerçekleştirilen eğitim sonucundan öğrencilerin gerçek dünya yazılım geliştirme deneyimi kazanmalarını sağlamak amacıyla geliştirilmiş bir online kitap satış platformudur. Proje, C# ve .NET kullanılarak, Entity Framework ORM ile veri tabanı işlemleri gerçekleştirilerek geliştirilmiştir. Ayrıca, jQuery ve AJAX teknolojileri front-end tarafında kullanılmıştır.

## Site İçeriği

Platform, kullanıcıların kitap ekleyebileceği, arayabileceği, satın alabileceği ve yönetebileceği bir çevrimiçi kitap satışı sağlar. Ayrıca, kullanıcılar; üye olabilir, sepet yönetimi yapabilir ve geçmiş siparişlerini görüntüleyebilir. Satın alma işlemleri üye olmadan da gerçekleştirilebilmektedir

## Alıcı Paneli İçeriği

Alıcılar, platformda kitapları arayabilir, sepetlerine ekleyebilir, sipariş verebilir ve geçmiş siparişlerini ve satın alımlarını görüntüleyebilir, Sepetten sildikleri ürünleri tekrar sepetlerine ekleyerek satın alabilirler.

## Satıcı Paneli İçeriği

Satıcılar, kitap ekleyebilir, düzenleyebilir, silebilir ve satış istatistiklerini görüntüleyebilirler. Ayrıca, aldıkları siparişleri yönetebilirler. Ek olarak satıcı kayıt olduktan sonra kitap ekleyebilmek için hesabının yönetici tarafından onaylanmasını beklemelidir

## Yönetici Paneli İçeriği

Yöneticiler, platformdaki tüm kullanıcıları (Alıcı & Satıcı), yüklenen kitapları, satın alımları, görüntüleyebilir . Satıcı ve alıcı hesaplarını onaylayabilir ve silebilirler [Hesap düzenlemesi (Kullancı adı değiştirme gibi) işlemler yapılamamaktadır]. Ayrıca, siparişleri yönetebilir ve raporlayabilirler.

## Teknolojiler

- **Backend:** C#, .NET Core, Entity Framework
- **Frontend:** HTML, CSS, JavaScript, jQuery, AJAX
- **Veri Tabanı:** SQL Server (Entity Framework ile)
- **Güvenlik:** Kimlik doğrulama ve yetkilendirme, OWASP güvenlik önlemleri, Hash şifre gizleme
- **Test:** Unit testler, otomatik test araçları
- **Dağıtım:** Azure, Docker

## Proje Geliştirme Süreci

Proje sürecinde kullanılan teknolojilerin yanı sıra, güvenlik, performans ve veri bütünlüğü gibi konular öncelikle ele alınmıştır. Her bir kullanıcı tipi için özelleştirilmiş panel ve işlevler geliştirilerek, kullanıcı deneyimi ön planda tutulmuştur.

## Nasıl Başlatılır?

1. **Gereksinimler**
   - Visual Studio / Rider 2022 veya üstü
   - .NET Core 8.0 SDK veya üstü
   - SQL Server Management Studio (SSMS)

2. **Kurulum**
   - Projeyi bilgisayarınıza klonlayın.
   - BookSale klasörü içinden BookSale.sql dosyasını alın
   - Veri tabanı bağlantı dizesini `appsettings.json` dosyasında güncelleyin.
   - Proje dosyalarını Visual Studio'da açın.
   - Paket bağımlılıklarını (eğer yüklü gelmezse) yükleyin ve projeyi derleyin.
   - Veri tabanını oluşturmak ve veri tabanı şemalarını güncellemek için migration'ları kullanın (`dotnet ef database update`).

3. **Çalıştırma**
   - Projeyi Visual Studio'da yada Rider'da başlatın.
   - Tarayıcınızda `https://localhost:{port}` adresinde uygulamayı görüntüleyin.

## Katkılar

Her türlü katkı ve geri bildirimleriniz için açığız! Lütfen GitHub üzerinden bir issue açın veya pull request gönderin.
