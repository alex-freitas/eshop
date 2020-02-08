namespace TaskScheduler.Entities
{
    public class WindowsCommandJob : Job
    {
        public WindowsCommandJob()
        {
            JobType = "WindowsCommandJob";
        }

        public string Command { get; set; }

        public string Arguments { get; set; }
        
        public override string Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}