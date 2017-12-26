using System.Linq;
using System.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
//source: http://www.tugberkugurlu.com/archive/creating-custom-csvmediatypeformatter-in-asp-net-web-api-for-comma-separated-values-csv-format

namespace DDAS.API.Helpers.Formatters
{
    public class csvOutputFormatter:BufferedMediaTypeFormatter
    {
        public csvOutputFormatter()
        {
            // Add the supported media type.
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return isTypeOfIEnumerable(type);
        }

        

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            //NOTE: We have check the type inside CanWriteType method
            //If request comes this far, the type is IEnumerable. We are safe.

            Type itemType = type.GetGenericArguments()[0];

            StringWriter _stringWriter = new StringWriter();

            _stringWriter.WriteLine(
                string.Join<string>(
                    ",", itemType.GetProperties().Select(x => x.Name)
                )
            );

            foreach (var obj in (IEnumerable<object>)value)
            {

                var vals = obj.GetType().GetProperties().Select(
                    pi => new
                    {
                        Value = pi.GetValue(obj, null)
                    }
                );

                string _valueLine = string.Empty;

                foreach (var val in vals)
                {

                    if (val.Value != null)
                    {

                        var _val = val.Value.ToString();

                        //Check if the value contans a comma and place it in quotes if so
                        if (_val.Contains(","))
                            _val = string.Concat("\"", _val, "\"");

                        //Replace any \r or \n special characters from a new line with a space
                        if (_val.Contains("\r"))
                            _val = _val.Replace("\r", " ");
                        if (_val.Contains("\n"))
                            _val = _val.Replace("\n", " ");

                        _valueLine = string.Concat(_valueLine, _val, ",");

                    }
                    else
                    {

                        _valueLine = string.Concat(string.Empty, ",");
                    }
                }

                _stringWriter.WriteLine(_valueLine.TrimEnd(','));
            }
        }


        private bool isTypeOfIEnumerable(Type type)
        {

            foreach (Type interfaceType in type.GetInterfaces())
            {

                if (interfaceType == typeof(IEnumerable))
                    return true;
            }

            return false;
        }
    }
}