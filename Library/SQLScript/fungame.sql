SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for forgetverifycodes
-- ----------------------------
DROP TABLE IF EXISTS `forgetverifycodes`;
CREATE TABLE `forgetverifycodes` (
  `Username` varchar(255) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `ForgetVerifyCode` varchar(255) DEFAULT NULL,
  `SendTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for regverifycodes
-- ----------------------------
DROP TABLE IF EXISTS `regverifycodes`;
CREATE TABLE `regverifycodes` (
  `Username` varchar(255) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `RegVerifyCode` varchar(255) DEFAULT NULL,
  `RegTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for rooms
-- ----------------------------
DROP TABLE IF EXISTS `rooms`;
CREATE TABLE `rooms` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Roomid` varchar(255) NOT NULL DEFAULT '-1',
  `CreateTime` datetime NOT NULL,
  `RoomMaster` bigint(20) NOT NULL DEFAULT '0',
  `RoomType` int(8) DEFAULT '0',
  `GameModule` varchar(255) DEFAULT '',
  `GameMap` varchar(255) DEFAULT '',
  `RoomState` int(8) DEFAULT '0',
  `IsRank` int(1) DEFAULT '0',
  `HasPass` int(1) DEFAULT '0',
  `Password` varchar(255) DEFAULT '',
  `MaxUsers` int(8) DEFAULT '0',
  PRIMARY KEY (`Id`,`Roomid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for serverloginlogs
-- ----------------------------
DROP TABLE IF EXISTS `serverloginlogs`;
CREATE TABLE `serverloginlogs` (
  `ServerName` varchar(255) DEFAULT NULL,
  `ServerKey` varchar(255) DEFAULT NULL,
  `LoginTime` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ----------------------------
-- Table structure for users
-- ----------------------------
DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `UID` bigint(20) NOT NULL AUTO_INCREMENT,
  `Username` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `RegTime` datetime DEFAULT NULL,
  `LastTime` datetime DEFAULT NULL,
  `LastIP` varchar(255) DEFAULT '',
  `Email` varchar(255) NOT NULL DEFAULT '',
  `Nickname` varchar(255) DEFAULT '',
  `IsAdmin` int(1) DEFAULT '0',
  `IsOperator` int(1) DEFAULT '0',
  `IsEnable` int(1) DEFAULT '1',
  `Credits` decimal(20,0) DEFAULT '0',
  `Materials` decimal(20,0) DEFAULT '0',
  `GameTime` decimal(20,0) DEFAULT '0',
  `AutoKey` varchar(255) DEFAULT '',
  PRIMARY KEY (`UID`,`Username`,`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4;
