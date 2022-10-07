-- +migrate Up
CREATE TABLE IF NOT EXISTS userstate (
    userid			    VARCHAR(60)     NOT NULL,
    currentstate		int			    DEFAULT 0,
    updatetime		    timestamp	    DEFAULT current_timestamp(),
    
    PRIMARY KEY (userid),
    FOREIGN KEY (userid) REFERENCES users (id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS userstate_log (
	id					int			        auto_increment	primary key,
	userid				VARCHAR(40)         NOT NULL,
    changestate			int,
    updatetime			timestamP			DEFAULT current_timestamp(),
    
    FOREIGN KEY (userid) REFERENCES users (id) ON DELETE CASCADE
);


-- +migrate Down