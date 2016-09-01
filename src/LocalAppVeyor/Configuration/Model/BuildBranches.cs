namespace LocalAppVeyor.Configuration.Model
{
    public class BuildBranches
    {
        public string[] Only { get; set; }

        public string[] Except { get; set; }
    }
}