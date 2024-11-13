PRAGMA foreign_keys = OFF;

-- ----------------------------
-- Table structure for forgetverifycodes
-- ----------------------------
DROP TABLE IF EXISTS "main"."forgetverifycodes";
CREATE TABLE forgetverifycodes (
  Username TEXT,
  Email TEXT,
  ForgetVerifyCode TEXT,
  SendTime DATETIME
);

-- ----------------------------
-- Table structure for regverifycodes
-- ----------------------------
DROP TABLE IF EXISTS "main"."regverifycodes";
CREATE TABLE regverifycodes (
  Username TEXT,
  Email TEXT,
  RegVerifyCode TEXT,
  RegTime DATETIME
);

-- ----------------------------
-- Table structure for rooms
-- ----------------------------
DROP TABLE IF EXISTS "main"."rooms";
CREATE TABLE "rooms" (
"Id"  INTEGER PRIMARY KEY AUTOINCREMENT,
"Roomid"  TEXT NOT NULL DEFAULT '-1',
"CreateTime"  DATETIME NOT NULL,
"RoomMaster"  INTEGER NOT NULL DEFAULT 0,
"RoomType"  INTEGER DEFAULT 0,
"GameModule"  TEXT DEFAULT '',
"GameMap"  TEXT DEFAULT '',
"RoomState"  INTEGER DEFAULT 0,
"IsRank"  INTEGER DEFAULT 0,
"HasPass"  INTEGER DEFAULT 0,
"Password"  TEXT DEFAULT ''
);

-- ----------------------------
-- Table structure for serverloginlogs
-- ----------------------------
DROP TABLE IF EXISTS "main"."serverloginlogs";
CREATE TABLE serverloginlogs (
  ServerName TEXT,
  ServerKey TEXT,
  LoginTime DATETIME
);

-- ----------------------------
-- Table structure for sqlite_sequence
-- ----------------------------
DROP TABLE IF EXISTS "main"."sqlite_sequence";
CREATE TABLE sqlite_sequence(name,seq);

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS "main"."users";
CREATE TABLE users (
  UID INTEGER PRIMARY KEY AUTOINCREMENT,
  Username TEXT NOT NULL,
  Password TEXT NOT NULL,
  RegTime DATETIME,
  LastTime DATETIME,
  LastIP TEXT DEFAULT '',
  Email TEXT NOT NULL DEFAULT '',
  Nickname TEXT DEFAULT '',
  IsAdmin INTEGER DEFAULT 0,
  IsOperator INTEGER DEFAULT 0,
  IsEnable INTEGER DEFAULT 1,
  Credits REAL DEFAULT 0,
  Materials REAL DEFAULT 0,
  GameTime REAL DEFAULT 0,
  AutoKey TEXT DEFAULT ''
);
