using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunc
{
    public static class Constants
    {
        public const string TableName = "CandidateTable";
    }

    public static class CsvHeaders
    {
        public const string CandidateReference = "CandidateReference";
        public const string CompletionDate = "CompletionDate";
        public const string Test = "Test";
        public const string TestFormReference = "TestFormReference";
        public const string CentreReference = "CentreReference";
        public const string ScannedImage = "ScannedImage";
        public const string IsWholeScriptMarkingOn = "IsWholeScriptMarkingOn";
        public const string IsMarkedinSecureMarker = "IsMarkedinSecureMarker";
    }
}
