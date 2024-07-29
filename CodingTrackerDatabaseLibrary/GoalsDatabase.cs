using System.ComponentModel.Design;
using System.Configuration;
using System.Data.SQLite;
using CodingGoalLibrary;
using CodingSessionLibrary;
using Dapper;
using Spectre.Console;

namespace CodingTrackerDatabaseLibrary
{
    public class GoalsDatabase
    {
        private string? databseConnection = ConfigurationManager.AppSettings.Get("GoalsDBConnection");

        public void CreateTable()
        {
            using (var connection = new SQLiteConnection(databseConnection))
            {
                connection.Open();

                string createCodeGoalTableQuery = @"
                    CREATE TABLE IF NOT EXISTS codeGoal(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    codingTask TEXT NOT NULL,
                    codingGoal TEXT NOT NULL,
                    startDate TEXT NOT NULL,
                    endDate TEXT NOT NULL
                    );
                ";

                connection.Execute(createCodeGoalTableQuery);
            }
        }

        public List<CodingGoals> ViewGoalTable()
        {
            using (var connection = new SQLiteConnection(databseConnection))
            {
                var sql = "SELECT * FROM codeGoal";
                var query =  connection.Query<CodingGoals>(sql).AsList();
                return query;
            }
        }

        public void InsertGoalRecord()
        {
            var userInput = new UserInput();
            try
            {

                using (var connection = new SQLiteConnection(databseConnection))
                {
                    var sql = "INSERT INTO codeGoal(codingTask, codingGoal, startDate, endDate)  VALUES(@CodingTask, @CodingGoal, @StartDate, @EndDate);";
                    var session = new CodingGoals()
                    {
                        Id = 1,
                        CodingTask = userInput.GetTask(),
                        CodingGoal = userInput.GetGoal(),
                        StartDate = userInput.GetTime(true),
                        EndDate = userInput.GetTime(false)
                    };
                    int r = connection.Execute(sql,session);
                    Console.WriteLine($"\nRow inserted: {r}\n");

                    var goals = ViewGoalTable();
                    DisplayGoalTable(goals);
                }
            }
            catch(Exception ex)
            {
                AnsiConsole.Markup($"\n[red]{ex.Message}[/]");
            }
        }


        public void DeleteGoalRecord()
        {
            var userInput = new UserInput();
            var goals = ViewGoalTable();
            DisplayGoalTable(goals);
           
            using (var connection = new SQLiteConnection(databseConnection))
            {
                var sql = "DELETE FROM codeGoal WHERE id = @Id";
                Console.Write("\nEnter id to be deleted: ");
                int id = userInput.GetIntValue();

                if (!CheckIdExists(id))
                {
                    AnsiConsole.Markup("[red]ID doesnt exists. Returning to Main Menu.[/]\n");
                    Thread.Sleep(1000);
                    return;
                }

                connection.Execute(sql, new { Id = id });
            }
            AnsiConsole.Markup("\n[red]Row deleted[/]\n");
            goals = ViewGoalTable();
            DisplayGoalTable(goals);
        }

        public void UpdateGoalRecord()
        {
            var userInput = new UserInput();
            int choice = userInput.UpdateGoalsRecordChoice();

            var goals = ViewGoalTable();
            DisplayGoalTable(goals);

            
            if (choice > 5)
            {
                AnsiConsole.Markup("[red]Invalid Option. Returning to Main Menu[/]\n");
                Thread.Sleep(1000);
                return;
            }

            using (var connection = new SQLiteConnection(databseConnection))
            {
                Console.Write("Enter id: ");
                int id = userInput.GetIntValue();


                if (!CheckIdExists(id))
                {
                    AnsiConsole.Markup("[red]ID doesnt exists. Returning to Main Menu.[/]\n");
                    Thread.Sleep(1000);
                    return;
                }
                try
                {
                    if(choice == 1)
                    {
                        string sql = "UPDATE codeGoal SET codingTask = @CodingTask WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            CodingTask = userInput.GetTask()
                        };
                        connection.Execute(sql, new { CodingTask = session.CodingTask, Id = id });
                    }
                    else if (choice == 2)
                    {
                        string sql = "UPDATE codeGoal SET codingTask = @CodingGoal WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            CodingGoal = userInput.GetGoal()
                        };
                        connection.Execute(sql, new { CodingTask = session.CodingGoal, Id = id });
                    }
                    else if (choice == 3) {

                        string sql = "UPDATE codeGoal SET startDate = @StartDate WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            StartDate = userInput.GetTime(true),
                        };
                        connection.Execute(sql, new { StartDate = session.StartDate, Id = id });

                    }
                    else if (choice == 4)
                    {
                        string sql = "UPDATE codeGoal SET endDate = @EndDate WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            StartDate = DateTime.Now,
                            EndDate = userInput.GetTime(false),
                        };
                        connection.Execute(sql, new { EndDate = session.EndDate, Id = id });
                    }
                    else if (choice == 5)
                    {
                        string sql = "UPDATE codeGoal SET startDate=@StartDate AND endDate = @EndDate WHERE id = @Id";
                        var session = new CodingGoals()
                        {
                            StartDate = userInput.GetTime(true),
                            EndDate = userInput.GetTime(false),
                        };
                        connection.Execute(sql, new { StartTime = session.StartDate, EndTime = session.EndDate, Id = id });
                    }

                    Console.WriteLine("\nRow updated\n");
                    ViewGoalTable();
                }
                catch(Exception ex)
                {
                    AnsiConsole.Markup($"\n[red]{ex.Message}[/]\n");
                }
                goals = ViewGoalTable();
                DisplayGoalTable(goals);

            }

        }

        public bool CheckIdExists(int id)
        {
            using (var connection = new SQLiteConnection(databseConnection))
            {
                string sql = "SELECT COUNT(1) FROM codeGoal WHERE id= @Id";
                int count = connection.ExecuteScalar<int>(sql, new { Id = id });
                return count > 0;
            }

        }

        public void DisplayGoalTable(List<CodingGoals> goals)
        {
            Console.WriteLine("Your Goals\n");
            if (goals.Count == 0)
            {
                AnsiConsole.Markup("[red]No records found[/]. Add by starting a session.");
                return;
            }

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Coding Task");
            table.AddColumn("Coding Goal");
            table.AddColumn("Start Date");
            table.AddColumn("End Date");
            foreach (var session in goals)
            {
                table.AddRow(
                    session.Id.ToString(),
                    session.CodingTask,
                    session.CodingGoal,
                    session.StartDate.Date.ToShortDateString(),
                    session.EndDate.Date.ToShortDateString());
            }
            AnsiConsole.Write(table);
     
        }
    }

}

