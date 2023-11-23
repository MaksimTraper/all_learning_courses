CREATE TABLE user_account
(
	passport_num VARCHAR(30) UNIQUE,
	name VARCHAR(30),
	surname VARCHAR(30),
	patronymic VARCHAR(30),
	pk_login VARCHAR(30) PRIMARY KEY,
	password VARCHAR(100),
	email VARCHAR(30) UNIQUE,
	role VARCHAR(10), CHECK (role IN ('admin','user')),
	birthday DATE
);

CREATE TABLE transport_card
(
	pk_id_card SERIAL PRIMARY KEY,
	fk_id_owner VARCHAR(30) REFERENCES User_account(pk_login) ON DELETE CASCADE ON UPDATE CASCADE,
	balance NUMERIC(9,2) DEFAULT 0 CHECK(balance >= 0),
	data_issue DATE,
	num_days INT DEFAULT 0 CHECK (num_days >= 0)
);

CREATE TABLE transport_vehicle
(
	pk_id_vehicle SERIAL PRIMARY KEY,
	brand VARCHAR(30),
	model VARCHAR(30)
);

CREATE TABLE driver
(
	pk_id_driver SERIAL PRIMARY KEY,
	name VARCHAR(30),
	surname VARCHAR(30),
	patronymic VARCHAR(30)
);

CREATE TABLE transport
(
	pk_car_num VARCHAR(10) PRIMARY KEY,
	fk_id_vehicle INT REFERENCES Transport_vehicle(pk_id_vehicle) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE route
(
	pk_id_route INT PRIMARY KEY,
	start_point VARCHAR(30),
	end_point VARCHAR(30)
);
CREATE TABLE trip
(
	pk_num_trip SERIAL PRIMARY KEY,
	fk_ID_Tr VARCHAR(10) REFERENCES Transport(PK_car_num) ON DELETE SET NULL ON UPDATE CASCADE,
	fk_ID_card INT REFERENCES Transport_card(PK_ID_card) ON DELETE SET NULL ON UPDATE CASCADE,
	fk_ID_driver INT REFERENCES Driver(PK_ID_driver) ON DELETE SET NULL ON UPDATE CASCADE,
	time_pay timestamp,
	num_route INT REFERENCES Route(PK_ID_Route) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE purchase
(
	pk_num_purchase SERIAL PRIMARY KEY,
	fk_id_card INT REFERENCES Transport_card(PK_ID_card) ON DELETE SET NULL ON UPDATE CASCADE,
	name_purchase VARCHAR(30) NOT NULL, CHECK(name_purchase IN ('add balance','buy days')),
	price NUMERIC (9, 2) NOT NULL,
	amount INT DEFAULT 0
)