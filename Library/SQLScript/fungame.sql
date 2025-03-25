SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for ApiTokens
-- ----------------------------
DROP TABLE IF EXISTS `ApiTokens`;
CREATE TABLE `ApiTokens` (
 `TokenID` varchar(255) NOT NULL DEFAULT '',
 `SecretKey` varchar(255) NOT NULL DEFAULT '',
 `Reference1` varchar(255) NOT NULL DEFAULT '',
 `Reference2` varchar(255) NOT NULL DEFAULT '',
 PRIMARY KEY (`TokenID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for ForgetVerifyCodes
-- ----------------------------
DROP TABLE IF EXISTS `ForgetVerifyCodes`;
CREATE TABLE `ForgetVerifyCodes` (
 `Username` varchar(255) NOT NULL DEFAULT '',
 `Email` varchar(255) NOT NULL DEFAULT '',
 `ForgetVerifyCode` varchar(255) NOT NULL DEFAULT '',
 `SendTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for GoodItems
-- ----------------------------
DROP TABLE IF EXISTS `GoodItems`;
CREATE TABLE `GoodItems` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `GoodId` bigint(20) NOT NULL DEFAULT '0',
 `ItemId` bigint(20) NOT NULL DEFAULT '0',
 PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for GoodPrices
-- ----------------------------
DROP TABLE IF EXISTS `GoodPrices`;
CREATE TABLE `GoodPrices` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `GoodId` bigint(20) NOT NULL DEFAULT '0',
 `Currency` varchar(255) NOT NULL DEFAULT '',
 `Price` double(20,0) NOT NULL DEFAULT '0',
 PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for Goods
-- ----------------------------
DROP TABLE IF EXISTS `Goods`;
CREATE TABLE `Goods` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `Name` varchar(255) NOT NULL DEFAULT '',
 `Description` varchar(255) NOT NULL DEFAULT '',
 `Stock` int(10) NOT NULL DEFAULT '0',
 PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for Inventories
-- ----------------------------
DROP TABLE IF EXISTS `Inventories`;
CREATE TABLE `Inventories` (
 `UserId` bigint(20) NOT NULL DEFAULT '0',
 `Name` varchar(255) NOT NULL DEFAULT '',
 `Credits` decimal(20,0) NOT NULL DEFAULT '0',
 `Materials` decimal(20,0) NOT NULL DEFAULT '0',
 `MainCharacter` bigint(20) NOT NULL DEFAULT '0',
 PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for MarketItems
-- ----------------------------
DROP TABLE IF EXISTS `MarketItems`;
CREATE TABLE `MarketItems` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `ItemId` bigint(255) NOT NULL DEFAULT '0',
 `UserId` bigint(255) NOT NULL DEFAULT '0',
 `Price` double(20,0) NOT NULL DEFAULT '0',
 `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
 `FinishTime` datetime DEFAULT NULL,
 `Status` int(10) NOT NULL DEFAULT '0',
 `Buyer` bigint(20) NOT NULL DEFAULT '0',
 PRIMARY KEY (`Id`,`ItemId`,`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for OfferItems
-- ----------------------------
DROP TABLE IF EXISTS `OfferItems`;
CREATE TABLE `OfferItems` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `OfferId` bigint(20) NOT NULL DEFAULT '0',
 `UserId` bigint(20) NOT NULL DEFAULT '0',
 `ItemId` bigint(20) NOT NULL DEFAULT '0',
 PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for Offers
-- ----------------------------
DROP TABLE IF EXISTS `Offers`;
CREATE TABLE `Offers` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `Offeror` bigint(20) NOT NULL DEFAULT '0',
 `Offeree` bigint(20) NOT NULL DEFAULT '0',
 `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
 `FinishTime` datetime DEFAULT NULL,
 `Status` int(10) NOT NULL DEFAULT '0',
 PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for RegVerifyCodes
-- ----------------------------
DROP TABLE IF EXISTS `RegVerifyCodes`;
CREATE TABLE `RegVerifyCodes` (
 `Username` varchar(255) NOT NULL DEFAULT '',
 `Email` varchar(255) NOT NULL DEFAULT '',
 `RegVerifyCode` varchar(255) NOT NULL DEFAULT '',
 `RegTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for Rooms
-- ----------------------------
DROP TABLE IF EXISTS `Rooms`;
CREATE TABLE `Rooms` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `Roomid` varchar(255) NOT NULL DEFAULT '-1',
 `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
 `RoomMaster` bigint(20) NOT NULL DEFAULT '0',
 `RoomType` int(8) NOT NULL DEFAULT '0',
 `GameModule` varchar(255) NOT NULL DEFAULT '',
 `GameMap` varchar(255) NOT NULL DEFAULT '',
 `RoomState` int(8) NOT NULL DEFAULT '0',
 `IsRank` int(1) NOT NULL DEFAULT '0',
 `HasPass` int(1) NOT NULL DEFAULT '0',
 `Password` varchar(255) NOT NULL DEFAULT '',
 `MaxUsers` int(8) NOT NULL DEFAULT '0',
 PRIMARY KEY (`Id`,`Roomid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for ServerLoginLogs
-- ----------------------------
DROP TABLE IF EXISTS `ServerLoginLogs`;
CREATE TABLE `ServerLoginLogs` (
 `ServerName` varchar(255) NOT NULL DEFAULT '',
 `ServerKey` varchar(255) NOT NULL DEFAULT '',
 `LoginTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for Stores
-- ----------------------------
DROP TABLE IF EXISTS `Stores`;
CREATE TABLE `Stores` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `StoreName` varchar(255) NOT NULL DEFAULT '',
 `StartTime` datetime DEFAULT NULL,
 `EndTime` datetime DEFAULT NULL,
 PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for UserCharacters
-- ----------------------------
DROP TABLE IF EXISTS `UserCharacters`;
CREATE TABLE `UserCharacters` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `CharacterId` bigint(20) NOT NULL DEFAULT '0',
 `UserId` bigint(20) NOT NULL DEFAULT '0',
 `Name` varchar(255) NOT NULL DEFAULT '',
 `FirstName` varchar(255) NOT NULL DEFAULT '',
 `NickName` varchar(255) NOT NULL DEFAULT '',
 `PrimaryAttribute` int(1) NOT NULL DEFAULT '0',
 `InitialATK` double(20,0) NOT NULL DEFAULT '0',
 `InitialDEF` double(20,0) NOT NULL DEFAULT '0',
 `InitialHP` double(20,0) NOT NULL DEFAULT '0',
 `InitialMP` double(20,0) NOT NULL DEFAULT '0',
 `InitialAGI` double(20,0) NOT NULL DEFAULT '0',
 `InitialINT` double(20,0) NOT NULL DEFAULT '0',
 `InitialSTR` double(20,0) NOT NULL DEFAULT '0',
 `InitialSPD` double(20,0) NOT NULL DEFAULT '0',
 `InitialHR` double(20,0) NOT NULL DEFAULT '0',
 `InitialMR` double(20,0) NOT NULL DEFAULT '0',
 `InSquad` int(1) NOT NULL DEFAULT '0',
 `TrainingTime` datetime DEFAULT NULL,
 PRIMARY KEY (`Id`,`CharacterId`,`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for UserItems
-- ----------------------------
DROP TABLE IF EXISTS `UserItems`;
CREATE TABLE `UserItems` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `ItemId` bigint(20) NOT NULL DEFAULT '0',
 `UserId` bigint(20) NOT NULL DEFAULT '0',
 `CharacterId` bigint(20) NOT NULL DEFAULT '0',
 `ItemName` varchar(255) NOT NULL DEFAULT '',
 `IsLock` int(1) NOT NULL DEFAULT '0',
 `Equipable` int(1) NOT NULL DEFAULT '0',
 `Unequipable` int(1) NOT NULL DEFAULT '0',
 `EquipSlotType` int(2) NOT NULL DEFAULT '0',
 `Key` int(10) NOT NULL DEFAULT '0',
 `Enable` int(1) NOT NULL DEFAULT '0',
 `Price` double(20,0) NOT NULL DEFAULT '0',
 `IsSellable` int(1) NOT NULL DEFAULT '0',
 `IsTradable` int(1) NOT NULL DEFAULT '0',
 `NextSellableTime` datetime DEFAULT NULL,
 `NextTradableTime` datetime DEFAULT NULL,
 `RemainUseTimes` int(10) NOT NULL DEFAULT '0',
 PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for UserLogs
-- ----------------------------
DROP TABLE IF EXISTS `UserLogs`;
CREATE TABLE `UserLogs` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `UserId` bigint(20) NOT NULL DEFAULT '0',
 `Title` varchar(255) NOT NULL DEFAULT '',
 `Description` varchar(255) NOT NULL DEFAULT '',
 `Remark` varchar(255) NOT NULL DEFAULT '',
 `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
 PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for Users
-- ----------------------------
DROP TABLE IF EXISTS `Users`;
CREATE TABLE `Users` (
 `Id` bigint(20) NOT NULL AUTO_INCREMENT,
 `Username` varchar(255) NOT NULL DEFAULT '',
 `Password` varchar(255) NOT NULL DEFAULT '',
 `RegTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
 `LastTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
 `LastIP` varchar(255) NOT NULL DEFAULT '',
 `Email` varchar(255) NOT NULL DEFAULT '',
 `Nickname` varchar(255) NOT NULL DEFAULT '',
 `IsAdmin` int(1) NOT NULL DEFAULT '0',
 `IsOperator` int(1) NOT NULL DEFAULT '0',
 `IsEnable` int(1) NOT NULL DEFAULT '1',
 `Credits` double(20,0) NOT NULL DEFAULT '0',
 `Materials` double(20,0) NOT NULL DEFAULT '0',
 `GameTime` double(20,0) NOT NULL DEFAULT '0',
 `AutoKey` varchar(255) NOT NULL DEFAULT '',
 PRIMARY KEY (`Id`,`Username`,`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for UserSignIns
-- ----------------------------
DROP TABLE IF EXISTS `UserSignIns`;
CREATE TABLE `UserSignIns` (
 `UserId` bigint(20) NOT NULL DEFAULT '0',
 `LastTime` datetime DEFAULT NULL,
 `Days` int(10) NOT NULL DEFAULT '0',
 `IsSigned` int(1) NOT NULL DEFAULT '0',
 PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
