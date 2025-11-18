using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace TWGKafkaConsumerService
{
    public class KafkaMessage
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("invoiceDate")]
        public string? InvoiceDate { get; set; }

        [JsonPropertyName("lineLevelTaxationIndicator")]
        public bool LineLevelTaxationIndicator { get; set; }

        [JsonPropertyName("supplierNote")]
        public string? SupplierNote { get; set; }

        [JsonPropertyName("paymentTerm")]
        public string? PaymentTerm { get; set; }

        [JsonPropertyName("totalAmount")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? TotalAmount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("comments")]
        public string? Comments { get; set; }

        [JsonPropertyName("documentType")]
        public string? DocumentType { get; set; }

        [JsonPropertyName("originalInvoiceIdentifier")]
        public string? OriginalInvoiceIdentifier { get; set; }

        [JsonPropertyName("originalInvoiceDate")]
        public string? OriginalInvoiceDate { get; set; }

        [JsonPropertyName("discountDueDate")]
        public string? DiscountDueDate { get; set; }

        [JsonPropertyName("paymentDueDate")]
        public string? PaymentDueDate { get; set; }

        [JsonPropertyName("discountAmount")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? DiscountAmount { get; set; }

        [JsonPropertyName("discountPercent")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? DiscountPercent { get; set; }

        [JsonPropertyName("grossTotalAmount")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? GrossTotalAmount { get; set; }

        [JsonPropertyName("netTotalAmount")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? NetTotalAmount { get; set; }

        [JsonPropertyName("taxableAmount")]  
        [JsonConverter(typeof(NullableDecimalConverter))]    
        public decimal? TaxableAmount { get; set; }

        [JsonPropertyName("purchaseOrderIdentifier")]
        public string? PurchaseOrderIdentifier { get; set; }

        [JsonPropertyName("numberOfLines")]
        public int NumberOfLines { get; set; }

        [JsonPropertyName("tax")]
        public Tax? Tax { get; set; }

        [JsonPropertyName("parties")]
        public List<Party>? Parties { get; set; }

        [JsonPropertyName("lines")]
        public List<Line>? Lines { get; set; }

        [JsonPropertyName("barcode")]
        public string? Barcode { get; set; }

        [JsonPropertyName("vendorId")]
        public string? VendorId { get; set; }
    }

    public class Tax
    {
        [JsonPropertyName("amount")]
        [JsonConverter(typeof(NullableDecimalConverter))]
        public decimal? Amount { get; set; }

        [JsonPropertyName("rate")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? Rate { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class Party
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("accountIdentifier")]
        public string? AccountIdentifier { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("location")]
        public Location? Location { get; set; }

        [JsonPropertyName("contact")]
        public Contact? Contact { get; set; }

        [JsonPropertyName("phoneNumber")]
        public PhoneNumber? PhoneNumber { get; set; }

        [JsonPropertyName("emailAddress")]
        public string? EmailAddress { get; set; }

        [JsonPropertyName("taxIdentifier")]
        public string? TaxIdentifier { get; set; }

        [JsonPropertyName("taxCountryCode")]
        public string? TaxCountryCode { get; set; }

        [JsonPropertyName("taxCountryName")]
        public string? TaxCountryName { get; set; }

        [JsonPropertyName("legalEntityName")]
        public string? LegalEntityName { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("typeCode")]
        public string? TypeCode { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("address")]
        public Address? Address { get; set; }
    }

    public class Address
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("addressLines")]
        public List<string>? AddressLines { get; set; }

        [JsonPropertyName("suburb")]
        public string? Suburb { get; set; }

        [JsonPropertyName("cityName")]
        public string? CityName { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("countryName")]
        public string? CountryName { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("streetName")]
        public string? StreetName { get; set; }

        [JsonPropertyName("addressVerificationIdentifier")]
        public string? AddressVerificationIdentifier { get; set; }
    }

    public class Contact
    {
        [JsonPropertyName("personName")]
        public PersonName? PersonName { get; set; }
    }

    public class PersonName
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("attentionOf")]
        public string? AttentionOf { get; set; }

        [JsonPropertyName("preferredName")]
        public string? PreferredName { get; set; }
    }

    public class PhoneNumber
    {
        [JsonPropertyName("countryDialingCode")]
        public string? CountryDialingCode { get; set; }

        [JsonPropertyName("areaDialingCode")]
        public string? AreaDialingCode { get; set; }

        [JsonPropertyName("dialNumber")]
        public string? DialNumber { get; set; }

        [JsonPropertyName("phoneExtension")]
        public string? PhoneExtension { get; set; }
    }

    public class Line
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("createdAtDateTime")]
        public string? CreatedAtDateTime { get; set; }

        [JsonPropertyName("updatedAtDateTime")]
        public string? UpdatedAtDateTime { get; set; }

        [JsonPropertyName("total")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? Total { get; set; }

        [JsonPropertyName("number")]
        public long? Number { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("purchaseOrderLine")]
        public PurchaseOrderLine? PurchaseOrderLine { get; set; }

        [JsonPropertyName("unitPrice")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? UnitPrice { get; set; }

        [JsonPropertyName("quantity")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? Quantity { get; set; }

        [JsonPropertyName("unitOfMeasure")]
        public string? UnitOfMeasure { get; set; }

        [JsonPropertyName("discountAmount")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? DiscountAmount { get; set; }

        [JsonPropertyName("supplierPartNumber")]
        public string? SupplierPartNumber { get; set; }

        [JsonPropertyName("globalTradeItemNumber")]
        public string? GlobalTradeItemNumber { get; set; }

        [JsonPropertyName("itemIdentifier")]
        public string? ItemIdentifier { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        [JsonPropertyName("discountPercentage")]
        [JsonConverter(typeof(NullableDecimalConverter))] 
        public decimal? DiscountPercentage { get; set; }

        [JsonPropertyName("tax")]
        public Tax? Tax { get; set; }
    }

    public class PurchaseOrderLine
    {
        [JsonPropertyName("identifier")]
        public string? Identifier { get; set; }

        [JsonPropertyName("number")]
        public long? Number { get; set; }
    }



public class NullableDecimalConverter : JsonConverter<decimal?>
{
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDecimal();
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
                return null;
            if (decimal.TryParse(s, out var d))
                return d;
        }
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
}