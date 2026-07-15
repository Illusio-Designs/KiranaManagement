namespace Kirana.Domain.Common.Enums;

/// <summary>KYC / verification document categories for store onboarding (PRD FR-1.2).</summary>
public enum DocumentType
{
    BusinessRegistration = 0,
    IdentityProof = 1,
    AddressProof = 2,
    GstCertificate = 3,
    PanCard = 4,
    Other = 5
}
