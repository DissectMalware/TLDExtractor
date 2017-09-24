# TLDExtractor

## C# Library
`TLDExtractor` is similar to [a link](https://github.com/john-kurkowski/tldextract) library. By using TLDExtractor, you can extract subdomain, domain, effective domain, and domain suffix of a given domain name.

It is a common mistake to split a domain name with '.' character and consider the last part as a domain suffix. For example, consider 'www.yahoo.co.uk', in this domain, 'co.uk' is the suffix not 'uk'.

`TLDExtractor` relies on [the Public Suffix List](https://www.publicsuffix.org/list/public_suffix_list.dat) to correctly parse a domain name.
