CREATE TABLE IF NOT EXISTS KeyValuePair 
(
	ItemKey TEXT NOT NULL PRIMARY KEY,
	ItemValue TEXT NULL
);

CREATE TABLE IF NOT EXISTS ParkInformation 
(
	ServerName TEXT NOT NULL,
	CharacterName TEXT NOT NULL,
	Account TEXT NULL,
	ZoneName TEXT NULL,
	BindZone TEXT NULL,
	SkyCorpseDate TEXT NULL,
	PRIMARY KEY (ServerName, CharacterName)
);

CREATE TABLE IF NOT EXISTS CharacterNote 
(
	ServerName TEXT NOT NULL,
	CharacterName TEXT NOT NULL,
	Note TEXT NULL,
	PRIMARY KEY (ServerName, CharacterName)
);