// See https://aka.ms/new-console-template for more information
using Lexer;

using (StreamReader r = new("in.txt"))
using (StreamWriter w = new("out.txt"))
{
    LexerController controller = new(r, w);
    controller.Start();
    controller.PrintTokens();
}
