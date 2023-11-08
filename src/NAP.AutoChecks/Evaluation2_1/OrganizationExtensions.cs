using TransportDataBe.Client.Models;

namespace NAP.AutoChecks.Evaluation2_1;

public static class OrganizationExtensions
{
    public static bool HasMMTISDeclaration(this Organization organization)
    {
        if (organization.agreement_declaration_mmtis is { Length: > 0 } &&
            organization.agreement_declaration_mmtis[0] == "Y")
        {
            return true;
        }

        return false;
    }

    public static bool HasSSTPDeclaration(this Organization organization)
    {
        return !string.IsNullOrWhiteSpace(organization.sstp_doc_document_upload);
    }

    public static bool HasSRTIDeclaration(this Organization organization)
    {
        return !string.IsNullOrWhiteSpace(organization.srti_doc_document_upload);
    }

    public static bool HasRTTIDeclaration(this Organization organization)
    {
        return !string.IsNullOrWhiteSpace(organization.rtti_doc_document_upload);
    }
}