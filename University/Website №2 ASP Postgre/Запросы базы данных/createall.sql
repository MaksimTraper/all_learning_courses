INSERT INTO transport_vehicle (pk_id_vehicle, brand, model) VALUES
(1, 'Volgabus', '5270'),
(2, 'Nefaz', '5299'),
(3, 'Volgabus', '4298'),
(4, 'Maz', '303'),
(5, 'Golden Dragon', 'XML6155');

INSERT INTO user_account (passport_num, name, surname, patronymic, pk_login, password, email, role, birthday) VALUES
('3456123567' ,'Maksim', 'Traper', 'Konstantinovich', 'MaksudUchiha', 'kedroviyles421', 'dekstor235@gmail.com', 'admin',  '2002-08-07'),
('2345920514', 'Viktor', 'Blud', 'Antonovich', 'viktorblud', 'kirov0grad', 'novayapochta1246@yandex.ru', 'user',  '1956-09-25'),
('2357257124', 'Anton', 'Blud', 'Vitalyevich', 'antoshkabluuud', 'y@bed@@@', 'bestboy228@gmail.com', 'user',  '2003-03-05'),
('5467357321', 'Kirill', 'Ston', 'Mikhaylovich', 'gazgaz', 'eafEFOIJfxz', 'gazgazgaz@inbox.com', 'user',  '1992-03-05'),
('4565772721', 'Stepan', 'Pyrin', 'Ahmatovich', 'styopamr', 'qwerty1234', 'misterx@mail.ru', 'user',  '2006-04-17'),
('5689563401', 'Kim', 'Luon', 'Kuoy', 'KimLuKu', 'panthers1423@', 'torch@gmail.com', 'user',  '1985-12-04');

INSERT INTO transport_card (pk_id_card, fk_id_owner, balance, data_issue, num_days) VALUES
(1, 'MaksudUchiha', 5678.44, '2023-12-24', 0),
(2, 'viktorblud', 456.13, '2021-06-16', 3),
(3, 'antoshkabluuud', 500.00, '2018-02-08', 0),
(4, 'gazgaz', 123.78, '2023-10-31', 10),
(5, 'styopamr', 12578.78, '2022-05-05', 0),
(6, 'KimLuKu', 1365.42, '2020-08-14', 0);

INSERT INTO DRIVER (pk_id_driver, name, surname, patronymic) VALUES
(1, 'Petr', 'Sidorov', 'Ivanovich'),
(2, 'Viktor', 'Kot', 'Stepanovich'),
(3, 'Maksim', 'Traper', 'Konstantinovich'),
(4, 'Leonid', 'Vorobyev', 'Alexeevich'),
(5, 'Andrey', 'Kirillov', 'Ivanovich');

INSERT INTO transport VALUES
('Л416СП78', 3),
('Р564АЦ78', 1),
('П647ПО178', 5),
('А536ПП178', 4),
('Я124ПО78', 1),
('У578АЦ178', 2),
('К235ПК78', 2),
('А923ММ98', 3),
('М435ЕР98', 5);

INSERT INTO Route (PK_ID_Route, start_point, end_point) VALUES
(191, 'Ст.м.Петроградская', 'Ст.м.Улица Дыбенко'),
(40, 'Улица кораблестроителей', 'Тихорецкий проспект'),
(35, 'Станция Сортировочная', 'Проспект Героев'),
(21, 'Ст.м.Ладожская', 'Финляндский вокзал'),
(100, 'Ст.м.Проспект просвещения', 'Ст.м.Гражданский проспект'),
(3, 'Финляндский вокзал', 'Площадь Репина');

INSERT INTO TRIP (fk_ID_Tr, fk_ID_card, fk_ID_driver, time_pay, num_route) VALUES
('А536ПП178', 5, 1, '2023-01-08 16:49:13', 3),
('Л416СП78', 5, 2, '2023-01-08 22:23:13', 3),
('П647ПО178', 3, 4, '2023-01-08 16:14:46', 191),
('А923ММ98', 2, 5, '2022-01-08 14:34:32', 100),
('А536ПП178', 1, 2, '2022-01-08 11:23:47', 3),
('А923ММ98', 1, 2, '2023-01-08 8:45:14', 191),
('П647ПО178', 3, 3, '2023-01-08 7:17:57', 100),
('А536ПП178', 4, 4, '2022-01-08 7:42:21', 3),
('К235ПК78', 4, 5, '2023-01-08 14:13:04', 40),
('К235ПК78', 1, 5, '2021-01-08 18:03:21', 3),
('Я124ПО78', 1, 1, '2023-01-08 12:14:45', 191),
('М435ЕР98', 1, 2, '2023-01-08 4:56:11', 21),
('Я124ПО78', 2, 3, '2021-01-08 5:00:35', 21),
('П647ПО178', 3, 1, '2022-01-08 18:56:01', 191);

INSERT INTO purchase (fk_id_card, name_purchase, price, amount) VALUES
(2, 'buy days', 250, 3),
(4, 'buy days', 750, 10),
(1, 'add balance', 5678.44, 0);