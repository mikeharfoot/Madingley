
namespace Madingley.Common
{
    public class RunState
    {
        public ModelState ModelState { get; set; }

        public IOutput Output { get; set; }

        public RunState(
            ModelState modelState,
            IOutput output)
        {
            this.ModelState = modelState;
            this.Output = output;
        }

        public RunState(RunState c)
        {
            this.ModelState = new ModelState(c.ModelState);
            this.Output = c.Output;
        }
    }
}
