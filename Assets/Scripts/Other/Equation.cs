namespace LumosLabs.Raindrops
{
    /// <summary>
    /// A single equation holds data associated with each droplet.
    /// Guessing correct answer gives player points.
    /// </summary>
    public class Equation
    {
        public enum Operator
        {
            ADD = 0,
            SUBTRACT,
            MULTIPLY,
            DIVIDE
        }

        #region Public fields
        public Operator OperatorType
        {
            get { return operatorType; }
        }
        public int OperandA
        {
            get { return operandA; }
        }
        public int OperandB
        {
            get { return operandB; }
        }
        public int Answer
        {
            get { return answer; }
        }
        #endregion

        #region Private fields
        private readonly Operator operatorType;
        private readonly int operandA;
        private readonly int operandB;
        private readonly int answer;
        #endregion

        public Equation(Operator operatorType, int operandA, int operandB)
        {
            this.operatorType = operatorType;
            this.operandA = operandA;
            this.operandB = operandB;

            switch (operatorType)
            {
                case Operator.ADD:
                    answer = operandA + operandB;
                    break;
                case Operator.DIVIDE:
                    answer = operandA / operandB;
                    break;
                case Operator.MULTIPLY:
                    answer = operandA * operandB;
                    break;
                case Operator.SUBTRACT:
                    answer = operandA - operandB;
                    break;
            }
        }

        public bool CheckAnswer(int guessedAnswer)
        {
            return answer == guessedAnswer;
        }

        public int GetAnswer()
        {
            return answer;
        }

        public override string ToString()
        {
            string operatorString = "";

            switch (operatorType)
            {
                case Operator.ADD:
                    operatorString = "+";
                    break;
                case Operator.DIVIDE:
                    operatorString = "%";
                    break;
                case Operator.MULTIPLY:
                    operatorString = "*";
                    break;
                case Operator.SUBTRACT:
                    operatorString = "-";
                    break;
            }

            return string.Format("{0}{1}{2}", operandA, operatorString, operandB);
        }
    }
}