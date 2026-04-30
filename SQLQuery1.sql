CREATE DATABASE SalesCarWebPage;
GO

USE SalesCarWebPage;
GO

CREATE TABLE UsersRole(
	UsersRoleId INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	RoleName NVARCHAR(100) NOT NULL UNIQUE,
	IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Brand(
	BrandId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	BrandName NVARCHAR(100) NOT NULL,
);
GO

CREATE TABLE [Address](
	AddressId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Street NVARCHAR(100) NOT NULL,
	Street2 NVARCHAR(100) NULL,
	DoorNumber NVARCHAR(50) NOT NULL,
	[Floor] NVARCHAR(50) NULL,
	PostalCode NVARCHAR(20) NOT NULL,
	[Locate] NVARCHAR(100) NOT NULL,
	City NVARCHAR(100) NOT NULL,
	Country NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Contact (
    ContactId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(20) NOT NULL,
    JobTitle NVARCHAR(100) NULL
);
GO

CREATE TABLE Model(
	ModelId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	BrandId INT NOT NULL,
	ModelName NVARCHAR(100) NOT NULL,	
	CONSTRAINT FK_Model_Brand FOREIGN KEY (BrandId) REFERENCES Brand(BrandId),
);
GO

CREATE TABLE Users(
	UserId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	ContactId INT NOT NULL, 
    UsersRoleId INT NOT NULL DEFAULT 1,
	[Name] NVARCHAR(255) NOT NULL,
    UserName NVARCHAR(255) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    ProfilePicture NVARCHAR(MAX) NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Salt NVARCHAR(64) NOT NULL,
    IsApproved BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdatedAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
	CONSTRAINT FK_Users_Contact FOREIGN KEY (ContactId) REFERENCES Contact(ContactId),
    CONSTRAINT FK_Users_UserRole FOREIGN KEY (UsersRoleId) REFERENCES UsersRole(UsersRoleId)
);
GO

CREATE TABLE UserSetting (
    UserSettingId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId INT NOT NULL,
    Theme NVARCHAR(50) NOT NULL DEFAULT 'Light',
    [Language] NVARCHAR(50) NOT NULL DEFAULT 'pt-PT',
    ReceiveNotifications BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_UserSetting_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE TABLE Provider (
    ProviderId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId INT NOT NULL,
    AddressId INT NOT NULL,
    NameProvider NVARCHAR(200) NOT NULL,
    NIF NVARCHAR(9) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Provider_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Provider_Address FOREIGN KEY (AddressId) REFERENCES [Address](AddressId)
);
GO

CREATE TABLE Client (
    ClientId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId INT NOT NULL,
    ContactId INT NOT NULL,
    AddressId INT NOT NULL,
    ClientName NVARCHAR(200) NOT NULL,
    NIF NVARCHAR(9) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Client_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Client_Contact FOREIGN KEY (ContactId) REFERENCES Contact(ContactId),
    CONSTRAINT FK_Client_Address FOREIGN KEY (AddressId) REFERENCES [Address](AddressId)
);
GO

CREATE TABLE Car (
    CarId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ModelId INT NOT NULL,
    ProviderId INT NOT NULL,
    TypeOfFuel NVARCHAR(100) NOT NULL,
    CarColor NVARCHAR(100) NOT NULL,
    EngineCapacity INT NOT NULL,
    CarTare DECIMAL(18,2) NOT NULL,
    Transmission NVARCHAR(50) NULL,
    Category NVARCHAR(50) NULL,
    CarPrice DECIMAL(18,2) NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    PlateNumber NVARCHAR(100) NOT NULL UNIQUE,
    [Year] INT NOT NULL,
    Kilometers INT NOT NULL,
    ImageUrl NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdatedAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsApproved BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Car_Model FOREIGN KEY (ModelId) REFERENCES Model(ModelId),
    CONSTRAINT FK_Car_Provider FOREIGN KEY (ProviderId) REFERENCES Provider(ProviderId)
);
GO

CREATE TABLE Favorites (
    FavoritesId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserId INT NOT NULL,
    CarId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Favorites_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Favorites_Car FOREIGN KEY (CarId) REFERENCES Car(CarId)
);
GO

CREATE TABLE MessageBox (
    MessageBoxId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    CarId INT NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    MessageText NVARCHAR(MAX) NOT NULL,
    SentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastUpdatedAt DATETIME2 NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    IsEdited BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Message_Sender FOREIGN KEY (SenderId) REFERENCES Users(UserId),
    CONSTRAINT FK_Message_Receiver FOREIGN KEY (ReceiverId) REFERENCES Users(UserId),
    CONSTRAINT FK_Message_Car FOREIGN KEY (CarId) REFERENCES Car(CarId)
);
GO

CREATE TABLE Sale (
    SaleId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CarId INT NOT NULL,
    ClientId INT NOT NULL,
    SaleDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    PurchaseDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    FinalPrice DECIMAL(18,2) NOT NULL,
    PaymentMethod NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_Sale_Car FOREIGN KEY (CarId) REFERENCES Car(CarId),
    CONSTRAINT FK_Sale_Client FOREIGN KEY (ClientId) REFERENCES Client(ClientId)
);
GO

IF NOT EXISTS (SELECT 1 FROM UsersRole WHERE RoleName = 'Admin')
BEGIN
    INSERT INTO UsersRole (RoleName, IsActive) VALUES ('Admin', 1);
END
GO

INSERT INTO Contact (FirstName, LastName, Email, PhoneNumber, JobTitle)
VALUES 
('Frederico', 'Admin', 'fredericocrf87@hotmail.com', '910000000', 'Administrator'),
('Fábio', 'Admin', 'fabio.salgado23@gmail.com', '920000000', 'Administrator');
GO

INSERT INTO Users (
    ContactId, 
    UsersRoleId, 
    [Name], 
    UserName, 
    Email, 
    PasswordHash, 
    Salt, 
    IsApproved, 
    IsActive
)
VALUES 
(
    (SELECT ContactId FROM Contact WHERE Email = 'fredericocrf87@hotmail.com'),
    (SELECT UsersRoleId FROM UsersRole WHERE RoleName = 'Admin'),
    'Frederico Admin',
    'frederico.admin',
    'fredericocrf87@hotmail.com',
    '$2a$12$/k1rSY/2xuiEfLCC.UJ.beMOE0aYKrhwGhxNdRJjBLG9FShbFlB2O',
    'BcryptHash',
    1, -- Já aprovado
    1  -- Ativo
),
(
    (SELECT ContactId FROM Contact WHERE Email = 'fabio.salgado23@gmail.com'),
    (SELECT UsersRoleId FROM UsersRole WHERE RoleName = 'Admin'),
    'Fábio Admin',
    'fabio.admin',
    'fabio.salgado23@gmail.com',
    '$2a$12$esJ4lhJKeBlYUc8LpG01.ujYz.ZLzwwlTjlkPKU0PuzuLWkSFje4.',
    'BcryptHash',
    1, -- Já aprovado
    1  -- Ativo
);
GO

select * from users

INSERT INTO Brand (BrandName) VALUES ('Ferrari'), ('Porsche'), ('Lamborghini'), ('Aston Martin'), ('McLaren'), ('BMW'), ('Mercedes');
GO

INSERT INTO Model (BrandId, ModelName) VALUES 
((SELECT BrandId FROM Brand WHERE BrandName = 'Ferrari'), 'F8 Tributo'),
((SELECT BrandId FROM Brand WHERE BrandName = 'Ferrari'), '488 Pista'),
((SELECT BrandId FROM Brand WHERE BrandName = 'Porsche'), '911 GT3'),
((SELECT BrandId FROM Brand WHERE BrandName = 'Porsche'), 'Taycan Turbo S'),
((SELECT BrandId FROM Brand WHERE BrandName = 'Lamborghini'), 'Aventador SVJ'),
((SELECT BrandId FROM Brand WHERE BrandName = 'BMW'), 'M4 Competition');
GO

INSERT INTO [Address] (Street, DoorNumber, PostalCode, [Locate], City, Country) VALUES 
('Avenida da Liberdade', '10', '1250-001', 'Lisboa', 'Lisboa', 'Portugal'),
('Rua de Santa Catarina', '250', '4000-442', 'Porto', 'Porto', 'Portugal');
GO

INSERT INTO Provider (UserId, AddressId, NameProvider, NIF, IsActive)
VALUES (
    (SELECT UserId FROM Users WHERE UserName = 'frederico.admin'),
    (SELECT TOP 1 AddressId FROM [Address] WHERE City = 'Lisboa'),
    'Premium Cars Lisboa', '123456789', 1
);
GO

INSERT INTO Car (ModelId, ProviderId, TypeOfFuel, CarColor, EngineCapacity, CarTare, CarPrice, IsAvailable, PlateNumber, [Year], Kilometers, ImageUrl, IsApproved, IsActive)
VALUES 
-- Ferrari em Lisboa
((SELECT ModelId FROM Model WHERE ModelName = 'F8 Tributo'), 
 (SELECT ProviderId FROM Provider WHERE NameProvider = 'Premium Cars Lisboa'), 
 'Gasolina', 'Rosso Corsa', 3902, 1435.00, 345000.00, 1, 'AA-01-FF', 2023, 1200, 
 'https://images.unsplash.com/photo-1592198084033-aade902d1aae', 1, 1),

-- Porsche Elétrico
((SELECT ModelId FROM Model WHERE ModelName = 'Taycan Turbo S'), 
 (SELECT ProviderId FROM Provider WHERE NameProvider = 'Premium Cars Lisboa'), 
 'Elétrico', 'Carrara White', 0, 2295.00, 195000.00, 1, 'EV-99-PP', 2024, 50, 
 'https://images.unsplash.com/photo-1614162692292-7ac56d7f7f1e', 1, 1),

-- Porsche 911 (Pesquisa por Porsche)
((SELECT ModelId FROM Model WHERE ModelName = '911 GT3'), 
 (SELECT ProviderId FROM Provider WHERE NameProvider = 'Premium Cars Lisboa'), 
 'Gasolina', 'Shark Blue', 3996, 1435.00, 220000.00, 1, 'GT-33-RS', 2022, 5400, 
 'https://images.unsplash.com/photo-1503376780353-7e6692767b70', 1, 1),

-- BMW M4
((SELECT ModelId FROM Model WHERE ModelName = 'M4 Competition'), 
 (SELECT ProviderId FROM Provider WHERE NameProvider = 'Premium Cars Lisboa'), 
 'Gasolina', 'Isle of Man Green', 2993, 1725.00, 115000.00, 1, 'BM-04-WM', 2023, 8500, 
 'https://images.unsplash.com/photo-1617531653332-bd46c24f2068', 1, 1);
GO

-- Verificação final
SELECT b.BrandName, m.ModelName, c.CarPrice, c.TypeOfFuel, c.PlateNumber
FROM Car c
INNER JOIN Model m ON c.ModelId = m.ModelId
INNER JOIN Brand b ON m.BrandId = b.BrandId;

ALTER TABLE Car ADD Transmission NVARCHAR(50) NULL;
ALTER TABLE Car ADD Category NVARCHAR(50) NULL; -- Para "SUV", "Citadino", etc.
GO

UPDATE Car SET Transmission = 'Automática', Category = 'Desportivo' WHERE PlateNumber = 'AA-01-FF'; -- Ferrari
UPDATE Car SET Transmission = 'Automática', Category = 'Sedan' WHERE PlateNumber = 'EV-99-PP';      -- Taycan
UPDATE Car SET Transmission = 'Manual', Category = 'Desportivo' WHERE PlateNumber = 'GT-33-RS';    -- 911 GT3
UPDATE Car SET Transmission = 'Automática', Category = 'Desportivo' WHERE PlateNumber = 'BM-04-WM'; -- BMW
GO

SELECT c.PlateNumber, ad.City 
FROM Car c
JOIN Provider p ON c.ProviderId = p.ProviderId
JOIN [Address] ad ON p.AddressId = ad.AddressId;

ALTER TABLE Provider ADD CompanyName NVARCHAR(255) NULL;

ALTER TABLE Users 
ADD FacebookUrl NVARCHAR(500) NULL,
    InstagramUrl NVARCHAR(500) NULL,
    TwitterUrl NVARCHAR(500) NULL; -- Pode ser usado para o X
GO

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users' AND COLUMN_NAME LIKE '%Url';
GO

ALTER TABLE Contact
ADD ClientId INT NULL;

SELECT * FROM UsersRole
SELECT * FROM Users

INSERT INTO UsersRole (RoleName, IsActive) 
VALUES ('User', 1);
GO

DELETE FROM Contact WHERE Email = 'teste1@teste.pt';

-- Garante também que não há nenhum utilizador com esse nome ou email
DELETE FROM Users WHERE UserName = 'Teste1' OR Email = 'teste1@teste.pt';

select * from sale

SELECT UserId, UserName FROM Users WHERE UserName = 'Frederico';

ALTER TABLE MessageBox 
ADD ParentMessageId INT NULL;
GO

ALTER TABLE MessageBox
ADD CONSTRAINT FK_Message_Parent FOREIGN KEY (ParentMessageId) 
REFERENCES MessageBox(MessageBoxId);
GO