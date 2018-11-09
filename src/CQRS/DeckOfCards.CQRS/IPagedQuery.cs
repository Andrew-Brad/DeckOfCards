namespace ApiKickstart.Queries
{
    /// <summary>
    /// A paged query is able to returns its resulting objects with standard paging metadata - page number and the contained results per page.
    /// </summary>
    public interface IPagedQuery : IQuery
    {
        int Page { get; set; }
        int ResultsPerPage { get; set; }
    }
}
