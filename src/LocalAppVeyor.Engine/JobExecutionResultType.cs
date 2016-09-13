namespace LocalAppVeyor.Engine
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