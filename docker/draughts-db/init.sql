-- Create databases --
CREATE DATABASE `draughts_user`;
CREATE DATABASE `draughts_auth`;
CREATE DATABASE `draughts_game`;
CREATE DATABASE `draughts_misc`;


-- Create users --
CREATE USER 'draughts_user'@'%' IDENTIFIED BY 'devapp';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts\_user`.* TO 'draughts_user'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts\_misc`.* TO 'draughts_user'@'%';

CREATE USER 'draughts_auth'@'%' IDENTIFIED BY 'devapp';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts\_auth`.* TO 'draughts_auth'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts\_misc`.* TO 'draughts_auth'@'%';

CREATE USER 'draughts_game'@'%' IDENTIFIED BY 'devapp';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts\_game`.* TO 'draughts_game'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts\_misc`.* TO 'draughts_game'@'%';

CREATE USER 'draughts_console'@'%' IDENTIFIED BY 'devapp';
GRANT ALL PRIVILEGES ON `draughts\_%`.* TO 'draughts_console'@'%';
