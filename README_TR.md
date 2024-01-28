# DynamicMongoRepositoryApi
Bu API, MongoDB'nin Döküman-Tabanlı yapısından faydalanarak, önceden tanımlanmış modellere gerek duymadan Database, Koleksiyon ve Dökümanlar ile Dinamik olarak çalışmaktadır.

### Languages

English version can be read [here](https://github.com/BarisClb/DynamicMongoRepositoryApi/blob/master/README.md).

### Anchor Links:

- [Proje Kurulumu](#proje-kurulumu)
  - [Gereksinimler](#gereksinimler)
- [API Kullanımı](#api-kullanimi)
  - [Secret Key](#secret-key)
  - [Request Body](#request-body)
  - [ByFields Methodları](#byfields-methodları)
  - [Update Methodları](#update-methodları)
  - [Response Body](#response-body)

### Proje Kurulumu

#### &nbsp;Gereksinimler

- Visual Studio

##### &nbsp; Tercihen

- Docker

##### &nbsp; Çalışan bir MongoDb'niz mevcut ise

&emsp;&nbsp; "mongodb://localhost:27017" bu proje için varsayılan bağlantıdır. Çalışan MongoDB farklı bir port üzerinde yayınlanıyor veya kullanıcı kimlik doğrulaması gerekiyor ise, Appsettings.json dosyasını düzenlemeniz gerekir (ASPNETCORE_ENVIRONMENT seçiminize bağlı olarak (Production veya Development)).

##### &nbsp; Eğer çalışan bir MongoDb'niz mevcut değil ise

&emsp;&nbsp; Solution içerisinde docker-compose dosyası tanımlanmıştır. WebApi'yi Startup-Project olarak seçip çalıştırabilirsiniz. Docker hakkında bilgi sahibi değilseniz ve bilgisayarınızda da yüklü değil ise, Docker-Desktop gibi ek kurulumlar gerekebilir).  
  
&emsp;&nbsp; Docker-compose, MongoDB için bir Volume'e bağlanacak şekilde yapılandırılmıştır. Mongo verilerinizin kalıcı olmasını tercih etmiyorsanız, docker-compose.yml ve docker-compose.override.yml dosyalarındaki Volume ilgili satırları silebilir veya yoruma alabilirsiniz.  
  
&emsp;&nbsp; Proje HTTP ile ve 8008 numaralı Port üzerinden yayınlanacaktır. MongoDB ise 27017 numaralı port üzerinden yayınlanacaktır. Bu bağlantı noktaları kullanımdaysa, bu ayarları launchSettings.json, appsettings.json (ASPNETCORE_ENVIRONMENT'a bağlı olarak) ve docker-compose dosyalarında değiştirebilirsiniz.

### API Kullanımı

##### &nbsp; Request URL

- Base Url: (by default) http://localhost:8008
- Controller: /api/db
- Endpoints: /{databaseName}/{collectionName}

&emsp;&nbsp; Bu iki Parametre Projeyi Dinamik yapan özelliklerden bazılarıdır. Koleksiyon ve hatta Database ismini gönderdiğiniz Request ile seçebilirsiniz. Farklı projelerde çalışıyorsanız, Database İsmi parametresini değiştirebilirsiniz ve başka bir ayarlama yapmaya gerek kalmadan her şey olması gerektiği gibi çalışacaktır. MongoDb'ye bağlanarak, bir Database veya Koleksiyon oluşturma ihtiyacı duymayacaksınız.

##### &nbsp; Örnek - Get Request

- http://localhost:8008/api/db/testdb/testcollection/getAll

#### &nbsp;Secret Key

&emsp;&nbsp; Endpointler öncelikle "api-key" Header'ını kontrol eder. Bu Header için varsayılan değer "secret-key" olarak belirlenmiştir. Eğer bu değeri değiştirmek istiyorsanız, appsettings.json dosyalarında değişiklik yapabilirsiniz. Eğer Header kontrolünü kullanmak istemiyorsanız, DynamicMongoRepositoryController'a giderek, [ServiceFilter(typeof(DynamicMongoRepositoryApiSecretKeyHandler))]'ın bulunduğu satırı silebilir veya yoruma alabilirsiniz.

#### &nbsp;Request Body

&emsp;&nbsp; Projeyi Dinamik yapan özelliklerden bir diğeri. Önceden tanımlanmış Modeller yerine geçerli bir JSON Objesi göndermek yeterlidir.

##### &nbsp; Örnek

- { "_id": 1, "id": 1, "Name": "Product1", "Category": "Category1", "Price": 24.5, "Quantity": 6 }

##### &nbsp; Önemli Not

&emsp;&nbsp; MongoDb Koleksiyonları Primary Key olarak "_id" fieldını kullanır. Proje içerisinde de "ById" Methodlarında bu field kullanılır. Bir Döküman eklerken bu fieldı göndermediğiniz durumlarda MongoDb ObjectId tipinde rastgele bir _id atayacaktır, (Proje içerisindeki Methodlarda bir string olarak işlem görecektir). Eğer _id adında bir field gönderirseniz, sizin gönderdiğiniz değer Primary Key olarak kullanılacaktır.

#### &nbsp;ByFields Methodları

&nbsp; Bu Methodlar için üç adet Anahtar Kelime tanımlanmıştır: 
- "$gt" (greater than)
- "$lt" (less than)  
&nbsp; Bu Anahtar Kelimeler, Object içerisindeki Keylerin önüne koyulmalıdır, aksi takdirde karşılaştırma için "eşittir" olarak kabul edilecektir.

##### &nbsp; Örnekler

- { "$eqid": 1 }
  - id 1'e eşit ise
- { "$gtPrice": 6 }
  - Price 6'dan büyük ise
- { "$ltPrice": 20 }
  - Price 20'den küçük ise
- { "Category": "Category1", "$gtQuantity": 5, "$ltPrice": 20 }
  - Category Category1'e eşit ve Quantity is 5'ten ve Price 20'den küçük ise
- { "$Price": 20 }
  - Geçersiz Karşılaştırma  

##### Önemli Not

&emsp;&nbsp; Büyük-Küçük harf hassassiyeti; _Id, _id yerine geçemez. Aynı kural, kullanıcının tanımladığı fieldlar için de geçerlidir, eğer bir field { "Price": 5 } olarak belirlendi ise, { "$gtprice": 4 } karşılaştırması bu Dökümanı getirmeyecektir.

#### &nbsp;Update Methodları

##### &nbsp; UpdateById

&emsp;&nbsp; UpdateById Methodu yalnızca kendisine gönderilen ve başında Anahtar Kelime olmayan fieldlar için Update işlemi gerçekleştirecektir. Eğer Databasedeki Döküman { "_id": 1, "Name": "Product1", "Price": 25 } ise ve Update Methoduna Id 1 ile birlikte { "Price": 20 } gönderiliyor ise, güncellenmiş Döküman { "_id": 1, "Name": "Product1", "Price": 20 } olacaktır.

##### &nbsp; UpdateByFields

&emsp;&nbsp; UpdateByFields Methodu yalnızca kendisine gönderilen ve başında Anahtar Kelime olmayan fieldlar için Update işlemi gerçekleştirecektir. Eğer Databasedeki Dökümanlarınız { "_id": 1, "Name": "Product1", "Price": 25, "Quantity": 6 }, { "_id": 2, "Name": "Product2", "Price": 30, "Quantity": 4 }, { "_id": 3, "Name": "Product3", "Price": 34, "Quantity": 7 }, { "_id": 4, "Name": "Product4", "Price": 42, "Quantity": 9 } ise ve UpdateByFields Methoduna { "$gtQuantity": 5, "$ltQuantity": 9, "Price": 20 } isteğini gönderdiyseniz, güncellenmiş Dökümanlar: { "_id": 1, "Name": "Product1", "Price": 20, "Quantity": 6 }, { "_id": 2, "Name": "Product2", "Price": 30, "Quantity": 4 }, { "_id": 3, "Name": "Product3", "Price": 20, "Quantity": 7 }, { "_id": 4, "Name": "Product4", "Price": 42, "Quantity": 9 } olacaktır.

#### Response Body

&nbsp; Proje, ApiResponse olarak tanımlanan standart bir Response'a sahiptir.

- Response Body Ornegi: { "data": object, "statusCode": int, "isSuccess": boolean, "error": { "apiErrorMessage": string, "exception": string, "exceptionMessage": string, "requestBody": object } }
- Basarılı Response Ornegi: { "data": { "_id": 1, "Name": "Product1", "Category": "Category1", "Price": 24.5, "Quantity": 6 }, "statusCode": 200, "isSuccess": true, "error": null }
- Başarısız Response Ornegi: { "data": null, "statusCode": 500, "isSuccess": false, "error": { "apiErrorMessage": "Unexpected Api error.", "exception": "Timeout", "exceptionMessage": "Timeout.", "requestBody": { "Price": 20 } } }
