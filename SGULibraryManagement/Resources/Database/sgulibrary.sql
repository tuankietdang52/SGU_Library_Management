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

 Date: 02/05/2025 19:44:53
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for account_violation
-- ----------------------------
DROP TABLE IF EXISTS `account_violation`;
CREATE TABLE `account_violation`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `violation_id` bigint NOT NULL,
  `create_at` datetime NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `FK_violation_account`(`violation_id` ASC) USING BTREE,
  INDEX `FK_av_accounts`(`user_id` ASC) USING BTREE,
  CONSTRAINT `FK_av_accounts` FOREIGN KEY (`user_id`) REFERENCES `accounts` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_av_violations` FOREIGN KEY (`violation_id`) REFERENCES `violations` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 34 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of account_violation
-- ----------------------------
INSERT INTO `account_violation` VALUES (33, 24, 1, '2025-05-02 19:30:47', 0);

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `username` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `password` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `first_name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `last_name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `phone` varchar(11) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `role_id` bigint NOT NULL,
  `avt` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `UNIQ_email`(`email` ASC) USING BTREE,
  INDEX `id`(`id` ASC) USING BTREE,
  INDEX `FK_account_role`(`role_id` ASC) USING BTREE,
  CONSTRAINT `FK_account_role` FOREIGN KEY (`role_id`) REFERENCES `roles` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 55 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of accounts
-- ----------------------------
INSERT INTO `accounts` VALUES (1, 's', '1', 'shiba', 'lmao', '0321312', 'lmaobruh', 1, 'C:/Users/ADMIN/Downloads/avatae.png', 0);
INSERT INTO `accounts` VALUES (2, 'kkk', '123456', 'kkk', 'bruh', '0321321321', 'kkk@gmail.com', 2, '', 0);
INSERT INTO `accounts` VALUES (23, 'user1', 'pass1', 'John', 'Doe', '1234567890', 'john.doe@example.com', 2, 'avatar1', 0);
INSERT INTO `accounts` VALUES (24, 'user2', 'pass2', 'Jane', 'Smith', '1234567891', 'Smith', 2, 'C:\\Users\\ADMIN\\Downloads\\damn.png', 0);
INSERT INTO `accounts` VALUES (25, 'user3', 'pass3', 'Alice', 'Johnson', '1234567892', 'alice.johnson@example.com', 2, 'avatar3', 0);
INSERT INTO `accounts` VALUES (26, 'user4', 'pass4', 'Bob', 'Brown', '1234567893', 'bob.brown@example.com', 2, 'avatar4', 0);

-- ----------------------------
-- Table structure for borrow_devices
-- ----------------------------
DROP TABLE IF EXISTS `borrow_devices`;
CREATE TABLE `borrow_devices`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `device_id` bigint NOT NULL,
  `quantity` int NOT NULL,
  `create_at` datetime NOT NULL,
  `date_borrow` datetime NOT NULL,
  `date_return` datetime NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  `is_return` tinyint NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `FK_device_borrow`(`device_id` ASC) USING BTREE,
  INDEX `FK_account_borrow`(`user_id` ASC) USING BTREE,
  CONSTRAINT `Fk_account_borrow` FOREIGN KEY (`user_id`) REFERENCES `accounts` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `FK_device_borrow` FOREIGN KEY (`device_id`) REFERENCES `devices` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

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
INSERT INTO `devices` VALUES (1, 'lmao', 21, 'C:/Users/ADMIN/Downloads/download.jpg', '', 0, 1);
INSERT INTO `devices` VALUES (2, 'bruh', 21, 'C:/Users/ADMIN/Downloads/download.jpg', '', 0, 1);
INSERT INTO `devices` VALUES (3, 'lmaodsa', 21, 'C:/Users/ADMIN/Downloads/music_note.png', 'kkkk', 0, 0);
INSERT INTO `devices` VALUES (5, 'wassup homie man bro', 31, 'adsad', 'yo mah niga', 0, 1);
INSERT INTO `devices` VALUES (6, 'me th', 31, 'shiba', '', 0, 1);
INSERT INTO `devices` VALUES (7, 'hola', 2, 'csaca', '', 0, 1);

-- ----------------------------
-- Table structure for reservations
-- ----------------------------
DROP TABLE IF EXISTS `reservations`;
CREATE TABLE `reservations`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` bigint NOT NULL,
  `device_id` bigint NOT NULL,
  `quantity` int NOT NULL,
  `create_at` date NOT NULL,
  `date_borrow` date NOT NULL,
  `date_return` date NOT NULL,
  `is_checked_out` tinyint NOT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `device_id`(`device_id` ASC) USING BTREE,
  INDEX `FK_account_reservation`(`user_id` ASC) USING BTREE,
  CONSTRAINT `FK_account_reservation` FOREIGN KEY (`user_id`) REFERENCES `accounts` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
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
-- Table structure for violations
-- ----------------------------
DROP TABLE IF EXISTS `violations`;
CREATE TABLE `violations`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL DEFAULT NULL,
  `is_deleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of violations
-- ----------------------------
INSERT INTO `violations` VALUES (1, 'Not return device', 'User is not return device on time', 0);

SET FOREIGN_KEY_CHECKS = 1;
