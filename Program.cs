using System;
using System.Collections.Generic;

namespace ConsoleApp10
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = "1 + 2 * 3 + 4 * ( 5 + 1 )".Split(' ');
            var rpn = new RPN();
            foreach (var str in data)
            {
                rpn.Update(str);
            }
            var result = rpn.GetResult();
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }

    public class RPN
    {
        readonly Stack<RPNInfo> Stacks = new Stack<RPNInfo>();

        RPNInfo Current;

        public RPN()
        {
            Current = new RPNInfo();
        }

        public void Update(string msg)
        {
            var sym = (Symbol)msg[0];
            switch(sym)
            {
                case Symbol.KakkoStart:
                    Stacks.Push(Current);
                    Current = new RPNInfo();
                    break;

                case Symbol.KakkoEnd:
                    var result = Current.GetResult();
                    Current = Stacks.Pop();
                    Current.Update(result.ToString());
                    break;

                default:
                    Current.Update(msg);
                    break;
            }
        }

        public int GetResult()
        {
            if (0 == Stacks.Count)
            {
                return Current.GetResult();
            }
            else
            {
                throw new Exception("");
            }
        }
    }

    /// <summary>
    /// RPN
    /// </summary>
    public class RPNInfo
    {
        int L;
        int R;
        Symbol Symbol1;
        Symbol Symbol2;
        int State;  //0,2(偶数)=数字,  {1,3}(基数)=記号

        public void Update(string msg)
        {
            bool isNumeric = int.TryParse(msg, out int num);
            var sym = (Symbol)msg[0];

            //数字だった
            switch (State)
            {
                case 0: //1番目数字
                    if (isNumeric)
                    {
                        L = num;
                        State++;
                    }
                    else
                    {
                        throw new Exception("");
                    }
                    break;

                case 1: //左辺記号
                    if (isNumeric)
                    {
                        throw new Exception("");
                    }
                    else
                    {
                        Symbol1 = sym;
                        State++;
                    }
                    break;

                case 2: //2番目数字
                    if (isNumeric)
                    {
                        if (Symbol1.IsHighPriority())
                        {
                            L = Symbol1.Calc(L, num);
                            State--;
                        }
                        else
                        {
                            R = num;
                            State++;
                        }
                    }
                    else
                    {
                        throw new Exception("");
                    }
                    break;

                case 3: //右辺記号
                    if (isNumeric)
                    {
                        throw new Exception("");
                    }
                    else
                    {
                        if (sym.IsLowPriority())
                        {
                            L = Symbol1.Calc(L, R);
                            Symbol1 = sym;
                            State--;
                        }
                        else
                        {
                            Symbol2 = sym;
                            State++;
                        }
                    }
                    break;

                case 4: //3番目数字(高優先度のみ)
                    if (isNumeric)
                    {
                        R = Symbol2.Calc(R, num);
                        State--;
                    }
                    else
                    {
                        throw new Exception("");
                    }
                    break;

                default:
                    throw new Exception("");
            }
        }

        /// <summary>
        /// 結果取得
        /// </summary>
        /// <returns></returns>
        public int GetResult()
        {
            if (State == 3)
            {
                L = Symbol1.Calc(L, R);
                State--;
            }
            if (2 == State)
            {
                return L;
            }
            else
            {
                throw new Exception("");
            }
        }
    }


    public enum Symbol
    {
        Plus = '+',
        Minus = '-',
        Mul = '*',
        Div = '/',
        KakkoStart = '(',
        KakkoEnd = ')',
    }

    public static class SymbolExtensions
    {
        public static int Calc(this Symbol self, int l, int r)
        {
            switch (self)
            {
                case Symbol.Plus:
                    return l + r;

                case Symbol.Minus:
                    return l - r;

                case Symbol.Mul:
                    return l * r;

                case Symbol.Div:
                    return l / r;

                default:
                    throw new Exception();
            }
        }

        public static bool IsKakkoStart(this Symbol s) => (Symbol.KakkoStart == s);
        public static bool IsKakkoEnd(this Symbol s) => (Symbol.KakkoEnd == s);
        public static bool IsLowPriority(this Symbol s) => (Symbol.Plus == s || Symbol.Minus == s);
        public static bool IsHighPriority(this Symbol s) => (Symbol.Mul == s || Symbol.Div == s);

    }

}
