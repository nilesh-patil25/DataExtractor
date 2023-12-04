# VisualAcuityReportExtractor

Overview

The &quot;Extract PDF Content&quot; project aims to application that can efficiently extract text and
data from PDF (Portable Document Format) files. PDFs are a widely used format for document
sharing and archiving, and extracting their content is often necessary for various purposes, such as
data analysis, content indexing, and document management. This project will create a tool that
automates the extraction process to save time and effort for users.
Objectives:
Develop a user-friendly application capable of extracting text and structured data from PDF
files. Support various types of PDF documents, including text-based, image-based, and scanned
PDFs. Implement text recognition (OCR) to handle scanned and image-based PDFs. Provide options
to extract content from PDF files and ensure accuracy and reliability in content extraction. Allow
users to save the extracted content in the database.

Business Case:

Healthcare Providers: Extract patient data from medical records to create electronic health records
and store them in the database.
Technical Analysis:

Step 1: Azure Blob Storage Configuration
Ensure you have an Azure Blob Storage account set up with the necessary containers and
permissions to access and retrieve PDF documents.
Step 2: Azure Document Recognizer Configuration
Set up an Azure Document Recognizer service in your Azure subscription.
Create a Document Model in the Azure Document Recognizer service to define the structure of the
PDF you want to extract data from. Train this model using sample forms.
Step 3: Azure SQL Database Configuration
Create an Azure SQL Database or use an existing one to store the extracted data.
Define the database schema, including tables and columns, to match the structure of the extracted
data.
Step 4: Application Development
Develop a microservice in c#. This application will act as the intermediary between Azure Blob
Storage, Azure Document Recognizer, and Azure SQL Database.

Step 5: Process Results from Azure Document Recognizer
Receive the results from Azure Document Recognizer, which will typically include extracted data
fields and values from the forms.
Parse the results to extract the relevant data fields from the response.
Step 6: Store Extracted Data in Azure SQL Database
Establish a connection to your Azure SQL Database from your application.
Insert the extracted data into the appropriate tables and columns in the database. Ensure data
consistency and error handling.

Technical Design Diagram:

![image](https://github.com/nilvpatil/VisualAcuityReportExtractor/assets/110890280/8d7a5c6e-74d6-4e8d-814b-294cb6223c61)

Result Screenshots:

SQL Database:

![image](https://github.com/nilvpatil/VisualAcuityReportExtractor/assets/110890280/7aa646c8-3cb0-490c-a043-f7ce17d2d0f9)

Azure Document Form Recognizer :

![image](https://github.com/nilvpatil/VisualAcuityReportExtractor/assets/110890280/a23403ea-4a53-45b2-91ca-18c81e42b36c)

Issues and Risks:

To increase accuracy more samples to train the models in the Azure Form Recognizer.
Configurations:

PdfPath : Provide the path of the pdf which needs to be processed
ConnectionString : Connection String of the Database where data has to be stored
AzureSettings :
ApiKey : api key of the resource for which the form recognizer is configured
Endpoint : endpoint of the resource for which the form recognizer is configured
