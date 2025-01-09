
START TRANSACTION;


INSERT INTO "ProductCategories" ("Id", "Name")
VALUES (1, 'Electrónicos y accesorios tecnológicos');
INSERT INTO "ProductCategories" ("Id", "Name")
VALUES (2, 'Deportivos y fitness');

INSERT INTO "Stores" ("Id", "Address", "Name", "Phone")
VALUES ('STR-01', 'Address 1', 'Store 1', '123456789');
INSERT INTO "Stores" ("Id", "Address", "Name", "Phone")
VALUES ('STR-02', 'Address 2', 'Store 2', '987654321');
INSERT INTO "Stores" ("Id", "Address", "Name", "Phone")
VALUES ('STR-03', 'Address 3', 'Store 3', '985632147');

INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-AUDIO-001', 1, 'Audio de alta calidad', 'Auriculares Wireless', 89.99, 'AUDIO003');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-CABL-001', 1, 'Cable HDMI', 'HDMI Premium 2m', 19.99, 'HDMI025');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-CAM-001', 1, 'Cámara compacta HD', 'Cámara Digital X20', 249.99, 'CAM007');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-CASE-001', 1, 'Funda tablet', 'Funda Protectora', 24.99, 'CASE027');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-CHRG-001', 1, 'Hub cargador', 'Charging Hub', 24.99, 'CHRG049');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-CLNR-001', 1, 'Limpiador pantalla', 'Screen Cleaner', 9.99, 'CLN037');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-FAN-001', 1, 'Mini ventilador', 'USB Fan', 12.99, 'FAN043');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-HUB-001', 1, 'Hub USB', 'USB Hub 4-Port', 29.99, 'USB021');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-KEYB-001', 1, 'Teclado gaming', 'Teclado RGB Pro', 129.99, 'KEY013');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-LED-001', 1, 'Luz LED USB', 'LED Light Strip', 19.99, 'LED031');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-MNT-001', 1, 'Soporte tablet', 'Tablet Mount', 16.99, 'MNT047');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-MOUS-001', 1, 'Mouse gaming', 'Mouse Gamer X', 59.99, 'MOUSE015');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-ORG-001', 1, 'Organizador cables', 'Cable Manager', 8.99, 'ORG039');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-PAD-001', 1, 'Almohadilla mouse', 'Mouse Pad Pro', 11.99, 'PAD045');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-PHST-001', 1, 'Soporte móvil', 'Phone Stand', 15.99, 'STND035');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-POWR-001', 1, 'Batería portátil', 'PowerBank 20000', 49.99, 'PWR019');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-PROT-001', 1, 'Protector cable', 'Cable Protector', 6.99, 'PROT041');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-SCRN-001', 1, 'Protector pantalla', 'Screen Guard Pro', 14.99, 'SCRN033');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-SMART-001', 1, 'Producto electrónico versátil', 'Smartphone X1', 299.99, 'SMRT001');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-SPKR-001', 1, 'Altavoz potente', 'Speaker Bluetooth', 79.99, 'SPK009');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-STND-001', 1, 'Soporte laptop', 'Soporte Ergonómico', 39.99, 'STND023');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-TABL-001', 1, 'Tablet última generación', 'Tablet Pro 10', 399.99, 'TAB005');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-WATCH-001', 1, 'Smartwatch avanzado', 'Reloj Smart V2', 199.99, 'WATCH011');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-WCAM-001', 1, 'Webcam HD', 'Webcam Pro 4K', 89.99, 'CAM017');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('ELEC-WIFI-001', 1, 'Adaptador wifi', 'WiFi Adapter Pro', 29.99, 'WIFI029');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-BAG-001', 2, 'Bolsa deporte', 'Bolsa Gym Pro', 34.99, 'GYM024');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-BALL-001', 2, 'Equipamiento deportivo', 'Balón Profesional', 29.99, 'BALL006');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-BAND-001', 2, 'Equipo de entrenamiento', 'Banda Elástica Pro', 19.99, 'BAND012');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-BOTL-001', 2, 'Botella deportiva', 'Botella Térmica', 24.99, 'BTL016');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-FITNS-001', 2, 'Equipo fitness resistente', 'Pesas Ajustables', 199.99, 'FIT004');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-GLOV-001', 2, 'Guantes fitness', 'Guantes Pro-Lift', 29.99, 'GLV020');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-H2O-001', 2, 'Bolsa agua', 'Water Bag', 15.99, 'H2O048');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-HEAD-001', 2, 'Banda cabeza', 'Banda Deportiva', 9.99, 'HEAD032');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-HRTE-001', 2, 'Banda cardio', 'Monitor Cardíaco', 49.99, 'HRT026');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-KNEE-001', 2, 'Rodilleras pro', 'Rodilleras Sport', 19.99, 'KNEE028');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-LOCK-001', 2, 'Candado gym', 'Gym Lock', 9.99, 'LOCK044');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-MEAS-001', 2, 'Banda medición', 'Measure Tape', 8.99, 'MEAS042');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-MTOW-001', 2, 'Toalla pequeña', 'Mini Towel', 7.99, 'TWL040');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-PACK-001', 2, 'Mochila deportiva', 'Mochila Sport Pro', 44.99, 'BAG018');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-RBAN-001', 2, 'Banda resistencia', 'Resistance Band', 13.99, 'BAND046');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-ROPE-001', 2, 'Cuerda saltar', 'Cuerda Pro-Jump', 14.99, 'JUMP030');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-SBAG-001', 2, 'Bolsa zapatos', 'Shoe Bag Sport', 11.99, 'BAG038');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-SHKR-001', 2, 'Botella shake', 'Shaker Pro', 16.99, 'SHAKE034');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-SHOE-001', 2, 'Calzado deportivo', 'Zapatillas Pro-Run', 89.99, 'SHOE010');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-TIME-001', 2, 'Timer digital', 'Sport Timer', 19.99, 'TIME050');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-TOWL-001', 2, 'Toalla deportiva', 'Toalla Microfibra', 14.99, 'TWL022');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-WATCH-001', 2, 'Accesorio deportivo premium', 'Reloj Deportivo Pro', 149.99, 'SPRT002');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-WEAR-001', 2, 'Ropa deportiva premium', 'Camiseta Tech-Fit', 34.99, 'WEAR008');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-WRST-001', 2, 'Muñequeras', 'Wrist Support', 12.99, 'WRST036');
INSERT INTO "Products" ("Id", "CategoryId", "Description", "Name", "Price", "Sku")
VALUES ('SPRT-YOGA-001', 2, 'Accesorio yoga', 'Mat Premium', 39.99, 'YOGA014');

COMMIT;

