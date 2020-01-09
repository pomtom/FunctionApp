using CsvHelper.Configuration;

namespace DurableFunc
{
    public class CandidateMapper : ClassMap<CandidatesModel>
    {
        public CandidateMapper()
        {
            Map(m => m.CandidateReference).Name(CsvHeaders.CandidateReference);
            Map(m => m.CompletionDate).Name(CsvHeaders.CompletionDate);
            Map(m => m.Test).Name(CsvHeaders.Test);
            Map(m => m.TestFormReference).Name(CsvHeaders.TestFormReference);
            Map(m => m.CentreReference).Name(CsvHeaders.CentreReference);
            Map(m => m.ScannedImage).Name(CsvHeaders.ScannedImage);
            Map(m => m.IsWholeScriptMarkingOn).Name(CsvHeaders.IsWholeScriptMarkingOn);
            Map(m => m.IsMarkedinSecureMarker).Name(CsvHeaders.IsMarkedinSecureMarker);
        }
    }
}
