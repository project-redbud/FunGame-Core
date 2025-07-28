PRAGMA foreign_keys = OFF;

BEGIN TRANSACTION;

ALTER TABLE MarketItems RENAME TO _MarketItems_old;

CREATE TABLE MarketItems (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 ItemGuid TEXT NOT NULL DEFAULT '',
 UserId INTEGER NOT NULL DEFAULT 0,
 Price REAL NOT NULL DEFAULT 0,
 CreateTime DATETIME NOT NULL DEFAULT (DATETIME('now')),
 FinishTime DATETIME DEFAULT NULL,
 Status INTEGER NOT NULL DEFAULT 0,
 Buyers TEXT NOT NULL DEFAULT ''
);

INSERT INTO MarketItems (
    Id,
    ItemGuid,
    UserId,
    Price,
    CreateTime,
    FinishTime,
    Status,
    Buyers
)
SELECT
    Id,
    ItemGuid,
    UserId,
    Price,
    CreateTime,
    FinishTime,
    Status,
    CAST(Buyer AS TEXT)
FROM _MarketItems_old;

DROP TABLE _MarketItems_old;

COMMIT;

PRAGMA foreign_keys = ON;
