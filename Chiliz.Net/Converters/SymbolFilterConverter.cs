using Chiliz.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace Chiliz.Net.Converters
{
    internal class SymbolFilterConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var type = new SymbolFilterTypeConverter(false).ReadString(obj["filterType"].ToString());
            ChilizSymbolFilter result;
            switch (type)
            {
                case SymbolFilterType.LotSize:
                    result = new ChilizSymbolLotSizeFilter
                    {
                        MaxQuantity = (decimal)obj["maxQty"],
                        MinQuantity = (decimal)obj["minQty"],
                        StepSize = (decimal)obj["stepSize"]
                    };
                    break;
                case SymbolFilterType.MinNotional:
                    result = new ChilizSymbolMinNotionalFilter
                    {
                        MinNotional = (decimal)obj["minNotional"],
                    };
                    break;
                case SymbolFilterType.Price:
                    result = new ChilizSymbolPriceFilter
                    {
                        MaxPrice = (decimal)obj["maxPrice"],
                        MinPrice = (decimal)obj["minPrice"],
                        TickSize = (decimal)obj["tickSize"]
                    };
                    break;
                case SymbolFilterType.MaxNumberAlgorithmicOrders:
                    result = new ChilizSymbolMaxAlgorithmicOrdersFilter
                    {
                        MaxNumberAlgorithmicOrders = (int)obj["maxNumAlgoOrders"]
                    };
                    break;
                case SymbolFilterType.MaxNumberOrders:
                    result = new ChilizSymbolMaxOrdersFilter
                    {
                        MaxNumberOrders = (int)obj["limit"]
                    };
                    break;

                case SymbolFilterType.IcebergParts:
                    result = new ChilizSymbolIcebergPartsFilter
                    {
                        Limit = (int)obj["limit"]
                    };
                    break;
                default:
                    Debug.WriteLine("Can't parse symbol filter of type: " + obj["filterType"]);
                    result = new ChilizSymbolFilter();
                    break;
            }
            result.FilterType = type;
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var filter = (ChilizSymbolFilter)value;
            writer.WriteStartObject();

            writer.WritePropertyName("filterType");
            writer.WriteValue(JsonConvert.SerializeObject(filter.FilterType, new SymbolFilterTypeConverter(false)));

            switch (filter.FilterType)
            {
                case SymbolFilterType.LotSize:
                    var lotSizeFilter = (ChilizSymbolLotSizeFilter)filter;
                    writer.WritePropertyName("maxQty");
                    writer.WriteValue(lotSizeFilter.MaxQuantity);
                    writer.WritePropertyName("minQty");
                    writer.WriteValue(lotSizeFilter.MinQuantity);
                    writer.WritePropertyName("stepSize");
                    writer.WriteValue(lotSizeFilter.StepSize);
                    break;
                case SymbolFilterType.MinNotional:
                    var minNotionalFilter = (ChilizSymbolMinNotionalFilter)filter;
                    writer.WritePropertyName("minNotional");
                    writer.WriteValue(minNotionalFilter.MinNotional);
                    break;
                case SymbolFilterType.Price:
                    var priceFilter = (ChilizSymbolPriceFilter)filter;
                    writer.WritePropertyName("maxPrice");
                    writer.WriteValue(priceFilter.MaxPrice);
                    writer.WritePropertyName("minPrice");
                    writer.WriteValue(priceFilter.MinPrice);
                    writer.WritePropertyName("tickSize");
                    writer.WriteValue(priceFilter.TickSize);
                    break;
                case SymbolFilterType.MaxNumberAlgorithmicOrders:
                    var algoFilter = (ChilizSymbolMaxAlgorithmicOrdersFilter)filter;
                    writer.WritePropertyName("maxNumAlgoOrders");
                    writer.WriteValue(algoFilter.MaxNumberAlgorithmicOrders);
                    break;
                case SymbolFilterType.MaxNumberOrders:
                    var orderFilter = (ChilizSymbolMaxOrdersFilter)filter;
                    writer.WritePropertyName("limit");
                    writer.WriteValue(orderFilter.MaxNumberOrders);
                    break;
                case SymbolFilterType.IcebergParts:
                    var icebergPartsFilter = (ChilizSymbolIcebergPartsFilter)filter;
                    writer.WritePropertyName("limit");
                    writer.WriteValue(icebergPartsFilter.Limit);
                    break;
                default:
                    Debug.WriteLine("Can't write symbol filter of type: " + filter.FilterType);
                    break;
            }

            writer.WriteEndObject();
        }
    }
}
