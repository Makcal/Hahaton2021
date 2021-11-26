using System.Text;

// XOR decoder
public class App
{
    private static async Task Main(string[] args)
    {
        DateTime date = DateTime.Now;
        int timeStamp = (int)date.ToFileTime();
        Random random = new Random(timeStamp);
        string? dayPassword = await GetDayPassword(date.DayOfYear-1);
        if (string.IsNullOrWhiteSpace(dayPassword))
        {
            Console.WriteLine("Internet connection failed...");
            dayPassword = null;
        }
        else
        {
            string str1 = await File.ReadAllTextAsync("passwords.enc", new CancellationToken());
            if (string.IsNullOrWhiteSpace(str1)) {
                Console.WriteLine("File with passwords not found...");
                dayPassword = null;
            }
            else
            {
                string[] strArray = str1.Split(Environment.NewLine, (StringSplitOptions)0);
                StringBuilder stringBuilder = new StringBuilder();
                int num = 0;
                foreach (string text in strArray)
                {
                    if (num >= 12)
                        num = 0;
                    int encryptionKey = dayPassword[num++];
                    string str2 = EncryptDecrypt(text, encryptionKey);
                    stringBuilder.AppendLine(str2);
                }
                await WriteEncryptedFile(stringBuilder.ToString(), timeStamp, date.DayOfYear);
                dayPassword = null;
            }
        }

        static async Task<string?> GetDayPassword(int day)
        {
            string stringAsync = await new HttpClient().GetStringAsync("https://pastebin.com/raw/syrk6zYd");
            string? str;
            if (stringAsync == null)
            {
                str = null;
            }
            else
            {
                string[] strArray = stringAsync.Split(Environment.NewLine, (StringSplitOptions)0);
                str = strArray != null ? strArray[day] : null;
            }
            return str;
        }

        static string EncryptDecrypt(string text, int encryptionKey)
        {
            StringBuilder stringBuilder = new StringBuilder(text.Length);
            string str = text;
            for (int index = 0; index < str.Length; ++index)
            {
                char ch = (char)(str[index] ^ encryptionKey);
                stringBuilder.Append(ch);
            }
            return stringBuilder.ToString();
        }

        static async Task WriteEncryptedFile(string passwords, int timestamp, int dayNumber)
        {
            using (FileStream fs = new FileStream("passwords", (FileMode)2, (FileAccess)2))
            {
                byte[] bytes3 = Encoding.UTF8.GetBytes(passwords);
                await fs.WriteAsync(bytes3, 0, bytes3.Length);
            }
        }
    }
}
