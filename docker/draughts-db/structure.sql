-- User --
CREATE DATABASE `draughts_user`;
USE `draughts_user`;

CREATE TABLE `user` (
    `id` BIGINT NOT NULL,
    `username` VARCHAR(25) NOT NULL UNIQUE,
    `rating` INT NOT NULL,
    `rank` VARCHAR(50) NOT NULL,
    `total_played` INT NOT NULL,
    `total_won` INT NOT NULL,
    `total_tied` INT NOT NULL,
    `total_lost` INT NOT NULL,
    `international_played` INT NOT NULL,
    `international_won` INT NOT NULL,
    `international_tied` INT NOT NULL,
    `international_lost` INT NOT NULL,
    `english_american_played` INT NOT NULL,
    `english_american_won` INT NOT NULL,
    `english_american_tied` INT NOT NULL,
    `english_american_lost` INT NOT NULL,
    `other_played` INT NOT NULL,
    `other_won` INT NOT NULL,
    `other_tied` INT NOT NULL,
    `other_lost` INT NOT NULL,
    `created_at` DATETIME NOT NULL,

    PRIMARY KEY (`id`)
);

CREATE TABLE `event` (
    `id` BIGINT NOT NULL,
    `type` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL,
    `last_attempted_at` DATETIME NOT NULL,
    `nr_of_attempts` SMALLINT UNSIGNED NOT NULL,

    PRIMARY KEY (`id`)
);


-- Auth user --
CREATE DATABASE `draughts_authuser`;
USE `draughts_authuser`;

CREATE TABLE `authuser` (
    `id` BIGINT NOT NULL,
    `username` VARCHAR(25) NOT NULL UNIQUE,
    `password_hash` VARCHAR(200) NOT NULL,
    `email` VARCHAR(200) NOT NULL,
    `created_at` DATETIME NOT NULL,

    PRIMARY KEY (`id`)
);

CREATE TABLE `role` (
    `id` BIGINT NOT NULL,
    `rolename` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL,

    PRIMARY KEY (`id`)
);

CREATE TABLE `authuser_role` (
    `user_id` BIGINT NOT NULL,
    `role_id` BIGINT NOT NULL,

    PRIMARY KEY (`user_id`, `role_id`),
    CONSTRAINT fk_aur_au FOREIGN KEY (`user_id`)
        REFERENCES `authuser` (`id`)
        ON UPDATE RESTRICT ON DELETE CASCADE,
    CONSTRAINT fk_aur_role FOREIGN KEY (`role_id`)
        REFERENCES `role` (`id`)
        ON UPDATE RESTRICT ON DELETE CASCADE
);

CREATE TABLE `permission_role` (
    `role_id` BIGINT NOT NULL,
    `permission` VARCHAR(50) NOT NULL,

    PRIMARY KEY (`role_id`, `permission`),
    CONSTRAINT fk_pr_role FOREIGN KEY (`role_id`)
        REFERENCES `role` (`id`)
        ON UPDATE RESTRICT ON DELETE CASCADE
);

CREATE TABLE `adminlog` (
    `id` BIGINT NOT NULL,
    `type` VARCHAR(50) NOT NULL,
    `parameters` VARCHAR(50) NULL,
    `user_id` BIGINT NOT NULL,
    `username` VARCHAR(25) NOT NULL,
    `permission` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL,

    PRIMARY KEY (`id`),
    CONSTRAINT fk_al_au FOREIGN KEY (`user_id`)
        REFERENCES `authuser` (`id`)
        ON UPDATE RESTRICT ON DELETE CASCADE
);

CREATE TABLE `event` (
    `id` BIGINT NOT NULL,
    `type` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL,
    `last_attempted_at` DATETIME NOT NULL,
    `nr_of_attempts` SMALLINT UNSIGNED NOT NULL,

    PRIMARY KEY (`id`)
);


-- Game --
CREATE DATABASE `draughts_game`;
USE `draughts_game`;

CREATE TABLE `game` (
    `id` BIGINT NOT NULL,
    `board_size` TINYINT UNSIGNED NOT NULL,
    `first_move_color_is_white` BIT NOT NULL,
    `flying_kings` BIT NOT NULL,
    `men_capture_backwards` BIT NOT NULL,
    `capture_constraints` VARCHAR(3) NOT NULL,
    `victor` BIGINT NULL,
    `created_at` DATETIME NOT NULL,
    `started_at` DATETIME NULL,
    `finished_at` DATETIME NULL,
    `turn_player_id` BIGINT NULL,
    `turn_created_at` DATETIME NULL,
    `turn_expires_at` DATETIME NULL,

    PRIMARY KEY (`id`)
);

CREATE TABLE `player` (
    `id` BIGINT NOT NULL,
    `user_id` BIGINT NOT NULL,
    `game_id` BIGINT NOT NULL,
    `username` VARCHAR(25) NOT NULL,
    `rank` VARCHAR(50) NOT NULL,
    `color` BIT NOT NULL,
    `created_at` DATETIME NOT NULL,

    PRIMARY KEY (`id`),
    CONSTRAINT fk_p_game FOREIGN KEY (`game_id`)
        REFERENCES `game` (`id`)
        ON UPDATE RESTRICT ON DELETE CASCADE
);

CREATE TABLE `gamestate` (
    `id` BIGINT NOT NULL,
    `initial_game_state` VARCHAR(72) NULL,

    PRIMARY KEY (`id`),
    CONSTRAINT fk_gs_game FOREIGN KEY (`id`)
        REFERENCES `game` (`id`)
        ON UPDATE RESTRICT ON DELETE CASCADE
);

CREATE TABLE `move` (
    `game_id` BIGINT NOT NULL,
    `index` SMALLINT NOT NULL,
    `from` TINYINT UNSIGNED NOT NULL,
    `to` TINYINT UNSIGNED NOT NULL,
    `is_capture` BIT NOT NULL,

    PRIMARY KEY (`game_id`, `index`),
    CONSTRAINT fk_move_gs FOREIGN KEY (`game_id`)
        REFERENCES `gamestate` (`id`)
        ON UPDATE RESTRICT ON DELETE CASCADE
);

CREATE TABLE `event` (
    `id` BIGINT NOT NULL,
    `type` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL,
    `last_attempted_at` DATETIME NOT NULL,
    `nr_of_attempts` SMALLINT UNSIGNED NOT NULL,

    PRIMARY KEY (`id`)
);


-- Misc --
CREATE DATABASE `draughts_misc`;
USE `draughts_misc`;

CREATE TABLE `id_generation` (
    `subject` CHAR(4) NOT NULL,
    `available_id` BIGINT NOT NULL
);
