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
            // default value is "suffix_publicdomain.txt"
            TLDExtractor.SuffixListFilePath = "change_the_default_file_name.txt";

            // Never expire local suffixpubliclist file
            TLDExtractor.RenewAfterNDays = -1; // default value is 30

            // Redownload suffixpubliclist if the local version was created more than 30 days ago
            // TLDExtractor.RenewAfterNDays = 30; 

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
