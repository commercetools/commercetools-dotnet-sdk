using System.Collections.Specialized;
using commercetools.Common;
using NUnit.Framework;

namespace commercetools.Tests
{
    public class ExtensionTest
    {
        [Test, TestCaseSource("values")]
        public void TestToQueryString(string expectedUri, NameValueCollection val)
        {
            var parameters = val.ToQueryString();
            Assert.AreEqual(expectedUri, parameters);
        }

        private static object[] values =
        {
            new object[] {
                "?expand=category&expand=taxCategory",
                new NameValueCollection()
                {
                    {"expand", "category"},
                    {"expand", "taxCategory"}
                }
            },
            new object[] {
                "?",
                new NameValueCollection()
                {
                    {"expand", " "}
                }
            },
            new object[] {
                "?",
                new NameValueCollection()
                {
                    {"expand", ""}
                }
            },
            new object[] {
                "",
                new NameValueCollection()
            }
        };
    }
}
