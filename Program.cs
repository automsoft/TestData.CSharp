using System;
using System.Collections.Generic;
using System.IO;
using RHDALLib;

namespace TestData.CSharp
{
    /// <summary>
    /// Program to show reading and wirting data to a named item in a database
    /// </summary>
    class Program
    {
        protected static string NodeName = "TestData";
        protected static string ItemName = "TestItem1";
        static void Main(string[] args)
        {
            //Check if we're reading or writing tags
            if (args.Length != 1)
            {
                Console.WriteLine("No parameters provided. Usage: TestData [-w]riteData [-r]eadData");
            }
            else
            {
                //Get a session with the default database for this system.
                var session = InitDatabase();
                var dbAccessor = new DatabaseHelpers(session);
                switch (args[0])
                {
                    case "-w":
                        {
                            Console.WriteLine("Writing data to the database...");
                            //Write data to the database
                            dbAccessor.CreateNodeAndItems(NodeName, new[] { ItemName });
                            //Create a point per hour, for 10 days
                            const int iterationsCount = 24*10;
                            var points = new List<DataPoint>();
                            var timestamp = DateTime.Now.AddDays(-10);
                            for(var i = 0; i < iterationsCount; i++)
                            {
                                //Settigns quality as good, or value 192.
                                points.Add(new DataPoint(timestamp, i, 192));
                                //Move on by 1 hour.
                                timestamp = timestamp.AddHours(1);
                            }
                            dbAccessor.StoreDataInDatabase(NodeName, ItemName, points);
                            Console.WriteLine("Data created.");
                        }
                        break;
                    case "-r":
                        {
                            //Read any simulated data from the database.
                            Console.WriteLine("Reading data from the database...");
                            var data = dbAccessor.RetrieveDataInDatabase(NodeName, ItemName);
                            Console.WriteLine(data.Count + " events returned from the database.");
                        }
                        break;
                    default:
                        Console.WriteLine("Incorrect parameters provided. Usage: TestData [-w]riteData [-r]eadData");
                        break;
                }
                //Close the session
                session.Uninitialise();
            }
        }

        private static ISession InitDatabase()
        {
            var dbDir = Directory.GetCurrentDirectory() + "\\" + "Database";
            //Check if we already have the db directory.
            var databaseExists = Directory.Exists(dbDir);
            // create the directory if it doesn't exist
            if (!databaseExists)
                Directory.CreateDirectory(dbDir);

            // Create a session on the DAL for this test
            var returnValue = new Session();
            returnValue.Initialise("", "", "", dbDir + "\\rapid.boot", false, true, Module_e.MOD_DBMAINT_E);
            if (!databaseExists)
                returnValue.CreateFederation(0, "", 0, 0);
            return returnValue;
        }

    }
}
