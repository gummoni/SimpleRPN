using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleRPN
{
    class Program
    {
        static void Main(string[] args)
        {
            var dat = new Lex("- 1 + 2 *3 + 4 * ( 5 + 1 )");
            var rpn = new RPN();
            foreach (var str in dat)
            {
                rpn.Update(str);
            }
            var result = rpn.GetResult();
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }

    public class Lex : IEnumerable<string>
    {
        readonly string Data;
        int Index = 0;
        int Length => Data.Length;
        char Current => Data[Index];

        bool IsBusy => Index < Length;

        bool MoveNext()
        {
            Index++;
            return IsBusy;
        }

        public Lex(string data)
        {
            Data = data;
        }

        bool IsNumeric0(char ch) => (ch == '+' || ch == '-' || ('0' <= ch && ch <= '9'));
        bool IsNumeric1(char ch) => ('0' <= ch && ch <= '9');
        bool IsKigo(char ch) => (ch == '(' || ch == ')' || ch == '*' || ch == '/');
        bool IsSpace(char ch) => (ch == ' ');

        public IEnumerator<string> GetEnumerator()
        {
            while (IsBusy)
            {
                var result = "";
                var ch = Current;
                if (IsSpace(ch))
                {
                    Index++;
                    continue;
                }
                else if (IsNumeric0(ch))
                {
                    result += ch;
                    while (MoveNext())
                    {
                        ch = Current;
                        if (IsSpace(ch))
                        {
                            continue;
                        }
                        else if (IsNumeric1(ch))
                        {
                            result += ch;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else if (IsKigo(ch))
                {
                    result += ch;
                    Index++;
                }
                yield return result;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class NestInfo
    {
        public Symbol Symbol;
        public int Pos;

        public NestInfo(Symbol symbol, int pos)
        {
            Symbol = symbol;
            Pos = pos;
        }
    }

    public class RPN
    {
        readonly Stack<NestInfo> Nests = new Stack<NestInfo>();
        readonly Stack<int> values = new Stack<int>();
        Symbol Symbol;
        bool IsCalc = false;

        public void Update(string msg)
        {
            switch(msg[0])
            {
                case '(':
                    Nests.Push(new NestInfo((IsCalc) ? Symbol : Symbol.None, values.Count + 1));
                    IsCalc = false;
                    break;

                case ')':
                    var info = Nests.Pop();
                    if (Symbol.None != info.Symbol)
                    {
                        Eval(info.Pos);
                        var r = info.Symbol.Calc(values.Pop(), values.Pop());
                        values.Push(r);
                    }
                    break;

                default:
                    if (int.TryParse(msg, out int num))
                    {
                        values.Push(num);
                        if (IsCalc)
                        {
                            IsCalc = false;
                            values.Push(Symbol.Calc(values.Pop(), values.Pop()));
                        }
                    }
                    else
                    {
                        if (IsCalc)
                        {
                            throw new Exception("");
                        }
                        else
                        {
                            IsCalc = true;
                            Symbol = (Symbol)msg[0];
                        }
                    }
                    break;
            }
        }

        public int GetResult()
        {
            Eval(1);
            return values.Pop();
        }

        void Eval(int stopPos)
        {
            while (stopPos < values.Count)
            {
                if (IsCalc)
                {
                    IsCalc = false;
                    values.Push(Symbol.Calc(values.Pop(), values.Pop()));
                }
                else
                {
                    values.Push(values.Pop() + values.Pop());
                }
            }
        }
    }

    public enum Symbol
    {
        None = 0,
        Mul = '*',
        Div = '/',
    }

    public static class SymbolExtensions
    {
        public static int Calc(this Symbol self, int l, int r)
        {
            switch (self)
            {
                case Symbol.Mul:    return l * r;
                case Symbol.Div:    return l / r;
                default:            throw new Exception();
            }
        }
    }
}
