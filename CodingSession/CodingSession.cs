namespace CodingSessionLibrary
{
    public class CodingSession
    {
        private DateTime startTime;
        private DateTime endTime;
        public int Id { get
            {
                return this.Id;
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
                if(endTime < startTime)
                {
                    throw new ArgumentException("End time cannot be before start");

                }
                endTime = value;
            }
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