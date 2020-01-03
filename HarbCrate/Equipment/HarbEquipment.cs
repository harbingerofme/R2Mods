using RoR2;

namespace HarbCrate.Equipment
{
    public abstract class HarbEquipment
    {
        public static readonly string Name;
        public static readonly float Cooldown;

        public static CustomEquipment Build() { return null; }
        public static bool Effect(EquipmentSlot equipmentSlot) { return false; }
    }
}
