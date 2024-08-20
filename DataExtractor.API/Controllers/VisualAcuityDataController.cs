using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using DataExtractor.API.Helpers;
using DataExtractor.API.Repositories.Interfaces;
using DataExtractor.API.Models;
using DataExtractorAPI.Models;

namespace DataExtractor.API.Controllers
{

    #region Process File 

    /// <summary>
    /// Controller to send the pdf form as stream to Azure form and receive the analyzed result for processing it.
    /// Sends the result to Repository for storing it in the SQL storage
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VisualAcuityDataController : ControllerBase
    {
        private readonly IVisualTestResultRepository _visualTestRepository;
        private readonly IConfiguration _config;

        public VisualAcuityDataController(IVisualTestResultRepository visualTestRepository, IConfiguration config)
        {
            _visualTestRepository = visualTestRepository;
            _config = config;
        }

        [HttpGet("ExtractReport")]
        public async Task<ActionResult<VisualTestResult>> ExtractPdfData()
        {
            // Read API key and endpoint of the Azure resource having Form Recognizer
            string apiKey = AppSettingsHelper.GetValue(ConfigConstants.ApiKey);
            string endpoint = AppSettingsHelper.GetValue(ConfigConstants.Endpoint);

            // File path of the PDF to be processed
            string pdfFilePath = AppSettingsHelper.GetValue(ConfigConstants.PdfPath);

            string modelId = "Sample2Static_Model"; // Created in Azure Form Recognizer

            AzureKeyCredential credential = new AzureKeyCredential(apiKey);
            DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

            // Read File as stream and send it to Form Recognizer model
            using (Stream stream = new FileStream(pdfFilePath, FileMode.Open))
            {
                AnalyzeDocumentOperation operation = client.AnalyzeDocument(WaitUntil.Completed, modelId, stream);
                await operation.WaitForCompletionAsync();
                AnalyzeResult result = operation.Value;

                List<KeyValuePair<string, string>> allKeyValuePairs = new();
                List<List<TableData>> allTableDataList = new();

                // Reading key-value pairs from the result
                foreach (AnalyzedDocument document in result.Documents)
                {
                    foreach (KeyValuePair<string, DocumentField> fieldKvp in document.Fields)
                    {
                        allKeyValuePairs.Add(new KeyValuePair<string, string>(fieldKvp.Key, fieldKvp.Value.Content));
                    }
                }

                // Reading Table cells from the result and organizing it
                for (int i = 0; i < result.Tables.Count; i++)
                {
                    DocumentTable table = result.Tables[i];
                    List<TableData> tableDataList = new();

                    int j = 0;
                    foreach (DocumentTableCell? cell in result.Tables[i].Cells)
                    {
                        if (cell.Kind == DocumentTableCellKind.ColumnHeader)
                        {
                            tableDataList.Add(new TableData
                            {
                                ColumnHeader = cell.Content,
                                Content = null
                            });
                        }
                        else if (cell.Kind == DocumentTableCellKind.Content)
                        {
                            tableDataList[j++].Content = cell.Content;
                        }
                    }

                    allTableDataList.Add(tableDataList);
                }

                // List to store all the processed data to store it in the DB
                List<VisualTestResult> visionDataList = new();

                string laterality = string.Empty;   // OD OS OU
                string method = string.Empty;       // Snellen: OS Comments.Method
                string recordedBy = string.Empty;   // Recorded by = OS Comments.Taken by 
                string qualifier = string.Empty;    // Distance or Near

                // Process the received response from the Form Recognizer to create the data for storing
                for (int i = 0; i < allTableDataList.Count; i++)
                {
                    List<TableData>? tableDataList = allTableDataList[i];

                    var kvp = allKeyValuePairs[i];

                    if (kvp.Value != null)
                    {
                        if (kvp.Value.Contains("OD") || kvp.Value.Contains("OS") || kvp.Value.Contains("OU"))
                        {
                            laterality = GetLaterality(kvp.Value);
                            qualifier = GetDistanceOrNear(kvp.Value);
                        }

                        // Table contains first column as "Correction" means it is the comments table, so read taken recorded by and method from it.
                        if (tableDataList.First().ColumnHeader.ToLower().Contains("correction"))
                        {
                            for (int j = 0; j < tableDataList.Count; j++)
                            {
                                TableData? tableData = tableDataList[j];
                                if (tableData.ColumnHeader.ToLower() == "taken by")
                                {
                                    recordedBy = tableData.Content;
                                }
                                if (tableData.ColumnHeader.ToLower() == "method")
                                {
                                    method = tableData.Content;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < tableDataList.Count; j++)
                            {
                                TableData? tableData = tableDataList[j];

                                if (tableData.Content.ToLower().Contains("dva"))
                                    qualifier = "Distance";
                                if (tableData.Content.ToLower().Contains("nva"))
                                    qualifier = "Near";

                                // Create new model based on the data and add it to the list
                                var visualTestResultModel = new VisualTestResult
                                {
                                    VisionType = tableData.ColumnHeader,
                                    Measurement = tableData.Content,

                                    Laterality = laterality,
                                    Qualifier = qualifier,

                                    Method = method,
                                    RecordedBy = recordedBy
                                };
                                visionDataList.Add(visualTestResultModel);
                            }
                        }
                    }
                }

                await _visualTestRepository.BulkInsert(visionDataList);

                return Ok(visionDataList);
            }
        }
    }


    /// <summary>
    /// Method to get the laterality value from the response
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static string GetLaterality(string input)
        {
            Match match = Regex.Match(input, @"(OD|OS|OU)");
            return match.Success ? match.Value : "N/A";
        }

        /// <summary>
        /// Method to get the qualifier value from the response
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetDistanceOrNear(string input)
        {
            Match match = Regex.Match(input, @"(Distance|Near)");
            return match.Success ? match.Value : "N/A";
        }

        /// <summary>
        /// Method to get the qualifier value from the response
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
    }

    #endregion
}
