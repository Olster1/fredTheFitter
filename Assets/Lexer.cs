using System;
namespace easy_lex
{
    public static class Lexer
    {
        public static bool LexIsNumeric(char charValue)
        {
            bool result = (charValue >= '0' && charValue <= '9');
            return result;
        }

        public static bool LexIsAlphaNumeric(char charValue)
        {
            bool result = (charValue >= 65 && charValue <= 122);
            return result;
        }

        public static int LexEatWhiteSpace(string str, int at)
        {
            while (at < str.Length && (str[at] == ' ' || str[at] == '\r' || str[at] == '\n' || str[at] == '\t'))
            {
                at++;
            }
            return at;
        }

        public static int LexEatWhiteSpaceExceptNewLine(string str, int at)
        {
            while (at < str.Length && (str[at] == ' ' || str[at] == '\t'))
            {
                at++;
            }
            return at;
        }

        public static int LexEatSpaces(string str, int at)
        {
            while (at < str.Length && (str[at] == ' '))
            {
                at++;
            }
            return at;
        }

        public static bool LexIsNewLine(char value)
        {
            return (value == '\n' || value == '\r');
        }
    }
}
