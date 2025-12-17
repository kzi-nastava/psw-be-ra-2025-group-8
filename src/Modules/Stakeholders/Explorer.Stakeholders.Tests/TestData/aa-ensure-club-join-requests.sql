-- Kreiranje tabele ClubJoinRequests ako ne postoji
-- Ovo je potrebno jer je tabela dodata naknadno u Init migraciju
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT FROM information_schema.tables 
        WHERE table_schema = 'stakeholders' 
        AND table_name = 'ClubJoinRequests'
    ) THEN
        CREATE TABLE stakeholders."ClubJoinRequests" (
            "Id" bigserial PRIMARY KEY,
            "ClubId" bigint NOT NULL,
            "TouristId" bigint NOT NULL,
            "Status" integer NOT NULL,
            "CreatedAt" timestamp with time zone NOT NULL
        );
        
        CREATE INDEX "IX_ClubJoinRequests_ClubId" 
        ON stakeholders."ClubJoinRequests" ("ClubId");
        
        CREATE INDEX "IX_ClubJoinRequests_TouristId" 
        ON stakeholders."ClubJoinRequests" ("TouristId");
    END IF;
END $$;
