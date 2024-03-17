namespace DigitClassifier.Models
{
    public class CalculatedResult
    {
        public int Label { get; private set; }
        public double Result { get; private set; }

        public CalculatedResult(int lable, double result)
        {
            Label = lable;
            Result = result;
        }
    }
}