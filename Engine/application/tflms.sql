/*
MySQL Data Transfer
Source Host: localhost
Source Database: tflms
Target Host: localhost
Target Database: tflms
Date: 2018/11/6 18:09:01
*/

SET FOREIGN_KEY_CHECKS=0;
-- ----------------------------
-- Table structure for tflms_account
-- ----------------------------
CREATE TABLE `tflms_account` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `account` varchar(50) NOT NULL,
  `password` varchar(50) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for tflms_log
-- ----------------------------
CREATE TABLE `tflms_log` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `logtext` text,
  `logtime` datetime DEFAULT NULL,
  `form` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records 
-- ----------------------------