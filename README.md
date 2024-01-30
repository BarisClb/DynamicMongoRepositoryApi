# DynamicMongoRepositoryApi
This API leverages MongoDB's Document-Based nature to dynamically manipulate Databases, Collections and Documents without predefined Models.

### Languages

Türkçe versiyonunu [burada](https://github.com/BarisClb/DynamicMongoRepositoryApi/blob/master/README_TR.md) okuyabilirsiniz.

### Anchor Links:

- [Setting up the Project](#setting-up-the-project)
  - [Requirements](#requirements)
- [Using the API](#using-the-api)
  - [Secret Key](#secret-key)
  - [Request Body](#request-body)
  - [ByFields Methods](#byfields-methods)
  - [Update Methods](#update-methods)
  - [Response Body](#response-body)

### Setting up the Project

#### &nbsp;Requirements

- Visual Studio

##### &nbsp; Optional

- Docker

##### &nbsp; If you already have MongoDb up and running in your environment

&emsp;&nbsp; "mongodb://localhost:27017" is our default connection string. If your MongoDB is exposed on a different port or requires user authentication, you'll need to modify the Appsettings.json file (Production or Development, depending on your choice of ASPNETCORE_ENVIRONMENT). Lastly, you can set WebApi as the Startup-Project and run it.

##### &nbsp; If you don't have MongoDb

&emsp;&nbsp; A docker-compose file is included inside the Solution. If you set this as the Startup-Project and run it, it will create an instance of MongoDb and the Project itself via Docker. (If you're not familiar with Docker and don't have it installed on your computer, you may need additional setups, such as Docker-Desktop).  
  
&emsp;&nbsp; I've configured the docker-compose to include a volume for MongoDB. If you prefer your Mongo data to not persist, you can delete or comment out volume-related lines in the docker-compose.yml and docker-compose.override.yml files.  
  
&emsp;&nbsp; The project will be running on HTTP and it will be exposed on port 8008. MongoDB will be exposed on port 27017. If these ports are already in use, you can modify these settings in the launchSettings.json, appsettings.json (depending on the ASPNETCORE_ENVIRONMENT), and docker-compose files.

### Using the API

##### &nbsp; The Request url

- Base Url: (by default) http://localhost:8008
- Controller: /api/db
- Endpoints: /{databaseName}/{collectionName}

&emsp;&nbsp; These two parameters and the RequestBody are what make this project Dynamic. You can choose the Collection and even the Database name with your request. If you're working on different projects, you can simply change the Database Name parameter and everything will function correctly without requiring to do any other configurations. You don't need to connect to the MongoDb and create the Database or the Collection beforehand.

##### &nbsp; Example - Get Request

- http://localhost:8008/api/db/testdb/testcollection/getAll

#### &nbsp;Secret Key

&emsp;&nbsp; Endpoints will require the Header with the following key: "api-key". The default value is set to "secret-key". If you want to change the value, you can do so by modifying the appsettings.json files. If you prefer not working with the Header, you can navigate to the DynamicMongoRepositoryController and remove or comment out the line that has [ServiceFilter(typeof(DynamicMongoRepositoryApiSecretKeyHandler))].

#### &nbsp;Request Body

&emsp;&nbsp; Another aspect of the Project that makes it Dynamic. Rather than a predefined model, you only need to send a valid JSON Object.

##### &nbsp; Example

- { "_id": 1, "id": 1, "Name": "Product1", "Category": "Category1", "Price": 24.5, "Quantity": 6 }

##### &nbsp; Important Note

&emsp;&nbsp; MongoDb Collections use "_id" as their Primary Key. This is also the field that is used for "ById" Methods. If you don't send it when Inserting a Document, Mongo will create a random ObjectId type Id for the said Document (which will be processed as a string in our Methods). But if you send it like it's shown in the example, your value will be the Primary Key instead.

#### &nbsp;ByFields Methods

&nbsp; Three Keywords are introduced for this Methods:  
- "$eq" (equals)
- "$gt" (greater than)
- "$lt" (less than)

&nbsp; You need to place Keywords in front of the field Keys; otherwise, it will not be processed.

##### &nbsp; Examples

- { "$eqid": 1 }
  - id equals to 1
- { "$gtPrice": 6 }
  - Price is greater than 6  
- { "$ltPrice": 20 }
  - Price is less than 20  
- { "$eqCategory": "Category1", "$gtQuantity": 5, "$ltPrice": 20 }
  - Category equals to Category1 and Quantity is greater than 5 and Price is less than 20
- { "Price": 20 }
  - Invalid Comparison  

##### Important Note

&emsp;&nbsp; Case sensitivity; _id can't be replaced by _Id. The same rule applies to the user defined fields, if you have an Object with the field { "Price": 5 }, requesting { "$gtprice": 4 } will not retrieve that Document.

#### &nbsp;Update Methods

##### &nbsp; UpdateById

&emsp;&nbsp; UpdateById Method will only update the fields that are sent and doesn't have Keywords in front of the Keys. If the Document is { "_id": 1, "Name": "Product1", "Price": 25 } and you have sent { "Price": 20 } along with the Id 1 to the UpdateById Method, the updated Document will be: { "_id": 1, "Name": "Product1", "Price": 20 }.

##### &nbsp; UpdateByFields

&emsp;&nbsp; UpdateByFields Method will only update the fields that are sent and doesn't have Keywords in front of the Keys. If you have these Documents { "_id": 1, "Name": "Product1", "Price": 25, "Quantity": 6 }, { "_id": 2, "Name": "Product2", "Price": 30, "Quantity": 4 }, { "_id": 3, "Name": "Product3", "Price": 34, "Quantity": 7 }, { "_id": 4, "Name": "Product4", "Price": 42, "Quantity": 9 } and you have sent { "$gtQuantity": 5, "$ltQuantity": 9, "Price": 20 } to the UpdateByFields Method, the updated Documents will be: { "_id": 1, "Name": "Product1", "Price": 20, "Quantity": 6 }, { "_id": 2, "Name": "Product2", "Price": 30, "Quantity": 4 }, { "_id": 3, "Name": "Product3", "Price": 20, "Quantity": 7 }, { "_id": 4, "Name": "Product4", "Price": 42, "Quantity": 9 }.

#### Response Body

&nbsp; Project has a standard Response, defined as ApiResponse.

- Example of an ApiResponse: { "data": object, "statusCode": int, "isSuccess": boolean, "error": { "apiErrorMessage": string, "exception": string, "exceptionMessage": string, "requestBody": object } }
- Example of a Successful Response: { "data": { "_id": 1, "Name": "Product1", "Category": "Category1", "Price": 24.5, "Quantity": 6 }, "statusCode": 200, "isSuccess": true, "error": null }
- Example of a Fail Response: { "data": null, "statusCode": 500, "isSuccess": false, "error": { "apiErrorMessage": "Unexpected Api error.", "exception": "Timeout", "exceptionMessage": "Timeout.", "requestBody": { "Price": 20 } } }
