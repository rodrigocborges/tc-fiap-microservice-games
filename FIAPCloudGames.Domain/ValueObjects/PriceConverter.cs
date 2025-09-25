using FIAPCloudGames.Domain.ValueObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public class PriceConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(Price));
    }

    // Lógica para LER o JSON e transformar em objeto Price
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        // Carrega o objeto JSON aninhado (ex: {"value": 79.90})
        JObject jo = JObject.Load(reader);
        // Extrai o valor decimal da propriedade "value"
        decimal value = jo["value"].Value<decimal>();
        // Cria uma nova instância do seu objeto de valor Price
        return new Price(value);
    }

    // Lógica para ESCREVER o objeto Price como JSON
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // Converte o objeto Price para um objeto JSON aninhado, como o Elasticsearch espera
        serializer.Serialize(writer, value);
    }
}