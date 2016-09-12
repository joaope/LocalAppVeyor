namespace LocalAppVeyor.Engine.Pipeline
{
    public enum JobExecutionResultType
    {
        Success,
        Failure,

        NotExecuted,
        JobNotFound,
        SolutionFileNotFound,
        UnhandledException
    }
}