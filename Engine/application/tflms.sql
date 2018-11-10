/*
MySQL Data Transfer
Source Host: localhost
Source Database: tflms
Target Host: localhost
Target Database: tflms
Date: 2018/11/10 21:22:27
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
-- Table structure for tflms_files
-- ----------------------------
CREATE TABLE `tflms_files` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `cid` int(11) NOT NULL,
  `fileUrl` varchar(255) NOT NULL,
  `fileType` varchar(30) DEFAULT NULL,
  `carousel` int(2) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
