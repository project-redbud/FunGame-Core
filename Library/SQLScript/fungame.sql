SET FOREIGN_KEY_CHECKS=0;

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
-- Table structure for Users
-- ----------------------------
DROP TABLE IF EXISTS `Users`;
CREATE TABLE `Users` (
  `UID` bigint(20) NOT NULL AUTO_INCREMENT,
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
  `Credits` decimal(20,0) NOT NULL DEFAULT '0',
  `Materials` decimal(20,0) NOT NULL DEFAULT '0',
  `GameTime` decimal(20,0) NOT NULL DEFAULT '0',
  `AutoKey` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`UID`,`Username`,`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4;
