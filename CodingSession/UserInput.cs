
namespace CodingSessionLibrary
{
    public class UserInput
    {
        public int GetIntValue()
        {
            int choice;
            while (true)
            {
                if(int.TryParse(Console.ReadLine(), out choice)){
                    return choice;
                }
                Console.WriteLine("Enter valid integer");
            }
        }

        public DateTime GetTime(bool start)
        {
            string format = "yyyy-MM-dd HH:mm";
            while (true)
            {
                if(start)
                    Console.Write("Enter Start Time (yyyy-MM-dd HH:mm): ");
                else
                    Console.Write("Enter End Time (yyyy-MM-dd HH:mm): ");

                string? startDate = Console.ReadLine();
                if (DateTime.TryParseExact(startDate, format, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                    return dateTime;
                Console.WriteLine("Enter valid format\n");
            }
        }

        
    }
}
