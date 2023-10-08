using System.Collections.Generic;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Represents a single difficulty used to create new equations.
    /// </summary>
    public class DifficultySetting
    {
        #region Public fields
        // Setting ranges for operations will mnake them available

        /// <summary>
        /// Operand ranges (A, B) for adding.
        /// </summary>
        public Vector2I AddRanges
        {
            get { return addRanges; }
            set { 
                addRanges = value;
                if (!availableOperators.Contains(Equation.Operator.ADD))
                    availableOperators.Add(Equation.Operator.ADD);
            }
        }
        /// <summary>
        /// Operand ranges (A, B) for subtracting.
        /// </summary>
        public Vector2I SubRanges
        {
            get { return subRanges; }
            set { 
                subRanges = value;
                if (!availableOperators.Contains(Equation.Operator.SUBTRACT))
                    availableOperators.Add(Equation.Operator.SUBTRACT);
            }
        }
        /// <summary>
        /// Operand ranges (A, B) for multiplying.
        /// </summary>
        public Vector2I MulRanges
        {
            get { return mulRanges; }
            set { 
                mulRanges = value;
                if (!availableOperators.Contains(Equation.Operator.MULTIPLY))
                    availableOperators.Add(Equation.Operator.MULTIPLY);
            }
        }
        /// <summary>
        /// Operand ranges (A, B) for dividing.
        /// </summary>
        public Vector2I DivRanges
        {
            get { return divRanges; }
            set {
                divRanges = value;
                if (!availableOperators.Contains(Equation.Operator.DIVIDE))
                    availableOperators.Add(Equation.Operator.DIVIDE);
            }
        }
        
        /// <summary>
        /// How many guessed we need to get to the next difficulty.
        /// </summary>
        public int Treshold { get; set; }

        /// <summary>
        /// Max droplets spawned at once.
        /// </summary>
        public int MaxDroplets { get; set; }

        /// <summary>
        /// Operators which have values in their Ranges property.
        /// </summary>
        public List<Equation.Operator> AvailableOperators
        {
            get { return availableOperators; }
        }
        #endregion 

        #region Private fields
        private List<Equation.Operator> availableOperators;

        private Vector2I addRanges;
        private Vector2I subRanges;
        private Vector2I mulRanges;
        private Vector2I divRanges;
        #endregion 

        public DifficultySetting()
        {
            availableOperators = new List<Equation.Operator>();
        }
    }
}