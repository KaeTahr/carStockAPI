-- Tables
CREATE TABLE IF NOT EXISTS Dealers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Location TEXT
);

CREATE TABLE IF NOT EXISTS Cars (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Make TEXT NOT NULL,
    Model TEXT NOT NULL,
    Year INTEGER,
    Price REAL,
    DealerId INTEGER,
    FOREIGN KEY (DealerId) REFERENCES Dealers(Id)
);

-- Seed data
INSERT INTO Dealers (Name, Location)
SELECT 'Default Dealer', 'Melbourne'
WHERE NOT EXISTS (SELECT 1 FROM Dealers);

INSERT INTO Cars (Make, Model, Year, Price, DealerId)
SELECT 'Toyota', 'Corolla', 2020, 20000, 1
WHERE NOT EXISTS (SELECT 1 FROM Cars);