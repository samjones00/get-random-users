using Newtonsoft.Json.Linq;

/// <summary>
/// We have provided you the code below for a proof of concept (PoC) console application that satisfies the following requirements:
/// - Reads random users from an API endpoint 5 times. 
/// - All responses from the API should be written to a file.
/// - All successful responses should be represented as a list of users with the following properties
///     - last name
///     - first name
///     - age
///     - city
///     - email
///   and be written to file as valid JSON.
/// 
/// There are now new requirements for this application, and we should like for you to update this console 
/// application to incorporate the following new requirements while satisfing the original requirements:
/// - Update this code so it follows .Net best practices and principles. The code should be extensible, reusable, and easy to test using Unit Tests.
/// - Add Unit tests.
/// - Make the the output file names configurable.
/// - Make the number of API calls configurable instead of 5.
/// - Add logging
/// </summary>

namespace MyHomework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Create a file

            HttpClient c = new HttpClient();
            HttpResponseMessage r = c.GetAsync("https://randomuser.me/api/").Result;
            string rb = r.Content.ReadAsStringAsync().Result;

            File.AppendAllText("c:\\temp\\MyTest.txt", rb);
            File.AppendAllText("c:\\temp\\MyTest.txt", Environment.NewLine);

            HttpClient c1 = new HttpClient();
            HttpResponseMessage r1 = c1.GetAsync("https://randomuser.me/api/").Result;
            string rb1 = r1.Content.ReadAsStringAsync().Result;

            File.AppendAllText("c:\\temp\\MyTest.txt", rb1);
            File.AppendAllText("c:\\temp\\MyTest.txt", Environment.NewLine);

            HttpClient c2 = new HttpClient();
            HttpResponseMessage r2 = c2.GetAsync("https://randomuser.me/api/").Result;
            string rb2 = r2.Content.ReadAsStringAsync().Result;

            File.AppendAllText("c:\\temp\\MyTest.txt", rb2);
            File.AppendAllText("c:\\temp\\MyTest.txt", Environment.NewLine);

            HttpClient c3 = new HttpClient();
            HttpResponseMessage r3 = c3.GetAsync("https://randomuser.me/api/").Result;
            string rb3 = r3.Content.ReadAsStringAsync().Result;

            File.AppendAllText("c:\\temp\\MyTest.txt", rb3);
            File.AppendAllText("c:\\temp\\MyTest.txt", Environment.NewLine);

            HttpClient c4 = new HttpClient();
            HttpResponseMessage r4 = c4.GetAsync("https://randomuser.me/api/").Result;
            string rb4 = r4.Content.ReadAsStringAsync().Result;

            File.AppendAllText("c:\\temp\\MyTest.txt", rb4);
            File.AppendAllText("c:\\temp\\MyTest.txt", Environment.NewLine);

            string[] res = new string[7];
            res[0] = "[";
            using (StreamReader reader = new StreamReader("c:\\temp\\MyTest.txt"))
            {
                for (int i = 1; i < 6; i++)
                {
                    string ln = reader.ReadLine();
                    JObject o = JObject.Parse(ln);
                    string s1 = o["results"][0]["name"]["last"].ToString();
                    string s2 = o["results"][0]["name"]["first"].ToString();
                    string s3 = o["results"][0]["location"]["city"].ToString();
                    string s4 = o["results"][0]["email"].ToString();
                    string s5 = o["results"][0]["dob"]["age"].ToString();

                    res[i] = "{\"last\":" + "\"" + s1 + "\"" + ",\"first\":" + "\"" + s2 + "\"" + ",\"city\":" + "\"" + s3 + "\"" + ",\"email\":" + "\"" + s4 + "\"" + ",\"age\":" + "\"" + s5 + "\"" + "}";

                    if (i < 5)
                        res[i] = res[i] + ",";
                }
            }

            res[6] = "]";
            File.WriteAllLines("c:\\temp\\MyTest.json", res);
        }
    }
}