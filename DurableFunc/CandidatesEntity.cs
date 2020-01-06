using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunc
{
    public class CandidatesEntity : TableEntity
    {
        public CandidatesEntity()
        {

        }

        public CandidatesEntity(string partitionKey, int rowkey)
        {
            PartitionKey = partitionKey;
            RowKey = rowkey.ToString("00000000");
        }

        public int Id { get; set; }
        public string CandidateReference { get; set; }
        public string CompletionDate { get; set; }
        public string Test { get; set; }
        public string TestFormReference { get; set; }
        public string CentreReference { get; set; }
        public string ScannedImage { get; set; }
        public string IsWholeScriptMarkingOn { get; set; }
        public string IsMarkedinSecureMarker { get; set; }
    }
}
