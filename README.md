# Contact Microservices

## Kullanılan Teknolojiler
- MassTransit
- ClosedXML
- EntityFrameworkCore
- Swashbuckle
- PostgreSQL

## Proje Açıklamaları

Users servisinde kişileri ve kişi iletişim bilgilerini getirme,silme ve ekleme işlemleri sağlanmıştır.
Report servisinden /createReport endpointine gidildiğinde Users servisine gider.
Rapor istekleri asenkron çalışmaktadır. 
Kullanıcı bir rapor oluşturmak istediğinde, darboğaz yaşamamak adına sistem raporu rabbitmq kuyruğuna aktarır. 
Rapor tamamlandığında ise kullanıcı raporlarını /getReports endpointi üzerinden raporun durumunu "Completed" olarak görebilmektedir.
Raporlar ReportsFiles klasörü altında Report-{raporun oluşturlma tarihi}.xlsx olarak raporun oluşturulma tarihiyle export edilmektedir.
Report servisten Users servise gidebilmek için yapılan konfigürasyon appsettings.json içerisinde ClientConfig propertysi içerisinde tutulmaktadır.

## EndPointler

<img src="https://github.com/tahsincanpolat/Contact-microservices/blob/main/reports-endpoints.jpg" width="auto">
<img src="https://github.com/tahsincanpolat/Contact-microservices/blob/main/users-endpoints.jpg" width="auto">


