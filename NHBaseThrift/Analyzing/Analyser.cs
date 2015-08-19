namespace Gridsum.NHBaseThrift.Analyzing
{
    internal static class Analyser
    {
        public static ToBytesIntellectTypeAnalyser ToBytesAnalyser = new ToBytesIntellectTypeAnalyser();
        public static GetObjectIntellectTypeAnalyser GetObjectAnalyser = new GetObjectIntellectTypeAnalyser();
    }
}