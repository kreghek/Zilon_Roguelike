namespace Zilon.Tournament.ApiGate.Models
{
    public class Measure
    {
        public MeasureValue AvgFrags { get; set; }

        public MeasureValue AvgIterationDuration { get; set; }

        public MeasureValue AvgScores { get; set; }

        public MeasureValue AvgTurns { get; set; }

        public string BotMode { get; set; }

        public string BotName { get; set; }

        public MeasureValue MaxFrags { get; set; }

        public MeasureValue MaxScores { get; set; }

        public MeasureValue MaxTurns { get; set; }

        public MeasureValue MinFrags { get; set; }

        public MeasureValue MinScores { get; set; }

        public MeasureValue MinTurns { get; set; }
    }
}