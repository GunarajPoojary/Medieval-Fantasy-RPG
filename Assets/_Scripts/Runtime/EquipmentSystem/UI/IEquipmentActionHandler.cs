﻿namespace RPG
{
    public interface IEquipmentActionHandler
    {
        abstract void Equip();

        abstract void Unequip();

        abstract void Enhance();
    }
}