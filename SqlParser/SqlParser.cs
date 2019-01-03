using System;
using System.Collections.Generic;
using System.Text;
using Sprache;
using System.Linq;

namespace SqlParser {
    class SqlParser {
        #region あとでprivateにしていい
        public readonly static string[] RESERVED_COMMANDS = new string[] {
            "select",
            "from",
            "where",
            "group by",
            "order by",
            "limit",
            "union",
        };
        /// <summary>
        /// 予約語を抽出
        /// </summary>
        public static readonly Parser<string> reservedCommands =
            Parse.Regex("(" +
                string.Join("|", (
                    SqlParser.RESERVED_COMMANDS
                             .Concat(SqlParser.RESERVED_COMMANDS.Select(x => x.ToUpper()))
                    )
                )
            + ")");
        /// <summary>
        /// 分割句
        /// </summary>
        public static readonly Parser<string> delimitor =
            Parse.Char(',').Select(x => x.ToString());

        /// <summary>
        /// 任意のテキストとか
        /// </summary>
        public static readonly Parser<string> text =
            Parse.AnyChar.Except(delimitor.Or(reservedCommands)).Many().Text().Token();

        public static readonly Parser<string> token =
            text.Contained(Parse.Char('('), Parse.Char(')')).Or(
                    text.Except(reservedCommands)
                );
        /// <summary>
        /// 予約後 + クエリ本体の組み合わせ
        /// ex) select hoge,fuga
        /// reservedCommand -> select
        /// tokens -> hoge, fuga
        /// </summary>
        public static readonly Parser<(string, string[])> command =
            from reservedCommand in reservedCommands.Token()
            from tokens in token.DelimitedBy(Parse.Char(',')).Select(x => x.ToArray())
            select (reservedCommand, tokens);
        #endregion


        public static readonly Parser<(string, string[])[]> Parser =
            command.Many().Select(x => x.ToArray());

        public static string PrettyText(string sql) {
            var result = Parser.Parse(sql);
            var sb = new StringBuilder();
            foreach (var r in result) {
                sb.AppendLine(r.Item1.ToUpper());
                foreach (var t in r.Item2.Select((x, i) => new { Index = i, Data = x })) {
                    if (r.Item2.Length - 1 == t.Index) {
                        sb.AppendLine($"\t{t.Data}");
                    } else {
                        sb.AppendLine($"\t{t.Data},");
                    }
                }
            }
            return sb.ToString();
        }
    }
}
