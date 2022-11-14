--liquibase formatted sql
--changeset vait:01_activity
--runOnChange:true
--preconditions onFail:MARK_RAN onError:HALT
--precondition-sql-check expectedResult:0 SELECT count(*) FROM information_schema.TABLES WHERE UPPER(TABLE_NAME) = UPPER('activity') AND TABLE_SCHEMA in (SELECT DATABASE())

CREATE TABLE `activity` (
    `activity_id` BIGINT NOT NULL AUTO_INCREMENT,
    `activity_type_id` INT NOT NULL,
    `description` VARCHAR(100) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
    `start_date` DATETIME NOT NULL,
    `duration` FLOAT NOT NULL,
    `distance` FLOAT NOT NULL,
    `exerciseId` BIGINT NOT NULL,
    PRIMARY KEY (`activity_id`),
    constraint fk_act_activity_type foreign key(activity_type_id) references activity_type(activity_type_id)
) ENGINE=InnoDB;