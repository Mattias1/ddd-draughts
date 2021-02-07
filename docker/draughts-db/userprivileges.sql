-- User --
CREATE USER 'draughts_user'@'%' IDENTIFIED BY 'devapp';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts_user`.* TO 'draughts_user'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts_misc`.* TO 'draughts_user'@'%';


-- Auth user --
CREATE USER 'draughts_authuser'@'%' IDENTIFIED BY 'devapp';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts_authuser`.* TO 'draughts_authuser'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts_misc`.* TO 'draughts_authuser'@'%';


-- Game --
CREATE USER 'draughts_game'@'%' IDENTIFIED BY 'devapp';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts_game`.* TO 'draughts_game'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON `draughts_misc`.* TO 'draughts_game'@'%';
