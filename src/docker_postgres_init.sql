CREATE TABLE IF NOT EXISTS persons (
  id SERIAL,
  name varchar(250) NOT NULL,
  features double precision[256] NOT NULL,
  PRIMARY KEY (id)
);