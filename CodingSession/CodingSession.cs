namespace CodingSessionLibrary
{
    public class CodingSession
    {
        private DateTime startTime;
        private DateTime endTime;
        private int id;
        string? codingGoal;
        public int Id { get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if(value > DateTime.Now)
                {
                    throw new ArgumentException("Start time cannot be in future");
                }
                startTime = value;
            }
        }
                
        public DateTime EndTime {
            get { return endTime;  }
            set
            {
                if(value < startTime)
                {
                    throw new ArgumentException("End time cannot be before start");

                }
                endTime = value;
            }
        }

        public string CodingGoal
        {
            get { return codingGoal; }
            set { codingGoal = value; }
        }

        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }
    }
}