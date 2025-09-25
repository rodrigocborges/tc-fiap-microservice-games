namespace FIAPCloudGames.Domain.Interfaces
{
    public interface IElasticSearchService<T> where T : class, IEntity
    {
        /// <summary>
        /// Indexa (cria ou atualiza) um documento no Elasticsearch.
        /// </summary>
        /// <param name="document">O objeto da entidade a ser indexado.</param>
        Task IndexDocumentAsync(T document);

        /// <summary>
        /// Obtém um documento pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do documento.</param>
        /// <returns>O documento encontrado ou nulo.</returns>
        Task<T?> GetDocumentAsync(string id);

        /// <summary>
        /// Deleta um documento pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do documento a ser deletado.</param>
        Task DeleteDocumentAsync(string id);

        /// <summary>
        /// Realiza uma busca com base em uma query complexa do Elasticsearch.
        /// </summary>
        /// <param name="query">O corpo da query em formato JSON.</param>
        /// <returns>Uma coleção de documentos que correspondem à busca.</returns>
        Task<ICollection<T>> SearchAsync(string query);

        /// <summary>
        /// Executa uma consulta de agregação para obter métricas.
        /// </summary>
        /// <param name="query">O corpo da query de agregação em formato JSON.</param>
        /// <returns>O resultado da agregação em formato de string JSON.</returns>
        Task<string> AggregateAsync(string query);
    }
}
