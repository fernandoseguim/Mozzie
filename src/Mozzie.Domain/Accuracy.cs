namespace Mozzie.Domain
{
    public class Accuracy
    {
        public Accuracy(float level) => Level = level;

        public static Accuracy Low => new Accuracy(25f);
        public static Accuracy Medium => new Accuracy(50f);
        public static Accuracy High => new Accuracy(75f);

        public float Level { get; }

        public bool IsLow => Level <= Low.Level;
        public bool IsMedium => Level <= Medium.Level;
        public bool IsHigh => Level >= High.Level;

        public static Accuracy GetAccuracy(float value)
        {
            if (value <= Low.Level) return Low;
            if (value <= Medium.Level) return Medium;
            return High;
        }
    }
}
