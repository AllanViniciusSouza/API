-- Script para atualizar a Foreign Key de ItensComanda para usar CASCADE DELETE
-- Execute este script no seu banco de dados SQL Server

-- Primeiro, vamos remover a constraint existente
IF EXISTS (
    SELECT * FROM sys.foreign_keys 
    WHERE name = 'FK_ItensComanda_Comandas_ComandaId' 
    AND parent_object_id = OBJECT_ID('ItensComanda')
)
BEGIN
    ALTER TABLE [ItensComanda] 
    DROP CONSTRAINT [FK_ItensComanda_Comandas_ComandaId];
    
    PRINT 'Foreign Key removida com sucesso.';
END

-- Agora, vamos recriar com CASCADE DELETE
ALTER TABLE [ItensComanda]
ADD CONSTRAINT [FK_ItensComanda_Comandas_ComandaId] 
FOREIGN KEY ([ComandaId]) 
REFERENCES [Comandas]([Id]) 
ON DELETE CASCADE;

PRINT 'Foreign Key recriada com CASCADE DELETE.';

-- Verificar a configuração
SELECT 
    fk.name AS ForeignKeyName,
    fk.delete_referential_action_desc AS DeleteAction
FROM sys.foreign_keys AS fk
WHERE fk.name = 'FK_ItensComanda_Comandas_ComandaId';
