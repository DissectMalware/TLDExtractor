# TLDExtractor

## C# Library
`TLDExtractor` is similar to [tldextract library (in Python)](https://github.com/john-kurkowski/tldextract). By using TLDExtractor, you can accurately extract subdomain, domain, and domain suffix (effective TLD) of a given domain name.

It is a common mistake to split a domain name with '.' character and consider the last part as a domain suffix. For example, consider 'www.yahoo.co.uk', in this domain, 'co.uk' is the suffix not 'uk'.

`TLDExtractor` utilize [the Public Suffix List](https://www.publicsuffix.org/list/public_suffix_list.dat) to correctly identify the suffix for a give domain name. 

By default, `TLDExtractor` downloads [the Public Suffix List](https://www.publicsuffix.org/list/public_suffix_list.dat) from Internet if it doesn't exist on the working directory or it is too old (it is created more than 30 days).


