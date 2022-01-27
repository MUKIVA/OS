using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Lexer
{
    public struct Token
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class LexerController
    {
        private TextReader _reader;
        private TextWriter _writer;
        private int _column = 0;
        private int _row = 1;
        private bool _skipBlock = false;
        private bool _skipRow = false;
        private Queue<(char ch, int column)> _buffer = new();
        private List<Token> _tokens = new();
        delegate (bool, int) ActionRef<T1, T2, T3, T4>(T1 item1, T2 item2, ref T3 item3, T4 item4);

        public LexerController(TextReader reader, TextWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        private char GetNextNoSpace()
        {
            char _;
            while (_reader.Peek() != -1)
            {
                _ = (char)_reader.Read();
                ++_column;
                if (_ != ' ' || _ != '\t') return _;
            }
            return '\0';
        }

        private void CreateToken(int column, int row, string name, string type)
        {
            _tokens.Add(new()
            {
                Column = column,
                Name = name,
                Row = row,
                Type = type,
            });
        }

        private (string word, int startColumn) ReadWord(ActionRef<char, Queue<(char ch, int column)>, int, TextReader> chacker, char ch)
        {
            string word = "";
            (bool IsId, int length) wordInfo = chacker(ch, _buffer, ref _column, _reader);
            int startColumn = _buffer.Peek().column;

            for (int i = 0; i < wordInfo.length; ++i)
            {
                word += _buffer.Dequeue().ch;
            }

            return (word, startColumn);
        }

        public void PrintTokens()
        {
            foreach (var token in _tokens)
            {
                _writer.WriteLine($"Name: {token.Name}\n\tType: {token.Type}\n\tRow: {token.Row}\n\tColumn: {token.Column}");
            }
        }

        public void Start()
        {
            while (_reader.Peek() != -1 || _buffer.Count != 0)
            {
                // Получаем символ и заносим в буфер
                char ch;
                if (_buffer.Count == 0)
                {
                    ch = GetNextNoSpace(); if (ch != ' ') _buffer.Enqueue((ch, _column));
                }
                else
                {
                    ch = _buffer.Dequeue().ch;
                }
                // Определяем его тип
                var charType = FindRule.GetCharType(ch);
                if (charType == CharType.NEW_ROW_CH)
                {
                    ++_row; _column = 0;
                    char _ = (char)_reader.Peek();
                    if (_ == '\n' || _ == '\r')
                    {
                        if (_buffer.Count != 0 && ch == _buffer.Peek().ch) _buffer.Dequeue();
                        _reader.Read();
                    }
                    _skipRow = false;
                }
                if (ch == '/' && (char)_reader.Peek() == '/' && !_skipBlock)
                    _skipRow = true;
                if (charType == CharType.LETTER && !_skipBlock && !_skipRow)
                {
                    (string word, int startColumn) = ReadWord(FindRule.IdentifierCheck, ch);

                    if (FindRule.KeyWord.ContainsKey(word.ToUpper()))
                        CreateToken(startColumn, _row, word, FindRule.KeyWord[word.ToUpper()]);
                    else
                        CreateToken(startColumn, _row, word, "Identifier");
                    
                }
                if (charType == CharType.DIGIT && !_skipBlock && !_skipRow)
                {
                    (string num, int startColumn) = ReadWord(FindRule.DigitCheck, ch);
                    CreateToken(startColumn, _row, num, "Number constant");
                }
                if (charType == CharType.STRING_START && !_skipBlock && !_skipRow)
                {
                    (string literal, int startColumn) = ReadWord(FindRule.StringCheck, ch);
                    CreateToken(startColumn, _row, literal, "literal");
                }
                if ((int)charType >= 7 && (int)charType <= 15 && !_skipBlock && !_skipRow)
                {
                    (string name, int startColumn) = ReadWord(FindRule.OperatorCheck, ch);
                    CreateToken(_column, _row, name, FindRule.Operator[name]);
                }
                if (charType == CharType.SPECIAL && !_skipBlock && !_skipRow) 
                {

                    if (FindRule.SpecialCh.ContainsKey(ch.ToString()))
                        CreateToken(_column, _row,
                            (_buffer.Count != 0) ? _buffer.Dequeue().ch.ToString() : ch.ToString(),
                            FindRule.SpecialCh[ch.ToString()]);
                }

                if (ch == '.') // end point
                    CreateToken(_column, _row, ".", FindRule.SpecialCh[ch.ToString()]);

                if (ch == '{')
                    _skipBlock = true;

                if (ch == '}')
                    _skipBlock = false;
            }
        }
    }
}
