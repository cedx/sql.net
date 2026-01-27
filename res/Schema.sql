DROP TABLE IF EXISTS "Characters";
CREATE TABLE "Characters" (
	"Id" integer PRIMARY KEY,
	"FirstName" text NOT NULL CHECK (LENGTH("FirstName") > 0),
	"LastName" text,
	"Gender" text NOT NULL CHECK ("Gender" IN ('Balrog', 'DarkLord', 'Dwarf', 'Elf', 'Hobbit', 'Human', 'Istari'))
);

CREATE INDEX "IX_Characters_Gender" ON "Characters" ("Gender");
INSERT INTO "Characters" ("FirstName", "LastName", "Gender") VALUES
	('Aragorn', NULL, 'Human'),
	('Balin', NULL, 'Dwarf'),
	('Boromir', NULL, 'Human'),
	('Durin''s Bane', NULL, 'Balrog'),
	('Elrond', NULL, 'Elf'),
	('Frodo', 'Baggins', 'Hobbit'),
	('Galadriel', NULL, 'Elf'),
	('Gandalf', NULL, 'Istari'),
	('Gimli', NULL, 'Dwarf'),
	('Gollum', NULL, 'Hobbit'),
	('Gothmog', NULL, 'Balrog'),
	('Legolas', NULL, 'Elf'),
	('Pippin', 'Took', 'Hobbit'),
	('Sam', 'Gamgee', 'Hobbit'),
	('Saruman', NULL, 'Istari'),
	('Sauron', NULL, 'DarkLord');
