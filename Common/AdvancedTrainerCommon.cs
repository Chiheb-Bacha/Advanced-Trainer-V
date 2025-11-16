using GTA;
using System;

namespace Advanced_Trainer
{
    /// <summary>
    /// This class serves as a helper, offering functions commonly used by other classes
    /// </summary>
    public static class AdvancedTrainerCommon
    {
        public static Vehicle GetCurrentVehicle()
        {
            Ped playerPed = Game.LocalPlayerPed;
            if (playerPed == null)
            {
                return null;
            }
            return playerPed.CurrentVehicle;
        }

        public static Ped GetPlayerPed()
        {
            return Game.LocalPlayerPed;
        }

        public static float Snap(float value, float step) => (float)Math.Round(value / step) * step;
    }
}
