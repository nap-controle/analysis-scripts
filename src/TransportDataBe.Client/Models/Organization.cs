using System.Text.Json.Serialization;

namespace TransportDataBe.Client.Models;

public class Organization
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Name { get; set; }

    public string image_url { get; set; }

    public string country { get; set; }

    public string administrative_area { get; set; }

    public string postal_code { get; set; }

    public string city { get; set; }

    public string street_address { get; set; }

    public string do_email { get; set; }

    public string do_tel { get; set; }

    public string[]? agreement_declaration_mmtis { get; set; }

    public string rtti_doc_document_upload { get; set; }

    public string srti_doc_document_upload { get; set; }

    public string sstp_doc_document_upload { get; set; }

    public string proxy_pdf_url { get; set; }
}