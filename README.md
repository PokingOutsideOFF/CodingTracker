# CodingTracker 
This console app provides CRUD operations to the Coding Session database and Coding Goals database.
I have used following directives: <b>SpectreConsole</b><i>for beautify the console</i>, <b>Dapper</b><i>for database operation</i>
The structure of the project is as follows:
- Coding Tracker: Contains the <b>CodingController</b> class which is responsible for maintaning the flow of the program.
- Coding Session: Contains the <b>UserInput</b> class which is responsible for the taking the input of the user
                  Contains the <b>CodeSession</b> Database Model class, which contains following attributes: id, startTime, endTime, codingGoal
- Coding Goal: Contains the <b>CodingGoal Database</b> Model class, which contains following attributes: id, codingGoal, codingTask, startDate,
               endDate
- CodingTrackerDatabaseLibrary: Contains the <b>GoalsDatabase</b> and <b>SessionDatabase class</b> which performs database operations 
                                using Dapper ORM for goals and session database.
- App.exe.config: Stores connection strings for the Goals and Sessions database. 

# Workflow of Project:
1. CodingController class presents user with Main Menu providing options to interact with the databases. There are two databases: SessionDatabse (stores the users coding sesssion info) and GoalsDatabase (stores the user coding goals and their expected date of completion).
2. CRUD operations are provided for both the databases. While inserting the date into the table, validation is performed in theri respective model classes. While inserting in session table , stopwatch is implemented to track the user's coding session.
3. The menu also provides options to filter sessions on the basis of days, months and years.
4. A progress tracking functionaloty is also provided to track how much of the goal is achieved by working during a coding session and how much hours per day the user have to work to complete the goal within the end date given by user. 