using UnityEngine;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Provides new equations based on current difficulty.
    /// </summary>
    public class EquationsManager
    {
        private RaindropsGameState gameState;

        public EquationsManager(RaindropsGameState gameState)
        {
            this.gameState = gameState;
        }

        /// <summary>
        /// Gets the next equation.
        /// </summary>
        /// <returns>The next random equation</returns>
        public Equation Next()
        {
            int operandA = 0;
            int operandB = 0;

            // Pick one of the available operators
            Equation.Operator op = gameState.CurrentDifficulty.AvailableOperators[Random.Range(0, gameState.CurrentDifficulty.AvailableOperators.Count)];

            // Based on that fill out the opernads with available ranges
            switch (op)
            {
                case Equation.Operator.ADD:
                    operandA = Random.Range(1, gameState.CurrentDifficulty.AddRanges.x + 1);
                    operandB = Random.Range(1, gameState.CurrentDifficulty.AddRanges.y + 1);
                    break;
                case Equation.Operator.DIVIDE:
                    operandB = Random.Range(1, gameState.CurrentDifficulty.DivRanges.y + 1);
                    operandA = operandB * Random.Range(1, gameState.CurrentDifficulty.DivRanges.x + 1);
                    break;
                case Equation.Operator.MULTIPLY:
                    operandA = Random.Range(1, gameState.CurrentDifficulty.MulRanges.x + 1);
                    operandB = Random.Range(1, gameState.CurrentDifficulty.MulRanges.y + 1);
                    break;
                case Equation.Operator.SUBTRACT:
                    operandA = Random.Range(1, gameState.CurrentDifficulty.SubRanges.x + 1);
                    operandB = Random.Range(1, gameState.CurrentDifficulty.SubRanges.y + 1);
                    if (operandB > operandA) 
                    {
                        // Swap the operands so we don't have negative values as answers
                        int temp = operandA;
                        operandA = operandB;
                        operandB = temp;
                    }
                    break;
            }

            return new Equation(op, operandA, operandB);
        }
    }
}