using Spectre.Console;
using CodingTrackerDatabaseLibrary;
using CodingSessionLibrary;
using System.Data.Entity.Migrations.Model;

namespace CodingTracker
{
    internal class CodingController
    {
        public void Menu()
        {
            var input = new UserInput();
            while (true)
            {
                Console.WriteLine("\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do\n");
                Console.WriteLine("Type 0 to Close Application");
                Console.WriteLine("Type 1 to View All Records");
                Console.WriteLine("Type 2 to Insert Record");
                Console.WriteLine("Type 3 to Delete Record");
                Console.WriteLine("Type 4 to Update Record");
                Console.WriteLine("Type 5 to Generate Report");
                Console.WriteLine("----------------------------\n");
                Console.Write("Enter choice: ");

                int choice = input.GetIntValue();

                DatabaseOperation(choice);
                if (choice == 0) return;
            }
        }

        void DatabaseOperation(int choice)
        {
            var db = new Database();
            db.CreateTable();
            switch (choice)
            {
                case 0:
                    Console.WriteLine("Exiting...........\n");
                    return;
                case 1:
                    db.ViewTable();
                    break;
                case 2:
                    db.InsertRecord();
                    break;
                case 3:
                    db.DeleteRecord();
                    break;
                case 4:
                    db.UpdateRecord();
                    break;
                default:
                    Console.WriteLine("Invalid choice\n");
                    break;
            }
        }

    }
}
