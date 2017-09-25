using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniakanl
{
    public class ExtractResult
    {
        public string SubDomain { get; set; }
        public string Domain { get; set; }
        public string Suffix { get; set; }
        public DomainSuffixType SuffixType { get; set; }
        public string EffectiveDomain
        {
            get
            {
                return Domain + "." + Suffix;
            }
            private set
            {

            }
        }

        public override string ToString()
        {
            return string.Format(
                "ExtractResult(subdomain='{0}', domain='{1}', suffix='{2}', suffix type='{3}')", 
                SubDomain, Domain, Suffix, SuffixType);
        }
    }

    public enum DomainSuffixType
    {
        ICANN,
        Private
    }
}
