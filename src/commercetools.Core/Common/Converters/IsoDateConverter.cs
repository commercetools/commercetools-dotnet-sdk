using Newtonsoft.Json.Converters;

namespace commercetools.Core.Common.Converters
{
    public class IsoDateConverter : IsoDateTimeConverter
    {
        public IsoDateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
