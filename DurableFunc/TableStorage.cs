using Microsoft.Azure;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DurableFunc
{
    /// <summary>
    /// Have error code references listed here https://docs.microsoft.com/en-us/rest/api/storageservices/table-service-error-codes
    /// </summary>
    public class TableStorage
    {
        /// <summary>
        /// Handle to the Azure table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        private CloudTable table { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorage"/> class.
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        public TableStorage(string TableName)
        {

            // Retrieve the storage account from the connection string.

            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage")); // Storage sdk has issue with getting keys from config file. Issue number 4010 [https://github.com/Azure/azure-sdk-for-net/issues/4010]

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=storageaccountrgukbc29;AccountKey=IV+Lp+naqQDGbgGIrNP7IfCd52AKJVjcLl7ubLs6FV0/N8YN179dg7OnkvTdGZQF/B3R28U2XR7kA7Jxj4s/wQ==;EndpointSuffix=core.windows.net");
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            this.table = tableClient.GetTableReference(TableName);
            if (!table.Exists())
            {
                table.CreateIfNotExists();
            }

        }

        /// <summary>
        /// Inserts the specified data.
        /// </summary>
        /// <typeparam name="T">DTO that inherits from TableEntity</typeparam>
        /// <param name="data">The data.</param>
        public void Insert<T>(T data) where T : TableEntity
        {
            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(data);

            // Execute the insert operation.
            this.table.Execute(insertOperation);
        }

        /// <summary>
        /// Inserts a list of table entries as a batch.
        /// </summary>
        /// <typeparam name="T">DTO that inherits from TableEntity</typeparam>
        /// <param name="data">The data.</param>
        public void InsertBatch<T>(IEnumerable<T> data) where T : TableEntity
        {
            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();

            foreach (var entityGroup in data.GroupBy(f => f.PartitionKey))
            {
                // Add both customer entities to the batch insert operation.
                foreach (TableEntity entity in entityGroup)
                {
                    if (batchOperation.Count < 100)
                    {
                        batchOperation.Add(TableOperation.InsertOrReplace(entity));
                    }
                    else
                    {
                        // Execute the batch operation.
                        table.ExecuteBatch(batchOperation);
                        batchOperation = new TableBatchOperation { TableOperation.InsertOrReplace(entity) };
                    }
                }
                table.ExecuteBatch(batchOperation);
                batchOperation = new TableBatchOperation();
            }
            if (batchOperation.Count > 0)
            {
                table.ExecuteBatch(batchOperation);
            }
        }

        /// <summary>
        /// Gets all data corresponding to a partition key.
        /// </summary>
        /// <typeparam name="T">DTO that inherits from TableEntity</typeparam>
        /// <param name="PartitionKey">The partition key.</param>
        /// <returns>A list of T that has the corresponding partion key</returns>
        public List<T> GetAll<T>(string PartitionKey) where T : TableEntity
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<CandidatesEntity> query = new TableQuery<CandidatesEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey));

            List<T> Results = new List<T>();
            // Print the fields for each customer.
            foreach (CandidatesEntity entity in table.ExecuteQuery(query))
            {
                Results.Add(entity as T);
                //Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                //    entity.Email, entity.PhoneNumber);
            }
            return Results;
        }

        /// <summary>
        /// Gets the single.
        /// </summary>
        /// <typeparam name="T">DTO that inherits from TableEntity</typeparam>
        /// <param name="PartitionKey">The partition key.</param>
        /// <param name="RowKey">The row key.</param>
        /// <returns></returns>
        public T GetSingle<T>(string PartitionKey, string RowKey) where T : TableEntity
        {

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(PartitionKey, RowKey);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            T result = null;
            // Print the phone number of the result.
            if (retrievedResult.Result != null)
            {
                result = retrievedResult.Result as T;
            }
            return result;
        }

        /// <summary>
        /// Replaces the specified partition key.
        /// </summary>
        /// <typeparam name="T">DTO that inherits from TableEntity</typeparam>
        /// <param name="PartitionKey">The partition key.</param>
        /// <param name="RowKey">The row key.</param>
        /// <param name="ReplacementData">The replacement data.</param>
        /// <param name="InsertOrReplace">The insert O replace.</param>
        public void Replace<T>(string PartitionKey, string RowKey,
               T ReplacementData, Boolean InsertOrReplace) where T : TableEntity
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(PartitionKey, RowKey);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            T updateEntity = retrievedResult.Result as T;

            if (updateEntity != null)
            {
                ReplacementData.PartitionKey = updateEntity.PartitionKey;
                ReplacementData.RowKey = updateEntity.RowKey;

                // Create the InsertOrReplace TableOperation
                TableOperation updateOperation;
                if (InsertOrReplace)
                {
                    updateOperation = TableOperation.InsertOrReplace(ReplacementData);
                }
                else
                {
                    updateOperation = TableOperation.Replace(ReplacementData);
                }

                // Execute the operation.
                table.Execute(updateOperation);

                Console.WriteLine("Entity updated.");
            }

            else
                Console.WriteLine("Entity could not be retrieved.");
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <typeparam name="T">DTO that inherits from TableEntity</typeparam>
        /// <param name="PartitionKey">The partition key.</param>
        /// <param name="RowKey">The row key.</param>
        /// <param name="ReplacementData">The replacement data.</param>
        public void DeleteEntry<T>(string PartitionKey, string RowKey, T ReplacementData) where T : TableEntity
        {

            // Create a retrieve operation that expects a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(PartitionKey, RowKey);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a CustomerEntity.
            T deleteEntity = retrievedResult.Result as T;

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                table.Execute(deleteOperation);

                Console.WriteLine("Entity deleted.");
            }

            else
                Console.WriteLine("Could not retrieve the entity.");

        }

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public void DeleteTable()
        {
            // Delete the table it if exists.
            table.DeleteIfExists();
        }
    }
}
