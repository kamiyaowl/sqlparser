using System;
using System.Linq;
using Sprache;

namespace SqlParser {
    class Program {
        static void Main(string[] args) {
            var sample = @"select count(goge) over(), hoge(*) from `hoge.fuga.piyo` where hoge = hoge group by hoge";
            var result = SqlParser.PrettyText(sample);
            Console.WriteLine(result);
        }
    }
}
