using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    using Rule = Dictionary<AutomatonNode, Dictionary<CharType, AutomatonNode>>;
    public enum CharType
    {
        // STRING
        LETTER                      = 0,
        SINGLE_ROW_COMMENTARY       = 1,
        MULTI_ROW_COMMENTARY        = 2,
        STRING_START                = 3,
        // NEW ROW
        NEW_ROW_CH                  = 4,
        // NUMBERS
        DIGIT                       = 5,
        DOUBLE_NUM_DELIMETER        = 6,
        // OPERATORS
        OP_MORE                     = 7, 
        OP_LESS                     = 8,
        OP_ASSIGN_DECLARATE         = 9,
        OP_EQUAL                    = 10,
        OP_PLUS                     = 11,
        OP_MINUS                    = 12,
        OP_MULTIPLY                 = 13,
        OP_DIV                      = 14,
        OP_MOD                      = 15,
        UNIVERSAL                   = 16,



        SPECIAL
    }

    public static class FindRule
    {
        public const string LETTER_UPPER_SET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string LETTER_LOWER_SET = "abcdefghijklmnopqrstuvwxyz";
        public const string LETTER_SET = $"{LETTER_LOWER_SET}{LETTER_UPPER_SET}";
        public const string DIGIT_SET = "0123456789";
        public const string SINGLE_COMMENTARY_SET = "/";
        public const string BLOCK_COMMENTARY_SET = "{";
        public const string OPERATOR_SET = "<>:=+-*/%";
        public const string STRING_WRAPPER_SET = "'";
        public const string DOUBLE_NUM_DELIMETER = ".";
        public const string NEW_ROW_SET = "\r\n";

        //                      CharSet  Type
        public static Dictionary<string, CharType> SetMatching { get; } = new()
        {
            { LETTER_SET,               CharType.LETTER                 },
            { DIGIT_SET,                CharType.DIGIT                  },
            { BLOCK_COMMENTARY_SET,     CharType.MULTI_ROW_COMMENTARY   },
            { NEW_ROW_SET,              CharType.NEW_ROW_CH             },
            { DOUBLE_NUM_DELIMETER,     CharType.DOUBLE_NUM_DELIMETER   },
            { STRING_WRAPPER_SET,       CharType.STRING_START           },
            { ">",                      CharType.OP_MORE                },
            { "<",                      CharType.OP_LESS                },
            { ":",                      CharType.OP_ASSIGN_DECLARATE    },
            { "=",                      CharType.OP_EQUAL               },
            { "-",                      CharType.OP_MINUS               },
            { "+",                      CharType.OP_PLUS                },
            { "/",                      CharType.OP_DIV                 },
            { "%",                      CharType.OP_MOD                 },
            { "*",                      CharType.OP_MULTIPLY            },
        };


        #region Word description 

        public static Dictionary<string, string> KeyWord { get; } = new Dictionary<string, string>()
        {
            {"PROGRAM", "start program token" },
            {"PROCEDURE", "Procedure token"},

            {"BEGIN", "Begin token"},
            {"BOOLEAN", "variables type token" },

            {"VAR", "declaring block variables token" },

            {"STRING", "variables type token" },

            {"INTEGER", "variables type token" },
            {"IF", "condition token" },
            {"INC", "increment token" },

            {"TEXT", "variables type token" },
            {"THEN", "should token" },
            {"TRUE", "true ans token" },

            {"CHAR", "variables type token" },
            {"CLOSE",  "close file token" },

            {"END", "end block token" },
            {"ELSE", "otherwise token" },

            {"FALSE", "false ans token" },
            {"FOR", "for token" },

            {"RESET", "reset file token" },
            {"REWRITE", "rewrite file token" },
            {"READ", "read token" },
            {"READLN", "readln token" },

            {"ASSIGN", "assign file token" },

            {"DO", "do token" },

            {"WHILE", "while token" },
            {"WRITE", "write token" },
            {"WRITELN", "writeln token" },

        };
        public static Dictionary<string, string> SpecialCh { get; } = new Dictionary<string, string>()
        {
            {";", "end action token" },
            {"(", "open bracket token" },
            {")", "close bracket token" },
            {",", "split token" },
            {".", "end program token" }
        };
        public static Dictionary<string, string> Operator { get; } = new Dictionary<string, string>()
        {
            {"DIV", "divide token" },
            {"MOD", "modulo token" },
            {"<", "less token" },
            {">", "more token" },
            {"*", "multiply token" },
            {"-", "minus token" },
            {"+", "plus token" },
            {":", "declaring variable token" },
            {"<>", "not equal token" },
            {":=", "equate token" },
            {">=", "more or equal token" },
            {"<=", "less or equal token" },
            {"=", "equality token" },
        };

        #endregion

        #region Find automaton
        public static Rule Identifier { get; } = new()
        {
            { new() { Name = "S", IsExit = false }, new() { { CharType.LETTER , new() { Name = "B", IsExit = true } } } },
            { new() { Name = "B", IsExit = true },  new()
                { 
                    { CharType.LETTER,  new() { Name = "E", IsExit = true } },
                    { CharType.DIGIT,   new() { Name = "D", IsExit = true } }
                } 
            },
            { new() { Name = "E", IsExit = true },  new() 
                { 
                    { CharType.LETTER, new() { Name = "E", IsExit = true } },
                    { CharType.DIGIT,  new() { Name = "D", IsExit = true } }
                }
            },
            { new() { Name = "D", IsExit = true },  new() { { CharType.DIGIT, new() { Name = "D", IsExit = false } } } }
        };
        public static Rule Digit { get; } = new()
        {
            { new() { Name = "S", IsExit = false }, new() { { CharType.DIGIT , new() { Name = "B", IsExit = true } } } },
            { new() { Name = "B", IsExit = true },  new()
                { 
                    { CharType.DOUBLE_NUM_DELIMETER,  new() { Name = "D", IsExit = false } },
                    { CharType.DIGIT,   new() { Name = "C", IsExit = true } }
                } 
            },
            { new() { Name = "C", IsExit = true },  new() 
                { 
                    { CharType.DOUBLE_NUM_DELIMETER, new() { Name = "D", IsExit = false } },
                    { CharType.DIGIT,  new() { Name = "C", IsExit = true } }
                }
            },
            { new() { Name = "D", IsExit = false },new() { { CharType.DIGIT, new() { Name = "G", IsExit = true } } } },
            { new() { Name = "G", IsExit = true }, new() { { CharType.DIGIT, new() { Name = "I", IsExit = true } } } },
            { new() { Name = "I", IsExit = true }, new() { { CharType.DIGIT, new() { Name = "I", IsExit = true } } } }
        };
        public static Rule StringRule { get; } = new()
        {
            { new() { Name = "S", IsExit = false }, new() { { CharType.STRING_START, new() { Name = "A", IsExit = false } } } },
            { new() { Name = "A", IsExit = false },  new()
                { 
                    { CharType.STRING_START,  new() { Name = "E", IsExit = true } },
                    { CharType.UNIVERSAL,   new() { Name = "A", IsExit = false} },
                } 
            },
            { new() { Name = "E", IsExit = true },  new() 
                { 
                    { CharType.STRING_START,  new() { Name = "A", IsExit = false } },
                }
            },
        };
        public static Rule OperatorRule { get; } = new()
        {
            { new() { Name = "S", IsExit = false }, new() 
                { 
                    { CharType.OP_MORE, new() { Name = "A", IsExit = true } },
                    { CharType.OP_LESS, new() { Name = "B", IsExit = true } },
                    { CharType.OP_PLUS, new() { Name = "H", IsExit = true } },
                    { CharType.OP_MINUS, new() { Name = "H", IsExit = true } },
                    { CharType.OP_EQUAL, new() { Name = "H", IsExit = true } },
                    { CharType.OP_MULTIPLY, new() { Name = "H", IsExit = true } },
                    { CharType.OP_DIV, new() { Name = "H", IsExit = true } },
                    { CharType.OP_MOD, new() { Name = "H", IsExit = true } },
                    { CharType.OP_ASSIGN_DECLARATE, new() { Name = "C", IsExit = true } }
                }
            },
            { new() { Name = "A", IsExit = true },  new()
                { 
                    { CharType.OP_EQUAL,  new() { Name = "H", IsExit = true } }
                } 
            },
            { new() { Name = "B", IsExit = true },  new() 
                { 
                    { CharType.OP_MORE,  new() { Name = "H", IsExit = true } },
                    { CharType.OP_EQUAL,   new() { Name = "H", IsExit = true } }
                }
            },
            { new() { Name = "C", IsExit = true },  new() { { CharType.OP_EQUAL, new() { Name = "H", IsExit = true } } } },
            { new() { Name = "H", IsExit = true },  new() {} },
        };
        public static Rule LineComment { get; } = new()
        {
            { new() { Name = "S", IsExit = false }, new() { { CharType.SINGLE_ROW_COMMENTARY , new() { Name = "B", IsExit = false } } } },
            { new() { Name = "B", IsExit = false }, new() { { CharType.SINGLE_ROW_COMMENTARY, new() { Name = "H", IsExit = true } } } },
            { new() { Name = "H", IsExit = true }, new() {} }
        };

        #endregion

        public static (bool, int) OperatorCheck(char ch, Queue<(char ch, int column)> buffer, ref int column, TextReader r)
        {
            return Check(OperatorRule, ch, buffer, ref column, r);
        }
        public static (bool, int) IdentifierCheck(char ch, Queue<(char ch, int column)> buffer, ref int column, TextReader r)
        {
            return Check(Identifier, ch, buffer, ref column, r);
        }
        public static (bool, int) DigitCheck(char ch, Queue<(char ch, int column)> buffer, ref int column, TextReader r)
        {
            return Check(Digit, ch, buffer, ref column, r);
        }
        public static (bool, int) StringCheck(char ch, Queue<(char ch, int column)> buffer, ref int column, TextReader r)
        {
            int readCount = 0;
            var type = GetCharType(ch);
            AutomatonNode current = new() { Name = "S", IsExit = false };
            while (true)
            {
                if (StringRule.ContainsKey(current) && StringRule[current].ContainsKey(type))
                    current = StringRule[current][type];
                else
                    return (current.IsExit, readCount);

                if (buffer.Count == 0) buffer.Enqueue((ch, column));
                ch = (char)r.Read(); ++column; ++readCount;
                type = (ch == '\'') ? CharType.STRING_START : CharType.UNIVERSAL;
                buffer.Enqueue((ch, column));
            }
        }
        public static (bool, int) Check(Rule rule, char ch, Queue<(char ch, int column)> buffer, ref int column, TextReader r)
        {
            int readCount = 0;
            var type = GetCharType(ch);
            AutomatonNode current = new() { Name = "S", IsExit = false };
            while (true)
            {
                if (rule.ContainsKey(current) && rule[current].ContainsKey(type))
                    current = rule[current][type];
                else
                    return (current.IsExit, readCount);
                if (buffer.Count == 0) buffer.Enqueue((ch, column));
                ch = (char)r.Read(); ++column; ++readCount;
                type = GetCharType(ch);
                buffer.Enqueue((ch, column));
            }
        }
        public static CharType GetCharType(char ch)
        {
            foreach (var set in SetMatching)
                if (set.Key.IndexOf(ch) != -1) return set.Value;

            return CharType.SPECIAL;
        }
    }
}
