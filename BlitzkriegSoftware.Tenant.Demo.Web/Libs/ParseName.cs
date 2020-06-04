namespace BlitzkriegSoftware.Tenant.Demo.Web.Libs
{
    public static class ParseName
    {
        public static string RemoveParts(string username, ParseParts parts)
        {
            var s = username;

            if (!string.IsNullOrWhiteSpace(s))
            {
                if (parts.HasFlag(ParseParts.PoundPart))
                {
                    int index = s.IndexOf('#');
                    if(index >=0 )
                    {
                        s = s.Substring(index + 1);
                    }
                }

                if (parts.HasFlag(ParseParts.AtPart))
                {
                    int index = s.LastIndexOf('@');
                    if(index >= 0)
                    {
                        s = s.Substring(0, index);
                    }
                }
            }

            return s;
        }
    }
}
