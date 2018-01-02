using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
//Source: https://codereview.stackexchange.com/questions/8228/converting-a-list-to-a-csv-string
namespace DDAS.API.Helpers
{
    public class CSVConvertor
    {

        //public string ConvertToCSVString(object value)
        public string ConvertToCSVString(object value)
        {
            var headers = new List<string>();
            return ConvertToCSVString(value, headers);
        }
        public string ConvertToCSVString(object value, List<string> headers)
        {
            //NOTE: We have check the type inside CanWriteType method
            //If request comes this far, the type is IEnumerable. We are safe.

            //Type itemType = type.GetGenericArguments()[0];

            StringWriter _stringWriter = new StringWriter();

            //_stringWriter.WriteLine(
            //    string.Join<string>(
            //        ",", itemType.GetProperties().Select(x => x.Name)
            //    )
            //);





            //foreach (var obj in (IEnumerable<object>)value)
            //{
            //    var name = nameof(obj);
            //}


            //List<string> properties = value.Select(o => o.StringProperty).ToList();


            _stringWriter.WriteLine(
                string.Join<string>(
                    ",", headers
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

                        //_valueLine = string.Concat(string.Empty, ",");
                        _valueLine = string.Concat(_valueLine, ",");
                    }
                }

                _stringWriter.WriteLine(_valueLine.TrimEnd(','));
               
            }
            return _stringWriter.ToString();
        }
       
    }

    public abstract class CsvBase<T>
    {
        private static readonly char[] trimEnd = new[] { ' ', ',' };

        private readonly IEnumerable<T> values;

        private readonly Func<T, object> getItem;

        protected CsvBase(IEnumerable<T> values, Func<T, object> getItem)
        {
            this.values = values;
            this.getItem = getItem;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var item in
                from element in this.values.Select(this.getItem)
                where element != null
                select element.ToString())
            {
                this.Build(builder, item).Append(", ");
            }

            return builder.ToString().TrimEnd(trimEnd);
        }

        protected abstract StringBuilder Build(StringBuilder builder, string item);
    }

    public class CsvBare<T> : CsvBase<T>
    {
        public CsvBare(IEnumerable<T> values, Func<T, object> getItem) : base(values, getItem)
        {
        }

        protected override StringBuilder Build(StringBuilder builder, string item)
        {
            return builder.Append(item);
        }
    }

    public sealed class CsvTrimBare<T> : CsvBare<T>
    {
        public CsvTrimBare(IEnumerable<T> values, Func<T, object> getItem) : base(values, getItem)
        {
        }

        protected override StringBuilder Build(StringBuilder builder, string item)
        {
            return base.Build(builder, item.Trim());
        }
    }

    public class CsvRfc4180<T> : CsvBase<T>
    {
        private static readonly char[] csvChars = new[] { ',', '"', ' ', '\n', '\r' };

        public CsvRfc4180(IEnumerable<T> values, Func<T, object> getItem) : base(values, getItem)
        {
        }

        protected override StringBuilder Build(StringBuilder builder, string item)
        {
            item = item.Replace("\"", "\"\"");
            return item.IndexOfAny(csvChars) >= 0
                ? builder.Append("\"").Append(item).Append("\"")
                : builder.Append(item);
        }
    }

    public sealed class CsvTrimRfc4180<T> : CsvRfc4180<T>
    {
        public CsvTrimRfc4180(IEnumerable<T> values, Func<T, object> getItem) : base(values, getItem)
        {
        }

        protected override StringBuilder Build(StringBuilder builder, string item)
        {
            return base.Build(builder, item.Trim());
        }
    }

    public class CsvAlwaysQuote<T> : CsvBare<T>
    {
        public CsvAlwaysQuote(IEnumerable<T> values, Func<T, object> getItem) : base(values, getItem)
        {
        }

        protected override StringBuilder Build(StringBuilder builder, string item)
        {
            return builder.Append("\"").Append(item.Replace("\"", "\"\"")).Append("\"");
        }
    }

    public sealed class CsvTrimAlwaysQuote<T> : CsvAlwaysQuote<T>
    {
        public CsvTrimAlwaysQuote(IEnumerable<T> values, Func<T, object> getItem) : base(values, getItem)
        {
        }

        protected override StringBuilder Build(StringBuilder builder, string item)
        {
            return base.Build(builder, item.Trim());
        }
    }

    public static class CsvExtensions
    {
        public static string ToCsv<T>(this IEnumerable<T> source, Func<T, object> getItem, Type csvProcessorType)
        {
            if ((source == null)
                || (getItem == null)
                || (csvProcessorType == null)
                || !csvProcessorType.IsSubclassOf(typeof(CsvBase<T>)))
            {
                return string.Empty;
            }

            return csvProcessorType
                .GetConstructor(new[] { source.GetType(), getItem.GetType() })
                .Invoke(new object[] { source, getItem })
                .ToString();
        }

        private static void Main()
        {
            var words = new[] { ",this", "   is   ", "a", "test", "Super, \"luxurious\" truck" };

            Console.WriteLine(words.ToCsv(x => x, typeof(CsvAlwaysQuote<string>)));
            Console.WriteLine(words.ToCsv(x => x, typeof(CsvRfc4180<string>)));
            Console.WriteLine(words.ToCsv(x => x, typeof(CsvBare<string>)));
            Console.WriteLine(words.ToCsv(x => x, typeof(CsvTrimAlwaysQuote<string>)));
            Console.WriteLine(words.ToCsv(x => x, typeof(CsvTrimRfc4180<string>)));
            Console.WriteLine(words.ToCsv(x => x, typeof(CsvTrimBare<string>)));
            Console.ReadLine();
        }
    }
}