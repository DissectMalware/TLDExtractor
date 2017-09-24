using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aniakanl
{
    public class TLDExtractor
    {
        static string SuffixListFilePath = "suffix_publicdomain.txt";
        static int RenewAfterNDays = 30;
        static Uri SuffixPublicListUrl = new Uri("https://www.publicsuffix.org/list/public_suffix_list.dat");
        static TLDExtractor()
        {
            if (File.Exists(SuffixListFilePath) == false || File.GetCreationTimeUtc(SuffixListFilePath) < DateTime.UtcNow.AddDays(-1 * RenewAfterNDays))
            {
                DownloadWebFile(SuffixPublicListUrl, SuffixListFilePath);
            }
        }

        static HashSet<string> icannSuffixes = null;

        public static HashSet<string> ICANNSuffixes
        {
            get
            {
                if (icannSuffixes == null)
                {
                    using (StreamReader sr = new StreamReader(SuffixListFilePath))
                    {
                        icannSuffixes = new HashSet<string>();
                        string line;
                        while (sr.EndOfStream == false)
                        {
                            line = sr.ReadLine().Trim();

                            if (line.StartsWith("//") == false && string.IsNullOrEmpty(line) == false)
                            {
                                icannSuffixes.Add(line);
                            }
                            else if (line.Contains("===END ICANN DOMAINS===") == true)
                            {
                                break;
                            }



                        }
                    }
                }
                return icannSuffixes;
            }
            set
            {
                icannSuffixes = value;
            }
        }

        public static ExtractResult Extract(Uri url)
        {
            return Extract(url.Host);
        }

        public static ExtractResult Extract(string hostName)
        {
            ExtractResult result = new ExtractResult();


            hostName = hostName.ToLowerInvariant();

            string[] sections = hostName.Split('.');

            string newSuffix = "";


            string state = "suffix";
            for (int i = sections.Length - 1; i >= 0; i--)
            {
                switch(state)
                {
                    case "suffix":

                        newSuffix = (sections[i] + "." + result.Suffix).Trim('.');

                        if (ICANNSuffixes.Contains(newSuffix) == true)
                        {
                            result.Suffix = newSuffix;     
                        }
                        else
                        {
                            result.Domain = sections[i];
                            state = "subdomain";
                        }
                        break;
                    case "subdomain":
                        result.SubDomain = sections[i] + "." + result.SubDomain;
                        break;

                }
                                           
            }

            result.SubDomain = result.SubDomain.TrimEnd('.');

            return result;
        }

        /// <summary>
        /// Dowload a web resource specified by a Uri and save it in a file specified by a string
        /// </summary>
        /// <param name="location">The URI specified as a Uri from which to download data</param>
        /// <param name="outputFile">The location of a file specified by a string to save the data</param>
        public static void DownloadWebFile(Uri location, string outputFile)
        {
            Stream result = null;

            using (WebClient client = new WebClient())
            {
                result = client.OpenRead(location);

                using (FileStream fs = new FileStream(outputFile, FileMode.Create))
                {
                    result.CopyTo(fs);
                }
            }

        }


    }
}
