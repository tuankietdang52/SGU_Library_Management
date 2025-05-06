using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Utilities
{
    public static class CodeUtility
    {
        private static string fullCharacter = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateRandom(int length)
        {
            Random rand = new();
            string result = "";

            for (int i = 0; i < length; i++)
            {
                int index = rand.Next(0, fullCharacter.Length - 1);
                result += fullCharacter[index];
            }

            return result;
        }
    }
}
