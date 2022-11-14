--liquibase formatted sql
--changeset vait:01_tag
--runOnChange:true
--preconditions onFail:MARK_RAN onError:HALT
--precondition-sql-check expectedResult:0 SELECT count(*) FROM information_schema.TABLES WHERE UPPER(TABLE_NAME) = UPPER('tag') AND TABLE_SCHEMA in (SELECT DATABASE())

CREATE TABLE `tag` (
	`tag_id` INT NOT NULL AUTO_INCREMENT,
	`tag` VARCHAR(20) CHARACTER SET utf8 NOT NULL,
	UNIQUE KEY `u_tag` (`tag`) USING BTREE,
	PRIMARY KEY (`tag_id`)
) ENGINE=InnoDB;