-- Database structure --
CREATE DATABASE `sqlquirybuilder_it_db`;
USE `sqlquirybuilder_it_db`;

CREATE TABLE `street` (
    `id` BIGINT NOT NULL,
    `name` VARCHAR(50) NOT NULL,

    PRIMARY KEY (`id`)
);

CREATE TABLE `user` (
    `id` BIGINT NOT NULL,
    `username` VARCHAR(25) NOT NULL UNIQUE,
    `age` INT NOT NULL,
    `color` VARCHAR(50) NULL,
    `counter` INT NOT NULL,
    `street_id` BIGINT NOT NULL,
    `created_at` DATETIME NOT NULL,

    PRIMARY KEY (`id`),
    CONSTRAINT fk_u_s FOREIGN KEY (`street_id`)
        REFERENCES `street` (`id`)
        ON UPDATE RESTRICT ON DELETE CASCADE
);


-- User privileges --
CREATE USER 'sqlqb_it_user'@'%' IDENTIFIED BY 'devapp';
GRANT SELECT, INSERT, UPDATE, DELETE ON `sqlquirybuilder_it_db`.* TO 'sqlqb_it_user'@'%';


-- Insert data --
INSERT INTO `street` VALUES
    (1, 'Swamp'),
    (2, 'Sesame Street');

INSERT INTO `user` VALUES
    (1, 'Kermit the Frog', 67, 'green', 42, 1, '1955-05-09 19:05:00'),
    (2, 'Miss Piggy', 67, 'pink', 5, 1, '1955-05-09 19:05:03'),
    (3, 'Elmo', 3, 'red', 5, 2, '1980-02-03 18:00:00');
