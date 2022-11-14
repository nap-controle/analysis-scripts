using System.Text.Json.Serialization;

namespace TransportDataBe.Client.Models;

public class Organization
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Name { get; set; }
    
    public string[]? agreement_declaration_mmtis { get; set; }
    
    public string rtti_doc_document_upload { get; set; }

    public string srti_doc_document_upload { get; set; }
    
    public string sstp_doc_document_upload { get; set; }
}