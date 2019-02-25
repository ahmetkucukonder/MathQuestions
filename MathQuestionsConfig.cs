using Rocket.API;

namespace CapScroLL.MathQuestions
{
    public class MathQuestionsConfig : IRocketPluginConfiguration
    {
        public bool MathQuestionsEnabled, Addition, Subtraction;
        public double Interval;
        public decimal Reward;
        public string MessageColor, UnfavorableMessageColor;

        public void LoadDefaults()
        {
            MathQuestionsEnabled = true;
            Addition = true;
            Subtraction = true;
            Interval = 10;
            Reward = 5;
            MessageColor = "yellow";
            UnfavorableMessageColor = "red";
        }
    }
}