using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Security.Cryptography.X509Certificates;

namespace Data
{
    public static class InitDatabse
    {
        public static void Run()
        {
            var connectionString = "Data Source=Data/database.db";
            using IDbConnection  connection = new SqliteConnection(connectionString);
            connection.Open();

            // Create Tables

            // Dealers
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Dealers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );
            ");

            // Cars
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Cars (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DealerId INTEGER NOT NULL,
                    Make TEXT NOT NULL,
                    Model TEXT NOT NULL,
                    Year INTEGER NOT NULL,
                    Stock INTEGER NOT NULL,
                    FOREIGN KEY (DealerId) REFERENCES Dealers(Id)
                );
            ");

            Console.WriteLine("Database initialized successfully!");


            // Seed Data
            connection.Execute(@"
                INSERT INTO Dealers (Name) VALUES
                ('Demo Dealer')
                ON CONFLICT(Name) DO NOTHING;
            ");

        }
    }
}