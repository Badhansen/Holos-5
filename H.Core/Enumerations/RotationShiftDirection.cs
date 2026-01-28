using H.Core.Properties;
using H.Infrastructure;

namespace H.Core.Enumerations
{
    /// <summary>
    /// Defines the direction in which crops shift across fields in a rotation.
    /// 
    /// In a rotation system with multiple fields, this determines how the crop sequence
    /// is staggered across fields over time.
    /// 
    /// Example with 3 crops [Wheat, Barley, Oats] and 3 fields:
    /// 
    /// None (No Shift):
    /// - Field 1: Wheat(2020), Barley(2021), Oats(2022), Wheat(2023)
    /// - Field 2: Wheat(2020), Barley(2021), Oats(2022), Wheat(2023)
    /// - Field 3: Wheat(2020), Barley(2021), Oats(2022), Wheat(2023)
    /// 
    /// RightShift (Traditional Rotation):
    /// - Field 1: Wheat(2020), Barley(2021), Oats(2022), Wheat(2023)
    /// - Field 2: Oats(2020), Wheat(2021), Barley(2022), Oats(2023)
    /// - Field 3: Barley(2020), Oats(2021), Wheat(2022), Barley(2023)
    /// 
    /// LeftShift (Reverse Rotation):
    /// - Field 1: Wheat(2020), Barley(2021), Oats(2022), Wheat(2023)
    /// - Field 2: Barley(2020), Oats(2021), Wheat(2022), Barley(2023)
    /// - Field 3: Oats(2020), Wheat(2021), Barley(2022), Oats(2023)
    /// </summary>
    public enum RotationShiftDirection
    {
        /// <summary>
        /// No shifting - all fields follow the exact same crop sequence in the same years.
        /// This results in all fields growing the same crop at the same time.
        /// Useful for: Simplified management, bulk operations, coordinated harvest
        /// </summary>
        [LocalizedDescription("EnumRotationShiftDirectionNone", typeof(Resources))]
        None = 0,

        /// <summary>
        /// Right shift - each subsequent field starts later in the crop sequence.
        /// Field 2 starts one position ahead of Field 1, Field 3 starts one position ahead of Field 2, etc.
        /// This is the traditional rotation pattern that spreads workload and risk.
        /// Useful for: Staggered planting/harvest, risk diversification, pest management
        /// </summary>
        [LocalizedDescription("EnumRotationShiftDirectionRight", typeof(Resources))]
        RightShift = 1,

        /// <summary>
        /// Left shift - each subsequent field starts earlier in the crop sequence.
        /// Field 2 starts one position behind Field 1, Field 3 starts one position behind Field 2, etc.
        /// This is a reverse rotation pattern, less common but useful for specific scenarios.
        /// Useful for: Alternative pest management strategies, experimental rotations
        /// </summary>
        [LocalizedDescription("EnumRotationShiftDirectionLeft", typeof(Resources))]
        LeftShift = 2
    }
}
