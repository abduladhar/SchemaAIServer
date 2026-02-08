namespace CommonEntities
{
    public class PagingRequest
    {
        private const int MaxPageSize = 200;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 20;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }

    public class ScrolledPagingRequest: PagingRequest
    {
        public int LastId { get; set; } = 1;
    }


}
