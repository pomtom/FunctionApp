using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using CsvHelper;

namespace DurableFunc
{
    public static class CsvUpload
    {
        //https://github.com/didourebai/CSVHelperExample/tree/master

        [FunctionName("CsvUpload")]
        public static void Run([BlobTrigger("inputcsv/{name}.csv", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            try
            {
                //var parser = new TextFieldParser(myBlob, Encoding.UTF8);
                //parser.TextFieldType = FieldType.Delimited;
                //parser.SetDelimiters(",");
                //parser.HasFieldsEnclosedInQuotes = true;
                //parser.TrimWhiteSpace = true;

                //int candidateid = 0;
                //List<CandidatesModel> candidatesList = new List<CandidatesModel>();
                //while (!parser.EndOfData)
                //{
                //    string[] row = parser.ReadFields();
                //    bool isHeader = (row[0] == "CandidateReference");
                //    if (isHeader)
                //        continue;

                //    var candidates = new CandidatesModel(row);
                //    candidates.Id = ++candidateid;
                //    candidatesList.Add(candidates);
                //}

                //InsertIntoCandidateEntity(candidatesList, name);

                try
                {
                    using (var reader = new StreamReader(myBlob, Encoding.Default))
                    using (var csv = new CsvReader(reader))
                    {
                        csv.Configuration.RegisterClassMap<CandidateMapper>();
                        var records = csv.GetRecords<CandidatesModel>().ToList();
                        InsertIntoCandidateEntity(records, name);
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    throw new Exception(e.Message);
                }
                catch (FieldValidationException e)
                {
                    throw new Exception(e.Message);
                }
                catch (CsvHelperException e)
                {
                    throw new Exception(e.Message);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                log.LogInformation($"Calling https://zixerapp.azurewebsites.net/api/ScheduleExam_HttpStart with {name}");

                BaseClient client = new BaseClient("https://zixerapp.azurewebsites.net/api/ScheduleExam_HttpStart");
                PartitionKeyGenerator keyGen = new PartitionKeyGenerator();
                keyGen.PartitionKey = new List<string> { name };
                string jsonSerialize = JsonConvert.SerializeObject(keyGen);
                var response = client.PostCallAsync("", jsonSerialize).GetAwaiter().GetResult();

                log.LogInformation(response.ResponseMessage);

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message == null ? ex.InnerException.ToString() : ex.Message);
            }
        }

        static void InsertIntoCandidateEntity(IEnumerable<CandidatesModel> candidateList, string partitionKey)
        {
            InsertIntoTableStore(candidateList.Select(candidate =>
            {
                var entity = new CandidatesEntity(partitionKey, candidate.CandidateReference);
                entity.CandidateReference = candidate.CandidateReference;
                entity.CompletionDate = candidate.CompletionDate;
                entity.Test = candidate.Test;
                entity.TestFormReference = candidate.TestFormReference;
                entity.CentreReference = candidate.CentreReference;
                entity.ScannedImage = candidate.ScannedImage;
                entity.IsWholeScriptMarkingOn = candidate.IsWholeScriptMarkingOn;
                entity.IsMarkedinSecureMarker = candidate.IsMarkedinSecureMarker;
                return entity;
            }));
        }

        static void InsertIntoTableStore(IEnumerable<CandidatesEntity> entities)
        {
            TableStorage tblStore = new TableStorage(Constants.TableName);
            tblStore.InsertBatch<CandidatesEntity>(entities);
        }
    }
   
}
