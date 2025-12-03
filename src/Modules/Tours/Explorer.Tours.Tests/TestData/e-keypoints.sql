-- KeyPoints za Test Tour1 (TourId = -10)
-- Tura kroz Novi Sad centar
INSERT INTO tours."KeyPoints" ("Id", "TourId", "Order", "Latitude", "Longitude", "Name", "Description", "ImageUrl", "Secret")
VALUES 
    (-1, -10, 1, 45.2671, 19.8335, 'Trg Slobode', 'Po?etna ta?ka - centar grada', '', ''),
    (-2, -10, 2, 45.2551, 19.8451, 'Petrovaradinska tvr?ava', 'Istorijska tvr?ava na Dunavu', '', ''),
    (-3, -10, 3, 45.2396, 19.8227, 'Exitova bina', 'Poznato mesto festivala', '', '');

-- KeyPoints za Test Tour2 (TourId = -11)
-- Tura kroz Beograd
INSERT INTO tours."KeyPoints" ("Id", "TourId", "Order", "Latitude", "Longitude", "Name", "Description", "ImageUrl", "Secret")
VALUES 
    (-4, -11, 1, 44.8176, 20.4633, 'Kalemegdan', 'Beogradska tvr?ava', '', ''),
    (-5, -11, 2, 44.8125, 20.4612, 'Knez Mihailova', 'Glavna peša?ka zona', '', ''),
    (-6, -11, 3, 44.8048, 20.4781, 'Hram Svetog Save', 'Najve?a pravoslavna crkva na Balkanu', '', '');

-- KeyPoints za Other Author Tour (TourId = -12)
-- Kratka tura kroz Kopaonik
INSERT INTO tours."KeyPoints" ("Id", "TourId", "Order", "Latitude", "Longitude", "Name", "Description", "ImageUrl", "Secret")
VALUES 
    (-7, -12, 1, 43.2900, 20.8081, 'Suvo Rudište', 'Po?etak staze', '', ''),
    (-8, -12, 2, 43.2950, 20.8100, 'Pan?i?ev vrh', 'Najviši vrh Kopaonika', '', '');
