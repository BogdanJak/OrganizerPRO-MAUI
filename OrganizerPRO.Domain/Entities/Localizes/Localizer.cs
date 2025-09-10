namespace OrganizerPRO.Domain.Entities.Localizes;


public class Localizer : BaseAuditableEntity, IAuditTrial
{
    [Key]
    public Guid Id { get; set; }
    public string Keytext { get; set; }

    [MaxLength(int.MaxValue)]
    public string? EnEN { get; set; }

    [MaxLength(int.MaxValue)]
    public string? PlPL { get; set; }

    [MaxLength(int.MaxValue)]
    public string? DeDE { get; set; }

    [MaxLength(int.MaxValue)]
    public string? FrFR { get; set; }

    [MaxLength(int.MaxValue)]
    public string? RuRU { get; set; }

    [MaxLength(int.MaxValue)]
    public string? EsES { get; set; }

    [MaxLength(int.MaxValue)]
    public string? ArAR { get; set; }
    [MaxLength(int.MaxValue)]
    public string? CsCS { get; set; }
    [MaxLength(int.MaxValue)]
    public string? DaDA { get; set; }
    [MaxLength(int.MaxValue)]
    public string? FiFI { get; set; }
    [MaxLength(int.MaxValue)]
    public string? HeHE { get; set; }
    [MaxLength(int.MaxValue)]
    public string? HrHR { get; set; }
    [MaxLength(int.MaxValue)]
    public string? HuHU { get; set; }
    [MaxLength(int.MaxValue)]
    public string? ItIT { get; set; }
    [MaxLength(int.MaxValue)]
    public string? NbNB { get; set; }
    [MaxLength(int.MaxValue)]
    public string? NlNL { get; set; }
    [MaxLength(int.MaxValue)]
    public string? PtPT { get; set; }
    [MaxLength(int.MaxValue)]
    public string? SkSK { get; set; }
    [MaxLength(int.MaxValue)]
    public string? SvSV { get; set; }
    [MaxLength(int.MaxValue)]
    public string? TrTR { get; set; }
}

