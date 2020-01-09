using CsvHelper.Configuration.Attributes;

namespace DurableFunc
{
    public class CandidatesModel
    {
        //public CandidatesModel(string[] row)
        //{
        //    int i = -1;

        //    CandidateReference = row[++i];
        //    CompletionDate = row[++i];
        //    Test = row[++i];
        //    TestFormReference = row[++i];
        //    CentreReference = row[++i];
        //    ScannedImage = row[++i];
        //    IsWholeScriptMarkingOn = row[++i];
        //    IsMarkedinSecureMarker = row[++i];
        //}

        
        [Name(CsvHeaders.CandidateReference)]
        public string CandidateReference { get; set; }

        [Name(CsvHeaders.CompletionDate)]
        public string CompletionDate { get; set; }

        [Name(CsvHeaders.Test)]
        public string Test { get; set; }

        [Name(CsvHeaders.TestFormReference)]
        public string TestFormReference { get; set; }

        [Name(CsvHeaders.CentreReference)]
        public string CentreReference { get; set; }

        [Name(CsvHeaders.ScannedImage)]
        public string ScannedImage { get; set; }

        [Name(CsvHeaders.IsWholeScriptMarkingOn)]
        public string IsWholeScriptMarkingOn { get; set; }

        [Name(CsvHeaders.IsMarkedinSecureMarker)]
        public string IsMarkedinSecureMarker { get; set; }
    }
}
