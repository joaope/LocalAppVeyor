using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalAppVeyor.Pipeline.Internal
{
    internal abstract class InternalEngineStep : IEngineStep
    {
        public abstract string Name { get; }

        public virtual bool ContinueOnFail => false;

        public string BeforeStepName => null;

        public string AfterStepName => null;

        public abstract bool Execute(ExecutionContext executionContext);

        public bool Execute(
            ExecutionContext executionContext, 
            IEnumerable<IEngineStep> possibleCustomSteps,
            UnhandledStepExceptionHandler unhandledExceptionHandler)
        {
            var customSteps = possibleCustomSteps as IEngineStep[] ?? possibleCustomSteps.ToArray();

            foreach (var beforeStep in customSteps.Where(s => s.BeforeStepName == Name))
            {
                if (!ExecuteSingleStepLogic(beforeStep, executionContext, unhandledExceptionHandler))
                {
                    return false;
                }
            }

            if (!ExecuteSingleStepLogic(this, executionContext, unhandledExceptionHandler))
            {
                return false;
            }

            foreach (var afterStep in customSteps.Where(s => s.AfterStepName == Name))
            {
                if (!ExecuteSingleStepLogic(afterStep, executionContext, unhandledExceptionHandler))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ExecuteSingleStepLogic(
            IEngineStep step, 
            ExecutionContext executionContext, 
            UnhandledStepExceptionHandler unhandledExceptionHandler)
        {
            executionContext.Outputter.Write($"Executing '{step.Name}'...");

            try
            {
                if (step.Execute(executionContext))
                {
                    executionContext.Outputter.Write($"'{step.Name}' successfully executed.");
                    return true;
                }

                if (step.ContinueOnFail)
                {
                    executionContext.Outputter.WriteWarning($"'{step.Name}' was not succedeed but execution is continuing anyway...");
                    return true;
                }

                executionContext.Outputter.WriteError($"'{step.Name}' failed. Stopping build...");
                return false;
            }
            catch (Exception e)
            {
                executionContext.Outputter.WriteError($"Unhandled exception on '{step.Name}' step: {e.Message}");

                var eventArgs = new UnhandledStepExceptionEventArgs(step.Name, e);
                unhandledExceptionHandler?.Invoke(this, eventArgs);

                if (!eventArgs.ContinueExecution)
                {
                    return false;
                }

                executionContext.Outputter.WriteWarning($"Continuing executing after recovering from exception...");
                return true;
            }
        }
    }
}
