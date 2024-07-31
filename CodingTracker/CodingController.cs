using Spectre.Console;
using CodingTrackerDatabaseLibrary;
using CodingSessionLibrary;

namespace CodingTracker
{
    internal class CodingController
    {
        public void Menu()
        {
            var input = new UserInput();
            while (true)
            {
                Console.Clear();
                string choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\n\rMAIN MENU")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "1. Close Application", "2. Start Session", "3. View Session Records", "4. View Goal Records", "5. Insert Coding Goal",
                        "6. Delete Coding Goal", "7. Update Records", "8. Filter Sessions", "9. Generate Reports"
                    }));
            
                int opt = int.Parse(choice.Substring(0,1));
                DatabaseOperation(opt);
                if (opt == 1) return;
              
            }
        }

        void DatabaseOperation(int choice)
        {
            var sessionDB = new SessionDatabase();
            var goalDB = new GoalsDatabase();
            sessionDB.CreateTable();
            goalDB.CreateTable();
            switch (choice)
            {
                case 1:
                    Console.WriteLine("\nExiting...........\n");
                    return;
                case 2:
                    sessionDB.InsertSession();
                    break;
                case 3:
                    var sessions = sessionDB.ViewSessionsTable();
                    sessionDB.DisplaySessionTable(sessions);
                    break;
                case 4:
                    var goals = goalDB.ViewGoalTable();
                    goalDB.DisplayGoalTable(goals);
                    break;
                case 5:
                    goalDB.InsertGoalRecord();
                    break;
                case 6:
                    goalDB.DeleteGoalRecord();
                    break;
                case 7:
                    string secondChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("\n\rUpdate Menu:")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "1. Update Session Records", "2. Update Goal Records"
                    }));

                    int opt = int.Parse(secondChoice.Substring(0, 1));
                    if (opt == 1)
                        sessionDB.UpdateSessionRecord();
                    else
                        goalDB.UpdateGoalRecord();
                    break;
                case 8:
                    FilterSessions(sessionDB);
                    break;
                case 9:
                    goalDB.ProgressReport();
                    break;
            }
            AnsiConsole.Markup("\n[blue]Press enter to continue....[/]");
            Console.ReadLine();
        }

        public void FilterSessions(SessionDatabase sessionDB)
        {
            var userInput = new UserInput();
            int choice = userInput.FilterSessionChoice();
            switch (choice)
            {
                case 1:
                    sessionDB.FilterByWeek();
                    break;
                case 2:
                    sessionDB.FilterByMonths();
                    break;
                case 3:
                    sessionDB.FilterByYear();
                    break;
            }
        }

    }
}
