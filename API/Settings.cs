namespace API
{
    public class Settings
    {
        /// <summary>
        /// String de conexão com o Postgre
        /// </summary>
        public static string PostgreSQLConnectionString => Environment.GetEnvironmentVariable("POSTGRE_CONNECTION_STRING");

        /// <summary>
        /// String de conexão com o Postgre
        /// </summary>
        public static string RedisConnectionString => Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");

        /// <summary>
        /// Path da api de Status
        /// </summary>
        public static string APIStatusPath => Environment.GetEnvironmentVariable("API_STATUS_PATH");
        /// <summary>
        /// Path da api de Pagamento
        /// </summary>
        public static string APIPagamentoPath => Environment.GetEnvironmentVariable("API_PAGAMENTO_PATH");


    }
}
