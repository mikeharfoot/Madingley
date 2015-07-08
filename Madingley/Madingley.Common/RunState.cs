
namespace Madingley.Common
{
    /// <summary>
    /// Holds both the full model state and reference to output implementation.
    /// </summary>
    public class RunState
    {
        /// <summary>
        /// Model state.
        /// </summary>
        public ModelState ModelState { get; set; }

        /// <summary>
        /// Output implementation.
        /// </summary>
        public IOutput Output { get; set; }

        /// <summary>
        /// RunState constructor.
        /// </summary>
        /// <param name="modelState">Model state.</param>
        /// <param name="output">Output implementation.</param>
        public RunState(
            ModelState modelState,
            IOutput output)
        {
            this.ModelState = modelState;
            this.Output = output;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="runState">RunState to copy</param>
        public RunState(RunState runState)
        {
            this.ModelState = new ModelState(runState.ModelState);
            this.Output = runState.Output;
        }
    }
}
