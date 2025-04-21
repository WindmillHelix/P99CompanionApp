CREATE TABLE IF NOT EXISTS KeyValuePair 
(
	ItemKey VARCHAR(50) NOT NULL PRIMARY KEY,
	ItemValue TEXT NULL
);

CREATE TABLE IF NOT EXISTS ParkInformation 
(
	ServerName VARCHAR(20) NOT NULL,
	CharacterName VARCHAR(20) NOT NULL,
	Account VARCHAR(20) NULL,
	ZoneName VARCHAR(50) NULL,
	BindZone VARCHAR(50) NULL,
	SkyCorpseDate INT NULL,
	PRIMARY KEY (ServerName, CharacterName)
);

CREATE TABLE IF NOT EXISTS CharacterNote 
(
	ServerName VARCHAR(20) NOT NULL,
	CharacterName VARCHAR(20) NOT NULL,
	Note TEXT NULL,
	PRIMARY KEY (ServerName, CharacterName)
);