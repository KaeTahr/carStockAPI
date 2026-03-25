-- For testing only
-- Delete database before creating new one
DROP TABLE IF EXISTS Cars;
DROP TABLE IF EXISTS Dealers;


-- Tables
CREATE TABLE IF NOT EXISTS Dealers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Username Text NOT NULL UNIQUE,
    PasswordHash Text NOT NULL
);

CREATE TABLE IF NOT EXISTS Cars (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Make TEXT NOT NULL,
    Model TEXT NOT NULL,
    Year INTEGER NOT NULL,
    Price REAL NOT NULL,
    DealerId INTEGER NOT NULL,
    Stock INTEGER NOT NULL,
    FOREIGN KEY (DealerId) REFERENCES Dealers(Id)
);

-- Seed data
INSERT INTO Dealers (Name, Username, PasswordHash)
SELECT 'Default Dealer', 'defaultdealer', '123'
WHERE NOT EXISTS (SELECT 1 FROM Dealers);

INSERT INTO Cars (Make, Model, Year, Price, DealerId, Stock)
SELECT 'Toyota', 'Corolla', 2020, 20000, 1, 10
WHERE NOT EXISTS (SELECT 1 FROM Cars);