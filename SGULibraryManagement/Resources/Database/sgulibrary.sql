/*
 Navicat Premium Data Transfer

 Source Server         : Mysql
 Source Server Type    : MySQL
 Source Server Version : 90001 (9.0.1)
 Source Host           : localhost:3306
 Source Schema         : sgulibrary

 Target Server Type    : MySQL
 Target Server Version : 90001 (9.0.1)
 File Encoding         : 65001

 Date: 23/05/2025 20:58:19
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for account_violation
-- ----------------------------
DROP TABLE IF EXISTS `account_violation`;
CREATE TABLE `account_violation`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `mssv` bigint NOT NULL,
  `violation_id` bigint NOT NULL,
  `create_at` datetime NOT NULL,
  `status` enum('Handled','BeingProcessed') CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `ban_expired` datetime NULL DEFAULT NULL,
  `compensation` bigint NULL DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  `is_ban_eternal` tinyint(1) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `FK_violation_account`(`violation_id` ASC) USING BTREE,
  INDEX `FK_av_accounts`(`mssv` ASC) USING BTREE,
  CONSTRAINT `FK_av_accounts` FOREIGN KEY (`mssv`) REFERENCES `accounts` (`mssv`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_av_violations` FOREIGN KEY (`violation_id`) REFERENCES `violations` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 40 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of account_violation
-- ----------------------------

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts`  (
  `mssv` bigint NOT NULL,
  `password` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `first_name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `last_name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `phone` varchar(11) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `role_id` bigint NOT NULL,
  `faculty` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT 'khoa',
  `major` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL COMMENT 'ngành',
  `avt` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  `otp_code` varchar(6) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `expired_code` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`mssv`) USING BTREE,
  UNIQUE INDEX `UNIQ_email`(`email` ASC) USING BTREE,
  INDEX `id`(`mssv` ASC) USING BTREE,
  INDEX `FK_account_role`(`role_id` ASC) USING BTREE,
  CONSTRAINT `FK_account_role` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of accounts
-- ----------------------------
INSERT INTO `accounts` VALUES (1, '1', 'shiba', 'lmao', '0321312', 'lmao@gmail.com', 1, 'CNTT', 'KTPM', '', 0, NULL, NULL);
INSERT INTO `accounts` VALUES (3122410160, '123', 'Nguyễn Võ', 'Trung Hưng ', '0906695646', 'hung@gmail.com', 1, 'CNTT', 'KTPM', '', 0, NULL, NULL);
INSERT INTO `accounts` VALUES (3122410321, '123', 'Huỳnh Minh', 'Phúc', '0949100204', 'phuc@gmail.com', 1, 'CNTT', 'KTPM', '', 0, NULL, NULL);
INSERT INTO `accounts` VALUES (3122410337, '456', 'Nguyễn Nhật', 'Quang', '0397575084', 'hackermiennam1@gmail.com', 1, 'CNTT', 'KTPM', '', 0, '023304', '2025-05-05 14:53:58');

-- ----------------------------
-- Table structure for borrow_devices
-- ----------------------------
DROP TABLE IF EXISTS `borrow_devices`;
CREATE TABLE `borrow_devices`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `mssv` bigint NOT NULL,
  `device_id` bigint NOT NULL,
  `code` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `quantity` int NOT NULL,
  `create_at` datetime NOT NULL,
  `date_borrow` datetime NOT NULL,
  `date_return` datetime NULL DEFAULT NULL,
  `date_return_expected` datetime NOT NULL,
  `is_return` tinyint NULL DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `UNIQ_code`(`code` ASC) USING BTREE,
  INDEX `FK_device_borrow`(`device_id` ASC) USING BTREE,
  INDEX `FK_account_borrow`(`mssv` ASC) USING BTREE,
  CONSTRAINT `Fk_account_borrow` FOREIGN KEY (`mssv`) REFERENCES `accounts` (`mssv`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_device_borrow` FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of borrow_devices
-- ----------------------------

-- ----------------------------
-- Table structure for devices
-- ----------------------------
DROP TABLE IF EXISTS `devices`;
CREATE TABLE `devices`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `quantity` int NOT NULL,
  `img` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  `is_available` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 8 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of devices
-- ----------------------------

-- ----------------------------
-- Table structure for reservations
-- ----------------------------
DROP TABLE IF EXISTS `reservations`;
CREATE TABLE `reservations`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `mssv` bigint NOT NULL,
  `device_id` bigint NOT NULL,
  `quantity` int NOT NULL,
  `create_at` date NOT NULL,
  `date_borrow` date NOT NULL,
  `date_return` date NOT NULL,
  `is_checked_out` tinyint NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `device_id`(`device_id` ASC) USING BTREE,
  INDEX `FK_account_reservation`(`mssv` ASC) USING BTREE,
  CONSTRAINT `FK_account_reservation` FOREIGN KEY (`mssv`) REFERENCES `accounts` (`mssv`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_device_reservation` FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of reservations
-- ----------------------------

-- ----------------------------
-- Table structure for roles
-- ----------------------------
DROP TABLE IF EXISTS `roles`;
CREATE TABLE `roles`  (
  `id` bigint NOT NULL,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of roles
-- ----------------------------
INSERT INTO `roles` VALUES (1, 'Admin', 0);
INSERT INTO `roles` VALUES (2, 'User', 0);

-- ----------------------------
-- Table structure for study_area
-- ----------------------------
DROP TABLE IF EXISTS `study_area`;
CREATE TABLE `study_area`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `mssv` bigint NOT NULL,
  `check_in_date` datetime NOT NULL,
  `is_deleted` tinyint NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `FK_sa_user`(`mssv` ASC) USING BTREE,
  CONSTRAINT `FK_sa_user` FOREIGN KEY (`mssv`) REFERENCES `accounts` (`mssv`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 11 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of study_area
-- ----------------------------

-- ----------------------------
-- Table structure for violations
-- ----------------------------
DROP TABLE IF EXISTS `violations`;
CREATE TABLE `violations`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of violations
-- ----------------------------
INSERT INTO `violations` VALUES (1, 'Not return device', 'User is not return device on time', 0);
INSERT INTO `violations` VALUES (5, 'shiba', '321312', 1);

SET FOREIGN_KEY_CHECKS = 1;
