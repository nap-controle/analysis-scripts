using NAP.AutoChecks.Domain;

namespace NAP.AutoChecks.Evaluation1_2;

public class StratifiedSamplingSetting
{
    public int SelectMMTIS { get; set; } = 25;

    public int SelectSRTI { get; set; } = 5;

    public int SelectRTTI { get; set; } = 5;

    public int SelectSSTP { get; set; } = 5;

    public int Total => this.SelectRTTI + this.SelectSRTI + this.SelectSSTP + this.SelectMMTIS;

    public int SelectCountFor(NAPType type)
    {
        return type switch
        {
            NAPType.RTTI => this.SelectRTTI,
            NAPType.SRTI => this.SelectSRTI,
            NAPType.SSTP => this.SelectSSTP,
            NAPType.MMTIS => this.SelectMMTIS,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}