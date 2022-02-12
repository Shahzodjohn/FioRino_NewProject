using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.DataTransferObjects
{
    public class AdditionalField
    {
        public string field_id { get; set; }
        public string type { get; set; }
        public string locate { get; set; }
        public string req { get; set; }
        public string active { get; set; }
        public string order { get; set; }
        public string field_value { get; set; }
        public string value { get; set; }
    }
    public class DeliveryAddress
    {
        public string address_id { get; set; }
        public string order_id { get; set; }
        public string type { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string company { get; set; }
        public string pesel { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string country_code { get; set; }
        public string tax_identification_number { get; set; }
    }

    public class BillingAddress
    {
        public string address_id { get; set; }
        public string order_id { get; set; }
        public string type { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string company { get; set; }
        public string pesel { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string country_code { get; set; }
        public string tax_identification_number { get; set; }
    }

    public class ShippingAdditionalFields
    {
        public string machine { get; set; }
    }

    public class List
    {
        public string order_id { get; set; }
        public string product_id { get; set; }
        public string stock_id { get; set; }
        public string price { get; set; }
        public string discount_perc { get; set; }
        public string quantity { get; set; }
        public string delivery_time { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string pkwiu { get; set; }
        public string tax { get; set; }
        public string tax_value { get; set; }
        public string unit { get; set; }
        public string option { get; set; }
        public string unit_fp { get; set; }
        public string weight { get; set; }
        public string type { get; set; }
        public object loyalty { get; set; }
        public string delivery_time_hours { get; set; }
        public List<object> text_options { get; set; }
        public List<object> file_options { get; set; }
        public string user_id { get; set; }
        public string date { get; set; }
        public string status_date { get; set; }
        public string confirm_date { get; set; }
        public string delivery_date { get; set; }
        public string status_id { get; set; }
        public string sum { get; set; }
        public string payment_id { get; set; }
        public string user_order { get; set; }
        public string shipping_id { get; set; }
        public string shipping_cost { get; set; }
        public string email { get; set; }
        public string delivery_code { get; set; }
        public string confirm { get; set; }
        public string notes { get; set; }
        public string notes_priv { get; set; }
        public string notes_pub { get; set; }
        public string currency_id { get; set; }
        public string currency_rate { get; set; }
        public string paid { get; set; }
        public string ip_address { get; set; }
        public string discount_client { get; set; }
        public string discount_group { get; set; }
        public string discount_levels { get; set; }
        public string discount_code { get; set; }
        public object code_id { get; set; }
        public string lang_id { get; set; }
        public string origin { get; set; }
        public object parent_order_id { get; set; }
        public string registered { get; set; }
        public string promo_code { get; set; }
        public List<AdditionalField> additional_fields { get; set; }
        public List<object> tags { get; set; }
        public bool is_paid { get; set; }
        public bool is_overpayment { get; set; }
        public bool is_underpayment { get; set; }
        public int total_parcels { get; set; }
        public int total_products { get; set; }
        public List<object> children { get; set; }
        public int loyalty_cost { get; set; }
        public int loyalty_score { get; set; }
        public bool vat_eu { get; set; }
        public string shipping_tax_name { get; set; }
        public string shipping_tax_value { get; set; }
        public string shipping_tax_id { get; set; }
        public DeliveryAddress delivery_address { get; set; }
        public BillingAddress billing_address { get; set; }
        public string pickup_point { get; set; }
        public ShippingAdditionalFields shipping_additional_fields { get; set; }
    }

    public class Root
    {
        public string count { get; set; }
        public int pages { get; set; }
        public int page { get; set; }
        public List<List> list { get; set; }
    }
}
