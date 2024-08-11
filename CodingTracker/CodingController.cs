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
                        "1. Close Application", "2. View Records", "3. Insert Records", "4. Delete Records", 
                        "5. Update Records", "6. Filter Sessions", "7. Track Progress"
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
            var userInput = new UserInput();
            int opt = 0;
            switch (choice)
            {
                case 1:
                    Console.WriteLine("\nExiting...........\n");
                    return;

                case 2:
                    opt = userInput.ViewChoice();
                    if (opt == 1)
                    {
                        var sessions = sessionDB.ViewSessionsTable();
                        sessionDB.DisplaySessionTable(sessions);
                    }
                    else if (opt == 2)
                    {
                        var goals = goalDB.ViewGoalTable();
                        goalDB.DisplayGoalTable(goals);
                    }
                    else
                        return;

                    break;

                case 3:
                    opt = userInput.InsertChoice();
                    if (opt == 1)
                        sessionDB.InsertSession();
                    else if (opt == 2)
                        goalDB.InsertGoalRecord();
                    else
                        return;
                    break;

                case 4:
                    opt = userInput.DeleteChoice();
                    if (opt == 1)
                        sessionDB.DeleteSession();
                    else if (opt == 2)
                        goalDB.DeleteGoalRecord();
                    else
                        return;
                    break;

                case 5:
                    opt = userInput.UpdateChoice();
                    if (opt == 1)
                        sessionDB.UpdateSessionRecord();
                    else if (opt == 2)
                        goalDB.UpdateGoalRecord();
                    else
                        return;
                    break;

                case 6:
                    opt = userInput.FilterSessionChoice();
                    if (opt == 1)
                        sessionDB.FilterByDay();
                    else if (opt == 2)
                        sessionDB.FilterByMonths();
                    else if (opt == 3)
                        sessionDB.FilterByYear();
                    else
                        return;
                    break;

                case 7:
                    goalDB.ProgressReport();
                    break;
            }
            AnsiConsole.Markup("\n[blue]Press enter to continue....[/]");
            Console.ReadLine();
        }
    }
}
