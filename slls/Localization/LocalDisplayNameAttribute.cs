using System.ComponentModel;
using Westwind.Globalization;

namespace slls.Localization
{
    public class LocalDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalDisplayNameAttribute(string ResId, string ResSet) : base(Lookup(ResId, ResSet))
        {
        }

        private static string Lookup(string ResId, string ResSet)
        {
            try
            {
                // get from Westwind.Globalization dbResourceManager (db lookup from Localization SQL table)
                return DbRes.T(ResId, ResSet);
            }
            catch
            {
                return ResId; // fallback
            }
        }
    }
}