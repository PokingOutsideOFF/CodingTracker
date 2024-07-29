namespace CodingGoalLibrary
{
    public class CodingGoals
    {
        private int id;
        private string codingGoal;
        private string codingTask;
        private DateTime startDate;
        private DateTime endDate;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string CodingTask
        {
            get { return codingTask; }
            set { codingTask = value; }
        }
        public string CodingGoal
        {
            get { return codingGoal;}
            set { codingGoal = value; }
        }
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Start time cannot be in future");
                }
                startDate = value;
            }
        }
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                if (value < startDate)
                {
                    throw new ArgumentException("End time cannot be before start");

                }
                endDate = value;
            }
        }
    }
}