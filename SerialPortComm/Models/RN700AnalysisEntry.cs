namespace RN700Communication.Models
{
    public class RN700AnalysisEntry
    {
        public decimal Whole { get; set; }
        public decimal WholePercent { get; set; }

        public decimal Cracked { get; set; }
        public decimal CrackedPercent { get; set; }

        public decimal Immature { get; set; }
        public decimal ImmaturePercent { get; set; }

        public decimal Pest { get; set; }
        public decimal PestPercent { get; set; }

        public decimal Colored { get; set; }
        public decimal ColoredPercent { get; set; }

        public decimal Dead { get; set; }
        public decimal DeadPercent { get; set; }


    }
}
