namespace API
{
    internal class Log
    {
        private static readonly ILoggerFactory _factory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        public static ILogger log => _factory.CreateLogger("");


        /// <summary>
        /// Log de informações
        /// </summary>
        /// <param name="message">Mensagem de informação</param>
        public static void Information(string message)
        {
            log.LogInformation(message);
        }

        /// <summary>
        /// Log de aviso
        /// </summary>
        /// <param name="message">Mensagem de aviso</param>
        public static void Warning(string message)
        {
            log.LogWarning(message);
        }

        /// <summary>
        /// Log de erros fatais
        /// </summary>
        /// <param name="message">Mensagem de erro fatal</param>
        public static void Critical(string message)
        {
            log.LogCritical(message);
        }

        /// <summary>
        /// Log de erros
        /// </summary>
        /// <param name="message">Mensagem de erro</param>
        public static void Error(string message)
        {
            log.LogError(message);
        }

    }
}
