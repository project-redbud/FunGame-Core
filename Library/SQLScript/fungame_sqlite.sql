PRAGMA foreign_keys = OFF;

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
-- Table structure for Configs
-- ----------------------------
DROP TABLE IF EXISTS "main"."Configs";
CREATE TABLE Configs (
  Id TEXT NOT NULL DEFAULT '',
  Content TEXT NOT NULL DEFAULT '',
  Description TEXT NOT NULL DEFAULT '',
  UpdateTime DATETIME NOT NULL DEFAULT (DATETIME('now')),
  PRIMARY KEY (Id)
);

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
-- Table structure for GoodsItems
-- ----------------------------
DROP TABLE IF EXISTS "main"."GoodsItems";
CREATE TABLE GoodsItems (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 GoodsId INTEGER NOT NULL DEFAULT 0,
 ItemId INTEGER NOT NULL DEFAULT 0
);

-- ----------------------------
-- Table structure for GoodsPrices
-- ----------------------------
DROP TABLE IF EXISTS "main"."GoodsPrices";
CREATE TABLE GoodsPrices (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 GoodsId INTEGER NOT NULL DEFAULT 0,
 Currency TEXT NOT NULL DEFAULT '',
 Price REAL NOT NULL DEFAULT 0
);

-- ----------------------------
-- Table structure for Goods
-- ----------------------------
DROP TABLE IF EXISTS "main"."Goods";
CREATE TABLE Goods (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 Name TEXT NOT NULL DEFAULT '',
 Description TEXT NOT NULL DEFAULT '',
 Stock INTEGER NOT NULL DEFAULT 0
);

-- ----------------------------
-- Table structure for Inventories
-- ----------------------------
DROP TABLE IF EXISTS "main"."Inventories";
CREATE TABLE Inventories (
 UserId INTEGER NOT NULL DEFAULT 0,
 Name TEXT NOT NULL DEFAULT '',
 Credits REAL NOT NULL DEFAULT 0,
 Materials REAL NOT NULL DEFAULT 0,
 MainCharacter INTEGER NOT NULL DEFAULT 0,
 PRIMARY KEY (UserId)
);

-- ----------------------------
-- Table structure for MarketItems
-- ----------------------------
DROP TABLE IF EXISTS "main"."MarketItems";
CREATE TABLE MarketItems (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 ItemGuid TEXT NOT NULL DEFAULT '',
 UserId INTEGER NOT NULL DEFAULT 0,
 Price REAL NOT NULL DEFAULT 0,
 CreateTime DATETIME NOT NULL DEFAULT (DATETIME('now')),
 FinishTime DATETIME DEFAULT NULL,
 Status INTEGER NOT NULL DEFAULT 0,
 Buyer INTEGER NOT NULL DEFAULT 0
);

-- ----------------------------
-- Table structure for OfferItems
-- ----------------------------
DROP TABLE IF EXISTS "main"."OfferItems";
CREATE TABLE OfferItems (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 OfferId INTEGER NOT NULL DEFAULT 0,
 UserId INTEGER NOT NULL DEFAULT 0,
 ItemGuid TEXT NOT NULL DEFAULT ''
);

-- ----------------------------
-- Table structure for OfferItemsBackup
-- ----------------------------
DROP TABLE IF EXISTS "main"."OfferItemsBackup";
CREATE TABLE OfferItemsBackup (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 OfferId INTEGER NOT NULL DEFAULT 0,
 UserId INTEGER NOT NULL DEFAULT 0,
 ItemGuid TEXT NOT NULL DEFAULT ''
);

