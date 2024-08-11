# CodingTracker 
This console app provides CRUD operations to the Coding Session database and Coding Goals database.
I have used following directives: <b>SpectreConsole</b><i> for beautifying the console</i>, <b>Dapper</b><i>for database operations.</i>

# Project Structure:
- <u>Coding Tracker</u>: Contains the <b>CodingController</b> class which is responsible for maintaning the flow of the program.
- <u>Coding Session</u>: Contains the <b>UserInput</b> class which is responsible for the taking the input of the user</li>
                  Contains the <b>CodeSession</b> Database Model class, which contains following attributes: id, startTime, endTime, codingGoal</li>
- <u>Coding Goal</u>: Contains the <b>CodingGoal Database</b> Model class, which contains following attributes: id, codingGoal, codingTask, startDate, endDate
- <u>CodingTrackerDatabaseLibrary</u>: Contains the <b>GoalsDatabase</b> and <b>SessionDatabase class</b> which performs database operations 
                                using Dapper ORM for goals and session database.
- <u>App.exe.config</u>: Stores connection strings for the Goals and Sessions database. 

# Workflow of Project:
1. The CodingController class presents the user with a main menu providing options to interact with the databases. There are two databases: SessionDatabase (stores the user's coding session info) and GoalsDatabase (stores the user's coding goals and their expected completion dates).
2. CRUD operations are provided for both databases. While inserting data into the tables, validation is performed in their respective model classes. When inserting into the session table, a stopwatch is implemented to track the user's coding session.
3. The menu also provides options to filter sessions based on days, months, and years.
4. A progress tracking functionality is also provided to monitor how much of the goal has been achieved during a coding session and how many hours per day the user needs to work to complete the goal within the end date given by the user.