
using System.ComponentModel.Design;
using System.Diagnostics;
using Spectre.Console;

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
            string format = "dd-MM-yyyy";
            while (true)
            {
                if(start)
                    Console.Write("Enter Start Time (dd-MM-yyyy): ");
                else
                    Console.Write("Enter End Time (dd-MM-yyyy): ");

                string? startDate = Console.ReadLine();
                if (DateTime.TryParseExact(startDate, format, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                    return dateTime;
                Console.WriteLine("Enter valid format\n");
            }
        }

        public string GetGoal()
        {
            while (true)
            {
                Console.Write("Enter goal(hours): ");
                string goal = Console.ReadLine();
                if (goal == null)
                {
                    AnsiConsole.Markup("[red]Invalid entry.[/]");
                }
                return goal.ToLower();

            }
        }

        public string GetTask()
        {
            while (true)
            {
                Console.Write("Enter goal task: ");
                string task = Console.ReadLine();
                if(task == null)
                {
                    AnsiConsole.Markup("[red]Invalid entry.[/]");
                }
                return task.ToLower();
            }
        }

        //Challenge 1 - Add the possibility of tracking the coding time via a stopwatch so the user can track the session as it happens.
        public (DateTime Start, DateTime End) GetSessionTime()
        {
            Stopwatch sw = new Stopwatch();
            var start = DateTime.Now;
            TimeSpan timeElapsed;
            Console.WriteLine("\nStarting Coding Session.......");
            AnsiConsole.Markup("\r[blue]Press any key to stop timer...[/]\n\n");
            sw.Start();
            while (!Console.KeyAvailable)
            {
                timeElapsed = sw.Elapsed;
                Console.Write($"\rTime elapsed: {timeElapsed:hh\\:mm\\:ss}");
            }
            sw.Stop();
            var end = DateTime.Now; 
            return (start, end);
        }

        public int ViewChoice()
        {
            string viewChoice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("\n\rView Menu:")
                        .PageSize(3)
                        .AddChoices(new[]
                        {
                            "1. View Session Records", "2. View Goals Records", "3. Back to Menu"
                        }));

            return int.Parse(viewChoice.Substring(0, 1));
        }

        public int InsertChoice()
        {
            string insertChoice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("\n\rInsert Menu:")
                        .PageSize(3)
                        .AddChoices(new[]
                        {
                            "1. Insert Session / Start Session", "2. Insert Goal", "3. Back to Menu"
                        }
                        ));

            return int.Parse(insertChoice.Substring(0, 1));
        }

        public int DeleteChoice()
        {
            string deleteChoice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("\n\rDelete Menu: ")
                        .PageSize(3)
                        .AddChoices(new[]
                        {
                            "1. Delete Session", "2. Delete Goal", "3. Back to Menu"
                        }));

            return int.Parse(deleteChoice.Substring(0, 1));
        }

        public int UpdateChoice()
        {
            string updateChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\n\rUpdate Menu:")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "1. Update Session Records", "2. Update Goal Records", "3. Back to Menu"
                    }));

            return int.Parse(updateChoice.Substring(0, 1));
        }

        public int UpdateGoalsRecordChoice()
        {
            Console.Clear();
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Update Choices:")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "1. Coding Task", "2. Coding Goal", "3. Start Date", "4. End Date", "5. Both Dates"
                }
            ));
            return int.Parse(choice.Substring(0,1));
        }

        public int FilterSessionChoice()
        {
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Filter Choices:")
                .PageSize(5)
                .AddChoices(new[]
                {
                    "1. Days", "2. Months", "3. Years", "4. Back to Menu"
                }));
            return int.Parse(choice.Substring(0, 1));
        }
    }
}
