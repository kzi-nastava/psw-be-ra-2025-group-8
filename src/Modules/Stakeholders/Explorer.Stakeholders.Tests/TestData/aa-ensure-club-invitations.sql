-- Kreiranje tabele ClubInvitations ako ne postoji
-- Ovo je potrebno jer je tabela dodata naknadno u Init migraciju
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT FROM information_schema.tables 
        WHERE table_schema = 'stakeholders' 
        AND table_name = 'ClubInvitations'
    ) THEN
        CREATE TABLE stakeholders."ClubInvitations" (
            "Id" bigserial PRIMARY KEY,
            "ClubId" bigint NOT NULL,
            "TouristId" bigint NOT NULL,
            "Status" integer NOT NULL,
            "CreatedAt" timestamp with time zone NOT NULL
        );
        
        CREATE INDEX "IX_ClubInvitations_ClubId" 
        ON stakeholders."ClubInvitations" ("ClubId");
        
        CREATE INDEX "IX_ClubInvitations_TouristId" 
        ON stakeholders."ClubInvitations" ("TouristId");
    END IF;
END $$;
