using Aniakanl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLDExtractorDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = TLDExtractor.Extract("tfb.test.www.yahoo.co.uk");
            Console.WriteLine(result);

            result = TLDExtractor.Extract("tfb.test.www.yahoo.invalid.suffix");
            Console.WriteLine(result);
        }
    }
}
