PRAGMA foreign_keys = OFF;

-- ----------------------------
-- Table structure for ForgetVerifyCodes
-- ----------------------------
DROP TABLE IF EXISTS "main"."ForgetVerifyCodes";
CREATE TABLE ForgetVerifyCodes (
 Username TEXT NOT NULL DEFAULT '',
 Email TEXT NOT NULL DEFAULT '',
 ForgetVerifyCode TEXT NOT NULL DEFAULT '',
 SendTime DATETIME NOT NULL DEFAULT (DATETIME('now'))
);

-- ----------------------------
-- Table structure for RegVerifyCodes
-- ----------------------------
DROP TABLE IF EXISTS "main"."RegVerifyCodes";
CREATE TABLE RegVerifyCodes (
 Username TEXT NOT NULL DEFAULT '',
 Email TEXT NOT NULL DEFAULT '',
 RegVerifyCode TEXT NOT NULL DEFAULT '',
 RegTime DATETIME NOT NULL DEFAULT (DATETIME('now'))
);

-- ----------------------------
-- Table structure for Rooms
-- ----------------------------
DROP TABLE IF EXISTS "main"."Rooms";
CREATE TABLE "Rooms" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT,
"Roomid"  TEXT NOT NULL DEFAULT '-1',
"CreateTime"  DATETIME NOT NULL DEFAULT (DATETIME('now')),
"RoomMaster"  INTEGER NOT NULL DEFAULT 0,
"RoomType"  INTEGER NOT NULL DEFAULT 0,
"GameModule"  TEXT NOT NULL DEFAULT '',
"GameMap"  TEXT NOT NULL DEFAULT '',
"RoomState"  INTEGER NOT NULL DEFAULT 0,
"IsRank"  INTEGER NOT NULL DEFAULT 0,
"HasPass"  INTEGER NOT NULL DEFAULT 0,
"Password"  TEXT NOT NULL DEFAULT '',
"MaxUsers" INTEGER NOT NULL DEFAULT 0
);

-- ----------------------------
-- Table structure for ServerLoginLogs
-- ----------------------------
DROP TABLE IF EXISTS "main"."ServerLoginLogs";
CREATE TABLE ServerLoginLogs (
 ServerName TEXT NOT NULL DEFAULT '',
 ServerKey TEXT NOT NULL DEFAULT '',
 LoginTime DATETIME NOT NULL DEFAULT (DATETIME('now'))
);

-- ----------------------------
-- Table structure for ApiTokens
-- ----------------------------
DROP TABLE IF EXISTS "main"."ApiTokens";
CREATE TABLE ApiTokens (
 TokenID TEXT NOT NULL DEFAULT '',
 SecretKey TEXT NOT NULL DEFAULT '',
 Reference1 TEXT NOT NULL DEFAULT '',
 Reference2 TEXT NOT NULL DEFAULT '',
 PRIMARY KEY (TokenID)
);

-- ----------------------------
-- Table structure for Users
-- ----------------------------
DROP TABLE IF EXISTS "main"."Users";
CREATE TABLE Users (
 UID INTEGER PRIMARY KEY AUTOINCREMENT,
 Username TEXT NOT NULL,
 Password TEXT NOT NULL,
 RegTime DATETIME NOT NULL DEFAULT (DATETIME('now')),
 LastTime DATETIME NOT NULL DEFAULT (DATETIME('now')),
 LastIP TEXT NOT NULL DEFAULT '',
 Email TEXT NOT NULL DEFAULT '',
 Nickname TEXT NOT NULL DEFAULT '',
 IsAdmin INTEGER NOT NULL DEFAULT 0,
 IsOperator INTEGER NOT NULL DEFAULT 0,
 IsEnable INTEGER NOT NULL DEFAULT 1,
 Credits REAL NOT NULL DEFAULT 0,
 Materials REAL NOT NULL DEFAULT 0,
 GameTime REAL NOT NULL DEFAULT 0,
 AutoKey TEXT NOT NULL DEFAULT ''
);
