namespace SMarket.Core.Model
{
    public class Logs
    {
        public Logs(string mensagem)
        {
            Mensagem = mensagem;
        }
        public int Id { get; private set; }
        public string Mensagem { get; private set; }
    }
}
