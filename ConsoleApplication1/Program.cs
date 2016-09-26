
using DDAS.Data.Mongo;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TestOne();
           test.AddTest();
           test.ReadTest();
        }
    }
}
