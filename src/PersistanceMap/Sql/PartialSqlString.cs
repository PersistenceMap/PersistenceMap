
namespace PersistanceMap.Sql
{
    public class PartialSqlString : ISqlString
    {
        public PartialSqlString(string text)
        {
            Text = text;
        }

        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
