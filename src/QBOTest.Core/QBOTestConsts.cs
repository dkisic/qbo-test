using QBOTest.Debugging;

namespace QBOTest
{
    public class QBOTestConsts
    {
        public const string LocalizationSourceName = "QBOTest";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "a375dc5f0acd4c338be7a5b0e80b43d3";
    }
}
