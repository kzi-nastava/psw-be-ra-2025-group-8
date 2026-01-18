-- =============================================
-- BRISANJE I KREIRANJE PERSON TABELE
-- =============================================

-- 1. OBRIŠI SVE IZ PERSON TABELE
DELETE FROM stakeholders."People";

-- 2. RESETUJ AUTO-INCREMENT (ID sequence)
ALTER SEQUENCE stakeholders."People_Id_seq" RESTART WITH 1;

-- 3. UBACI PERSON ZAPISE NA OSNOVU USERS TABELE
INSERT INTO stakeholders."People" ("UserId", "Name", "Surname", "Email", "ProfilePicture", "Bio", "Motto", "Experience", "Level")
SELECT 
    u."Id" as "UserId",
    CASE 
        WHEN u."Username" = 'rade' THEN 'Rade'
        WHEN u."Username" = 'milos' THEN 'Miloš'
        WHEN u."Username" = 'zoran' THEN 'Zoran'
        ELSE INITCAP(u."Username")
    END as "Name",
    CASE 
        WHEN u."Username" = 'rade' THEN 'Radi?'
        WHEN u."Username" = 'milos' THEN 'Miloševi?'
        WHEN u."Username" = 'zoran' THEN 'Zori?'
        ELSE 'Prezime'
    END as "Surname",
    LOWER(u."Username") || '@example.com' as "Email",
    NULL as "ProfilePicture",
    NULL as "Bio",
    NULL as "Motto",
    0 as "Experience",
    1 as "Level"
FROM stakeholders."Users" u
WHERE u."IsActive" = true;

-- 4. PROVERA - PRIKAŽI SVE PERSON ZAPISE
SELECT * FROM stakeholders."People";
