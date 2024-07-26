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
                        "1. Close Application", "2. View All Records", "3. Insert Record",
                        "4. Delete Record", "5. Update Record"
                    }));
            
                int opt = int.Parse(choice.Substring(0,1));
                DatabaseOperation(opt);
                if (opt == 1) return;
                Console.ReadLine();

            }
        }

        void DatabaseOperation(int choice)
        {
            var db = new Database();
            db.CreateTable();
            switch (choice)
            {
                case 1:
                    Console.WriteLine("\nExiting...........\n");
                    return;
                case 2:
                    db.ViewTable();
                    break;
                case 3:
                    db.InsertRecord();
                    break;
                case 4:
                    db.DeleteRecord();
                    break;
                case 5:
                    db.UpdateRecord();
                    break;
            }
        }

    }
}
