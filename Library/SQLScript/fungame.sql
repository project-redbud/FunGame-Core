SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for apitokens
-- ----------------------------
DROP TABLE IF EXISTS `apitokens`;
CREATE TABLE `apitokens` (
  `TokenID` varchar(255) NOT NULL DEFAULT '',
  `SecretKey` varchar(255) NOT NULL DEFAULT '',
  `Reference1` varchar(255) NOT NULL DEFAULT '',
  `Reference2` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`TokenID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for forgetverifycodes
-- ----------------------------
DROP TABLE IF EXISTS `forgetverifycodes`;
CREATE TABLE `forgetverifycodes` (
  `Username` varchar(255) NOT NULL DEFAULT '',
  `Email` varchar(255) NOT NULL DEFAULT '',
  `ForgetVerifyCode` varchar(255) NOT NULL DEFAULT '',
  `SendTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for gooditems
-- ----------------------------
DROP TABLE IF EXISTS `gooditems`;
CREATE TABLE `gooditems` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `GoodId` bigint(20) NOT NULL DEFAULT '0',
  `ItemId` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for goodprices
-- ----------------------------
DROP TABLE IF EXISTS `goodprices`;
CREATE TABLE `goodprices` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `GoodId` bigint(20) NOT NULL DEFAULT '0',
  `Currency` varchar(255) NOT NULL DEFAULT '',
  `Price` double(20,0) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for goods
-- ----------------------------
DROP TABLE IF EXISTS `goods`;
CREATE TABLE `goods` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) NOT NULL DEFAULT '',
  `Description` varchar(255) NOT NULL DEFAULT '',
  `Stock` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for inventories
-- ----------------------------
DROP TABLE IF EXISTS `inventories`;
CREATE TABLE `inventories` (
  `UserId` bigint(20) NOT NULL DEFAULT '0',
  `Name` varchar(255) NOT NULL DEFAULT '',
  `Credits` decimal(20,0) NOT NULL DEFAULT '0',
  `Materials` decimal(20,0) NOT NULL DEFAULT '0',
  `MainCharacter` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for marketitems
-- ----------------------------
DROP TABLE IF EXISTS `marketitems`;
CREATE TABLE `marketitems` (
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
-- Table structure for offeritems
-- ----------------------------
DROP TABLE IF EXISTS `offeritems`;
CREATE TABLE `offeritems` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `OfferId` bigint(20) NOT NULL DEFAULT '0',
  `UserId` bigint(20) NOT NULL DEFAULT '0',
  `ItemId` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for offers
-- ----------------------------
DROP TABLE IF EXISTS `offers`;
CREATE TABLE `offers` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Offeror` bigint(20) NOT NULL DEFAULT '0',
  `Offeree` bigint(20) NOT NULL DEFAULT '0',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FinishTime` datetime DEFAULT NULL,
  `Status` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for regverifycodes
-- ----------------------------
DROP TABLE IF EXISTS `regverifycodes`;
CREATE TABLE `regverifycodes` (
  `Username` varchar(255) NOT NULL DEFAULT '',
  `Email` varchar(255) NOT NULL DEFAULT '',
  `RegVerifyCode` varchar(255) NOT NULL DEFAULT '',
  `RegTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for rooms
-- ----------------------------
DROP TABLE IF EXISTS `rooms`;
CREATE TABLE `rooms` (
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
-- Table structure for serverloginlogs
-- ----------------------------
DROP TABLE IF EXISTS `serverloginlogs`;
CREATE TABLE `serverloginlogs` (
  `ServerName` varchar(255) NOT NULL DEFAULT '',
  `ServerKey` varchar(255) NOT NULL DEFAULT '',
  `LoginTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for stores
-- ----------------------------
DROP TABLE IF EXISTS `stores`;
CREATE TABLE `stores` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `StoreName` varchar(255) NOT NULL DEFAULT '',
  `StartTime` datetime DEFAULT NULL,
  `EndTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for usercharacters
-- ----------------------------
DROP TABLE IF EXISTS `usercharacters`;
CREATE TABLE `usercharacters` (
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
-- Table structure for useritems
-- ----------------------------
DROP TABLE IF EXISTS `useritems`;
CREATE TABLE `useritems` (
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
-- Table structure for userlogs
-- ----------------------------
DROP TABLE IF EXISTS `userlogs`;
CREATE TABLE `userlogs` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `UserId` bigint(20) NOT NULL DEFAULT '0',
  `Title` varchar(255) NOT NULL DEFAULT '',
  `Description` varchar(255) NOT NULL DEFAULT '',
  `Remark` varchar(255) NOT NULL DEFAULT '',
  `CreateTime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
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
-- Table structure for usersignins
-- ----------------------------
DROP TABLE IF EXISTS `usersignins`;
CREATE TABLE `usersignins` (
  `UserId` bigint(20) NOT NULL DEFAULT '0',
  `LastTime` datetime DEFAULT NULL,
  `Days` int(10) NOT NULL DEFAULT '0',
  `IsSigned` int(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
