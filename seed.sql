\connect ksummarized

CREATE TABLE blog
(
 id serial PRIMARY KEY,
 title VARCHAR (50) NOT NULL,
 description VARCHAR (100) NOT NULL
);

ALTER TABLE blog OWNER TO ksummarized;

Insert into blog(title,description) values( 'Title 1','Description 1');
Insert into blog(title,description) values( 'Title 2','Description 2');
Insert into blog(title,description) values( 'Title 3','Description 3');
Insert into blog(title,description) values( 'Title 4','Description 4');
