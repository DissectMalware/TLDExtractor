using Aniakanl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniakanl
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = TLDExtractor.Extract("tfb.test.www.yahoo.co.uk.ybo.review");
            Console.WriteLine(result);

            try
            {
                result = TLDExtractor.Extract("tfb.test.www.yahoo.invalid.suffix");
                Console.WriteLine(result);
            }
            catch(TLDExtractorException  tldExp)
            {
                Console.WriteLine(tldExp.Message);
            }

            try
            {
                result = TLDExtractor.Extract("tfb.test...com");
                Console.WriteLine(result);
            }
            catch (TLDExtractorException tldExp)
            {
                Console.WriteLine(tldExp.Message);
            }

        }
    }
}
