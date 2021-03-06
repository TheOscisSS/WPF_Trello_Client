﻿using System.IO;
using System.Threading.Tasks;

namespace WPF_Trello.Utils
{
    public class AccessToken
    {
        public static string Token { get; private set; }
        public const string FILENAME = "AccessToken.txt";
        public static async void Save(string token)
        {
            using (StreamWriter writer = new StreamWriter(FILENAME))
            {
                await writer.WriteAsync(token);  
            }

            Token = token;
        }

        public static bool isExist()
        {
            return File.Exists(AccessToken.FILENAME);
        }

        public static async Task<string> Load()
        {
            using (StreamReader reader = new StreamReader(FILENAME))
            {
                Token = await reader.ReadToEndAsync();
            }

            return Token;
        }

        public static void Remove()
        {
            if (File.Exists(FILENAME))
            { 
                File.Delete(FILENAME);
            }
        }
    }
}