-- ----------------------------
-- Table structure for Offers
-- ----------------------------
DROP TABLE IF EXISTS "main"."Offers";
CREATE TABLE Offers (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 Offeror INTEGER NOT NULL DEFAULT 0,
 Offeree INTEGER NOT NULL DEFAULT 0,
 CreateTime DATETIME NOT NULL DEFAULT (DATETIME('now')),
 FinishTime DATETIME DEFAULT NULL,
 Status INTEGER NOT NULL DEFAULT 0,
 NegotiatedTimes INTEGER NOT NULL DEFAULT 0
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
CREATE TABLE Rooms (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 Roomid TEXT NOT NULL DEFAULT '-1',
 CreateTime DATETIME NOT NULL DEFAULT (DATETIME('now')),
 RoomMaster INTEGER NOT NULL DEFAULT 0,
 RoomType INTEGER NOT NULL DEFAULT 0,
 GameModule TEXT NOT NULL DEFAULT '',
 GameMap TEXT NOT NULL DEFAULT '',
 RoomState INTEGER NOT NULL DEFAULT 0,
 IsRank INTEGER NOT NULL DEFAULT 0,
 HasPass INTEGER NOT NULL DEFAULT 0,
 Password TEXT NOT NULL DEFAULT '',
 MaxUsers INTEGER NOT NULL DEFAULT 0
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
-- Table structure for StoreGoods
-- ----------------------------
DROP TABLE IF EXISTS "main"."StoreGoods";
CREATE TABLE StoreGoods (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 StoreId INTEGER NOT NULL DEFAULT 0,
 GoodsId INTEGER NOT NULL DEFAULT 0
);

-- ----------------------------
-- Table structure for Stores
-- ----------------------------
DROP TABLE IF EXISTS "main"."Stores";
CREATE TABLE Stores (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 StoreName TEXT NOT NULL DEFAULT '',
 StartTime DATETIME DEFAULT NULL,
 EndTime DATETIME DEFAULT NULL
);

-- ----------------------------
-- Table structure for UserCharacters
-- ----------------------------
DROP TABLE IF EXISTS "main"."UserCharacters";
CREATE TABLE UserCharacters (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 CharacterId INTEGER NOT NULL DEFAULT 0,
 CharacterGuid TEXT NOT NULL DEFAULT '',
 UserId INTEGER NOT NULL DEFAULT 0,
 Name TEXT NOT NULL DEFAULT '',
 FirstName TEXT NOT NULL DEFAULT '',
 NickName TEXT NOT NULL DEFAULT '',
 PrimaryAttribute INTEGER NOT NULL DEFAULT 0,
 InitialATK REAL NOT NULL DEFAULT 0,
 InitialDEF REAL NOT NULL DEFAULT 0,
 InitialHP REAL NOT NULL DEFAULT 0,
 InitialMP REAL NOT NULL DEFAULT 0,
 InitialAGI REAL NOT NULL DEFAULT 0,
 InitialINT REAL NOT NULL DEFAULT 0,
 InitialSTR REAL NOT NULL DEFAULT 0,
 InitialSPD REAL NOT NULL DEFAULT 0,
 InitialHR REAL NOT NULL DEFAULT 0,
 InitialMR REAL NOT NULL DEFAULT 0,
 Level INTEGER NOT NULL DEFAULT 0,
 LevelBreak INTEGER NOT NULL DEFAULT 0,
 InSquad INTEGER NOT NULL DEFAULT 0,
 TrainingTime DATETIME DEFAULT NULL
);

-- ----------------------------
-- Table structure for UserItems
-- ----------------------------
DROP TABLE IF EXISTS "main"."UserItems";
CREATE TABLE UserItems (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 ItemId INTEGER NOT NULL DEFAULT 0,
 ItemGuid TEXT NOT NULL DEFAULT '',
 UserId INTEGER NOT NULL DEFAULT 0,
 CharacterGuid TEXT NOT NULL DEFAULT '',
 ItemName TEXT NOT NULL DEFAULT '',
 IsLock INTEGER NOT NULL DEFAULT 0,
 Equipable INTEGER NOT NULL DEFAULT 0,
 Unequipable INTEGER NOT NULL DEFAULT 0,
 EquipSlotType INTEGER NOT NULL DEFAULT 0,
 Key INTEGER NOT NULL DEFAULT 0,
 Enable INTEGER NOT NULL DEFAULT 0,
 Price REAL NOT NULL DEFAULT 0,
 IsSellable INTEGER NOT NULL DEFAULT 0,
 IsTradable INTEGER NOT NULL DEFAULT 0,
 NextSellableTime DATETIME DEFAULT NULL,
 NextTradableTime DATETIME DEFAULT NULL,
 RemainUseTimes INTEGER NOT NULL DEFAULT 0
);

-- ----------------------------
-- Table structure for UserLogs
-- ----------------------------
DROP TABLE IF EXISTS "main"."UserLogs";
CREATE TABLE UserLogs (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 UserId INTEGER NOT NULL DEFAULT 0,
 Title TEXT NOT NULL DEFAULT '',
 Description TEXT NOT NULL DEFAULT '',
 Remark TEXT NOT NULL DEFAULT '',
 CreateTime DATETIME NOT NULL DEFAULT (DATETIME('now'))
);

-- ----------------------------
-- Table structure for UserProfiles
-- ----------------------------
DROP TABLE IF EXISTS "main"."UserProfiles";
CREATE TABLE UserProfiles (
 UserId INTEGER NOT NULL DEFAULT 0,
 AvatarUrl TEXT NOT NULL DEFAULT '',
 Signature TEXT NOT NULL DEFAULT '',
 Gender TEXT NOT NULL DEFAULT '',
 BirthDay DATETIME DEFAULT NULL,
 Followers INTEGER NOT NULL DEFAULT 0,
 Following INTEGER NOT NULL DEFAULT 0,
 Title TEXT NOT NULL DEFAULT '',
 UserGroup TEXT NOT NULL DEFAULT '',
 PRIMARY KEY (UserId)
);

-- ----------------------------
-- Table structure for Users
-- ----------------------------
DROP TABLE IF EXISTS "main"."Users";
CREATE TABLE Users (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 Username TEXT NOT NULL DEFAULT '',
 Password TEXT NOT NULL DEFAULT '',
 RegTime DATETIME NOT NULL DEFAULT (DATETIME('now')),
 LastTime DATETIME NOT NULL DEFAULT (DATETIME('now')),
 LastIP TEXT NOT NULL DEFAULT '',
 Email TEXT NOT NULL DEFAULT '',
 Nickname TEXT NOT NULL DEFAULT '',
 IsAdmin INTEGER NOT NULL DEFAULT 0,
 IsOperator INTEGER NOT NULL DEFAULT 0,
 IsEnable INTEGER NOT NULL DEFAULT 1,
 GameTime REAL NOT NULL DEFAULT 0,
 AutoKey TEXT NOT NULL DEFAULT ''
);

-- ----------------------------
-- Table structure for UserSignIns
-- ----------------------------
DROP TABLE IF EXISTS "main"."UserSignIns";
CREATE TABLE UserSignIns (
 UserId INTEGER NOT NULL DEFAULT 0,
 LastTime DATETIME DEFAULT NULL,
 Days INTEGER NOT NULL DEFAULT 0,
 IsSigned INTEGER NOT NULL DEFAULT 0,
 PRIMARY KEY (UserId)
);
