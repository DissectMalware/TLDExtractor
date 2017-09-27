using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aniakanl
{
    public static class TLDExtractor
    {
        public static string SuffixListFilePath { get; set; }
        public static int RenewAfterNDays { get; set; }
        public static Uri SuffixPublicListUrl { get; set; }

        private static Dictionary<string, DomainSuffixType> suffixes;

        static TLDExtractor()
        {
            SuffixListFilePath = "suffix_publicdomain.txt";
            RenewAfterNDays = 30;
            SuffixPublicListUrl = new Uri("https://www.publicsuffix.org/list/public_suffix_list.dat");
        }

        public static Dictionary<string, DomainSuffixType> Suffixes
        {
            get
            {
                if (suffixes == null)
                {
                    if (File.Exists(SuffixListFilePath) == false ||
                        ( RenewAfterNDays != -1 &&
                          File.GetCreationTimeUtc(SuffixListFilePath) < DateTime.UtcNow.AddDays(-1 * RenewAfterNDays)))
                    {
                        DownloadWebFile(SuffixPublicListUrl, SuffixListFilePath);
                    }

                    using (StreamReader sr = new StreamReader(SuffixListFilePath))
                    {
                        suffixes = new Dictionary<string, DomainSuffixType>();
                        string line;
                        bool isICANNSectionStarted = false;
                        bool isPrivateSectionStarted = false;
                        while (sr.EndOfStream == false)
                        {
                            line = sr.ReadLine().Trim();

                            if (isICANNSectionStarted && line.StartsWith("//") == false && string.IsNullOrEmpty(line) == false)
                            {
                                if (suffixes.ContainsKey(line) == false)
                                {
                                    suffixes.Add(line, DomainSuffixType.ICANN);
                                }

                            }
                            else if (isPrivateSectionStarted && line.StartsWith("//") == false && string.IsNullOrEmpty(line) == false)
                            {
                                if (suffixes.ContainsKey(line) == false)
                                {
                                    suffixes.Add(line, DomainSuffixType.Private);
                                }
                            }
                            else if (isICANNSectionStarted == false && line.Contains("===BEGIN ICANN DOMAINS===") == true)
                            {
                                isICANNSectionStarted = true;
                                isPrivateSectionStarted = false;
                            }
                            else if (isPrivateSectionStarted == false && line.Contains("===BEGIN PRIVATE DOMAINS===") == true)
                            {
                                isPrivateSectionStarted = true;
                                isICANNSectionStarted = false;
                            }



                        }
                    }
                }
                return suffixes;
            }
            set
            {
                suffixes = value;
            }
        }

        public static ExtractResult Extract(Uri url)
        {
            return Extract(url.Host);
        }

        public static bool TryExtract(Uri url, out ExtractResult result )
        {
            return TryExtract(url.Host, out result);
        }

        /// <summary>
        /// Extract parts from a given domain spesified as a string
        /// </summary>
        /// <param name="hostName">A domain name specified as a string</param>
        /// <returns>Returns ExractResult object that contains various parts of the given domain</returns>
        /// <exception cref="TLDExtractorException">
        /// TLDExtractorException will raise if the domain name is not valid</exception>
        public static ExtractResult Extract(string hostName)
        {
            ExtractResult result = new ExtractResult();


            hostName = hostName.ToLowerInvariant().Trim();

            string[] sections = hostName.Split('.');

            if(hostName.Length > 255)
            {
                throw new TLDExtractorException(
                    "Domain name length cannot be longer than 255 characters");
            }

            string newSuffix = "";


            string state = "suffix";
            for (int i = sections.Length - 1; i >= 0; i--)
            {
                // TODO make exception messages more clear
                if(string.IsNullOrEmpty(sections[i]) == true)
                {
                    throw new TLDExtractorException("Domain label cannot be null or empty");
                }
                else if(sections.Length>63)
                {
                    throw new TLDExtractorException(
                        "Domain label length cannot be more than 63 characters (ref: rfc1035)");
                }

                switch(state)
                {
                    case "suffix":

                        newSuffix = (sections[i] + "." + result.Suffix).Trim('.');

                        if (Suffixes.ContainsKey(newSuffix) == true)
                        {
                            result.Suffix = newSuffix;
                            result.SuffixType = Suffixes[newSuffix];
                        }
                        else
                        {
                            if(string.IsNullOrWhiteSpace(result.Suffix) ==true)
                            {
                                throw new TLDExtractorException("Domain suffix cannot be empty");
                            }
                            
                            result.Domain = sections[i];
                            state = "subdomain";
                        }
                        break;
                    case "subdomain":
                        result.SubDomain = sections[i] + "." + result.SubDomain;
                        break;
                    default:
                        throw new NotImplementedException();

                }
                                           
            }

            result.SubDomain = result.SubDomain.TrimEnd('.');

            return result;
        }

        /// <summary>
        /// Try to extract parts from a given domain spesified as a string
        /// </summary>
        /// <param name="hostName">A domain name specified as a string</param>
        /// <param name="result">If function executed successfully, result points to 
        /// an ExractResult object that contains various parts of the given domain</param>
        /// <returns>Returns true if the extraction was successful</returns>
        public static bool TryExtract(string hostName, out ExtractResult result)
        {
            bool isExtracted = true;
            result = new ExtractResult();


            hostName = hostName.ToLowerInvariant().Trim();

            string[] sections = hostName.Split('.');

            if (hostName.Length > 255)
            {
                isExtracted = false;
            }

            string newSuffix = "";


            string state = "suffix";
            for (int i = sections.Length - 1; i >= 0; i--)
            {
                
                if (string.IsNullOrEmpty(sections[i]) == true || sections.Length > 63)
                {
                    isExtracted = false;
                    break;
                }

                switch (state)
                {
                    case "suffix":

                        newSuffix = (sections[i] + "." + result.Suffix).Trim('.');

                        if (Suffixes.ContainsKey(newSuffix) == true)
                        {
                            result.Suffix = newSuffix;
                            result.SuffixType = Suffixes[newSuffix];
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(result.Suffix) == true)
                            {
                                isExtracted = false;
                                break;
                            }

                            result.Domain = sections[i];
                            state = "subdomain";
                        }
                        break;
                    case "subdomain":
                        result.SubDomain = sections[i] + "." + result.SubDomain;
                        break;
                    default:
                        throw new NotImplementedException();

                }

            }

            result.SubDomain = result.SubDomain.TrimEnd('.');

            if (isExtracted == false)
                result = null;

            return isExtracted;
        }

        /// <summary>
        /// Dowload a web resource specified by a Uri and save it in a file specified by a string
        /// </summary>
        /// <param name="location">The URI specified as a Uri from which to download data</param>
        /// <param name="outputFile">The location of a file specified by a string to save the data
        /// </param>
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
